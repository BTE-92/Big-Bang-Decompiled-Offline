using System.Collections.Generic;
using UnityEngine;

public class UIEditorDrawButtonsWindow : UICanvas
{
	private UIFittedSprite m_drawAddButton;

	private UIFittedSprite m_drawSubButton;

	private UIFittedSprite m_drawAddHighlight;

	private UIFittedSprite m_drawSubHighlight;

	private UIComponent m_addAreaContainer;

	private UIComponent m_subAreaContainer;

	private List<PrefabC> m_materialBGs;

	private UIText m_hintText;

	private Vector3 m_dragStartPosOffset;

	private bool m_dragging;

	private UIComponent m_addArea;

	private UIComponent m_subArea;

	private bool m_buttonsActive = true;

	private bool m_addPressed;

	private bool m_subPressed;

	private bool m_addReleased = true;

	private bool m_subReleased = true;

	public UIEditorDrawButtonsWindow(UIScrollableCanvas _parent, string _tag)
		: base(_parent, false, _tag, null, string.Empty)
	{
		SetWidth(0.5f, RelativeTo.ParentHeight);
		SetHeight(0.7f, RelativeTo.ParentHeight);
		SetAlign(PsState.m_drawButtonWindowPosition, 0f);
		SetDepthOffset(20f);
		RemoveDrawHandler();
		m_materialBGs = new List<PrefabC>();
		m_addAreaContainer = new UIComponent(this, false, "DrawButtonAdd", null, null, string.Empty);
		m_addAreaContainer.RemoveDrawHandler();
		m_addAreaContainer.SetSize(0.23f, 0.23f, RelativeTo.ScreenShortest);
		if (!PsState.m_editorIsLefty)
		{
			m_addAreaContainer.SetAlign(0f, 1f);
		}
		else
		{
			m_addAreaContainer.SetAlign(1f, 1f);
		}
		m_addArea = new UIComponent(m_addAreaContainer, false, "button", null, null, string.Empty);
		m_addArea.RemoveDrawHandler();
		m_addArea.SetSize(0.2f, 0.2f, RelativeTo.ScreenShortest);
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("editor_draw_sensory_button");
		Frame frame2 = PsState.m_uiSheet.m_atlas.GetFrame("editor_draw_sensory_button_plus");
		if (PsState.m_editorIsLefty)
		{
			frame.flipX = true;
			frame2.flipX = true;
		}
		m_drawAddButton = new UIFittedSprite(m_addArea, true, "rim", PsState.m_uiSheet, frame);
		UIFittedSprite uIFittedSprite = new UIFittedSprite(m_addArea, false, "plus", PsState.m_uiSheet, frame2);
		uIFittedSprite.SetSize(0.5f, 0.5f, RelativeTo.ParentHeight);
		m_subAreaContainer = new UIComponent(this, false, "DrawButtonSub", null, null, string.Empty);
		m_subAreaContainer.RemoveDrawHandler();
		m_subAreaContainer.SetSize(0.23f, 0.23f, RelativeTo.ScreenShortest);
		if (!PsState.m_editorIsLefty)
		{
			m_subAreaContainer.SetAlign(0.25f, 0.55f);
		}
		else
		{
			m_subAreaContainer.SetAlign(0.75f, 0.55f);
		}
		m_subArea = new UIComponent(m_subAreaContainer, false, "button", null, null, string.Empty);
		m_subArea.RemoveDrawHandler();
		m_subArea.SetSize(0.2f, 0.2f, RelativeTo.ScreenShortest);
		Frame frame3 = PsState.m_uiSheet.m_atlas.GetFrame("editor_draw_sensory_button");
		Frame frame4 = PsState.m_uiSheet.m_atlas.GetFrame("editor_draw_sensory_button_minus");
		if (PsState.m_editorIsLefty)
		{
			frame3.flipX = true;
			frame4.flipX = true;
		}
		m_drawSubButton = new UIFittedSprite(m_subArea, true, "rim", PsState.m_uiSheet, frame3);
		UIFittedSprite uIFittedSprite2 = new UIFittedSprite(m_subArea, false, "minus", PsState.m_uiSheet, frame4);
		uIFittedSprite2.SetSize(0.5f, 0.5f, RelativeTo.ParentHeight);
		Update();
		RegenerateDrawButtonBackgrounds();
	}

	private PrefabC CreateMaterialPrefab(TransformC _tc, Vector3 _offset, Camera _camera)
	{
		float num = 0.2f * (float)Mathf.Min(Screen.height, Screen.width);
		Vector2[] array = new Vector2[7]
		{
			new Vector2(0.275f * num, 0.9f * num),
			new Vector2(0.725f * num, 0.9f * num),
			new Vector2(0.925f * num, 0.5f * num),
			new Vector2(0.8f * num, 0.125f * num),
			new Vector2(0.5f * num, 0.05f * num),
			new Vector2(0.2f * num, 0.2f * num),
			new Vector2(0.075f * num, 0.6f * num)
		};
		if (PsState.m_editorIsLefty)
		{
			Vector2[] array2 = new Vector2[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i].x *= -1f;
				array2[array.Length - i - 1] = array[i];
			}
			array2.CopyTo(array, 0);
			array2 = null;
		}
		uint num2 = DebugDraw.ColorToUInt(Color.white);
		Material material = null;
		material = ((PsState.m_drawLayer == 0) ? ResourceManager.GetMaterial("SandButtonMat_Material") : ((PsState.m_drawLayer == 1) ? ResourceManager.GetMaterial("IceButtonMat_Material") : ((PsState.m_drawLayer == 2) ? ResourceManager.GetMaterial("MudButtonMat_Material") : ((PsState.m_drawLayer == 3) ? ResourceManager.GetMaterial("MetalButtonMat_Material") : ((PsState.m_drawLayer == 4) ? ResourceManager.GetMaterial("DangerButtonMat_Material") : ((PsState.m_drawLayer == 5) ? ResourceManager.GetMaterial("LavaButtonMat_Material") : ((PsState.m_drawLayer != 6) ? ResourceManager.GetMaterial("Framework/SolidMat") : ResourceManager.GetMaterial("ConcreteButtonMat_Material"))))))));
		UVRect normalizeRect = new UVRect(0f, 0f, 1f, 1f);
		return PrefabS.CreateFlatPrefabComponentsFromVectorArray(_tc, _offset, array, num2, num2, material, _camera, string.Empty, normalizeRect);
	}

	protected void MoveHandler(TouchAreaC _touchArea, int _touchCount, TLTouch[] _touches, TouchAreaPhase[] _touchPhases, bool[] _touchIsSecondary)
	{
		if (_touchPhases[0] == TouchAreaPhase.Began)
		{
			m_dragStartPosOffset = m_TC.transform.position - (Vector3)_touches[0].m_currentPosition;
			m_dragging = true;
		}
		else if (m_dragging && (_touchPhases[0] == TouchAreaPhase.MoveIn || _touchPhases[0] == TouchAreaPhase.MoveOut))
		{
			TransformS.SetPosition(m_TC, (Vector3)_touches[0].m_currentPosition + m_dragStartPosOffset);
		}
		else if (_touchPhases[0] == TouchAreaPhase.DragEnd)
		{
			m_dragging = true;
		}
	}

	public void RegenerateDrawButtonBackgrounds()
	{
		while (m_materialBGs.Count > 0)
		{
			int index = m_materialBGs.Count - 1;
			PrefabS.RemoveComponent(m_materialBGs[index]);
			m_materialBGs.RemoveAt(index);
		}
		if (PsState.m_editorIsLefty)
		{
			if (m_addPressed || (!m_addPressed && !m_subPressed))
			{
				m_materialBGs.Add(CreateMaterialPrefab(m_addArea.m_TC, new Vector3(m_addArea.m_actualWidth * 0.5f, m_addArea.m_actualHeight * -0.5f, 2f), m_addArea.m_camera));
			}
			if (m_subPressed || (!m_addPressed && !m_subPressed))
			{
				m_materialBGs.Add(CreateMaterialPrefab(m_subArea.m_TC, new Vector3(m_subArea.m_actualWidth * 0.5f, m_subArea.m_actualHeight * -0.5f, 2f), m_subArea.m_camera));
			}
		}
		else
		{
			if (m_addPressed || (!m_addPressed && !m_subPressed))
			{
				m_materialBGs.Add(CreateMaterialPrefab(m_addArea.m_TC, new Vector3(m_addArea.m_actualWidth * -0.5f, m_addArea.m_actualHeight * -0.5f, 2f), m_addArea.m_camera));
			}
			if (m_subPressed || (!m_addPressed && !m_subPressed))
			{
				m_materialBGs.Add(CreateMaterialPrefab(m_subArea.m_TC, new Vector3(m_subArea.m_actualWidth * -0.5f, m_subArea.m_actualHeight * -0.5f, 2f), m_subArea.m_camera));
			}
		}
	}

	public void SetDrawButtonsActivity(bool _active)
	{
		if (_active != m_buttonsActive)
		{
			EntityManager.SetActivityOfEntity(m_addArea.m_TC.p_entity, _active);
			EntityManager.SetActivityOfEntity(m_subArea.m_TC.p_entity, _active);
			m_buttonsActive = _active;
		}
	}

	public override void Step()
	{
		if (!m_buttonsActive)
		{
			return;
		}
		if (m_subReleased && (m_drawAddButton.m_began || Input.GetKeyDown(KeyCode.LeftShift)))
		{
			EditorBaseState.RemoveTransformGizmo();
			if (PsState.m_currentTool == EditorTool.None)
			{
				PsState.m_currentTool = EditorTool.Paint;
			}
			PsState.m_addDown = true;
			m_addPressed = true;
			m_addReleased = false;
			Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("editor_draw_sensory_button_bg");
			if (PsState.m_editorIsLefty)
			{
				frame.flipX = true;
			}
			if (m_drawAddHighlight != null)
			{
				m_drawAddHighlight.Destroy();
			}
			m_drawAddHighlight = new UIFittedSprite(m_addAreaContainer, true, "highlight", PsState.m_uiSheet, frame);
			m_drawAddHighlight.SetDepthOffset(5f);
			Update();
			EntityManager.SetActivityOfEntitiesWithTag(new string[6] { "DrawMenu", "MinigameMenu", "EditorMenu", "ObjectMenu", "DrawModeButton", "DrawButtonSub" }, false, true, true, true, true);
			if (m_hintText != null)
			{
				m_hintText.Destroy();
			}
			m_hintText = new UIText(null, false, string.Empty, "HOLD the button, and simultaneously\nuse another finger to <color=green>DRAW</color> ground", "HurmeRegular_Font", 0.04f, RelativeTo.ScreenShortest);
			m_hintText.SetAlign(0.5f, 1f);
			m_hintText.SetMargins(0.02f, RelativeTo.ScreenShortest);
			m_hintText.Update();
			SoundS.PlaySingleShot("/UI/ButtonDraw", Vector3.zero);
		}
		else if (m_addPressed && (m_drawAddButton.m_end || Input.GetKeyUp(KeyCode.LeftShift)))
		{
			if (PsState.m_currentTool == EditorTool.Paint)
			{
				PsState.m_currentTool = EditorTool.None;
			}
			PsState.m_addDown = false;
			m_addReleased = true;
			m_addPressed = false;
			m_drawAddHighlight.Destroy();
			m_drawAddHighlight = null;
			Update();
			EntityManager.SetActivityOfEntitiesWithTag(new string[6] { "DrawMenu", "MinigameMenu", "EditorMenu", "ObjectMenu", "DrawModeButton", "DrawButtonSub" }, true, true, true, true, true);
			if (m_hintText != null)
			{
				m_hintText.Destroy();
				m_hintText = null;
			}
		}
		if (m_addReleased && (m_drawSubButton.m_began || Input.GetKeyDown(KeyCode.LeftAlt)))
		{
			EditorBaseState.RemoveTransformGizmo();
			if (PsState.m_currentTool == EditorTool.None)
			{
				PsState.m_currentTool = EditorTool.Paint;
			}
			PsState.m_subDown = true;
			m_subPressed = true;
			m_subReleased = false;
			Frame frame2 = PsState.m_uiSheet.m_atlas.GetFrame("editor_draw_sensory_button_bg");
			if (PsState.m_editorIsLefty)
			{
				frame2.flipX = true;
			}
			if (m_drawSubHighlight != null)
			{
				m_drawSubHighlight.Destroy();
			}
			m_drawSubHighlight = new UIFittedSprite(m_subAreaContainer, true, "highlight", PsState.m_uiSheet, frame2);
			m_drawSubHighlight.SetDepthOffset(5f);
			Update();
			EntityManager.SetActivityOfEntitiesWithTag(new string[6] { "DrawMenu", "MinigameMenu", "EditorMenu", "ObjectMenu", "DrawModeButton", "DrawButtonAdd" }, false, true, true, true, true);
			if (m_hintText != null)
			{
				m_hintText.Destroy();
			}
			m_hintText = new UIText(null, false, string.Empty, "HOLD the button, and simultaneously\nuse another finger to <color=red>ERASE</color> ground", "HurmeRegular_Font", 0.04f, RelativeTo.ScreenShortest);
			m_hintText.SetAlign(0.5f, 1f);
			m_hintText.SetMargins(0.02f, RelativeTo.ScreenShortest);
			m_hintText.Update();
			SoundS.PlaySingleShot("/UI/ButtonErase", Vector3.zero);
		}
		else if (m_subPressed && (m_drawSubButton.m_end || Input.GetKeyUp(KeyCode.LeftAlt)))
		{
			if (PsState.m_currentTool == EditorTool.Paint)
			{
				PsState.m_currentTool = EditorTool.None;
			}
			PsState.m_subDown = false;
			m_subPressed = false;
			m_subReleased = true;
			m_drawSubHighlight.Destroy();
			m_drawSubHighlight = null;
			Update();
			EntityManager.SetActivityOfEntitiesWithTag(new string[6] { "DrawMenu", "MinigameMenu", "EditorMenu", "ObjectMenu", "DrawModeButton", "DrawButtonAdd" }, true, true, true, true, true);
			if (m_hintText != null)
			{
				m_hintText.Destroy();
				m_hintText = null;
			}
		}
		base.Step();
	}
}
