using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Traduction du jeu Big Bang Racing 2.4.9 : la langue est choisie dans les parametres (comme dans BBRo).
// Les textes anglais du jeu sont traduits au moment de l'affichage (voir TextMeshS) a l'aide du dictionnaire "loc.tsv"
// (colonnes : anglais, Fi, Fr, It, De, Es, La, Pt, PtBr, Ru).
public static class Loc
{
    public const string PrefKey = "bbr_language";

    // meme ordre que l'enum Language de BBRo : En, Fi, Fr, It, De, Es, La, Pt, PtBr, Ru
    public static readonly string[] Names = new string[10]
    {
        "English", "Suomi", "Français", "Italiano", "Deutsch", "Español", "Español (LatAm)", "Português", "Português (Br)", "Russian"
    };

    private static bool s_init;
    private static int s_lookupLang;
    private static HashSet<string> s_seen = new HashSet<string>();
    private static HashSet<string> s_outputs = new HashSet<string>();
    private static string s_logPath;
    public static bool LogMisses = false;
    private static int s_current;
    private static int s_dbg;
    private static string s_lastExc = string.Empty;
    private static Dictionary<string, string[]> s_map = new Dictionary<string, string[]>();
    private static Dictionary<string, string> s_upper = new Dictionary<string, string>();
    private static Dictionary<string, string> s_lower = new Dictionary<string, string>();

    private class Wild
    {
        public string[] Parts;
        public int FixedLen;
        public string[] Tr;
    }
    private static List<Wild> s_wild = new List<Wild>();
    private static Dictionary<string, string> s_cache = new Dictionary<string, string>();

    public static int Current
    {
        get { EnsureInit(); return s_current; }
    }

    public static void SetLanguage(int index)
    {
        if (index < 0 || index >= Names.Length) index = 0;
        s_current = index;
        s_cache.Clear();
        s_outputs.Clear();
        try
        {
            PlayerPrefs.SetInt(PrefKey, index);
            PlayerPrefs.Save();
        }
        catch (Exception) { }
    }

    private static void EnsureInit()
    {
        if (s_init) return;
        s_init = true;
        try { s_current = PlayerPrefs.GetInt(PrefKey, 0); } catch (Exception) { s_current = 0; }
        if (s_current < 0 || s_current >= Names.Length) s_current = 0;
        try
        {
            TextAsset asset = Resources.Load<TextAsset>("BBR/loc");
            if (asset == null) return;
            using (StringReader sr = new StringReader(asset.text))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;
                    string[] parts = line.Split('	');
                    if (parts.Length < 2) continue;
                    string key = Unescape(parts[0]);
                    string[] tr = new string[9];
                    for (int i = 0; i < 9; i++)
                        tr[i] = (i + 1 < parts.Length) ? Unescape(parts[i + 1]) : string.Empty;
                    if (key.IndexOf("{*}") >= 0)
                    {
                        Wild w = new Wild();
                        w.Parts = key.Split(new string[] { "{*}" }, StringSplitOptions.None);
                        w.FixedLen = key.Length - 3 * (w.Parts.Length - 1);
                        w.Tr = tr;
                        s_wild.Add(w);
                    }
                    else if (!s_map.ContainsKey(key))
                    {
                        s_map.Add(key, tr);
                        string up = key.ToUpper();
                        if (!s_upper.ContainsKey(up)) s_upper.Add(up, key);
                        string lo = key.ToLower();
                        if (!s_lower.ContainsKey(lo)) s_lower.Add(lo, key);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Loc: could not load translations: " + e.Message);
        }
    }

    private static string Unescape(string s)
    {
        if (s.IndexOf('\\') < 0) return s;
        StringBuilder sb = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (c == '\\' && i + 1 < s.Length)
            {
                char n = s[i + 1];
                if (n == 'n') { sb.Append('\n'); i++; continue; }
                if (n == 't') { sb.Append('\t'); i++; continue; }
                if (n == '\\') { sb.Append('\\'); i++; continue; }
            }
            sb.Append(c);
        }
        return sb.ToString();
    }

    private static bool HasLetter(string s)
    {
        for (int i = 0; i < s.Length; i++)
            if (char.IsLetter(s[i])) return true;
        return false;
    }

    // traduction du texte exact (ou null)
    private static string Exact(string s)
    {
        string[] tr;
        if (s_map.TryGetValue(s, out tr))
        {
            string r = tr[s_lookupLang - 1];
            if (r.Length > 0) return r;
        }
        return null;
    }

    private static string ExactOrUpper(string s)
    {
        string r = Exact(s);
        if (r != null) return r;
        string key;
        if (s_upper.TryGetValue(s, out key))
        {
            r = Exact(key);
            if (r != null) return r.ToUpper();
        }
        if (s_lower.TryGetValue(s.ToLower(), out key))
        {
            r = Exact(key);
            if (r != null) return r;
        }
        return null;
    }

    // nombre -> {0}
    private static string WithNumberPattern(string s, out string number)
    {
        number = null;
        int i = 0;
        while (i < s.Length && !char.IsDigit(s[i])) i++;
        if (i >= s.Length) return null;
        int j = i;
        while (j < s.Length && (char.IsDigit(s[j]) || ((s[j] == ',' || s[j] == '.') && j + 1 < s.Length && char.IsDigit(s[j + 1])))) j++;
        number = s.Substring(i, j - i);
        return s.Substring(0, i) + "{0}" + s.Substring(j);
    }

    private static string StripTags(string s)
    {
        StringBuilder sb = new StringBuilder(s.Length);
        int i = 0;
        while (i < s.Length)
        {
            if (s[i] == '<')
            {
                int gt = s.IndexOf('>', i);
                if (gt > i) { i = gt + 1; continue; }
            }
            sb.Append(s[i]);
            i++;
        }
        return sb.ToString();
    }

    private static string WildMatch(string s)
    {
        string best = null;
        int bestLen = -1;
        for (int i = 0; i < s_wild.Count; i++)
        {
            Wild w = s_wild[i];
            if (s.Length <= w.FixedLen || w.FixedLen <= bestLen) continue;
            string tr = w.Tr[s_lookupLang - 1];
            if (tr.Length == 0) continue;
            string[] parts = w.Parts;
            int last = parts.Length - 1;
            if (!s.StartsWith(parts[0], StringComparison.Ordinal) || !s.EndsWith(parts[last], StringComparison.Ordinal)) continue;
            if (parts[0].Length + parts[last].Length > s.Length) continue;
            int limit = s.Length - parts[last].Length;
            int pos = parts[0].Length;
            string[] caps = new string[last];
            bool ok = true;
            for (int k = 1; k <= last; k++)
            {
                int at;
                if (k == last) at = limit;
                else
                {
                    at = s.IndexOf(parts[k], pos, StringComparison.Ordinal);
                    if (at < 0 || at + parts[k].Length > limit) { ok = false; break; }
                }
                caps[k - 1] = s.Substring(pos, at - pos);
                pos = at + parts[k].Length;
            }
            if (!ok) continue;
            StringBuilder sb = new StringBuilder();
            int ci = 0, from = 0;
            while (true)
            {
                int at = tr.IndexOf("{*}", from, StringComparison.Ordinal);
                if (at < 0) { sb.Append(tr.Substring(from)); break; }
                sb.Append(tr.Substring(from, at - from));
                sb.Append(ci < caps.Length ? caps[ci] : string.Empty);
                ci++;
                from = at + 3;
            }
            best = sb.ToString();
            bestLen = w.FixedLen;
        }
        return best;
    }

    private static string Core(string s)
    {
        string r = ExactOrUpper(s);
        if (r != null) return r;

        // espaces autour
        int a = 0, b = s.Length;
        while (a < b && (char.IsWhiteSpace(s[a]) || s[a] < ' ')) a++;
        while (b > a && (char.IsWhiteSpace(s[b - 1]) || s[b - 1] < ' ')) b--;
        if (a > 0 || b < s.Length)
        {
            if (b > a)
            {
                string inner = Core(s.Substring(a, b - a));
                if (inner != null) return s.Substring(0, a) + inner + s.Substring(b);
            }
            // les cles avec espaces exacts ont deja ete testees plus haut
        }

        // un nombre dans le texte
        string number;
        string pat = WithNumberPattern(s, out number);
        if (pat != null)
        {
            r = ExactOrUpper(pat);
            if (r != null) return r.Replace("{0}", number);
        }

        // modeles avec joker {*}
        r = WildMatch(s);
        if (r != null) return r;
        if (s.IndexOf('<') >= 0)
        {
            string plain = StripTags(s);
            if (plain != s)
            {
                r = ExactOrUpper(plain);
                if (r == null) r = WildMatch(plain);
                if (r != null) return r;
            }
        }

        // plusieurs lignes
        if (s.IndexOf('\n') >= 0)
        {
            string[] lines = s.Split('\n');
            bool any = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length == 0 || !HasLetter(lines[i])) continue;
                string t = Core(lines[i]);
                if (t != null) { lines[i] = t; any = true; }
            }
            if (any) return string.Join("\n", lines);
        }

        // balises <color=...> : on traduit chaque morceau de texte
        if (s.IndexOf('<') >= 0 && s.IndexOf('>') > s.IndexOf('<'))
        {
            StringBuilder sb = new StringBuilder();
            bool any = false;
            int pos = 0;
            while (pos < s.Length)
            {
                int lt = s.IndexOf('<', pos);
                if (lt < 0) lt = s.Length;
                if (lt > pos)
                {
                    string seg = s.Substring(pos, lt - pos);
                    string t = HasLetter(seg) ? Core(seg) : null;
                    if (t != null) { sb.Append(t); any = true; } else sb.Append(seg);
                }
                if (lt >= s.Length) break;
                int gt = s.IndexOf('>', lt);
                if (gt < 0) { sb.Append(s.Substring(lt)); break; }
                sb.Append(s.Substring(lt, gt - lt + 1));
                pos = gt + 1;
            }
            if (any) return sb.ToString();
        }
        return null;
    }

    private static string Escape(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\\t");
    }

    private static void LogMiss(string s)
    {
        if (!LogMisses || s.Length > 400) return;
        try
        {
            if (!s_seen.Add(s)) return;
            Debug.LogError("LOCMISS|" + Escape(s) + "|END");
            if (s_dbg < 40) { s_dbg++; Debug.LogError("LOCDBG|cur=" + s_current + " look=" + s_lookupLang + " map=" + s_map.Count + " wild=" + s_wild.Count + " exc=" + s_lastExc + "|END"); }
        }
        catch (Exception) { LogMisses = false; }
    }

    // objets de la categorie "Dev Items" : leurs noms et descriptions restent en anglais dans toutes les langues
    private static HashSet<string> s_keep = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private static bool s_devDone;
    private static int s_devTries;

    // texte a ne jamais traduire (noms de fichiers...)
    public static void Keep(string s)
    {
        if (!string.IsNullOrEmpty(s)) { s_keep.Add(s.Trim()); s_cache.Remove(s); }
    }

    private static void ScanDevItems()
    {
        if (s_devDone) return;
        if ((s_devTries++ % 50) != 0) return;
        try
        {
            if (PsMetagameData.m_units == null) return;
            for (int i = 0; i < PsMetagameData.m_units.Count; i++)
            {
                PsUnlockableCategory cat = PsMetagameData.m_units[i];
                if (cat == null || cat.m_name != "Dev Items" || cat.m_items == null) continue;
                for (int j = 0; j < cat.m_items.Count; j++)
                {
                    PsEditorItem it = cat.m_items[j] as PsEditorItem;
                    if (it == null) continue;
                    if (!string.IsNullOrEmpty(it.m_name)) s_keep.Add(it.m_name.Trim());
                    if (!string.IsNullOrEmpty(it.m_description)) s_keep.Add(it.m_description.Trim());
                }
                s_devDone = s_keep.Count > 0;
                if (s_devDone) s_cache.Clear();
                return;
            }
        }
        catch (Exception) { }
    }

    // point d'entree : appele pour chaque texte affiche
    public static string T(string s)
    {
        if (s == null || s.Length == 0) return s;
        EnsureInit();
        if (!HasLetter(s)) return s;
        if (!s_devDone) ScanDevItems();
        if (s_current == 0)
        {
            if (LogMisses && !s_seen.Contains(s))
            {
                s_lookupLang = 1;
                string probe = null;
                try { probe = Core(s); } catch (Exception) { }
                if (probe == null) LogMiss(s); else s_seen.Add(s);
            }
            return s;
        }
        if (s_outputs.Contains(s)) return s;
        string cached;
        if (s_cache.TryGetValue(s, out cached)) return cached;
        if (s_keep.Count > 0 && s_keep.Contains(StripTags(s).Trim())) { s_cache[s] = s; return s; }
        s_lookupLang = s_current;
        string res = null;
        try { res = Core(s); } catch (Exception ex) { s_lastExc = ex.GetType().Name + ":" + ex.Message; }
        if (res == null) { LogMiss(s); res = s; } else if (res != s) { s_outputs.Add(res); }
        if (s_cache.Count > 4000) s_cache.Clear();
        s_cache[s] = res;
        return res;
    }
}
