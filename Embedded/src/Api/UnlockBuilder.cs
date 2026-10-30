using BBR249Server.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace BBR249Server.Api;

public static class UnlockBuilder
{
	public const string FallbackUnlockName = "unlock_car";

	private static readonly string[] ItemTypes = new string[3] { "Unlockable", "UnlockableSimple", "UnlockableUpgradeable" };

	private static string _firstUnlockName = "unlock_car";

	public static string FirstUnlockName => _firstUnlockName;

	// Le client 2.4.9 ne connait que les modes Race et StarCollect pour les niveaux de joueur (enum PsGameMode).
	// La carte contient aussi une carte "GameModeSignal" : la toucher dans l'editeur plante le jeu
	// (Enum.Parse "Signal"). On retire du graphe tout mode de jeu non gere, ainsi que ses liens.
	public static int RemoveUnsupportedGameModes(JsonObject graph)
	{
		if (!(graph["nodes"] is JsonArray jsonArray) || !(graph["edges"] is JsonArray jsonArray2))
		{
			return 0;
		}
		HashSet<int> hashSet = new HashSet<int>();
		foreach (JsonNode item in jsonArray)
		{
			string text = item?["data"]?["itemClass"]?.ToString() ?? "";
			if (text.StartsWith("GameMode") && text != "GameModeRace" && text != "GameModeStarCollect")
			{
				hashSet.Add(item["id"].GetValue<int>());
			}
		}
		if (hashSet.Count == 0)
		{
			return 0;
		}
		for (int num = jsonArray.Count - 1; num >= 0; num--)
		{
			if (hashSet.Contains(jsonArray[num]["id"].GetValue<int>()))
			{
				jsonArray.RemoveAt(num);
			}
		}
		for (int num2 = jsonArray2.Count - 1; num2 >= 0; num2--)
		{
			if (hashSet.Contains(jsonArray2[num2]["source"].GetValue<int>()) || hashSet.Contains(jsonArray2[num2]["target"].GetValue<int>()))
			{
				jsonArray2.RemoveAt(num2);
			}
		}
		return hashSet.Count;
	}

	// Vrai si le noeud appartient a une section dont le nom est une sequence "cinematique seule" (on remonte les liens jusqu'au bloc Unlock).
	private static bool CutsceneSection(JsonArray nodes, JsonArray edges, int nodeId)
	{
		Dictionary<int, JsonNode> byId = new Dictionary<int, JsonNode>();
		foreach (JsonNode n in nodes)
		{
			byId[n["id"].GetValue<int>()] = n;
		}
		int current = nodeId;
		for (int guard = 0; guard < 200; guard++)
		{
			int prev = -1;
			foreach (JsonNode e in edges)
			{
				if (e["target"].GetValue<int>() == current && byId.TryGetValue(e["source"].GetValue<int>(), out JsonNode? src))
				{
					string st = src["data"]?["nodeType"]?.ToString() ?? "";
					if (st == "Unlock" || st == "Level" || st == "Block" || st == "Signal" || st == "EditorPuzzle" || st == "LevelTemplate" || st == "VersusRace")
					{
						prev = e["source"].GetValue<int>();
						break;
					}
				}
			}
			if (prev < 0)
			{
				return false;
			}
			JsonNode pn = byId[prev];
			if ((pn["data"]?["nodeType"]?.ToString() ?? "") == "Unlock")
			{
				return CutsceneNames.Contains(pn["name"]?.ToString() ?? "");
			}
			current = prev;
		}
		return false;
	}

	// Le jeu retient les sections deja passees d'apres le NOM des noeuds du chemin (un seul noeud par section porte le nom de sa section).
	// Renommer un noeud television sans renommer sa section ferait croire au jeu que la section n'est pas passee : au bout du chemin il
	// repartirait du debut. On renomme donc aussi les blocs "Unlock" du graphe, avec les memes noms que ceux donnes aux noeuds.
	public static int RenameTvSections(JsonObject graph)
	{
		if (!(graph["nodes"] is JsonArray nodes))
		{
			return 0;
		}
		int count = 0;
		foreach (JsonNode item in nodes)
		{
			if ((item?["data"]?["nodeType"]?.ToString() ?? "") != "Unlock" || item["data"] is not JsonObject data)
			{
				continue;
			}
			string name = item["name"]?.ToString() ?? "";
			if (name == "unlock_begin" || !TvSequenceNames.TryGetValue(name, out string? newName))
			{
				continue;
			}
			item["name"] = newName;
			foreach (string key in data.Select<KeyValuePair<string, JsonNode>, string>((KeyValuePair<string, JsonNode> kv) => kv.Key).ToList())
			{
				if (data[key] is JsonValue jv && jv.TryGetValue<string>(out string? value) && value == name)
				{
					data[key] = newName;
				}
			}
			count++;
		}
		return count;
	}

	// Noeuds "Signal" (antenne / television) : le client non modifie ne sait pas les jouer dans deux cas.
	//  - antenne SANS niveau : le jeu ouvre la scene de jeu sans niveau puis plante (GameStartState / menu en jeu sans niveau)
	//  - dialogue "MinigameEnter" sur une antenne : plante l'effet de television avant qu'il soit initialise
	// On donne donc a chaque antenne un identifiant de niveau officiel (le serveur sert un niveau de remplacement, voir
	// MinigameApi.GetOrStandIn) et on retire les dialogues MinigameEnter. Le chemin joue est alors celui prevu par le jeu.
	public static int FixSignalNodes(JsonObject graph)
	{
		if (!(graph["nodes"] is JsonArray jsonArray) || !(graph["edges"] is JsonArray jsonArray2))
		{
			return 0;
		}
		int num = 0;
		HashSet<int> hashSet = new HashSet<int>();
		foreach (JsonNode item in jsonArray)
		{
			if (!(item?["data"] is JsonObject jsonObject))
			{
				continue;
			}
			string text = jsonObject["nodeType"]?.ToString() ?? "";
			if (text == "Dialogue" && (jsonObject["dialogueTrigger"]?.ToString() ?? "") == "MinigameEnter")
			{
				hashSet.Add(item["id"].GetValue<int>());
			}
			else if (text == "Block" && (jsonObject["blockType"]?.ToString() ?? "") == "Antenna" && (jsonObject["levelMinigameId"]?.ToString() ?? "") == "" && !CutsceneSection(jsonArray, jsonArray2, item["id"].GetValue<int>()))
			{
				jsonObject["levelMinigameId"] = "5b1d0c" + item["id"].GetValue<int>().ToString("x6") + "0000" + "7a" + "5e1f00";
				num++;
			}
		}
		for (int num2 = jsonArray.Count - 1; num2 >= 0; num2--)
		{
			if (hashSet.Contains(jsonArray[num2]["id"].GetValue<int>()))
			{
				jsonArray.RemoveAt(num2);
			}
		}
		for (int num3 = jsonArray2.Count - 1; num3 >= 0; num3--)
		{
			if (hashSet.Contains(jsonArray2[num3]["source"].GetValue<int>()) || hashSet.Contains(jsonArray2[num3]["target"].GetValue<int>()))
			{
				jsonArray2.RemoveAt(num3);
			}
		}
		foreach (JsonNode item in jsonArray)
		{
			string t = item?["data"]?["nodeType"]?.ToString() ?? "";
			bool antenna = t == "Block" && (item["data"]["blockType"]?.ToString() ?? "") == "Antenna";
			if ((t == "Signal" || antenna) && (item["data"]["levelMinigameId"]?.ToString() ?? "") != "" && CutsceneSection(jsonArray, jsonArray2, item["id"].GetValue<int>()))
			{
				item["data"]["levelMinigameId"] = "";
				num++;
			}
		}
		num += AddTvDialogues(jsonArray);
		return num + hashSet.Count;
	}

	// Dialogues des sequences "television" que le jeu sait jouer avec une vraie image (au lieu du gris de la sequence par defaut).
	// Un dialogue sans entree dans le graphe est enregistre par le jeu sous son identifiant ; il en faut un par sequence.
	private static readonly (string Id, string Text, string Proceed)[] TvDialogues = new (string, string, string)[]
	{
		("treadmill_tv_dialogue_1", "Meh, next channel.", "OK"),
		("unlock_barrel_dialogue_1", "Whoa, something is blowing up on this channel!", "Cool!"),
		("unlock_boulder_dialogue_1", "Look at those rocks! Let's give this one a try.", "Rock on!"),
		("unlock_airbomb_dialogue_1", "Incoming! Bombs from above!", "Duck!"),
		("unlock_lava_dialogue_1", "Whoa, that channel is way too hot for me. Next!", "Phew!")
	};

	private static int AddTvDialogues(JsonArray nodes)
	{
		int added = 0;
		int nextId = 0;
		HashSet<string> existing = new HashSet<string>();
		foreach (JsonNode item in nodes)
		{
			nextId = System.Math.Max(nextId, item?["id"]?.GetValue<int>() ?? 0);
			string ident = item?["data"]?["dialogueIdentifier"]?.ToString() ?? "";
			if (ident != "")
			{
				existing.Add(ident);
			}
		}
		foreach ((string Id, string Text, string Proceed) dlg in TvDialogues)
		{
			if (existing.Contains(dlg.Id))
			{
				continue;
			}
			nodes.Add(new JsonObject
			{
				["id"] = ++nextId,
				["name"] = "",
				["x"] = 0.0,
				["y"] = 0.0,
				["data"] = new JsonObject
				{
					["nodeType"] = "Dialogue",
					["dialogueTrigger"] = "Manual",
					["dialogueCharacter"] = "Scientist",
					["dialogueCharacterPosition"] = "Right",
					["dialogueText"] = dlg.Text,
					["dialogueProceed"] = dlg.Proceed,
					["dialogueCancel"] = "",
					["dialogueIdentifier"] = dlg.Id
				}
			});
			added++;
		}
		return added;
	}

	// Noeuds television qui tomberaient sur la sequence par defaut (ecran gris de 2 secondes) : on leur donne le nom d'une sequence
	// qui a une image. Seul le nom du noeud change dans le chemin enregistre ; les noms des blocs "Unlock" du graphe restent intacts
	// (le jeu s'en sert pour enchainer les sections et pour certains tutoriels).
	// Sequences television qui se terminent par un retour direct a la carte (sans course) : le jeu les a prevues pour des noeuds SANS niveau.
	// Avec un niveau, l'etat de camera du jeu plante a chaque image apres le retour (le jeu gele). Ces noeuds doivent donc rester sans niveau.
	private static readonly HashSet<string> CutsceneNames = new HashSet<string>
	{
		"unlock_treadmill_signal", "unlock_barrel", "unlock_boulder", "unlock_airbomb", "unlock_lava", "unlock_timeattack_signal", "unlock_dynamics_signal"
	};

	private static readonly Dictionary<string, string> TvSequenceNames = new Dictionary<string, string>
	{
		["unlock_begin"] = "unlock_start",
		["unlock_upgrade"] = "unlock_treadmill_signal",
		["unlock_editor"] = "unlock_barrel",
		["unlock_timeattack"] = "unlock_boulder",
		["unlock_bike_signal"] = "unlock_airbomb",
		["unlock_rentals"] = "unlock_lava"
	};

	private static long AsLongLocal(JsonNode? n)
	{
		return long.TryParse(n?.ToString(), out long v) ? v : -1L;
	}

	// Meme correctif pour le chemin principal ENREGISTRE du joueur (c'est lui que le client utilise, pas le graphe) :
	// antenne sans niveau -> identifiant de niveau officiel (remplace par un niveau de remplacement), et on retire le
	// declencheur de dialogue "MinigameEnter" (avec son identifiant, meme position dans la liste).
	// Dialogues de test ("start", "claimed", "released"...) accroches au garage dans les donnees recreees : jamais vus dans le vrai jeu.
	private static bool IsTestDialogue(string? identifier)
	{
		return (identifier ?? "").StartsWith("test_");
	}

	public static int RemoveTestDialogues(JsonObject graph)
	{
		if (!(graph["nodes"] is JsonArray nodes) || !(graph["edges"] is JsonArray edges))
		{
			return 0;
		}
		HashSet<int> ids = new HashSet<int>();
		for (int i = nodes.Count - 1; i >= 0; i--)
		{
			if ((nodes[i]?["data"]?["nodeType"]?.ToString() ?? "") == "Dialogue" && IsTestDialogue(nodes[i]?["data"]?["dialogueIdentifier"]?.ToString()))
			{
				ids.Add(nodes[i]["id"].GetValue<int>());
				nodes.RemoveAt(i);
			}
		}
		for (int i = edges.Count - 1; i >= 0; i--)
		{
			if (ids.Contains(edges[i]["source"].GetValue<int>()) || ids.Contains(edges[i]["target"].GetValue<int>()))
			{
				edges.RemoveAt(i);
			}
		}
		return ids.Count;
	}

	private static void StripTestDialogues(JsonObject data)
	{
		if (!(data["dialogueIdentifiers"] is JsonArray idents) || !(data["dialogueTriggers"] is JsonArray trigs) || idents.Count != trigs.Count)
		{
			return;
		}
		for (int i = idents.Count - 1; i >= 0; i--)
		{
			if (IsTestDialogue(idents[i]?.ToString()))
			{
				idents.RemoveAt(i);
				trigs.RemoveAt(i);
			}
		}
		if (idents.Count == 0)
		{
			data.Remove("dialogueIdentifiers");
			data.Remove("dialogueTriggers");
		}
	}

	public static int FixStoredSignalNodes(JsonArray? nodes, long currentNode = 1)
	{
		if (nodes == null)
		{
			return 0;
		}
		int num = 0;
		foreach (JsonNode tdItem in nodes)
		{
			if (tdItem?["data"] is JsonObject tdData)
			{
				StripTestDialogues(tdData);
			}
		}
		// tous les noeuds (pas seulement les noeuds television) suivent le renommage des sections du graphe
		foreach (JsonNode item in nodes)
		{
			if (!(item?["data"] is JsonObject any))
			{
				continue;
			}
			string pName = any["unlockedProgression"]?.ToString() ?? "";
			if (TvSequenceNames.TryGetValue(pName, out string? renamed))
			{
				any["unlockedProgression"] = renamed;
				num++;
			}
			string wName = any["willUnlockProgression"]?.ToString() ?? "";
			if (TvSequenceNames.TryGetValue(wName, out string? wRenamed))
			{
				any["willUnlockProgression"] = wRenamed;
			}
		}
		// The first section is called "unlock_start" again: the game plays the introduction by itself when a player has no path, and
		// treats a player as new when no checkpoint at or before their current block carries that name. An old starter path (levels,
		// chest, introduction last) names its chest after the next section: its chest, the first checkpoint, takes the name.
		JsonNode? firstCheckpoint = null;
		foreach (JsonNode item in nodes)
		{
			string c = item?["data"]?["context"]?.ToString() ?? "";
			if ((c == "Block" || c == "Signal") && (firstCheckpoint == null || AsLongLocal(item["id"]) < AsLongLocal(firstCheckpoint["id"])))
			{
				firstCheckpoint = item;
			}
		}
		if (firstCheckpoint?["data"] is JsonObject firstData && (firstData["context"]?.ToString() ?? "") == "Block" && (firstData["unlockedProgression"]?.ToString() ?? "") == "unlock_car")
		{
			foreach (JsonNode item in nodes)
			{
				if ((item?["data"]?["context"]?.ToString() ?? "") == "Signal" && (item["data"]["unlockedProgression"]?.ToString() ?? "") == "unlock_start" && AsLongLocal(item["id"]) > AsLongLocal(firstCheckpoint["id"]))
				{
					firstData["unlockedProgression"] = "unlock_start";
					num++;
					break;
				}
			}
		}
		foreach (JsonNode item in nodes)
		{
			if (!(item?["data"] is JsonObject jsonObject) || (jsonObject["context"]?.ToString() ?? "") != "Signal")
			{
				continue;
			}
			string progression = jsonObject["unlockedProgression"]?.ToString() ?? "";
			if (CutsceneNames.Contains(jsonObject["unlockedProgression"]?.ToString() ?? ""))
			{
				if ((jsonObject["minigameId"]?.ToString() ?? "") != "")
				{
					jsonObject["minigameId"] = "";
					num++;
				}
			}
			else if ((jsonObject["minigameId"]?.ToString() ?? "") == "" && int.TryParse(item["id"]?.ToString(), out var id))
			{
				jsonObject["minigameId"] = "5b1d0c" + id.ToString("x6") + "0000" + "7a" + "5e1f01";
				num++;
			}
			if (jsonObject["dialogueTriggers"] is JsonArray triggers)
			{
				JsonArray ids = jsonObject["dialogueIdentifiers"] as JsonArray;
				for (int i = triggers.Count - 1; i >= 0; i--)
				{
					if (triggers[i]?.ToString() == "MinigameEnter")
					{
						triggers.RemoveAt(i);
						if (ids != null && i < ids.Count)
						{
							ids.RemoveAt(i);
						}
						num++;
					}
				}
			}
		}
		// Cases deja enregistrees (le client ne les relit pas dans le graphe) : silhouette de l'objet debloque par une case
		// television, et dialogue de debut de section sur la case qui ferme la section precedente (tant qu'elle n'est pas passee).
		foreach (JsonNode item in nodes)
		{
			if (!(item?["data"] is JsonObject d))
			{
				continue;
			}
			string ctx = d["context"]?.ToString() ?? "";
			if (ctx != "Block" && ctx != "Signal")
			{
				continue;
			}
			string section = d["unlockedProgression"]?.ToString() ?? "";
			if ((d["blockType"]?.ToString() ?? "") == "Antenna" && (d["blockRewardCue"]?.ToString() ?? "") == "")
			{
				d["blockRewardCue"] = NewItemTitle;
				num++;
			}
			if (ctx == "Signal" && (d["blockType"]?.ToString() ?? "") == "Antenna" && (d["willUnlockProgression"]?.ToString() ?? "") == "")
			{
				string will = WillUnlockOf(section);
				if (will != "")
				{
					d["willUnlockProgression"] = will;
					num++;
				}
			}
			string claim = (section == "unlock_begin" || section == "unlock_start") ? "" : ClaimDialogueOf(section);
			if (claim != "" && AsLongLocal(item["id"]) >= currentNode)
			{
				JsonArray idents = d["dialogueIdentifiers"] as JsonArray;
				JsonArray trigs = d["dialogueTriggers"] as JsonArray;
				if (idents == null || trigs == null || idents.Count != trigs.Count)
				{
					idents = new JsonArray();
					trigs = new JsonArray();
					d["dialogueIdentifiers"] = idents;
					d["dialogueTriggers"] = trigs;
				}
				bool has = false;
				foreach (JsonNode? t in trigs)
				{
					if (t?.ToString() == "LoopClaimed")
					{
						has = true;
					}
				}
				if (!has)
				{
					idents.Add(claim);
					trigs.Add("LoopClaimed");
					num++;
				}
			}
		}
		return num;
	}

	// Le client 2.4.9 n'a pas de table de traduction pour ces cles : il afficherait "EDITOR_GUI_VEHICLE_NAME_CAR" tel quel.
	private static readonly Dictionary<string, string> TextKeys = new Dictionary<string, string>
	{
		["EDITOR_GUI_VEHICLE_NAME_CAR"] = "Offroader",
		["EDITOR_GUI_VEHICLE_DESC_CAR"] = "Big wheels, big power. Crushes anything in its way!",
		["EDITOR_GUI_VEHICLE_NAME_BIKE"] = "Dirtbike",
		["EDITOR_GUI_VEHICLE_DESC_BIKE"] = "Light and nimble. Perfect for jumps and tight turns!"
	};

	public static int FixTextKeys(JsonObject graph)
	{
		if (!(graph["nodes"] is JsonArray jsonArray))
		{
			return 0;
		}
		int num = 0;
		foreach (JsonNode item in jsonArray)
		{
			if (!(item?["data"] is JsonObject jsonObject))
			{
				continue;
			}
			foreach (string key in new string[2] { "itemName", "itemDescription" })
			{
				string text = jsonObject[key]?.ToString() ?? "";
				if (text.StartsWith("EDITOR_GUI_"))
				{
					jsonObject[key] = TextKeys.TryGetValue(text, out string value) ? value : text.Substring("EDITOR_GUI_".Length).Replace('_', ' ').ToLowerInvariant();
					num++;
				}
			}
		}
		return num;
	}

	// Donnees de la carte ajustees cote serveur (marche donc avec n'importe quel client, patche ou non) :
	//  - la categorie d'objets "DELETE LATER" s'appelle "Dev Items"
	//  - les objets de decor n'avaient pas d'icone dans les donnees recreees (le jeu cherchait l'image "" -> erreur) : on donne
	//    le nom des images deja presentes dans l'atlas du jeu
	//  - la caisse en bois "PsWoodenBlock" prend l'icone du carton (la caisse dure garde celle de la caisse en bois)
	public static int FixDevItemData(JsonObject graph)
	{
		if (!(graph["nodes"] is JsonArray jsonArray))
		{
			return 0;
		}
		System.Collections.Generic.Dictionary<string, string> icons = new System.Collections.Generic.Dictionary<string, string>
		{
			["DecorationalTree"] = "item_deco_trees",
			["DecorationalSign"] = "item_deco_signs",
			["DecorationalRock"] = "item_deco_rocks",
			["DecorationalPyrotechnics"] = "item_pyro_confetti",
			["DecorationalPyroDragon"] = "item_pyro_fire",
			["DecorationalPyroChickoon"] = "item_pyro_chicken"
		};
		int num = 0;
		foreach (JsonNode item in jsonArray)
		{
			if (!(item?["data"] is JsonObject d))
			{
				continue;
			}
			if (d["itemName"]?.ToString() == "DELETE LATER")
			{
				d["itemName"] = "Dev Items";
				num++;
			}
			string cls = d["itemClass"]?.ToString() ?? "";
			if (icons.TryGetValue(cls, out string icon) && string.IsNullOrEmpty(d["itemIcon"]?.ToString()))
			{
				d["itemIcon"] = icon;
				num++;
			}
			if (cls == "PsWoodenBlock" && d["itemIcon"]?.ToString() != "item_carboardbox_icon")
			{
				d["itemIcon"] = "item_carboardbox_icon";
				num++;
			}
			// the atlas has a picture of its own for the rolling boulder; the recreated data gave it the rock's
			if (cls == "PsRollingBoulder" && d["itemIcon"]?.ToString() != "item_boulder_icon")
			{
				d["itemIcon"] = "item_boulder_icon";
				num++;
			}
			// Un objet sans icone fait planter l'ecran "New items" (le jeu cherche une image au nom vide) : il lui faut toujours une icone
			// qui existe dans l'atlas du jeu d'origine. "Desert" (classe vide) et le capteur d'objectif n'en avaient pas.
			string nodeType = d["nodeType"]?.ToString() ?? "";
			if (ItemTypes.Contains(nodeType) && string.IsNullOrEmpty(d["itemIcon"]?.ToString()))
			{
				d["itemIcon"] = cls == "SensorGoal" ? "item_mode_timeattack" : "item_dome_empty_icon";
				num++;
			}
		}
		return num;
	}

	public static int DisableTutorialStart(JsonObject graph)
	{
		if (!(graph["nodes"] is JsonArray jsonArray))
		{
			return 0;
		}
		int num = 0;
		foreach (JsonNode item in jsonArray)
		{
			if (item is JsonObject jsonObject && jsonObject["name"]?.ToString() == "unlock_start")
			{
				jsonObject["name"] = "unlock_begin";
				num++;
			}
			if (!(item?["data"] is JsonObject jsonObject2))
			{
				continue;
			}
			foreach (string item2 in jsonObject2.Select<KeyValuePair<string, JsonNode>, string>((KeyValuePair<string, JsonNode> kv) => kv.Key).ToList())
			{
				if (jsonObject2[item2] is JsonValue jsonValue && jsonValue.TryGetValue<string>(out string value) && value == "unlock_start")
				{
					jsonObject2[item2] = "unlock_begin";
					num++;
				}
			}
		}
		return num;
	}

	public static int LinkAllItemsToFirstUnlock(JsonObject graph)
	{
		if (!(graph["nodes"] is JsonArray jsonArray) || !(graph["edges"] is JsonArray jsonArray2))
		{
			return 0;
		}
		int? num = null;
		// The very first section ("unlock_start", the introduction) is flagged when the game generates a new path, before the
		// first "new items" screen: linking the items there makes them all available without cluttering that screen with them.
		foreach (JsonNode item in jsonArray)
		{
			if ((item?["data"]?["nodeType"]?.ToString() ?? "") == "Unlock" && (item["data"]["itemClass"]?.ToString() ?? "") == "unlock_start")
			{
				num = item["id"].GetValue<int>();
				_firstUnlockName = "unlock_start";
				break;
			}
		}
		if (!num.HasValue)
		{
			foreach (JsonNode item in jsonArray)
			{
				if (!(item?["data"]?["nodeType"]?.ToString() != "Unlock"))
				{
					string text = item["data"]["itemClass"]?.ToString() ?? "";
					if (!(text == ""))
					{
						num = item["id"].GetValue<int>();
						_firstUnlockName = text;
						break;
					}
				}
			}
		}
		if (!num.HasValue)
		{
			return 0;
		}
		HashSet<int> hashSet = new HashSet<int>();
		foreach (JsonNode item2 in jsonArray2)
		{
			if (item2["target"].GetValue<int>() == num)
			{
				hashSet.Add(item2["source"].GetValue<int>());
			}
		}
		int num2 = 0;
		foreach (JsonNode item3 in jsonArray)
		{
			string value = item3?["data"]?["nodeType"]?.ToString() ?? "";
			if (ItemTypes.Contains(value))
			{
				int value2 = item3["id"].GetValue<int>();
				if (!hashSet.Contains(value2))
				{
					jsonArray2.Add(new JsonObject
					{
						["target"] = num.Value,
						["source"] = value2
					});
					num2++;
				}
			}
		}
		return num2;
	}

	// ---- Silhouette des cases "antenne / television" et dialogues de section ------------------------------------------------
	// Le client 2.4.9 affiche sur chaque case television de la carte (PsUICheckpointBanner) l'objet que la case va debloquer, en
	// silhouette noire sur le disque vert ("menu_cp_image_signal"). Il le lit dans l'Unlock vers lequel la case a un lien SORTANT
	// dans le graphe (PsMetagameData.AddUnlocks : sortie de type Unlock = "blockWillUnlock") : on prend la premiere icone de cet
	// Unlock (PsUnlock.m_unlockables). Les donnees recreees n'ont aucun de ces liens : le disque vert restait vide.
	// title shown on the sign of a television checkpoint (blockRewardCue)
	public const string NewItemTitle = "New item";

	private static readonly Dictionary<string, string> WillUnlockBySection = new Dictionary<string, string>();

	// Dialogues de debut de section : dans les donnees recreees ils sont branches sur le bloc "Unlock" avec le declencheur "Manual",
	// que le client 2.4.9 ne joue jamais (seuls LoopStart, LoopClaimed... d'un noeud du chemin declenchent un dialogue).
	private static readonly Dictionary<string, string> ClaimDialogueBySection = new Dictionary<string, string>();

	public static string WillUnlockOf(string section)
	{
		return WillUnlockBySection.TryGetValue(section ?? "", out string? v) ? v : "";
	}

	public static string ClaimDialogueOf(string section)
	{
		return ClaimDialogueBySection.TryGetValue(section ?? "", out string? v) ? v : "";
	}

	private sealed class GraphIndex
	{
		public readonly Dictionary<int, JsonNode> ById = new Dictionary<int, JsonNode>();
		public readonly Dictionary<int, List<int>> Out = new Dictionary<int, List<int>>();
		public readonly Dictionary<int, List<int>> In = new Dictionary<int, List<int>>();

		public GraphIndex(JsonArray nodes, JsonArray edges)
		{
			foreach (JsonNode n in nodes)
			{
				int id = n["id"].GetValue<int>();
				ById[id] = n;
				Out[id] = new List<int>();
				In[id] = new List<int>();
			}
			foreach (JsonNode e in edges)
			{
				int s = e["source"].GetValue<int>();
				int t = e["target"].GetValue<int>();
				if (ById.ContainsKey(s) && ById.ContainsKey(t))
				{
					Out[s].Add(t);
					In[t].Add(s);
				}
			}
		}

		public string Type(int id)
		{
			return ById[id]["data"]?["nodeType"]?.ToString() ?? "";
		}

		public string Text(int id, string key)
		{
			return ById[id]["data"]?[key]?.ToString() ?? "";
		}
	}

	// Noeuds d'une section dans l'ordre du chemin, comme le client les lit (PsMetagameData.AddUnlocks) : chaque sortie Level /
	// EditorPuzzle / Signal du bloc Unlock demarre une chaine, la derniere sortie de chemin de chaque noeud donne le suivant.
	private static List<int> SectionNodes(GraphIndex g, int unlockId)
	{
		List<int> chain = new List<int>();
		foreach (int o in g.Out[unlockId])
		{
			string t = g.Type(o);
			if (t != "Level" && t != "EditorPuzzle" && t != "Signal")
			{
				continue;
			}
			int cur = o;
			for (int guard = 0; cur >= 0 && guard < 500; guard++)
			{
				chain.Add(cur);
				int next = -1;
				foreach (int o2 in g.Out[cur])
				{
					string t2 = g.Type(o2);
					if (t2 == "Level" || t2 == "EditorPuzzle" || t2 == "VersusRace" || t2 == "LevelTemplate" || t2 == "Block" || t2 == "Signal")
					{
						next = o2;
					}
				}
				cur = next;
			}
		}
		return chain;
	}

	private static int NextUnlockOf(GraphIndex g, int unlockId)
	{
		int next = -1;
		foreach (int o in g.Out[unlockId])
		{
			if (g.Type(o) == "Unlock")
			{
				next = o;
			}
		}
		return next;
	}

	// Premiere icone d'objet de l'Unlock, dans l'ordre des liens entrants (c'est celle que le client affiche).
	private static string FirstUnlockableIcon(GraphIndex g, int unlockId)
	{
		foreach (int i in g.In[unlockId])
		{
			if (ItemTypes.Contains(g.Type(i)))
			{
				string icon = g.Text(i, "itemIcon");
				if (icon != "")
				{
					return icon;
				}
			}
		}
		return "";
	}

	// "item_dome_empty_icon" est l'icone de secours donnee aux objets sans image (FixDevItemData) : en silhouette elle ne montre rien.
	private static bool IsRealIcon(string icon)
	{
		return icon != "" && icon != "item_dome_empty_icon";
	}

	public static int AddWillUnlockEdges(JsonObject graph)
	{
		WillUnlockBySection.Clear();
		if (!(graph["nodes"] is JsonArray nodes) || !(graph["edges"] is JsonArray edges))
		{
			return 0;
		}
		GraphIndex g = new GraphIndex(nodes, edges);
		int added = 0;
		foreach (int unlockId in g.ById.Keys.ToList())
		{
			if (g.Type(unlockId) != "Unlock")
			{
				continue;
			}
			string section = g.Text(unlockId, "itemClass");
			foreach (int nodeId in SectionNodes(g, unlockId))
			{
				bool antenna = g.Type(nodeId) == "Signal" || (g.Type(nodeId) == "Block" && g.Text(nodeId, "blockType") == "Antenna");
				if (!antenna)
				{
					continue;
				}
				// the title of the sign (the game upper-cases it): "NEW ITEM". Only a "Block" node can carry it in the graph; television checkpoints
				// of type "Signal" get it in the saved path (FixStoredSignalNodes)
				if (g.Type(nodeId) == "Block" && g.ById[nodeId]["data"] is JsonObject antennaData && (antennaData["blockRewardCue"]?.ToString() ?? "") == "")
				{
					antennaData["blockRewardCue"] = NewItemTitle;
				}
				// l'objet debloque = premier Unlock suivant (dans l'ordre du chemin) dont la premiere icone est une vraie image
				int target = NextUnlockOf(g, unlockId);
				for (int guard = 0; target >= 0 && guard < 60 && !IsRealIcon(FirstUnlockableIcon(g, target)); guard++)
				{
					target = NextUnlockOf(g, target);
				}
				if (target < 0 || !IsRealIcon(FirstUnlockableIcon(g, target)))
				{
					continue;
				}
				if (!g.Out[nodeId].Contains(target))
				{
					edges.Add(new JsonObject { ["target"] = target, ["source"] = nodeId });
					g.Out[nodeId].Add(target);
					g.In[target].Add(nodeId);
					added++;
				}
				string targetName = g.Text(target, "itemClass");
				if (section != "" && !WillUnlockBySection.ContainsKey(section))
				{
					WillUnlockBySection[section] = targetName;
					// la premiere section porte le nom "unlock_begin" quand l'introduction est desactivee, mais ses noeuds television
					// s'appellent "unlock_start" dans le chemin enregistre
					if (section == "unlock_begin" && !WillUnlockBySection.ContainsKey("unlock_start"))
					{
						WillUnlockBySection["unlock_start"] = targetName;
					}
				}
			}
		}
		return added;
	}

	// Texte de travail laisse par les auteurs ("WIP: ...", "NOTE: ...") : pas pour les joueurs.
	private static bool IsPlaceholderText(string text)
	{
		string t = (text ?? "").TrimStart();
		return t.StartsWith("WIP", System.StringComparison.OrdinalIgnoreCase) || t.StartsWith("NOTE", System.StringComparison.OrdinalIgnoreCase);
	}

	// Les dialogues "Manual" branches sur un bloc Unlock sont rejoues par le client quand la case qui FERME la section precedente
	// est reclamee (declencheur LoopClaimed), comme la chaine "unlock_garage_tease_dialogue" / "unlock_editor_signal_1" du graphe.
	public static int WireSectionDialogues(JsonObject graph)
	{
		ClaimDialogueBySection.Clear();
		if (!(graph["nodes"] is JsonArray nodes) || !(graph["edges"] is JsonArray edges))
		{
			return 0;
		}
		GraphIndex g = new GraphIndex(nodes, edges);
		int wired = 0;
		foreach (int unlockId in g.ById.Keys.ToList())
		{
			if (g.Type(unlockId) != "Unlock")
			{
				continue;
			}
			int prev = -1;
			foreach (int i in g.In[unlockId])
			{
				if (g.Type(i) == "Unlock")
				{
					prev = i;
					break;
				}
			}
			if (prev < 0)
			{
				continue;
			}
			List<int> chain = SectionNodes(g, prev);
			if (chain.Count == 0)
			{
				continue;
			}
			int last = chain[chain.Count - 1];
			if (g.Type(last) != "Block" && g.Type(last) != "Signal")
			{
				continue;
			}
			foreach (int d in g.In[unlockId].ToList())
			{
				// seules les tetes de chaine de dialogue (les suites sont lues avec elle)
				if (g.Type(d) != "Dialogue" || g.In[d].Count > 0 || g.Text(d, "dialogueTrigger") != "Manual" || !(g.ById[d]["data"] is JsonObject data))
				{
					continue;
				}
				if (IsPlaceholderText(g.Text(d, "dialogueText")))
				{
					continue;
				}
				bool taken = false;
				foreach (int x in g.In[last])
				{
					if (g.Type(x) == "Dialogue" && g.Text(x, "dialogueTrigger") == "LoopClaimed")
					{
						taken = true;
					}
				}
				if (taken || g.Out[d].Contains(last))
				{
					continue;
				}
				data["dialogueTrigger"] = "LoopClaimed";
				edges.Add(new JsonObject { ["target"] = last, ["source"] = d });
				g.Out[d].Add(last);
				g.In[last].Add(d);
				// les suites "WIP" / "NOTE" d'une chaine ne sont pas jouees : on coupe la chaine avant elles
				int cur = d;
				for (int guard = 0; guard < 50; guard++)
				{
					int step = -1;
					foreach (int o in g.Out[cur])
					{
						if (g.Type(o) == "Dialogue")
						{
							step = o;
							break;
						}
					}
					if (step < 0)
					{
						break;
					}
					if (IsPlaceholderText(g.Text(step, "dialogueText")))
					{
						for (int k = edges.Count - 1; k >= 0; k--)
						{
							if (edges[k]["source"].GetValue<int>() == cur && edges[k]["target"].GetValue<int>() == step)
							{
								edges.RemoveAt(k);
							}
						}
						g.Out[cur].Remove(step);
						g.In[step].Remove(cur);
						break;
					}
					cur = step;
				}
				string section = g.Text(prev, "itemClass");
				if (section != "" && !ClaimDialogueBySection.ContainsKey(section))
				{
					ClaimDialogueBySection[section] = g.Text(d, "dialogueIdentifier");
				}
				wired++;
			}
		}
		return wired;
	}

	// The recreated map has some televisions twice: an "Antenna" checkpoint with stars (0 / 6) followed, with no level in between, by a "Signal"
	// checkpoint of its own section with no star requirement (0 / 0) that shows the same item. The real game has one television per item.
	// The lone one is dropped from the map (the section stays, empty, so the chain of sections is unchanged); the dialogue that was hooked
	// on it continues the dialogue of the checkpoint before it, so nothing is lost. Run after WireSectionDialogues.
	public static int RemoveLoneTvSections(JsonObject graph)
	{
		if (!(graph["nodes"] is JsonArray nodes) || !(graph["edges"] is JsonArray edges))
		{
			return 0;
		}
		GraphIndex g = new GraphIndex(nodes, edges);
		HashSet<int> drop = new HashSet<int>();
		foreach (int unlockId in g.ById.Keys.ToList())
		{
			if (g.Type(unlockId) != "Unlock")
			{
				continue;
			}
			List<int> chain = SectionNodes(g, unlockId);
			if (chain.Count != 1 || g.Type(chain[0]) != "Signal")
			{
				continue;
			}
			int prev = -1;
			foreach (int i in g.In[unlockId])
			{
				if (g.Type(i) == "Unlock")
				{
					prev = i;
					break;
				}
			}
			if (prev < 0)
			{
				continue;
			}
			List<int> before = SectionNodes(g, prev);
			if (before.Count == 0)
			{
				continue;
			}
			int last = before[before.Count - 1];
			if (!(g.Type(last) == "Signal" || (g.Type(last) == "Block" && g.Text(last, "blockType") == "Antenna")))
			{
				continue;
			}
			int lone = chain[0];
			foreach (int d in g.In[lone].ToList())
			{
				if (g.Type(d) != "Dialogue" || g.Text(d, "dialogueTrigger") != "LoopClaimed" || !(g.ById[d]["data"] is JsonObject dData))
				{
					continue;
				}
				int head = -1;
				foreach (int x in g.In[last])
				{
					if (g.Type(x) == "Dialogue" && g.Text(x, "dialogueTrigger") == "LoopClaimed")
					{
						head = x;
					}
				}
				if (head < 0)
				{
					edges.Add(new JsonObject { ["target"] = last, ["source"] = d });
					g.Out[d].Add(last);
					g.In[last].Add(d);
					continue;
				}
				int tail = head;
				for (int guard = 0; guard < 50; guard++)
				{
					int next = -1;
					foreach (int o in g.Out[tail])
					{
						if (g.Type(o) == "Dialogue")
						{
							next = o;
							break;
						}
					}
					if (next < 0 || next == d)
					{
						break;
					}
					tail = next;
				}
				if (tail == d)
				{
					continue;
				}
				dData["dialogueTrigger"] = "Manual";
				edges.Add(new JsonObject { ["target"] = d, ["source"] = tail });
				g.Out[tail].Add(d);
				g.In[d].Add(tail);
			}
			drop.Add(lone);
		}
		if (drop.Count == 0)
		{
			return 0;
		}
		for (int i = edges.Count - 1; i >= 0; i--)
		{
			if (drop.Contains(edges[i]["source"].GetValue<int>()) || drop.Contains(edges[i]["target"].GetValue<int>()))
			{
				edges.RemoveAt(i);
			}
		}
		for (int i = nodes.Count - 1; i >= 0; i--)
		{
			if (drop.Contains(nodes[i]["id"].GetValue<int>()))
			{
				nodes.RemoveAt(i);
			}
		}
		return drop.Count;
	}

	private static JsonObject LevelNode(int id, int levelNumber)
	{
		return new JsonObject
		{
			["id"] = id,
			["levelNumber"] = levelNumber,
			["score"] = 0,
			["rented"] = false,
			["data"] = new JsonObject
			{
				["context"] = "Level",
				["minigameId"] = "",
				["unlocked"] = true,
				["levelGameMode"] = "Race",
				["levelPlayerUnit"] = "Any"
			}
		};
	}

	public static JsonObject SyntheticMainPath()
	{
		JsonObject path = SyntheticMainPathBase();
		if (!(path["nodes"] is JsonArray nodes) || nodes.Count != 4)
		{
			return path;
		}
		// Comme dans le jeu d'origine, la premiere case d'un nouveau chemin est la television d'introduction ("Big Bang Racing TV",
		// section "unlock_start") : les niveaux et le coffre de depart viennent apres.
		nodes.Insert(0, IntroNode());
		// joueur neuf : le puzzle suit le premier niveau (introduction, niveau 1, puzzle, niveau 2, niveau 3, coffre)
		if (Settings.PuzzleMission && Settings.PuzzleSection == "")
		{
			nodes.Insert(2, MapContent.PuzzleNode(0, 0));
		}
		RenumberPath(nodes);
		return path;
	}

	// Le noeud television d'introduction est fourni DEJA enregistre sous son nom de sequence ("unlock_start") : si le jeu le creait
	// lui-meme, il lui donnerait le nom de la section ("unlock_begin") et la sequence serait un ecran gris. Il porte aussi le lien
	// "va debloquer" vers l'objet de la section suivante (le vehicule, en silhouette noire sur le disque vert de la case).
	private static JsonObject IntroNode()
	{
		string will = WillUnlockOf("unlock_start");
		return new JsonObject
		{
			["id"] = 1,
			["levelNumber"] = -1,
			["score"] = 0,
			["rented"] = true,
			["data"] = new JsonObject
			{
				["context"] = "Signal",
				["requiredStars"] = 0,
				["unlocked"] = false,
				["minigameId"] = "5549c475310000f82c7490fb",
				["unlockedProgression"] = "unlock_start",
				["willUnlockProgression"] = will != "" ? will : FallbackUnlockName,
				["blockRewardCue"] = NewItemTitle,
				["blockCoins"] = 0,
				["blockDiamonds"] = 0,
				["blockKeys"] = 0,
				["blockBolts"] = 0,
				["blockType"] = "Antenna",
				["dialogueIdentifiers"] = new JsonArray("unlock_1_quest_2"),
				["dialogueTriggers"] = new JsonArray("LoopClaimed")
			}
		};
	}

	// numeros des cases (le client retrouve une case par son rang) et numeros de niveau : 1, 2, 3... pour les niveaux et le puzzle,
	// 0 pour les coffres, -1 pour les cases television
	private static void RenumberPath(JsonArray nodes)
	{
		int level = 0;
		for (int i = 0; i < nodes.Count; i++)
		{
			string ctx = nodes[i]["data"]?["context"]?.ToString() ?? "";
			nodes[i]["id"] = i + 1;
			if (ctx == "Level" || ctx == "EditorPuzzle")
			{
				level++;
				nodes[i]["levelNumber"] = level;
			}
			else
			{
				nodes[i]["levelNumber"] = ctx == "Signal" ? -1 : 0;
			}
		}
	}

	private static JsonObject SyntheticMainPathBase()
	{
		return new JsonObject
		{
			["name"] = "MainPath",
			["currentNode"] = 1,
			["nodes"] = new JsonArray
			{
				LevelNode(1, 1),
				LevelNode(2, 2),
				LevelNode(3, 3),
				new JsonObject
				{
					["id"] = 4,
					["levelNumber"] = 0,
					["score"] = 0,
					["rented"] = false,
					["data"] = new JsonObject
					{
						["context"] = "Block",
						["minigameId"] = "",
						["unlocked"] = false,
						["blockType"] = "Chest",
						["requiredStars"] = 3,
						["unlockedProgression"] = _firstUnlockName,
						["willUnlockProgression"] = "",
						// recompense du premier coffre : sans pieces ni gemmes, le jeu n'affiche pas l'animation d'ouverture
						["blockCoins"] = 250,
						["blockDiamonds"] = 20,
						["blockKeys"] = 0,
						["blockBolts"] = 0
					}
				}
			}
		};
	}
}
