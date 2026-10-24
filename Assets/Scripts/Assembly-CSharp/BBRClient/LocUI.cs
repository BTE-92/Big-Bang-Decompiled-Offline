using System;
using System.Collections.Generic;
using UnityEngine;

// Bouton "Language" des parametres + fenetre de choix de la langue (meme affichage que BBRo : une grille de boutons + OK).
public static class LocUI
{
    private static PsUIGenericButton s_button;

    // appele a la creation de l'ecran des parametres (PsUICenterSettings.CreateContent)
    public static void AddSettingsButton(UIComponent parent)
    {
        try
        {
            s_button = new PsUIGenericButton(parent);
            s_button.SetText("Language");
        }
        catch (Exception e)
        {
            Debug.Log("LocUI.AddSettingsButton failed: " + e.Message);
        }
    }

    // appele a la creation de chaque composant uGUI : traduit les textes fixes du prefab
    public static void TranslateUI(GameObject root)
    {
        try
        {
            if (root == null) return;
            UnityEngine.UI.Text[] texts = root.GetComponentsInChildren<UnityEngine.UI.Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                string cur = texts[i].text;
                if (!string.IsNullOrEmpty(cur)) texts[i].text = Loc.T(cur);
            }
        }
        catch (Exception) { }
    }

    // TextMeshS.FitTextToRect : marge de securite quand le texte a du etre reduit
    public static int ShrinkFit(int size)
    {
        if (Loc.Current == 0) return size;
        int r = Mathf.FloorToInt(size * 0.88f);
        return r < 1 ? 1 : r;
    }

    // UIText.Update : si le texte traduit est plus large que le conteneur (largeur fixe), on reduit la police
    public static int FitFont(UIText ut, int size)
    {
        try
        {
            if (Loc.Current == 0 || size < 8) return size;
            UIComponent p = ut.m_parent;
            if (p == null) return size;
            float avail = p.m_actualWidth - p.m_actualMargins.l - p.m_actualMargins.r;
            if (avail <= 4f) return size;
            Vector2 sz = TextMeshS.GetTextSize(ut.m_tmc.m_textMesh.font, size, ut.m_tmc.m_textMesh.lineSpacing, Loc.T(ut.m_text));
            if (sz.x > avail * 1.01f)
            {
                float k = avail / sz.x;
                if (k < 0.55f) k = 0.55f;
                int ns = Mathf.FloorToInt(size * k);
                return ns < 6 ? 6 : ns;
            }
        }
        catch (Exception) { }
        return size;
    }

    // UITextbox.Update : un mot plus long que la boite (ex. "Contre-la-montre") est coupe au milieu -> on reduit la police
    public static float FitBoxFont(UITextbox tb, float size)
    {
        try
        {
            if (Loc.Current == 0 || size < 8f || tb.m_text == null) return size;
            float width = (tb.m_actualWidth - tb.m_actualMargins.l - tb.m_actualMargins.r) * ((float)TextMeshS.m_defaultCharacterSize / tb.m_tmc.m_textMesh.characterSize);
            if (width <= 4f) return size;
            string plain = Loc.T(tb.m_text);
            int isz = Mathf.FloorToInt(size * ((float)TextMeshS.m_defaultCharacterSize / tb.m_tmc.m_textMesh.characterSize));
            float worst = 0f;
            if (tb.m_text == "Liked this level?")
            {
                float full = TextMeshS.GetTextSize(tb.m_tmc.m_textMesh.font, isz, tb.m_tmc.m_textMesh.lineSpacing, plain).x;
                if (full > width * 0.9f) return Mathf.Floor(size * width * 0.95f / full);
            }
            string[] words = plain.Replace('\n', ' ').Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string w = words[i];
                if (w.Length < 3) continue;
                int lt = w.IndexOf('<');
                while (lt >= 0)
                {
                    int gt = w.IndexOf('>', lt);
                    if (gt < 0) break;
                    w = w.Remove(lt, gt - lt + 1);
                    lt = w.IndexOf('<');
                }
                if (w.Length < 3) continue;
                float x = TextMeshS.GetTextSize(tb.m_tmc.m_textMesh.font, isz, tb.m_tmc.m_textMesh.lineSpacing, w).x;
                if (x > worst) worst = x;
            }
            if (worst > width * 0.97f)
            {
                float k = width * 0.95f / worst;
                if (k < 0.55f) k = 0.55f;
                return Mathf.Floor(size * k);
            }
        }
        catch (Exception) { }
        return size;
    }

    // appele a chaque image par PsUICenterSettings.Step
    public static void StepSettings()
    {
        // the button is kept: it must keep working each time the language window has been closed
        if (s_button != null && s_button.m_hit)
        {
            new PsUIBasePopup(typeof(LangSelect), null, null, null, false);
        }
    }
}

public class LangSelect : PsUIHeaderedCanvas
{
    private List<PsUIGenericButton> m_buttons = new List<PsUIGenericButton>();
    private PsUIGenericButton m_ok;
    private int m_selected;
    private int m_previous;

    public LangSelect(UIComponent _parent)
        : base(_parent, string.Empty, false)
    {
        SetWidth(0.75f, RelativeTo.ScreenWidth);
        SetHeight(0.75f, RelativeTo.ScreenHeight);
        SetVerticalAlign(0.4f);
        SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
        (GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
        SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
        m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
        m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
        m_selected = Loc.Current;
        m_previous = m_selected;
        CreateHeaderContent(m_header);
        CreateContent(this);
    }

    public void CreateHeaderContent(UIComponent _parent)
    {
        new UIText(_parent, false, string.Empty, "LANGUAGE", "KGSecondChances_Font", 0.065f, RelativeTo.ScreenHeight, "#7AF2FF");
    }

    public void CreateContent(UIComponent _parent)
    {
        UIVerticalList list = new UIVerticalList(_parent, string.Empty);
        list.SetWidth(1f, RelativeTo.ParentWidth);
        list.SetSpacing(0.03f, RelativeTo.ScreenHeight);
        list.SetMargins(0f, 0f, 0.035f, 0f);
        list.RemoveDrawHandler();
        int total = Loc.Names.Length;
        int rows = (total + 2) / 3;
        for (int i = 0; i < rows; i++)
        {
            UIHorizontalList row = new UIHorizontalList(list, string.Empty);
            row.SetSpacing(0.05f, RelativeTo.ScreenHeight);
            row.RemoveDrawHandler();
            int count = Math.Min(total - i * 3, 3);
            for (int j = 0; j < count; j++)
            {
                int lang = i * 3 + j;
                PsUIGenericButton b = new PsUIGenericButton(row);
                b.SetSpacing(0.015f, RelativeTo.ScreenHeight);
                b.SetText(Loc.Names[lang], 0.03f, 0.2f);
                b.SetHeight(0.08f, RelativeTo.ScreenHeight);
                if (lang == m_selected)
                {
                    b.SetBlueColors(false);
                    b.DisableTouchAreas();
                }
                m_buttons.Add(b);
            }
        }
        UICanvas box = new UICanvas(_parent, false, string.Empty, null, string.Empty);
        box.SetHeight(0.1f, RelativeTo.ScreenHeight);
        box.SetVerticalAlign(0f);
        box.SetWidth(0.5f, RelativeTo.ParentWidth);
        box.SetMargins(0f, 0f, 0.06f, -0.06f);
        box.RemoveDrawHandler();
        m_ok = new PsUIGenericButton(box);
        m_ok.SetGreenColors();
        m_ok.SetText("OK", 0.04f, 0.175f);
    }

    public override void Step()
    {
        bool changed = false;
        for (int i = 0; i < m_buttons.Count; i++)
        {
            if (m_buttons[i].m_hit)
            {
                changed = true;
                m_selected = i;
                break;
            }
        }
        if (changed)
        {
            for (int j = 0; j < m_buttons.Count; j++)
            {
                if (j == m_selected)
                {
                    m_buttons[j].SetBlueColors(false);
                    m_buttons[j].DisableTouchAreas();
                }
                else
                {
                    m_buttons[j].SetBlueColors();
                    m_buttons[j].EnableTouchAreas();
                }
                m_buttons[j].Update();
            }
        }
        if (m_ok.m_hit)
        {
            Loc.SetLanguage(m_selected);
            if (m_previous == m_selected)
            {
                (GetRoot() as PsUIBasePopup).Destroy();
            }
            else
            {
                // on relance la scene de demarrage : toute l'interface est recreee dans la nouvelle langue (sans quitter le jeu)
                m_previous = m_selected;
                Main.m_currentGame.m_sceneManager.ChangeScene(new StartupScene("StartupScene"), new FadeLoadingScene(Color.black));
                return;
            }
        }
        base.Step();
    }
}
