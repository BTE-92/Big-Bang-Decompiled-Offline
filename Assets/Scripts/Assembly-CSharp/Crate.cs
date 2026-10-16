using UnityEngine;

public class Crate : Unit
{
	public ChipmunkBodyC m_cmb;

	protected PrefabC boxPrefab;

	public Crate(GraphElement _graphElement, float _size, float _friction = 0.8f, float _elasticity = 0.5f, float _weight = 1f, string _resource = "")
		: base(_graphElement, UnitType.Basic)
	{
		GameObject gameObject = ResourceManager.GetGameObject(_resource);
		TransformC transformC = TransformS.AddComponent(m_entity, _graphElement.m_name);
		TransformS.SetTransform(transformC, _graphElement.m_position, _graphElement.m_rotation);
		ucpPolyShape shape = new ucpPolyShape(_size, _size, Vector2.zero, 17895696u, _weight, _elasticity, _friction, (ucpCollisionType)4);
		ChipmunkBodyC cmb = ChipmunkProS.AddDynamicBody(transformC, shape, m_unitC);
		m_cmb = cmb;
		boxPrefab = PrefabS.AddComponent(transformC, new Vector3(0f, 0f, 0f), gameObject);
		boxPrefab.p_gameObject.transform.localScale = new Vector3(_size, _size, _size) * 1.15f;
		boxPrefab.p_gameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
		CreateEditorTouchArea(_size, _size);
	}
}
