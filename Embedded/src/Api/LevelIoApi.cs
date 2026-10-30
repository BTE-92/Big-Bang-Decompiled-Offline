using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BBR249Server.Data;
using BBR249Server.Net;

namespace BBR249Server.Api;

// Export / import d'un niveau publie sous forme de fichier .zip (le client enregistre / lit le fichier sur le telephone).
// Contenu du zip (meme organisation que BBRo) : <id>/metadata (JSON), <id>/level (octets du niveau), <id>/screenshot (facultatif), et le fantome du
// createur s'il existe. Un fantome est un bloc "JSON + octets du fantome", avec un fichier .header qui donne les deux tailles ("268,8240") :
//   - course (Race) : <id>/Ghosts/ghost<nombre> et <id>/Ghosts/ghost<nombre>.header (le nombre est un horodatage)
//   - autres modes   : <id>/ghost et <id>/Ghosts/ghost.header
// L'import accepte aussi l'ancien nom <id>/levelData et l'ancien fantome brut (<id>/ghost sans .header).
public static class LevelIoApi
{
	// A level with its screenshot and ghost is well under a megabyte. The limits below keep a wrong or hostile file from using up memory:
	// the whole zip, the number of entries, and what each entry may decompress to (checked while reading, not only from the size the
	// zip claims, since a zip can lie about it).
	private const int MaxZipBytes = 8 * 1024 * 1024;

	private const int MaxEntries = 16;

	private const int MaxMetaBytes = 256 * 1024;

	private const int MaxLevelBytes = 6 * 1024 * 1024;

	private const int MaxImageBytes = 3 * 1024 * 1024;

	private static JsonNode ParseOr(string json, JsonNode fallback)
	{
		try
		{
			return JsonNode.Parse(json) ?? fallback;
		}
		catch
		{
			return fallback;
		}
	}

	private static void AddEntry(ZipArchive zip, string name, byte[]? data)
	{
		if (data == null || data.Length == 0)
		{
			return;
		}
		ZipArchiveEntry zipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal);
		using Stream stream = zipArchiveEntry.Open();
		stream.Write(data, 0, data.Length);
	}

	private static byte[]? ReadEntry(ZipArchive zip, string name, int maxBytes)
	{
		foreach (ZipArchiveEntry entry in zip.Entries)
		{
			if (!string.Equals(entry.Name, name, StringComparison.Ordinal))
			{
				continue;
			}
			if (entry.Length > maxBytes)
			{
				throw new InvalidDataException("entry too large: " + name);
			}
			using Stream stream = entry.Open();
			using MemoryStream ms = new MemoryStream();
			byte[] buffer = new byte[64 * 1024];
			int read;
			while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				if (ms.Length + read > maxBytes)
				{
					throw new InvalidDataException("entry too large: " + name);
				}
				ms.Write(buffer, 0, read);
			}
			return ms.ToArray();
		}
		return null;
	}

	// The ghost of the creator, written the way BBRo does: one file holding a small JSON description followed by the ghost bytes, and a
	// .header file with the two sizes. A race level keeps both in the Ghosts folder, any other level keeps the ghost at the root.
	private static void AddGhost(ZipArchive zip, string id, Row row)
	{
		byte[]? ghost = row.B("creatorGhost");
		if (ghost == null || ghost.Length == 0)
		{
			return;
		}
		JsonObject info = new JsonObject
		{
			["name"] = row.S("creatorName"),
			["playerId"] = row.S("creatorId"),
			["time"] = row.L("bestTime"),
			["ghostId"] = Guid.NewGuid().ToString("N"),
			["version"] = 3,
			["countryCode"] = "US",
			["trophies"] = 123,
			["trophyLoss"] = 0,
			["trophyWin"] = 0,
			["ghostWin"] = 0,
			["ghostLoss"] = 0,
			["teamId"] = "",
			["teamName"] = ""
		};
		byte[] infoBytes = Encoding.UTF8.GetBytes(info.ToJsonString());
		byte[] blob = new byte[infoBytes.Length + ghost.Length];
		Buffer.BlockCopy(infoBytes, 0, blob, 0, infoBytes.Length);
		Buffer.BlockCopy(ghost, 0, blob, infoBytes.Length, ghost.Length);
		byte[] header = Encoding.UTF8.GetBytes(infoBytes.Length + "," + ghost.Length);
		if (row.S("gameMode", "Race") == "Race")
		{
			string name = "ghost" + DateTime.UtcNow.Ticks;
			AddEntry(zip, id + "/Ghosts/" + name, blob);
			AddEntry(zip, id + "/Ghosts/" + name + ".header", header);
		}
		else
		{
			AddEntry(zip, id + "/ghost", blob);
			AddEntry(zip, id + "/Ghosts/ghost.header", header);
		}
	}

	// The ghost bytes of an imported level. With a .header file the ghost file is "JSON + ghost bytes" and only the ghost bytes are kept;
	// without one (a level exported by an older version of this game) the file already holds the ghost bytes only.
	private static byte[]? ReadGhost(ZipArchive zip)
	{
		string? headerName = null;
		foreach (ZipArchiveEntry entry in zip.Entries)
		{
			if (entry.Name.StartsWith("ghost", StringComparison.Ordinal) && entry.Name.EndsWith(".header", StringComparison.Ordinal))
			{
				headerName = entry.Name;
				break;
			}
		}
		if (headerName == null)
		{
			return ReadEntry(zip, "ghost", MaxImageBytes);
		}
		byte[]? header = ReadEntry(zip, headerName, 64);
		byte[]? blob = ReadEntry(zip, headerName.Substring(0, headerName.Length - ".header".Length), MaxImageBytes);
		if (header == null || blob == null)
		{
			return null;
		}
		string[] parts = Encoding.UTF8.GetString(header).Trim().Split(',');
		if (parts.Length != 2 || !int.TryParse(parts[0], out int infoLength) || !int.TryParse(parts[1], out int ghostLength) || infoLength < 0 || ghostLength <= 0 || (long)infoLength + ghostLength != blob.Length)
		{
			return null;
		}
		byte[] ghost = new byte[ghostLength];
		Buffer.BlockCopy(blob, infoLength, ghost, 0, ghostLength);
		return ghost;
	}

	public static void Register()
	{
		Router.Map("/v1/minigame/export", async delegate(Ctx c)
		{
			Row? row = MinigameApi.GetMinigame(c.Q("id"));
			byte[]? levelData = row?.B("levelData");
			if (row == null || levelData == null || levelData.Length == 0)
			{
				await c.Error();
				return;
			}
			string id = row.S("id");
			JsonObject meta = new JsonObject
			{
				["format"] = "bbr249-level",
				["formatVersion"] = 1,
				["id"] = id,
				["name"] = row.S("name"),
				["description"] = row.S("description"),
				["gameMode"] = row.S("gameMode", "Race"),
				["playerUnit"] = row.S("playerUnit", "Any"),
				["difficulty"] = row.S("difficulty", "New"),
				["clientVersion"] = row.L("clientVersion"),
				["complexity"] = row.L("complexity"),
				["levelRequirement"] = row.L("levelRequirement"),
				["bestTime"] = row.L("bestTime"),
				["editSessionCount"] = row.L("editSessionCount"),
				["creatorName"] = row.S("creatorName"),
				["publishTime"] = row.S("publishTime"),
				["itemsUsed"] = ParseOr(row.S("itemsUsed", "[]"), new JsonArray()),
				["creatorUpgrades"] = ParseOr(row.S("creatorUpgrades", "{}"), new JsonObject())
			};
			using MemoryStream ms = new MemoryStream();
			using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
			{
				AddEntry(zip, id + "/metadata", Encoding.UTF8.GetBytes(meta.ToJsonString()));
				AddEntry(zip, id + "/level", levelData);
				AddEntry(zip, id + "/screenshot", row.B("screenshot"));
				AddGhost(zip, id, row);
			}
			Console.WriteLine($"[Export] {row.S("name")} ({id}) by {c.PlayerId}: {ms.Length} bytes");
			await c.Bytes(ms.ToArray());
		});
		Router.Map("/v1/minigame/import", async delegate(Ctx c)
		{
			if (c.PlayerId == "" || c.Body.Length < 22)
			{
				await c.JsonText("{\"ok\":false,\"reason\":\"invalid\"}");
				return;
			}
			if (c.Body.Length > MaxZipBytes)
			{
				Console.WriteLine($"[Import] refused: {c.Body.Length} bytes is too big for a level");
				await c.JsonText("{\"ok\":false,\"reason\":\"toolarge\"}");
				return;
			}
			byte[]? metaBytes;
			byte[]? levelData;
			byte[]? screenshot;
			byte[]? ghost;
			try
			{
				using ZipArchive zip = new ZipArchive(new MemoryStream(c.Body), ZipArchiveMode.Read);
				if (zip.Entries.Count > MaxEntries)
				{
					throw new InvalidDataException("too many entries");
				}
				metaBytes = ReadEntry(zip, "metadata", MaxMetaBytes);
				levelData = ReadEntry(zip, "level", MaxLevelBytes) ?? ReadEntry(zip, "levelData", MaxLevelBytes);
				screenshot = ReadEntry(zip, "screenshot", MaxImageBytes);
				ghost = ReadGhost(zip);
			}
			catch (Exception ex)
			{
				Console.WriteLine("[Import] refused: " + ex.Message);
				await c.JsonText("{\"ok\":false,\"reason\":\"invalid\"}");
				return;
			}
			if (metaBytes == null || levelData == null || levelData.Length < 2)
			{
				await c.JsonText("{\"ok\":false,\"reason\":\"invalid\"}");
				return;
			}
			JsonObject meta;
			try
			{
				meta = (JsonNode.Parse(Encoding.UTF8.GetString(metaBytes)) as JsonObject) ?? new JsonObject();
			}
			catch
			{
				await c.JsonText("{\"ok\":false,\"reason\":\"invalid\"}");
				return;
			}
			if (meta["itemsUsed"] is JsonArray usedItems)
			{
				List<string> unknown = new List<string>();
				foreach (JsonNode? usedItem in usedItems)
				{
					string itemName = usedItem?.ToString() ?? "";
					if (itemName != "" && !KnownItems.IsKnown(itemName))
					{
						unknown.Add(itemName);
					}
				}
				if (unknown.Count > 0)
				{
					Console.WriteLine("[Import] refused: the level uses objects this version does not have: " + string.Join(", ", unknown));
					await c.JsonText("{\"ok\":false,\"reason\":\"incompatible\"}");
					return;
				}
			}
			string id = meta["id"]?.ToString() ?? "";
			if (!Regex.IsMatch(id, "^[A-Za-z0-9_-]{6,64}$"))
			{
				id = PlayerApi.NewId();
			}
			Row? existing = MinigameApi.GetMinigame(id);
			if (existing != null)
			{
				if (existing.S("creatorId") == c.PlayerId)
				{
					await c.JsonText("{\"ok\":false,\"reason\":\"already\"}");
					return;
				}
				id = PlayerApi.NewId();
			}
			Row? player = PlayerApi.GetPlayer(c.PlayerId);
			string creatorName = meta["creatorName"]?.ToString();
			if (string.IsNullOrWhiteSpace(creatorName))
			{
				creatorName = player?.S("name") ?? "??";
			}
			Db.Exec("INSERT INTO minigames (id, creatorId, creatorName) VALUES ($0,$1,$2)", id, c.PlayerId, creatorName);
			List<string> sets = new List<string>();
			List<object?> args = new List<object?>();
			void Set(string col, object? v)
			{
				sets.Add($"{col}=${args.Count}");
				args.Add(v);
			}
			void Text(string key, int max = 200)
			{
				string s = meta[key]?.ToString();
				if (s != null)
				{
					Set(key, s.Length > max ? s.Substring(0, max) : s);
				}
			}
			void Num(string key)
			{
				if (meta[key] != null && double.TryParse(meta[key].ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var result))
				{
					Set(key, (long)result);
				}
			}
			Text("name", 100);
			Text("description", 500);
			Text("gameMode", 40);
			Text("playerUnit", 40);
			Text("difficulty", 40);
			Num("clientVersion");
			Num("complexity");
			Num("levelRequirement");
			Num("bestTime");
			Num("editSessionCount");
			if (meta["itemsUsed"] is JsonArray itemsUsed)
			{
				Set("itemsUsed", itemsUsed.ToJsonString());
			}
			if (meta["creatorUpgrades"] is JsonObject creatorUpgrades)
			{
				Set("creatorUpgrades", creatorUpgrades.ToJsonString());
			}
			Set("state", "public");
			Set("publishTime", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
			Set("updatedAt", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
			Set("levelData", levelData);
			if (screenshot != null && screenshot.Length > 1)
			{
				Set("screenshot", screenshot);
			}
			if (ghost != null && ghost.Length > 1)
			{
				Set("creatorGhost", ghost);
			}
			args.Add(id);
			Db.Exec($"UPDATE minigames SET {string.Join(",", sets)} WHERE id=${args.Count - 1}", args.ToArray());
			Console.WriteLine($"[Import] {meta["name"]} -> {id} for {c.PlayerId}");
			await c.JsonText("{\"ok\":true,\"id\":\"" + id + "\"}");
		});
	}
}
