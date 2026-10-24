using System;
using System.Collections.Generic;
using UnityEngine;

// Hat chooser: a "HAT" button above the money buttons of the main menu opens a window showing a picture of every head prop
// of the game. The choice is kept in PlayerPrefs and used by AlienCharacter.SetRandomHat() for every rider (race, editor test).
public static class HatChoice
{
    public const string PrefKey = "bbr_hat";

    // index of the choice -> label (English; translated through Loc). 0 = random, 1..9 = the hats (0..8 in SetRandomHat)
    public static readonly string[] Names = new string[]
    {
        "Random", "Helmet", "Baseball cap", "Barbarian helmet", "Cowboy hat", "Horse mask", "Chicken hat", "Mushroom hat", "Paper bag", "Death mask"
    };

    public const int Random = 0;

    private static int s_current = -1;
    private static Texture2D[] s_icons;

    public static int Current
    {
        get
        {
            if (s_current < 0)
            {
                try { s_current = PlayerPrefs.GetInt(PrefKey, Random); } catch (Exception) { s_current = Random; }
                if (s_current < 0 || s_current >= Names.Length) s_current = Random;
            }
            return s_current;
        }
    }

    public static void Set(int index)
    {
        if (index < 0 || index >= Names.Length) index = Random;
        s_current = index;
        try
        {
            PlayerPrefs.SetInt(PrefKey, index);
            PlayerPrefs.Save();
        }
        catch (Exception) { }
    }

    // pictures rendered from the hat models (Assets/Resources/BBR/hats/hat_<index>.png)
    public static Texture2D Icon(int index)
    {
        if (s_icons == null) s_icons = new Texture2D[Names.Length];
        if (index < 0 || index >= s_icons.Length) return null;
        if (s_icons[index] == null) s_icons[index] = Resources.Load<Texture2D>("BBR/hats/hat_" + index);
        return s_icons[index];
    }
}

// Frame drawn over a tile: thin blue frame, or thick green frame with a tick badge when the hat is the chosen one.
// It is not touchable and only its own picture is redrawn when the choice changes, so the tiles' touch areas are never disturbed.
public class HatMark : UIComponent
{
    public bool m_selected;

    public HatMark(UIComponent _parent, bool _selected)
        : base(_parent, false, "HatMark", null, null, string.Empty)
    {
        m_selected = _selected;
        SetWidth(1f, RelativeTo.ParentWidth);
        SetHeight(1f, RelativeTo.ParentHeight);
        SetMargins(0f);
    }

    public void SetSelected(bool _selected)
    {
        m_selected = _selected;
        if (!m_hidden && m_actualWidth > 0f)
        {
            DrawHandler(this);
        }
    }

    public override void DrawHandler(UIComponent _c)
    {
        PrefabS.RemoveComponentsByEntity(m_TC.p_entity, false);
        float h = (float)Screen.height;
        float w = m_actualWidth;
        float ht = m_actualHeight;
        float corner = 0.022f * h;
        Vector2[] rect = DebugDraw.GetRoundedRect(w - 0.012f * h, ht - 0.012f * h, corner, 6, Vector2.zero);
        if (m_selected)
        {
            Color green = DebugDraw.HexToColor("#59e24a");
            PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward * -1f, rect, 0.016f * h, green, green, ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true);
            // round badge with a tick, top right corner
            Vector2 c = new Vector2(w * 0.5f - 0.02f * h, ht * 0.5f - 0.02f * h);
            float r = 0.034f * h;
            Vector2[] circle = DebugDraw.GetCircle(r, 20, c);
            uint fill = DebugDraw.HexToUint("#2fb52a");
            PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.forward * -2f, circle, fill, fill, ResourceManager.GetMaterial("Framework/SolidMat"), m_camera, string.Empty);
            PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward * -3f, circle, 0.006f * h, Color.white, ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true);
            Vector2[] tick = new Vector2[]
            {
                c + new Vector2(-0.45f, 0.02f) * r,
                c + new Vector2(-0.1f, -0.32f) * r,
                c + new Vector2(0.5f, 0.38f) * r
            };
            PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward * -4f, tick, 0.011f * h, Color.white, ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, false);
        }
        else
        {
            Color blue = DebugDraw.HexToColor("#86d9f9");
            PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward * -1f, rect, 0.008f * h, blue, blue, ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true);
        }
    }
}

// One square tile of the chooser: the picture of a hat in a rounded frame. Drawn once; the HatMark child shows the selection.
public class HatTile : UIComponent
{
    public HatMark m_mark;
    private Material m_material;

    public HatTile(UIComponent _parent, Texture2D _icon, bool _selected)
        : base(_parent, true, "HatTile", null, null, string.Empty)
    {
        m_material = UnityEngine.Object.Instantiate(ResourceManager.GetMaterial(RESOURCE.ScreenshotMat_Material));
        if (_icon != null) m_material.mainTexture = _icon;
        SetListenCameraEvents(true);
        m_mark = new HatMark(this, _selected);
    }

    private TweenC m_popTween;

    // small "pop" when the hat gets selected: the tile shrinks a little and springs back with a bounce
    public void Pop()
    {
        try
        {
            if (m_popTween != null)
            {
                TweenS.RemoveComponent(m_popTween);
                m_popTween = null;
            }
            m_popTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.BackOut, new Vector3(0.8f, 0.8f, 1f), Vector3.one, 0.32f, 0f, true);
        }
        catch (Exception e)
        {
            Debug.Log("HatTile.Pop: " + e.Message);
        }
    }

    public override void Destroy()
    {
        PrefabS.RemoveComponentsByEntity(m_TC.p_entity, false);
        if (m_material != null)
        {
            UnityEngine.Object.Destroy(m_material);
            m_material = null;
        }
        base.Destroy();
    }

    public override void DrawHandler(UIComponent _c)
    {
        // false: keep the picture's material alive between two drawings (it is destroyed in Destroy)
        PrefabS.RemoveComponentsByEntity(m_TC.p_entity, false);
        float h = (float)Screen.height;
        Vector2[] rect = DebugDraw.GetRoundedRect(_c.m_actualWidth - 0.012f * h, _c.m_actualHeight - 0.012f * h, 0.022f * h, 6, Vector2.zero);
        UVRect uv = new UVRect(0f, 0f, 1f, 1f);
        PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.forward, rect, uint.MaxValue, uint.MaxValue, m_material, m_camera, string.Empty, uv);
    }
}

public class HatSelect : PsUIHeaderedCanvas
{
    private List<HatTile> m_tiles = new List<HatTile>();
    private PsUIGenericButton m_ok;
    private int m_selected;
    private const int PerRow = 5;

    public HatSelect(UIComponent _parent)
        : base(_parent, string.Empty, false)
    {
        SetWidth(0.8f, RelativeTo.ScreenWidth);
        SetHeight(0.78f, RelativeTo.ScreenHeight);
        SetVerticalAlign(0.4f);
        SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
        (GetRoot() as PsUIBasePopup).m_scrollableCanvas.SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
        SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
        m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
        m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
        m_selected = HatChoice.Current;
        new UIText(m_header, false, string.Empty, "CHOOSE YOUR HAT", "KGSecondChances_Font", 0.065f, RelativeTo.ScreenHeight, "#7AF2FF");
        CreateContent(this);
    }

    public void CreateContent(UIComponent _parent)
    {
        UIVerticalList list = new UIVerticalList(_parent, string.Empty);
        list.SetWidth(1f, RelativeTo.ParentWidth);
        list.SetSpacing(0.03f, RelativeTo.ScreenHeight);
        list.SetMargins(0f, 0f, 0.035f, 0f);
        list.RemoveDrawHandler();
        int total = HatChoice.Names.Length;
        int rows = (total + PerRow - 1) / PerRow;
        for (int i = 0; i < rows; i++)
        {
            UIHorizontalList row = new UIHorizontalList(list, string.Empty);
            row.SetSpacing(0.03f, RelativeTo.ScreenHeight);
            row.RemoveDrawHandler();
            int count = Math.Min(total - i * PerRow, PerRow);
            for (int j = 0; j < count; j++)
            {
                int index = i * PerRow + j;
                HatTile tile = new HatTile(row, HatChoice.Icon(index), index == m_selected);
                tile.SetSize(0.19f, 0.19f, RelativeTo.ScreenHeight);
                m_tiles.Add(tile);
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
        for (int i = 0; i < m_tiles.Count; i++)
        {
            if (m_tiles[i].m_hit)
            {
                m_selected = i;
                HatChoice.Set(i);
                SoundS.PlaySingleShot("/UI/ButtonNormal", Vector3.zero);
                m_tiles[i].Pop();
                for (int j = 0; j < m_tiles.Count; j++)
                {
                    m_tiles[j].m_mark.SetSelected(j == m_selected);
                }
                break;
            }
        }
        if (m_ok.m_hit || (m_exitButton != null && m_exitButton.m_hit))
        {
            HatChoice.Set(m_selected);
            (GetRoot() as PsUIBasePopup).Destroy();
            return;
        }
        base.Step();
    }
}
