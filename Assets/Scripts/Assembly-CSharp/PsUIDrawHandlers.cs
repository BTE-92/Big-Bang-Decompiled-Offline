using System;
using UnityEngine;

public static class PsUIDrawHandlers
{
	public static void Test(TransformC tc)
	{
	}

	public static void UpgradeBarsBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth * 1.1f, _c.m_actualHeight * 1.1f, (float)Screen.width * 0.03f, 8, Vector2.zero);
		Vector2[] roundedRect2 = DebugDraw.GetRoundedRect(_c.m_actualWidth * 1.1f - (float)Screen.height * 0.02f, _c.m_actualHeight * 1.1f - (float)Screen.height * 0.02f, (float)Screen.width * 0.02f, 8, Vector2.zero);
		Vector2[] roundedRect3 = DebugDraw.GetRoundedRect(_c.m_actualWidth * 1.1f - (float)Screen.height * 0.02f, _c.m_actualHeight * 1.1f - (float)Screen.height * 0.02f, (float)Screen.width * 0.02f, 8, Vector2.zero);
		string hex = "#2b7db0";
		string text = "#86d9f9";
		string hex2 = "#41acee";
		string hex3 = text;
		string hex4 = "#0f689f";
		string hex5 = "#0a3c5a";
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 10f, roundedRect, DebugDraw.HexToUint(hex), DebugDraw.HexToUint(text), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 4f, roundedRect, (float)Screen.height * 0.005f, DebugDraw.HexToColor(hex), DebugDraw.HexToColor(text), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 6f + Vector3.down * 2f, roundedRect, (float)Screen.height * 0.02f, DebugDraw.HexToColor(hex5), DebugDraw.HexToColor(hex4), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 5f, roundedRect2, DebugDraw.HexToUint("#072a3e"), DebugDraw.HexToUint("#072a3e"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 4f, roundedRect3, (float)Screen.height * 0.0075f, DebugDraw.HexToColor(hex2), DebugDraw.HexToColor(hex3), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -3f, roundedRect3, (float)Screen.height * 0.025f, new Color(1f, 1f, 1f, 0.25f), ResourceManager.GetMaterial("Framework/Line8Gradient2Mat"), _c.m_camera, Position.Inside, true);
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_button_glare");
		SpriteC spriteC = SpriteS.AddComponent(_c.m_TC, frame, PsState.m_uiSheet);
		SpriteS.SetOffset(spriteC, new Vector3(0f, _c.m_actualHeight * 1.1f * 0.5f - (float)Screen.height * 0.055f, -5f), 0f);
		SpriteS.SetDimensions(spriteC, _c.m_actualWidth * 1.1f - (float)Screen.height * 0.065f, (float)Screen.height * 0.05f);
		SpriteS.SetColor(spriteC, new Color(1f, 1f, 1f, 0.7f));
		Frame frame2 = PsState.m_uiSheet.m_atlas.GetFrame("menu_button_shine");
		SpriteC spriteC2 = SpriteS.AddComponent(_c.m_TC, frame2, PsState.m_uiSheet);
		SpriteS.SetOffset(spriteC2, new Vector3(_c.m_actualWidth * 1.1f * -0.5f + (float)Screen.height * 0.055f, _c.m_actualHeight * 1.1f * 0.5f - (float)Screen.height * 0.05f, -6f), 25f);
		SpriteS.SetDimensions(spriteC2, (float)Screen.height * 0.03f, (float)Screen.height * 0.015f);
		SpriteC spriteC3 = SpriteS.AddComponent(_c.m_TC, frame2, PsState.m_uiSheet);
		SpriteS.SetOffset(spriteC3, new Vector3(_c.m_actualWidth * 1.1f * 0.5f - (float)Screen.height * 0.055f, _c.m_actualHeight * 1.1f * -0.5f + (float)Screen.height * 0.05f, -6f), 205f);
		SpriteS.SetDimensions(spriteC3, (float)Screen.height * 0.015f, (float)Screen.height * 0.0095f);
		SpriteS.SetColor(spriteC3, new Color(1f, 1f, 1f, 0.6f));
		SpriteS.ConvertSpritesToPrefabComponent(_c.m_TC, _c.m_camera, true);
	}

	public static void ScrollingUIHeader(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		string hex = "#3c8195";
		string hex2 = "#1d4964";
		Vector2[] roundedBezierRect = DebugDraw.GetRoundedBezierRect(_c.m_actualWidth - (float)Screen.height * 0.02f, _c.m_actualHeight - (float)Screen.height * 0.01f, (float)Screen.height * 0.045f, -1f, 0f, 0f, true, true, false, false);
		GGData polygon = new GGData(roundedBezierRect);
		PrefabS.CreateFlatPrefabComponentsFromPolygon(_c.m_TC, Vector3.up * -Screen.height * 0.01f + Vector3.forward * 6f, polygon, DebugDraw.HexToColor(hex2), DebugDraw.HexToColor(hex2), ResourceManager.GetMaterial(RESOURCE.MenuPopupBackgroundMat_Material), _c.m_camera);
		Vector2[] line = DebugDraw.GetLine(new Vector2(_c.m_actualWidth * -0.5f + (float)Screen.height * 0.01f, _c.m_actualHeight * -0.55f), new Vector2(_c.m_actualWidth * 0.5f - (float)Screen.height * 0.01f, _c.m_actualHeight * -0.55f), 0);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 0f, line, (float)Screen.height * 0.0075f, DebugDraw.HexToColor(hex), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, false);
	}

	public static void ScrollingUIBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedBezierRect = DebugDraw.GetRoundedBezierRect(_c.m_actualWidth, _c.m_actualHeight, (float)Screen.height * 0.05f, -1f, -1f);
		Vector2[] roundedBezierRect2 = DebugDraw.GetRoundedBezierRect(_c.m_actualWidth - (float)Screen.height * 0.02f, _c.m_actualHeight - (float)Screen.height * 0.02f, (float)Screen.height * 0.045f, -1f, -1f);
		string hex = "#217eb3";
		string hex2 = "#165882";
		string hex3 = "#86d9f9";
		string hex4 = "#1fb3e9";
		string hex5 = "#3c90be";
		string hex6 = "#114d71";
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 10f, roundedBezierRect, DebugDraw.HexToUint(hex2), DebugDraw.HexToUint(hex), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 4f, roundedBezierRect, (float)Screen.height * 0.0075f, DebugDraw.HexToColor(hex6), DebugDraw.HexToColor(hex5), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 7f + Vector3.down * 2f, roundedBezierRect, (float)Screen.height * 0.02f, DebugDraw.HexToColor("#0a3c5a"), DebugDraw.HexToColor("#0f689f"), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 8f, roundedBezierRect2, DebugDraw.HexToUint("#072a3e"), DebugDraw.HexToUint("#072a3e"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 0f, roundedBezierRect2, (float)Screen.height * 0.0075f, DebugDraw.HexToColor(hex4), DebugDraw.HexToColor(hex3), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 1f, roundedBezierRect2, (float)Screen.height * 0.025f, new Color(1f, 1f, 1f, 0.25f), ResourceManager.GetMaterial("Framework/Line8Gradient2Mat"), _c.m_camera, Position.Inside, true);
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_button_glare");
		SpriteC spriteC = SpriteS.AddComponent(_c.m_TC, frame, PsState.m_uiSheet);
		SpriteS.SetOffset(spriteC, new Vector3(0f, _c.m_actualHeight * 0.5f - (float)Screen.height * 0.0425f, -5f), 0f);
		SpriteS.SetDimensions(spriteC, _c.m_actualWidth - (float)Screen.height * 0.065f, (float)Screen.height * 0.05f);
		Frame frame2 = PsState.m_uiSheet.m_atlas.GetFrame("menu_button_shine");
		SpriteC spriteC2 = SpriteS.AddComponent(_c.m_TC, frame2, PsState.m_uiSheet);
		SpriteS.SetOffset(spriteC2, new Vector3(_c.m_actualWidth * -0.5f + (float)Screen.height * 0.055f, _c.m_actualHeight * 0.5f - (float)Screen.height * 0.035f, -6f), 25f);
		SpriteS.SetDimensions(spriteC2, (float)Screen.height * 0.03f, (float)Screen.height * 0.015f);
		SpriteC spriteC3 = SpriteS.AddComponent(_c.m_TC, frame2, PsState.m_uiSheet);
		SpriteS.SetOffset(spriteC3, new Vector3(_c.m_actualWidth * 0.5f - (float)Screen.height * 0.04f, _c.m_actualHeight * -0.5f + (float)Screen.height * 0.04f, -6f), 205f);
		SpriteS.SetDimensions(spriteC3, (float)Screen.height * 0.015f, (float)Screen.height * 0.0095f);
		SpriteS.SetColor(spriteC3, new Color(1f, 1f, 1f, 0.6f));
		SpriteS.ConvertSpritesToPrefabComponent(_c.m_TC, _c.m_camera, true);
	}

	public static void CoinLabelBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, 0.1f * _c.m_actualHeight, 8, Vector2.zero);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward, roundedRect, 0.005f * (float)Screen.height, DebugDraw.HexToColor("#fbdd28"), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 2f, roundedRect, DebugDraw.HexToUint("#FFFFFF"), DebugDraw.HexToUint("#FFFFFF"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}

	public static void BoltLabelBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, 0.1f * _c.m_actualHeight, 8, Vector2.zero);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward, roundedRect, 0.005f * (float)Screen.height, DebugDraw.HexToColor("#4dcfff"), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 2f, roundedRect, DebugDraw.HexToUint("#FFFFFF"), DebugDraw.HexToUint("#FFFFFF"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}

	public static void KeyLabelBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, 0.1f * _c.m_actualHeight, 8, Vector2.zero);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -2f, roundedRect, 0.005f * (float)Screen.height, DebugDraw.HexToColor("#416d87"), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * -1f, roundedRect, DebugDraw.HexToUint("#FFFFFF"), DebugDraw.HexToUint("#FFFFFF"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}

	public static void DiamondLabelBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, 0.1f * _c.m_actualHeight, 8, Vector2.zero);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward, roundedRect, 0.005f * (float)Screen.height, DebugDraw.HexToColor("#f15bfb"), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 2f, roundedRect, DebugDraw.HexToUint("#FFFFFF"), DebugDraw.HexToUint("#FFFFFF"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}

	public static void TicketLabelBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, 0.1f * _c.m_actualHeight, 8, Vector2.zero);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward, roundedRect, 0.005f * (float)Screen.height, DebugDraw.HexToColor("#5bd7fb"), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 2f, roundedRect, DebugDraw.HexToUint("#FFFFFF"), DebugDraw.HexToUint("#FFFFFF"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}

	public static void ResourceLabelBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_background");
		SpriteC spriteC = SpriteS.AddComponent(_c.m_TC, frame, PsState.m_uiSheet);
		SpriteS.SetOffset(spriteC, new Vector3(0f, 0f, 0f), 0f);
		SpriteS.SetDimensions(spriteC, _c.m_actualWidth, _c.m_actualHeight * 0.65f);
		SpriteS.ConvertSpritesToPrefabComponent(_c.m_TC, _c.m_camera, true);
	}

	public static void TransparentRoundedRectBackground(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, 0.1f * _c.m_actualHeight, 8, Vector2.zero);
		Color black = Color.black;
		black.a = 0.3f;
		uint num = DebugDraw.ColorToUInt(black);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * 2f, roundedRect, num, num, ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * 2f, roundedRect, 0.005f * (float)Screen.height, black, ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
	}

	public static void DebriefBackground(UIComponent _c)
	{
		float num = _c.m_actualWidth;
		float num2 = _c.m_actualHeight;
		UIScrollableCanvas uIScrollableCanvas = _c as UIScrollableCanvas;
		if (uIScrollableCanvas != null)
		{
			num = Mathf.Max(num, uIScrollableCanvas.m_contentWidth);
			num2 = Mathf.Max(num2, uIScrollableCanvas.m_contentHeight);
			num *= 1.5f;
			num2 *= 1.5f;
		}
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] rect = DebugDraw.GetRect(num, num2 * 0.5f, new Vector2(0f, num2 * 0.25f), false);
		Vector2[] rect2 = DebugDraw.GetRect(num, num2 * 0.5f, new Vector2(0f, (0f - num2) * 0.25f), false);
		uint topColor = DebugDraw.ColorToUInt(DebugDraw.GetColor(0f, 0f, 0f, 150f));
		uint num3 = DebugDraw.ColorToUInt(DebugDraw.GetColor(0f, 0f, 0f, 150f));
		uint bottomColor = DebugDraw.ColorToUInt(DebugDraw.GetColor(0f, 0f, 0f, 150f));
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_camera != null)
		{
			camera = _c.m_camera;
		}
		else if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * (_c.m_TC.transform.position.z + 10f), rect, num3, topColor, ResourceManager.GetMaterial(RESOURCE.MenuPopupBackgroundMat_Material), camera, string.Empty);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * (_c.m_TC.transform.position.z + 10f), rect2, bottomColor, num3, ResourceManager.GetMaterial(RESOURCE.MenuPopupBackgroundMat_Material), camera, string.Empty);
	}

	public static void DescriptionField(UIComponent _c)
	{
		float num = 0.078f;
		float num2 = 0.024f;
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, _c.m_actualWidth * num2, 8, Vector2.zero);
		Vector2[] array = new Vector2[roundedRect.Length + 2];
		Array.Copy(roundedRect, 0, array, 0, 16);
		array[16] = new Vector2(roundedRect[16].x, roundedRect[16].y - _c.m_actualWidth * num);
		array[17] = new Vector2(roundedRect[16].x - _c.m_actualWidth * num, roundedRect[16].y - _c.m_actualWidth * (num / 2f));
		Array.Copy(roundedRect, 16, array, 18, roundedRect.Length - 16);
		Color color = DebugDraw.HexToColor("#fffec6");
		uint num3 = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, new Vector3(_c.m_actualWidth * 0.004f, _c.m_actualWidth * -0.004f, 0.3f), array, _c.m_actualWidth * 0.03f, DebugDraw.HexToColor("#666666"), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, array, num3, num3, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -0.5f, array, 4f, color, ResourceManager.GetMaterial("Framework/Line4Mat"), camera, Position.Center, true);
	}

	public static void RatingSpeechBubble(UIComponent _c)
	{
		float num = 0.1f;
		float num2 = 0.024f;
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, _c.m_actualWidth * num2, 8, Vector2.zero);
		Vector2[] array = new Vector2[roundedRect.Length + 2];
		Array.Copy(roundedRect, 0, array, 0, 16);
		array[16] = new Vector2(roundedRect[16].x, roundedRect[16].y - _c.m_actualWidth * num);
		array[17] = new Vector2(roundedRect[16].x - _c.m_actualWidth * num, roundedRect[16].y - _c.m_actualWidth * (num / 2f));
		Array.Copy(roundedRect, 16, array, 18, roundedRect.Length - 16);
		Color color = DebugDraw.HexToColor("#ffffff");
		uint num3 = DebugDraw.ColorToUInt(color);
		Camera camera = CameraS.m_uiCamera;
		if (_c.m_parent != null)
		{
			camera = _c.m_parent.m_camera;
		}
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, new Vector3(_c.m_actualWidth * 0.004f, _c.m_actualWidth * -0.004f, 0.3f), array, _c.m_actualWidth * 0.05f, DebugDraw.HexToColor("#00b6ea"), ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, array, num3, num3, ResourceManager.GetMaterial("Framework/SolidMat"), camera, string.Empty);
	}

	public static void NotificationDrawhandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] circle = DebugDraw.GetCircle(_c.m_actualHeight / 2f, 16, Vector2.zero, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -1f, circle, (float)Screen.height * 0.0075f, Color.white, Color.white, ResourceManager.GetMaterial("Framework/Line4Mat"), _c.m_camera, Position.Center, true);
		GGData polygon = new GGData(circle);
		Color bottomColor = DebugDraw.HexToColor("#cc0909");
		Color topColor = DebugDraw.HexToColor("#FF0C0C");
		PrefabS.CreateFlatPrefabComponentsFromPolygon(_c.m_TC, Vector3.zero, polygon, bottomColor, topColor, ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera);
	}
}
