using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoGeometryProps
{
	public class AutoGeometryProp
	{
		public AGPropSettings m_settings;

		public GameObject m_go;

		public IntPtr m_cpShape;

		public float m_radius;

		public AutoGeometryProp()
		{
			m_go = null;
			m_cpShape = IntPtr.Zero;
			m_radius = 0f;
		}
	}

	public List<AutoGeometryProp> m_props;

	public AgTileC m_tileC;

	public ChipmunkBodyC m_staticBody;

	public AGPropStorage m_propStorage;

	public AutoGeometryProps(AgTile _tile)
	{
		m_tileC = new AgTileC();
		m_tileC.m_tile = _tile;
		m_props = new List<AutoGeometryProp>();
		m_staticBody = ChipmunkProS.AddStaticBody(m_tileC.m_tile.TC, m_tileC);
		m_propStorage = _tile.agLayer.m_groundPropStorage;
	}

	public void GenerateRandomPropsForPolys(AgPolygon[] _polys)
	{
		foreach (AgPolygon agPolygon in _polys)
		{
			if (agPolygon.vertices.Count <= 0 || agPolygon.extraData.Count > 0)
			{
			}
		}
	}

	public bool PointInsideGround(Vector3 _pos)
	{
		byte b = AutoGeometryManager.ReadMaxLayerValueFromWorldPos(_pos).b;
		return b >= 64 || m_tileC.m_tile.agLayer.ReadDataFromWorldPos(_pos) >= 64;
	}

	public void AddTestProp(Vector3 _pos, Vector3 _normal, int _propIndex)
	{
		AutoGeometryProp autoGeometryProp = new AutoGeometryProp();
		autoGeometryProp.m_settings = m_propStorage.m_propSettingsLookup[_propIndex];
		autoGeometryProp.m_go = UnityEngine.Object.Instantiate(autoGeometryProp.m_settings.gameObject);
		autoGeometryProp.m_go.transform.position = _pos + new Vector3(0f, 0f, -90f);
		autoGeometryProp.m_go.transform.rotation = Quaternion.LookRotation(_normal);
		autoGeometryProp.m_radius = autoGeometryProp.m_settings.m_radius;
		Vector2 offset = ChipmunkProWrapper.ucpBodyWorld2Local(m_staticBody.body, _pos);
		IntPtr intPtr = ChipmunkProWrapper.ucpCircleShapeNew(m_staticBody.body, autoGeometryProp.m_radius, offset, ucpCollisionType.None);
		ChipmunkProWrapper.ucpShapeSetLayers(intPtr, 268435456u);
		ChipmunkProWrapper.ucpSpaceAddShape(intPtr);
		autoGeometryProp.m_cpShape = intPtr;
		m_props.Add(autoGeometryProp);
	}

	public void RemoveProps()
	{
		for (int num = m_props.Count - 1; num >= 0; num--)
		{
			RemoveProp(m_props[num]);
		}
	}

	public void Update(AgPolygon[] _polys)
	{
		GenerateRandomPropsForPolys(_polys);
	}

	private void RemoveProp(AutoGeometryProp _prop)
	{
		UnityEngine.Object.Destroy(_prop.m_go);
		ChipmunkProWrapper.ucpRemoveShape(_prop.m_cpShape);
		_prop.m_go = null;
		_prop.m_cpShape = IntPtr.Zero;
		m_props.Remove(_prop);
	}

	public void Destroy()
	{
		RemoveProps();
		ChipmunkProS.RemoveBody(m_staticBody);
		m_staticBody = null;
		m_tileC.m_tile = null;
		m_tileC = null;
		m_props = null;
	}
}
