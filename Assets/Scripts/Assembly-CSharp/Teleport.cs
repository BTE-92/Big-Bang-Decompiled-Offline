using System;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : TeleportBase
{
	public static bool m_forceRefresh = false;

	private List<Material> m_materials = new List<Material>();

	public int m_index;

	private static List<Color32> m_colors;

	private static List<Color32> m_spiralColors;

	private static List<Color32> m_backgroundColors;

	private static List<int> m_reservedIndices = new List<int>();

	private static List<Teleport> m_teleportObjects = new List<Teleport>();

	protected List<Transform> m_transforms = new List<Transform>();

	private float m_length;

	private PrefabC m_linePc;

	private Camera m_camera1;

	private Camera m_camera2;

	private RenderTexture m_renderTexture1;

	private RenderTexture m_renderTexture2;

	private static int CAMERA_FPS;

	private int m_camera1Ticks;

	private int m_camera2Ticks;

	private bool m_portal1Visible;

	private bool m_portal2Visible;

	private int m_initialRenderTimer;

	public Teleport(GraphElement _graphElement)
		: base(_graphElement, 50f, "TeleportPrefab", 60f)
	{
		m_length = 0f;
		if (m_colors == null)
		{
			InitializeColors();
		}
		UpdateIndices();
		bool flag = ColorExists(m_graphElement.m_storedRotation);
		if (m_graphElement.m_storedRotation == Vector3.zero || !flag)
		{
			m_index = 0;
			while (m_reservedIndices.Contains(m_index))
			{
				m_index++;
			}
			m_graphElement.m_storedRotation = new Vector3((int)m_colors[m_index].r, (int)m_colors[m_index].g, (int)m_colors[m_index].b);
		}
		else
		{
			m_index = GetIndex(m_graphElement.m_storedRotation);
		}
		m_reservedIndices.Add(m_index);
		m_teleportObjects.Add(this);
		AddProperties(m_portal1.p_gameObject, m_portal2.p_gameObject);
		CreateConnection();
		SetCameras();
		m_initialRenderTimer = 2;
	}

	public void SetCameras()
	{
		GameObject gameObject = new GameObject("TeleportCamera1");
		GameObject gameObject2 = new GameObject("TeleportCamera2");
		gameObject.transform.parent = m_portal1.p_gameObject.transform;
		gameObject2.transform.parent = m_portal2.p_gameObject.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, -750f - m_depth);
		gameObject2.transform.localPosition = new Vector3(0f, 0f, -750f - m_depth);
		m_camera1 = gameObject.AddComponent<Camera>();
		m_camera2 = gameObject2.AddComponent<Camera>();
		int cullingMask = 768;
		m_camera1.cullingMask = cullingMask;
		m_camera2.cullingMask = cullingMask;
		m_camera1.enabled = false;
		m_camera2.enabled = false;
		m_renderTexture1 = new RenderTexture(256, 256, 24, ToolBox.GetSupportedRenderTextureFormat(RenderTextureFormat.RGB565));
		m_renderTexture2 = new RenderTexture(256, 256, 24, ToolBox.GetSupportedRenderTextureFormat(RenderTextureFormat.RGB565));
		m_camera1.targetTexture = m_renderTexture1;
		m_camera1.gameObject.layer = CameraS.m_mainCameraLayer;
		m_camera1.fieldOfView = CameraS.m_mainCameraFov;
		m_camera1.farClipPlane = CameraS.m_mainCameraFarClip;
		m_camera1.nearClipPlane = CameraS.m_mainCameraNearClip;
		m_camera1.fieldOfView = CameraS.m_mainCameraFov;
		m_camera1.depth = -5f;
		m_camera1.backgroundColor = Color.black;
		m_camera2.targetTexture = m_renderTexture2;
		m_camera2.gameObject.layer = CameraS.m_mainCameraLayer;
		m_camera2.fieldOfView = CameraS.m_mainCameraFov;
		m_camera2.farClipPlane = CameraS.m_mainCameraFarClip;
		m_camera2.nearClipPlane = CameraS.m_mainCameraNearClip;
		m_camera2.fieldOfView = CameraS.m_mainCameraFov;
		m_camera2.depth = -5f;
		m_camera2.backgroundColor = Color.black;
		m_camera1.Render();
		m_camera2.Render();
		m_materials[0].mainTexture = m_camera2.targetTexture;
		m_materials[1].mainTexture = m_camera1.targetTexture;
		m_camera1Ticks = m_reservedIndices.IndexOf(m_index) * 4;
		m_camera2Ticks = m_camera1Ticks + 2;
	}

	public bool ColorExists(Vector3 _storedColor)
	{
		bool result = false;
		for (int i = 0; i < m_colors.Count; i++)
		{
			Vector3 vector = new Vector3((int)m_colors[i].r, (int)m_colors[i].g, (int)m_colors[i].b);
			if (_storedColor == vector)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public int GetIndex(Vector3 _storedColor)
	{
		int result = 0;
		for (int i = 0; i < m_colors.Count; i++)
		{
			Vector3 vector = new Vector3((int)m_colors[i].r, (int)m_colors[i].g, (int)m_colors[i].b);
			if (_storedColor == vector)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public void InitializeColors()
	{
		m_colors = new List<Color32>();
		m_colors.Add(new Color32(byte.MaxValue, 90, 0, 240));
		m_colors.Add(new Color32(30, 90, 190, 240));
		m_colors.Add(new Color32(50, 110, 10, 240));
		m_colors.Add(new Color32(130, 60, 45, 240));
		m_colors.Add(new Color32(110, 50, 110, 240));
		m_backgroundColors = new List<Color32>();
		m_backgroundColors.Add(new Color32(246, 232, 121, 149));
		m_backgroundColors.Add(new Color32(180, 180, 180, 149));
		m_backgroundColors.Add(new Color32(180, 180, 180, 149));
		m_backgroundColors.Add(new Color32(180, 180, 180, 149));
		m_backgroundColors.Add(new Color32(180, 180, 180, 149));
		m_spiralColors = new List<Color32>();
		m_spiralColors.Add(new Color32(byte.MaxValue, 203, 92, 22));
		m_spiralColors.Add(new Color32(180, 180, 180, 22));
		m_spiralColors.Add(new Color32(180, 180, 180, 22));
		m_spiralColors.Add(new Color32(180, 180, 180, 22));
		m_spiralColors.Add(new Color32(180, 180, 180, 22));
	}

	public void UpdateIndices()
	{
		if (m_reservedIndices == null)
		{
			m_reservedIndices = new List<int>();
		}
		else
		{
			m_reservedIndices.Clear();
		}
		for (int i = 0; i < PsS.m_units.m_aliveCount; i++)
		{
			UnitC unitC = PsS.m_units.m_array[PsS.m_units.m_aliveIndices[i]];
			Teleport teleport = unitC.m_unit as Teleport;
			if (teleport != null && teleport != this)
			{
				m_reservedIndices.Add(teleport.m_index);
			}
		}
	}

	public Vector2[] GetSegmentChainPoints(Vector2 _start, Vector2 _end, float _segmentLength)
	{
		Vector2 vector = _end - _start;
		float magnitude = vector.magnitude;
		int num = Mathf.CeilToInt(magnitude / _segmentLength);
		float num2 = magnitude / (float)num;
		Vector2[] array = new Vector2[num + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = _start + vector.normalized * num2 * i - (Vector2)m_node1.m_position;
		}
		return array;
	}

	public void MapUVs(GGData _data, float _x)
	{
		float num = 0f;
		for (int i = 0; i < _data.m_vertices.Count; i++)
		{
			_data.m_vertices[i].uv = new Vector2(_x, num);
			num += 0.1f;
		}
	}

	public void CreateConnection()
	{
		Vector3 normalized = (m_node2.m_position - m_node1.m_position).normalized;
		Vector3 vector = m_node1.m_position + normalized * m_radius;
		Vector3 vector2 = m_node2.m_position - normalized * m_radius;
		if ((vector2 - vector).magnitude - m_length > 0.1f)
		{
			Vector2[] segmentChainPoints = GetSegmentChainPoints(vector, vector2, 10f);
			if (segmentChainPoints.Length > 3)
			{
				GGData gGData = new GGData(segmentChainPoints);
				gGData.Expand(5f);
				GGData gGData2 = gGData.Copy();
				gGData2.Expand(-10f);
				MapUVs(gGData2, 0f);
				MapUVs(gGData, 0.5f);
				if (m_linePc != null)
				{
					PrefabS.RemoveComponent(m_linePc);
				}
				Mesh mesh = GeometryGenerator.GenerateBeltMesh(0f, gGData, gGData2, false);
				m_linePc = PrefabS.CreatePrefabFromMesh(m_node1TC, mesh, CameraS.m_mainCamera, m_materials[m_materials.Count - 1], true, true);
			}
			else if (m_linePc != null)
			{
				PrefabS.RemoveComponent(m_linePc);
				m_linePc = null;
			}
		}
		m_materials[m_materials.Count - 1].SetColor("_TintColor", m_spiralColors[m_index]);
	}

	public void AddProperties(GameObject _portal1, GameObject _portal2)
	{
		m_materials.Add(_portal1.transform.Find("TeleportBackground").GetComponent<Renderer>().material);
		m_materials.Add(_portal2.transform.Find("TeleportBackground").GetComponent<Renderer>().material);
		m_materials.Add(_portal1.transform.Find("TeleportColor").GetComponent<Renderer>().material);
		m_materials.Add(_portal1.transform.Find("TeleportOutline").GetComponent<Renderer>().material);
		m_materials.Add(ResourceManager.GetMaterial(RESOURCE.TeleportLine_Material));
		_portal2.transform.Find("TeleportColor").GetComponent<Renderer>().sharedMaterial = m_materials[2];
		_portal2.transform.Find("TeleportOutline").GetComponent<Renderer>().sharedMaterial = m_materials[3];
		m_materials[2].color = m_colors[m_index];
		m_materials[3].SetColor("_TintColor", m_spiralColors[m_index]);
		_portal1.transform.Find("TeleportBackground").localRotation = Quaternion.Euler(new Vector3(0f, 0f, -15f));
		_portal2.transform.Find("TeleportBackground").localRotation = Quaternion.Euler(new Vector3(0f, 0f, -15f));
		m_transforms.Add(_portal1.transform.Find("TeleportOutline"));
		m_transforms.Add(_portal2.transform.Find("TeleportOutline"));
	}

	public override void Destroy()
	{
		if (m_linePc != null)
		{
			m_linePc = null;
		}
		for (int i = 0; i < m_materials.Count - 1; i++)
		{
			UnityEngine.Object.Destroy(m_materials[i]);
		}
		UnityEngine.Object.Destroy(m_renderTexture1);
		UnityEngine.Object.Destroy(m_renderTexture2);
		UnityEngine.Object.Destroy(m_camera1);
		UnityEngine.Object.Destroy(m_camera2);
		m_reservedIndices.Remove(m_index);
		m_teleportObjects.Remove(this);
		base.Destroy();
	}

	public void UpdateMaterials()
	{
		for (int i = 2; i < m_materials.Count; i++)
		{
			if (i < m_materials.Count - 1 || (i >= m_materials.Count - 1 && m_index == m_reservedIndices[0]))
			{
				m_materials[i].mainTextureOffset += Vector2.up * 0.01f;
				if (m_materials[i].mainTextureOffset.y > 1f)
				{
					m_materials[i].mainTextureOffset = new Vector2(m_materials[i].mainTextureOffset.x, 0f);
				}
			}
		}
	}

	public override void Update()
	{
		base.Update();
		m_portal1Visible = IsVisible(this, 1);
		m_portal2Visible = IsVisible(this, 2);
		if (m_portal1Visible || m_portal2Visible)
		{
			float num = 1f;
			for (int i = 0; i < m_transforms.Count; i++)
			{
				if (i % 2 == 0 && i != 0)
				{
					num += 0.5f;
				}
				m_transforms[i].rotation *= Quaternion.Euler(-Vector3.forward * 1f * num);
			}
			UpdateMaterials();
		}
		if (m_minigame.m_editing && m_graphElement.m_selected)
		{
			CreateConnection();
		}
		else if (m_linePc != null)
		{
			PrefabS.RemoveComponent(m_linePc);
			m_linePc = null;
		}
		if (PsState.m_performanceClass == PerformanceClass.FAST)
		{
			GetCameraFPS();
			RenderCams();
			return;
		}
		if (m_initialRenderTimer > 0)
		{
			m_initialRenderTimer--;
		}
		if (m_initialRenderTimer == 0 || m_forceRefresh)
		{
			m_camera1.Render();
			m_camera2.Render();
			m_initialRenderTimer = -1;
			m_forceRefresh = false;
		}
	}

	public void RenderCams()
	{
		if (CAMERA_FPS <= 0)
		{
			return;
		}
		m_camera1Ticks++;
		m_camera2Ticks++;
		if (m_camera1Ticks >= 60 / CAMERA_FPS)
		{
			if (m_portal2Visible)
			{
				m_camera1Ticks = 0;
				m_camera1.Render();
			}
			else
			{
				m_camera1Ticks = 60;
			}
		}
		if (m_camera2Ticks >= 60 / CAMERA_FPS)
		{
			if (m_portal1Visible)
			{
				m_camera2Ticks = 0;
				m_camera2.Render();
			}
			else
			{
				m_camera2Ticks = 60;
			}
		}
	}

	public bool IsVisible(Teleport _teleport, int _portalNumber)
	{
		int[] array = m_renderIndices1;
		if (_portalNumber == 2)
		{
			array = m_renderIndices2;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (_teleport.m_renderers[array[i]].isVisible)
			{
				return true;
			}
		}
		return false;
	}

	public void GetCameraFPS()
	{
		int num = 0;
		for (int i = 0; i < m_teleportObjects.Count; i++)
		{
			if (m_teleportObjects[i] != null)
			{
				if (m_teleportObjects[i].m_portal1Visible)
				{
					num++;
				}
				if (m_teleportObjects[i].m_portal2Visible)
				{
					num++;
				}
			}
		}
		if (num > 0)
		{
			CAMERA_FPS = 30 / num;
		}
		else
		{
			CAMERA_FPS = 0;
		}
	}

	public override void TeleportCollisionHandler(ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
		ChipmunkBodyC chipmunkBodyC = ChipmunkProS.m_bodies.m_array[_pair.ucpComponentIndexB];
		UnitC unitC = chipmunkBodyC.customComponent as UnitC;
		ucpBodyType ucpBodyType2 = ChipmunkProWrapper.ucpBodyGetType(chipmunkBodyC.body);
		if (unitC == null || unitC.m_unit == null || m_node1.m_TC == null || m_node2.m_TC == null || ucpBodyType2 != ucpBodyType.DYNAMIC)
		{
			return;
		}
		if (_phase == ucpCollisionPhase.Begin && !unitC.m_unit.m_teleported)
		{
			if (_pair.shapeA == m_node1Shape)
			{
				unitC.m_unit.SetAsTeleporting(15, m_node1.m_TC.transform, m_node2.m_TC.transform);
			}
			else if (_pair.shapeA == m_node2Shape)
			{
				unitC.m_unit.SetAsTeleporting(15, m_node2.m_TC.transform, m_node1.m_TC.transform);
			}
		}
		else
		{
			if (_phase != ucpCollisionPhase.Separate || !unitC.m_unit.m_teleported)
			{
				return;
			}
			ucpPointQueryInfo[] array = new ucpPointQueryInfo[20];
			bool flag = false;
			if ((_pair.shapeA == m_node1Shape && unitC.m_unit.m_teleportStartPos == m_node2.m_TC.transform.position) || (_pair.shapeA == m_node2Shape && unitC.m_unit.m_teleportStartPos == m_node1.m_TC.transform.position))
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			bool flag2 = true;
			ChipmunkProWrapper.ucpSpaceShapeQuery(_pair.shapeA, array, 20);
			List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, unitC.p_entity);
			for (int i = 0; i < componentsByEntity.Count; i++)
			{
				bool flag3 = false;
				ChipmunkBodyC chipmunkBodyC2 = componentsByEntity[i] as ChipmunkBodyC;
				for (int j = 0; j < array.Length && !(array[j].shape == IntPtr.Zero); j++)
				{
					IntPtr intPtr = ChipmunkProWrapper.ucpShapeGetBody(array[j].shape);
					if (intPtr == IntPtr.Zero)
					{
						break;
					}
					if (chipmunkBodyC2.body == intPtr)
					{
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				unitC.m_unit.m_teleported = false;
			}
		}
	}
}
