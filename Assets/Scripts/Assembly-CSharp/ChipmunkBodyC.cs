using System;
using System.Collections.Generic;
using UnityEngine;

public class ChipmunkBodyC : BasicComponent
{
	public TransformC TC;

	public IntPtr body;

	public List<ChipmunkBodyShape> shapes;

	public float m_mass;

	public float m_moment;

	public float m_angularDamp;

	public Vector2 m_linearDamp;

	public Vector2 m_savedGravity;

	public bool m_isDisabled;

	public bool m_updateStaticVisuals;

	public bool m_isSleeping;

	public CollisionDelegate[,] m_collisionDelegates;

	public ucpCollisionType[,] m_collisionDelegateTypes;

	public IComponent customComponent;

	public ChipmunkBodyC()
		: base(ComponentType.ChipmunkBody)
	{
		shapes = new List<ChipmunkBodyShape>();
	}

	public override void Reset()
	{
		base.Reset();
		shapes.Clear();
		m_angularDamp = 1f;
		m_linearDamp = Vector2.one;
		m_isDisabled = false;
		m_updateStaticVisuals = true;
		body = IntPtr.Zero;
		customComponent = null;
		m_collisionDelegates = new CollisionDelegate[ChipmunkProS.m_collisionTypeCount, 3];
		m_collisionDelegateTypes = new ucpCollisionType[ChipmunkProS.m_collisionTypeCount, 3];
	}

	~ChipmunkBodyC()
	{
	}
}
