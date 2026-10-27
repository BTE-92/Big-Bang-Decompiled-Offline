using System.Collections.Generic;
using UnityEngine;

public class UITestState : BasicState
{
	public static int m_healthType = 1;

	public static string[] m_healthLabels = new string[3] { "No Health", "Hearths", "Health Bar" };

	public static string[] m_healthShortLabels = new string[3] { "None", "Hearths", "Bar" };

	public static Frame[] m_healthFrameLabels = new Frame[3]
	{
		new Frame(0f, 0f, 64f, 64f),
		new Frame(64f, 0f, 64f, 64f),
		new Frame(128f, 0f, 64f, 64f)
	};

	public static int m_healthAmount = 3;

	public static float m_speed = 0.5f;

	public static int m_controller = 0;

	public static string[] m_controllerLabels = new string[6] { "Player 1", "Player 2", "Player3", "Player4", "Computer", "Artificial Intelligence" };

	public static string[] m_controllerLabelsShort = new string[6] { "P1", "P2", "P3", "P4", "CPU", "AI" };

	public static bool m_CPU_hostile = true;

	public static string[] m_CPU_hostileLabels = new string[2] { "No", "Yes" };

	public static float m_CPU_skill = 0.35f;

	public static int m_PLAYER_controlMethod = 0;

	public static string[] m_controlMethodLabels = new string[5] { "Tilt", "1-Button", "2-Button", "Joystick & 1-Button", "Joystick & 2-Button" };

	public static string[] m_controlMethodLabelsShort = new string[5] { "Tilt", "1B", "2B", "J+1B", "J+2B" };

	public UIModel m_model;

	// the UI of the game exists before this test: only what the test creates is destroyed, and the shared canvas is put back
	private int m_uiStartCount;

	private UICanvas m_previousCanvas;

	// the property panel of this test hands its camera over to the shared UI camera of the game (it resizes it and shrinks its viewport),
	// and the camera it created stays alive: both are put back when the test is left
	private List<Camera> m_camerasBefore;

	private Rect m_uiCameraRect;

	private float m_uiCameraSize;

	private Vector3 m_uiCameraPosition;

	private SpriteSheet m_symbolSheet;

	public override void Enter(IStatedObject _parent)
	{
		m_model = new UIModel(this);
		m_camerasBefore = new List<Camera>(CameraS.m_cameras);
		m_uiCameraRect = CameraS.m_uiCamera.rect;
		m_uiCameraSize = CameraS.m_uiCamera.orthographicSize;
		m_uiCameraPosition = CameraS.m_uiCamera.transform.position;
		m_uiStartCount = UIManager.m_uiComponents.Count;
		m_previousCanvas = UIManager.m_canvas;
		UIManager.m_canvas = new UICanvas(null, false, "BaseCanvas", null, string.Empty);
		UIManager.m_canvas.SetMargins(0.01f);
		UIPanel uIPanel = new UIPanel(UIManager.m_canvas, "PropertyPanel", new UIComponent(null, false, string.Empty, null, null, string.Empty), new UIComponent(null, false, string.Empty, null, null, string.Empty));
		uIPanel.SetHeight(1f, RelativeTo.ParentHeight);
		uIPanel.SetWidth(0.382f, RelativeTo.ParentHeight);
		uIPanel.SetHorizontalAlign(1f);
		Debug.Log("propertyPanel " + uIPanel.m_TC.m_index);
		Debug.Log("propertyPanelContainer " + uIPanel.m_container.m_TC.m_index);
		m_symbolSheet = SpriteS.AddSpriteSheet(uIPanel.m_camera, ResourceManager.GetMaterial("Framework/SymbolMat"), 1f);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIPanel, "HorizontalArea");
		Debug.Log("horizontal " + uIHorizontalList.m_TC.m_index);
		uIHorizontalList.SetHorizontalAlign(0f);
		uIHorizontalList.SetVerticalAlign(1f);
		UIVerticalList uIVerticalList = new UIVerticalList(uIHorizontalList, "VerticalArea");
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.SetMargins(0f, 0f, 0.04f, 0.04f, RelativeTo.ParentShortest);
		uIVerticalList.SetSpacing(0.04f, RelativeTo.ParentShortest);
		UIManager.m_canvas.Update();
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		while (UIManager.m_uiComponents.Count > m_uiStartCount)
		{
			int index = UIManager.m_uiComponents.Count - 1;
			UIManager.m_uiComponents[index].Destroy(false);
			if (UIManager.m_uiComponents.Count > index)
			{
				UIManager.m_uiComponents.RemoveAt(index);
			}
		}
		SpriteS.RemoveSpriteSheet(m_symbolSheet);
		m_symbolSheet = null;
		UIManager.m_canvas = m_previousCanvas;
		m_previousCanvas = null;
		for (int num = CameraS.m_cameras.Count - 1; num >= 0; num--)
		{
			Camera camera = CameraS.m_cameras[num];
			if (camera != null && !m_camerasBefore.Contains(camera))
			{
				CameraS.RemoveCamera(camera);
			}
		}
		m_camerasBefore = null;
		CameraS.m_uiCamera.rect = m_uiCameraRect;
		CameraS.m_uiCamera.orthographicSize = m_uiCameraSize;
		CameraS.m_uiCamera.transform.position = m_uiCameraPosition;
	}
}
