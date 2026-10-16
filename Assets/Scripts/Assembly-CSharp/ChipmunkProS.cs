using System;
using System.Collections.Generic;
using UnityEngine;

public static class ChipmunkProS
{
	public static bool CHIPMUNK_DEBUG_DRAW = false;

	public static DynamicArray<ChipmunkBodyC> m_bodies;

	public static DynamicArray<ChipmunkConstraintC> m_constraints;

	private static int m_maxRemovedConstraintResultsCount = 50;

	private static ucpConstraintData[] m_removedConstraintResults = new ucpConstraintData[m_maxRemovedConstraintResultsCount];

	private static CollisionDelegate[,,] m_globalCollisionDelegates;

	private static uint[,] m_collisionDelegatePhases;

	private static int[,] m_collisionDelegateCounts;

	public static int m_collisionTypeCount;

	private static Entity m_staticBodyEntity;

	public static ChipmunkBodyC m_staticBody;

	public static TransformC m_staticBodyTC;

	public static bool m_isInsideHandleCollisions = false;

	public static List<ChipmunkBodyC> m_handleCollisionsRemoveBodyQueue = new List<ChipmunkBodyC>();

	public static void Initialize(int _collisionTypeCount)
	{
		m_collisionTypeCount = _collisionTypeCount + 1;
		m_bodies = new DynamicArray<ChipmunkBodyC>();
		m_constraints = new DynamicArray<ChipmunkConstraintC>();
		ChipmunkProWrapper.ucpInitialize();
		ChipmunkProWrapper.ucpSpaceSetGravity(Vector2.up * -400f);
		ChipmunkProWrapper.ucpSpaceSetCollisionBias(0.001f);
		m_staticBodyEntity = EntityManager.AddEntity();
		m_staticBodyEntity.m_persistent = true;
		m_staticBodyTC = TransformS.AddComponent(m_staticBodyEntity, "Chipmunk Static Body", Vector3.zero);
		m_staticBody = AddStaticBody(m_staticBodyTC);
		m_globalCollisionDelegates = new CollisionDelegate[m_collisionTypeCount, m_collisionTypeCount, 3];
		m_collisionDelegatePhases = new uint[m_collisionTypeCount, m_collisionTypeCount];
		m_collisionDelegateCounts = new int[m_collisionTypeCount, m_collisionTypeCount];
	}

	public static void Free()
	{
		if (m_bodies.m_aliveCount == 0 && m_constraints.m_aliveCount == 0)
		{
			EntityManager.RemoveEntity(m_staticBodyEntity);
			m_staticBodyEntity = null;
			m_staticBodyTC = null;
			m_staticBody = null;
			ChipmunkProWrapper.ucpFree();
		}
		else
		{
			Debug.LogError("Trying to clear populated chipmunk world. Bodies left: " + m_bodies.m_aliveCount + " / Constraints left:" + m_constraints.m_aliveCount);
		}
	}

	public static int GetTotalCollisionDelegateCount()
	{
		int num = 0;
		for (uint num2 = 0u; num2 < m_collisionTypeCount; num2++)
		{
			for (uint num3 = 0u; num3 < m_collisionTypeCount; num3++)
			{
				num += m_collisionDelegateCounts[num2, num3];
			}
		}
		return num;
	}

	public static IntPtr AddShapeToBody(ChipmunkBodyC _cmb, ucpShape _shape, float _currentMass, float _currentMoment)
	{
		IntPtr intPtr = IntPtr.Zero;
		float num = _currentMoment;
		if (_shape.shapeType == ucpShapeType.Circle)
		{
			ucpCircleShape ucpCircleShape2 = _shape as ucpCircleShape;
			num += ChipmunkProWrapper.ucpMomentForCircle(ucpCircleShape2.mass, ucpCircleShape2.innerRadius, ucpCircleShape2.outerRadius, ucpCircleShape2.offset);
			intPtr = ChipmunkProWrapper.ucpCircleShapeNew(_cmb.body, ucpCircleShape2.outerRadius, ucpCircleShape2.offset, ucpCircleShape2.collisionType);
		}
		else if (_shape.shapeType == ucpShapeType.Poly)
		{
			ucpPolyShape ucpPolyShape2 = _shape as ucpPolyShape;
			num += ChipmunkProWrapper.ucpMomentForPoly(ucpPolyShape2.mass, ucpPolyShape2.numVerts, ucpPolyShape2.verts, ucpPolyShape2.offset, 0f);
			if (num <= 0f)
			{
				Debug.LogError("Probably WRONG WINDING! should be CCW");
			}
			intPtr = ChipmunkProWrapper.ucpPolyShapeNew(_cmb.body, ucpPolyShape2.numVerts, ucpPolyShape2.verts, new ucpTransform(ucpPolyShape2.offset.x, ucpPolyShape2.offset.y), 0f, ucpPolyShape2.collisionType);
		}
		else if (_shape.shapeType == ucpShapeType.Segment)
		{
			ucpSegmentShape ucpSegmentShape2 = _shape as ucpSegmentShape;
			num += ChipmunkProWrapper.ucpMomentForSegment(ucpSegmentShape2.mass, ucpSegmentShape2.a, ucpSegmentShape2.b, 0f);
			intPtr = ChipmunkProWrapper.ucpSegmentShapeNew(_cmb.body, ucpSegmentShape2.a + ucpSegmentShape2.offset, ucpSegmentShape2.b + ucpSegmentShape2.offset, ucpSegmentShape2.radius, ucpSegmentShape2.collisionType);
		}
		if (intPtr != IntPtr.Zero)
		{
			ChipmunkProWrapper.ucpSpaceAddShape(intPtr);
		}
		else
		{
			Debug.LogError("NOOOO!! Shape ptr is zero");
		}
		if (ChipmunkProWrapper.ucpBodyGetType(_cmb.body) == ucpBodyType.DYNAMIC)
		{
			float num2 = _currentMass + _shape.mass;
			if (num2 > 0f)
			{
				ChipmunkProWrapper.ucpBodySetMass(_cmb.body, num2);
			}
			_cmb.m_mass = num2;
			if (num > 0f)
			{
				ChipmunkProWrapper.ucpBodySetMoment(_cmb.body, num);
			}
			_cmb.m_moment = num;
		}
		ChipmunkProWrapper.ucpShapeSetSensor(intPtr, _shape.sensor);
		ChipmunkProWrapper.ucpShapeSetElasticity(intPtr, _shape.elasticity);
		ChipmunkProWrapper.ucpShapeSetFriction(intPtr, _shape.friction);
		ucpShapeFilter filter = default(ucpShapeFilter);
		filter.ucpShapeFilterNone();
		filter.group = _shape.group;
		filter.categories = _shape.layers;
		filter.mask = _shape.layers;
		ChipmunkProWrapper.ucpShapeSetFilter(intPtr, filter);
		ChipmunkProWrapper.ucpShapeSetSurfaceVelocity(intPtr, _shape.surfaceVelocity);
		_cmb.shapes.Add(new ChipmunkBodyShape(intPtr, _shape.layers, _shape.tag));
		_shape.shapePtr = intPtr;
		return intPtr;
	}

	public static ChipmunkBodyC AddDynamicBody(TransformC _tc, ucpShape _shape, IComponent _customComponent = null)
	{
		ucpShape[] shapes = new ucpShape[1] { _shape };
		return AddDynamicBody(_tc, shapes, _customComponent);
	}

	public static ChipmunkBodyC AddDynamicBody(TransformC _tc, ucpShape[] _shapes, IComponent _customComponent = null)
	{
		ChipmunkBodyC chipmunkBodyC = m_bodies.AddItem();
		chipmunkBodyC.customComponent = _customComponent;
		chipmunkBodyC.TC = _tc;
		chipmunkBodyC.body = ChipmunkProWrapper.ucpAddBody(1f, 1f, chipmunkBodyC.m_index);
		ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, _tc.transform.position);
		float num = _tc.transform.rotation.eulerAngles.z;
		if (num > 180f)
		{
			num -= 360f;
		}
		ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, num * ((float)Math.PI / 180f));
		if (_shapes != null)
		{
			for (int i = 0; i < _shapes.Length; i++)
			{
				float currentMass = 0f;
				float currentMoment = 0f;
				if (chipmunkBodyC.shapes.Count > 0)
				{
					currentMass = ChipmunkProWrapper.ucpBodyGetMass(chipmunkBodyC.body);
					currentMoment = ChipmunkProWrapper.ucpBodyGetMoment(chipmunkBodyC.body);
				}
				AddShapeToBody(chipmunkBodyC, _shapes[i], currentMass, currentMoment);
			}
		}
		EntityManager.AddComponentToEntity(_tc.p_entity, chipmunkBodyC);
		return chipmunkBodyC;
	}

	public static ChipmunkBodyC AddKinematicBody(TransformC _tc, ucpShape _shape, IComponent _customComponent = null)
	{
		ucpShape[] shapes = new ucpShape[1] { _shape };
		return AddKinematicBody(_tc, shapes, _customComponent);
	}

	public static ChipmunkBodyC AddKinematicBody(TransformC _tc, ucpShape[] _shapes, IComponent _customComponent = null)
	{
		ChipmunkBodyC chipmunkBodyC = m_bodies.AddItem();
		chipmunkBodyC.customComponent = _customComponent;
		chipmunkBodyC.TC = _tc;
		chipmunkBodyC.body = ChipmunkProWrapper.ucpAddKinematicBody(chipmunkBodyC.m_index);
		ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, _tc.transform.position);
		float num = _tc.transform.rotation.eulerAngles.z;
		if (num > 180f)
		{
			num -= 360f;
		}
		ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, num * ((float)Math.PI / 180f));
		if (_shapes != null)
		{
			for (int i = 0; i < _shapes.Length; i++)
			{
				float currentMass = 0f;
				float currentMoment = 0f;
				if (chipmunkBodyC.shapes.Count > 0)
				{
					currentMass = ChipmunkProWrapper.ucpBodyGetMass(chipmunkBodyC.body);
					currentMoment = ChipmunkProWrapper.ucpBodyGetMoment(chipmunkBodyC.body);
				}
				AddShapeToBody(chipmunkBodyC, _shapes[i], currentMass, currentMoment);
			}
		}
		EntityManager.AddComponentToEntity(_tc.p_entity, chipmunkBodyC);
		return chipmunkBodyC;
	}

	public static ChipmunkBodyC AddStaticBody(TransformC _transformComponent, IComponent _customComponent = null)
	{
		ucpShape[] shapes = null;
		return AddStaticBody(_transformComponent, shapes, _customComponent);
	}

	public static ChipmunkBodyC AddStaticBody(TransformC _transformComponent, ucpShape _shape, IComponent _customComponent = null)
	{
		ucpShape[] shapes = new ucpShape[1] { _shape };
		return AddStaticBody(_transformComponent, shapes, _customComponent);
	}

	public static ChipmunkBodyC AddStaticBody(TransformC _transformComponent, ucpShape[] _shapes, IComponent _customComponent = null)
	{
		ChipmunkBodyC chipmunkBodyC = m_bodies.AddItem();
		chipmunkBodyC.customComponent = _customComponent;
		chipmunkBodyC.TC = _transformComponent;
		chipmunkBodyC.body = ChipmunkProWrapper.ucpAddStaticBody(chipmunkBodyC.m_index);
		ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, chipmunkBodyC.TC.transform.position);
		float num = chipmunkBodyC.TC.transform.rotation.eulerAngles.z;
		if (num > 180f)
		{
			num -= 360f;
		}
		ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, num * ((float)Math.PI / 180f));
		if (_shapes != null)
		{
			for (int i = 0; i < _shapes.Length; i++)
			{
				AddShapeToBody(chipmunkBodyC, _shapes[i], 0f, 0f);
			}
		}
		EntityManager.AddComponentToEntity(_transformComponent.p_entity, chipmunkBodyC);
		return chipmunkBodyC;
	}

	public static void SetBodyGravity(ChipmunkBodyC _body, Vector2 _gravity)
	{
		ChipmunkProWrapper.ucpBodySetGravity(_body.body, _gravity);
	}

	public static void SetBodyLinearDamp(ChipmunkBodyC _body, Vector2 _linearDamp)
	{
		_body.m_linearDamp = _linearDamp;
		ChipmunkProWrapper.ucpBodySetLinearDamp(_body.body, new Vector2(Mathf.Pow(_linearDamp.x, Main.m_targetFPS), Mathf.Pow(_linearDamp.y, Main.m_targetFPS)));
	}

	public static void SetBodyAngularDamp(ChipmunkBodyC _body, float _angularDamp)
	{
		_body.m_angularDamp = _angularDamp;
		ChipmunkProWrapper.ucpBodySetAngularDamp(_body.body, Mathf.Pow(_angularDamp, Main.m_targetFPS));
	}

	public static ChipmunkConstraintC AddDampedRotarySpring(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _restAngle, float _stiffness, float _damping)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpDampedRotarySpringNew(_bodyA.body, _bodyB.body, _restAngle, _stiffness, _damping);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.type = ucpConstraintType.DampedRotarySpring;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddDampedSpring(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor1, TransformC _anchor2, float _restLength, float _stiffness, float _damping)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_anchor1, _bodyA.TC);
		TransformS.ParentComponent(_anchor2, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpDampedSpringNew(_bodyA.body, _bodyB.body, _anchor1.transform.localPosition, _anchor2.transform.localPosition, _restLength, _stiffness, _damping);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.DampedSpring;
		chipmunkConstraintC.anchor1 = _anchor1;
		chipmunkConstraintC.anchor2 = _anchor2;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor1.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddGearJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _phase, float _ratio)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpGearJointNew(_bodyA.body, _bodyB.body, _phase, _ratio);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.GearJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddGrooveJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _grooveA, TransformC _grooveB, TransformC _anchor)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_grooveA, _bodyA.TC);
		TransformS.ParentComponent(_grooveB, _bodyA.TC);
		TransformS.ParentComponent(_anchor, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpGrooveJointNew(_bodyA.body, _bodyB.body, _grooveA.transform.localPosition, _grooveB.transform.localPosition, _anchor.transform.localPosition);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.GrooveJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.grooveA = _grooveA;
		chipmunkConstraintC.grooveB = _grooveB;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddPinJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor1, TransformC _anchor2)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_anchor1, _bodyA.TC);
		TransformS.ParentComponent(_anchor2, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpPinJointNew(_bodyA.body, _bodyB.body, _anchor1.transform.localPosition, _anchor2.transform.localPosition);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.PinJoint;
		chipmunkConstraintC.anchor1 = _anchor1;
		chipmunkConstraintC.anchor2 = _anchor2;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor1.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddPivotJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpPivotJointNew(_bodyA.body, _bodyB.body, _anchor.transform.position);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.PivotJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddPivotJoint2(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor1, TransformC _anchor2)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_anchor1, _bodyA.TC);
		TransformS.ParentComponent(_anchor2, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpPivotJointNew2(_bodyA.body, _bodyB.body, _anchor1.transform.localPosition, _anchor2.transform.localPosition);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.PivotJoint;
		chipmunkConstraintC.anchor1 = _anchor1;
		chipmunkConstraintC.anchor2 = _anchor2;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor1.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddRatchetJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _phase, float _ratchet)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpRatchetJointNew(_bodyA.body, _bodyB.body, _phase, _ratchet);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.RatchetJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddRotaryLimitJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _min, float _max)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpRotaryLimitJointNew(_bodyA.body, _bodyB.body, _min, _max);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.RotaryLimitJoint;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddSimpleMotor(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor, float _rate, float _maxForce)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		if (_anchor.parent == null)
		{
			TransformS.ParentComponent(_anchor, _bodyA.TC);
		}
		IntPtr constraint = ChipmunkProWrapper.ucpSimpleMotorNew(_bodyA.body, _bodyB.body, _rate);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		ChipmunkProWrapper.ucpConstraintSetMaxForce(constraint, _maxForce);
		chipmunkConstraintC.type = ucpConstraintType.SimpleMotor;
		chipmunkConstraintC.anchor1 = _anchor;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static ChipmunkConstraintC AddSlideJoint(ChipmunkBodyC _bodyA, ChipmunkBodyC _bodyB, TransformC _anchor1, TransformC _anchor2, float _min, float _max)
	{
		ChipmunkConstraintC chipmunkConstraintC = m_constraints.AddItem();
		TransformS.ParentComponent(_anchor1, _bodyA.TC);
		TransformS.ParentComponent(_anchor2, _bodyB.TC);
		IntPtr constraint = ChipmunkProWrapper.ucpSlideJointNew(_bodyA.body, _bodyB.body, _anchor1.transform.localPosition, _anchor2.transform.localPosition, _min, _max);
		ChipmunkProWrapper.ucpAddConstraint(constraint, chipmunkConstraintC.m_index);
		chipmunkConstraintC.type = ucpConstraintType.SlideJoint;
		chipmunkConstraintC.anchor1 = _anchor1;
		chipmunkConstraintC.anchor2 = _anchor2;
		chipmunkConstraintC.bodyA = _bodyA;
		chipmunkConstraintC.bodyB = _bodyB;
		chipmunkConstraintC.constraint = constraint;
		EntityManager.AddComponentToEntity(_anchor1.p_entity, chipmunkConstraintC);
		return chipmunkConstraintC;
	}

	public static void RemoveBody(ChipmunkBodyC _c)
	{
		if (m_isInsideHandleCollisions)
		{
			Debug.LogWarning("Removing body inside HandleCollisions... inserting in to queue.");
			if (!m_handleCollisionsRemoveBodyQueue.Contains(_c))
			{
				m_handleCollisionsRemoveBodyQueue.Add(_c);
			}
			return;
		}
		if (_c.body == IntPtr.Zero)
		{
			Debug.LogError("Trying to remove a body twice!");
			return;
		}
		ChipmunkProWrapper.ucpClearCollisionLists();
		int num = ChipmunkProWrapper.ucpRemoveBody(_c.body, m_removedConstraintResults, m_maxRemovedConstraintResultsCount);
		HandleCollisionEvents();
		RemoveAllCollisionHandlers(_c);
		_c.TC = null;
		_c.customComponent = null;
		for (int i = 0; i < num; i++)
		{
			if (m_removedConstraintResults[i].ucpComponentIndex > -1)
			{
				ChipmunkConstraintC chipmunkConstraintC = m_constraints.m_array[m_removedConstraintResults[i].ucpComponentIndex];
				chipmunkConstraintC.constraint = IntPtr.Zero;
				RemoveConstraint(chipmunkConstraintC);
			}
		}
		_c.body = IntPtr.Zero;
		EntityManager.RemoveComponentFromEntity(_c);
		m_bodies.RemoveItem(_c);
	}

	public static void RemoveConstraintsFromBody(ChipmunkBodyC _c)
	{
		for (int num = m_constraints.m_aliveCount - 1; num > -1; num--)
		{
			ChipmunkConstraintC chipmunkConstraintC = m_constraints.m_array[m_constraints.m_aliveIndices[num]];
			if (chipmunkConstraintC.bodyA == _c || chipmunkConstraintC.bodyB == _c)
			{
				RemoveConstraint(chipmunkConstraintC);
			}
		}
	}

	public static void RemoveConstraint(ChipmunkConstraintC _c)
	{
		if (_c.constraint != IntPtr.Zero)
		{
			if (!_c.addedToSpace)
			{
				ChipmunkProWrapper.ucpSpaceAddConstraint(_c.constraint);
			}
			ChipmunkProWrapper.ucpRemoveConstraint(_c.constraint);
		}
		EntityManager.RemoveComponentFromEntity(_c);
		m_constraints.RemoveItem(_c);
	}

	public static void AddGlobalCollisionHandler(CollisionDelegate _collisionHandler, ucpCollisionType _collisionTypeA, ucpCollisionType _collisionTypeB, bool _handleBegin, bool _handlePersist, bool _handleSeparate)
	{
		if (_handleBegin)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 0] = _collisionHandler;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num | 0x100;
		}
		if (_handlePersist)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 1] = _collisionHandler;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num2 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num2 | 0x10;
		}
		if (_handleSeparate)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 2] = _collisionHandler;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num3 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num3 | 1;
		}
		uint num4 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] | m_collisionDelegatePhases[(uint)_collisionTypeB, (uint)_collisionTypeA];
		ChipmunkProWrapper.ucpSpaceAddCollisionHandler(_collisionTypeA, _collisionTypeB, (num4 & 0x100) == 256, (num4 & 0x10) == 16, (num4 & 1) == 1);
	}

	public static void AddCollisionHandler(ChipmunkBodyC _c, CollisionDelegate _collisionHandler, ucpCollisionType _collisionTypeA, ucpCollisionType _collisionTypeB, bool _handleBegin, bool _handlePersist, bool _handleSeparate)
	{
		if (_handleBegin)
		{
			_c.m_collisionDelegates[(uint)_collisionTypeB, 0] = _collisionHandler;
			_c.m_collisionDelegateTypes[(uint)_collisionTypeB, 0] = _collisionTypeA;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num | 0x100;
		}
		if (_handlePersist)
		{
			_c.m_collisionDelegates[(uint)_collisionTypeB, 1] = _collisionHandler;
			_c.m_collisionDelegateTypes[(uint)_collisionTypeB, 1] = _collisionTypeA;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num2 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num2 | 0x10;
		}
		if (_handleSeparate)
		{
			_c.m_collisionDelegates[(uint)_collisionTypeB, 2] = _collisionHandler;
			_c.m_collisionDelegateTypes[(uint)_collisionTypeB, 2] = _collisionTypeA;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]++;
			uint num3 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB];
			m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] = num3 | 1;
		}
		uint num4 = m_collisionDelegatePhases[(uint)_collisionTypeA, (uint)_collisionTypeB] | m_collisionDelegatePhases[(uint)_collisionTypeB, (uint)_collisionTypeA];
		ChipmunkProWrapper.ucpSpaceAddCollisionHandler(_collisionTypeA, _collisionTypeB, (num4 & 0x100) == 256, (num4 & 0x10) == 16, (num4 & 1) == 1);
	}

	public static void RemoveGlobalCollisionHandler(ucpCollisionType _collisionTypeA, ucpCollisionType _collisionTypeB, bool _began, bool _persist, bool _separate)
	{
		if (_began)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 0] = null;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]--;
			if (m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB] == 0 && m_collisionDelegateCounts[(uint)_collisionTypeB, (uint)_collisionTypeA] == 0)
			{
				ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_collisionTypeA, _collisionTypeB);
			}
		}
		if (_persist)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 1] = null;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]--;
			if (m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB] == 0 && m_collisionDelegateCounts[(uint)_collisionTypeB, (uint)_collisionTypeA] == 0)
			{
				ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_collisionTypeA, _collisionTypeB);
			}
		}
		if (_separate)
		{
			m_globalCollisionDelegates[(uint)_collisionTypeA, (uint)_collisionTypeB, 2] = null;
			m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB]--;
			if (m_collisionDelegateCounts[(uint)_collisionTypeA, (uint)_collisionTypeB] == 0 && m_collisionDelegateCounts[(uint)_collisionTypeB, (uint)_collisionTypeA] == 0)
			{
				ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_collisionTypeA, _collisionTypeB);
			}
		}
	}

	public static void RemoveCollisionHandler(ChipmunkBodyC _c, CollisionDelegate _collisionHandler)
	{
		for (int i = 0; i < _c.m_collisionDelegates.GetLength(0); i++)
		{
			if (_c.m_collisionDelegates[i, 0] == _collisionHandler)
			{
				_c.m_collisionDelegates[i, 0] = null;
				m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 0], i]--;
				if (m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 0], i] == 0 && m_collisionDelegateCounts[i, (uint)_c.m_collisionDelegateTypes[i, 0]] == 0)
				{
					ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_c.m_collisionDelegateTypes[i, 0], (ucpCollisionType)i);
				}
			}
			if (_c.m_collisionDelegates[i, 1] == _collisionHandler)
			{
				_c.m_collisionDelegates[i, 1] = null;
				m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 1], i]--;
				if (m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 1], i] == 0 && m_collisionDelegateCounts[i, (uint)_c.m_collisionDelegateTypes[i, 1]] == 0)
				{
					ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_c.m_collisionDelegateTypes[i, 1], (ucpCollisionType)i);
				}
			}
			if (_c.m_collisionDelegates[i, 2] == _collisionHandler)
			{
				_c.m_collisionDelegates[i, 2] = null;
				m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 2], i]--;
				if (m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 2], i] == 0 && m_collisionDelegateCounts[i, (uint)_c.m_collisionDelegateTypes[i, 2]] == 0)
				{
					ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_c.m_collisionDelegateTypes[i, 2], (ucpCollisionType)i);
				}
			}
		}
	}

	public static void RemoveAllCollisionHandlers(ChipmunkBodyC _c)
	{
		for (int i = 0; i < _c.m_collisionDelegates.GetLength(0); i++)
		{
			if (_c.m_collisionDelegates[i, 0] != null)
			{
				_c.m_collisionDelegates[i, 0] = null;
				m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 0], i]--;
				if ((m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 0], i] == 0) & (m_collisionDelegateCounts[i, (uint)_c.m_collisionDelegateTypes[i, 0]] == 0))
				{
					ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_c.m_collisionDelegateTypes[i, 0], (ucpCollisionType)i);
				}
			}
			if (_c.m_collisionDelegates[i, 1] != null)
			{
				_c.m_collisionDelegates[i, 1] = null;
				m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 1], i]--;
				if ((m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 1], i] == 0) & (m_collisionDelegateCounts[i, (uint)_c.m_collisionDelegateTypes[i, 1]] == 0))
				{
					ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_c.m_collisionDelegateTypes[i, 1], (ucpCollisionType)i);
				}
			}
			if (_c.m_collisionDelegates[i, 2] != null)
			{
				_c.m_collisionDelegates[i, 2] = null;
				m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 2], i]--;
				if ((m_collisionDelegateCounts[(uint)_c.m_collisionDelegateTypes[i, 2], i] == 0) & (m_collisionDelegateCounts[i, (uint)_c.m_collisionDelegateTypes[i, 2]] == 0))
				{
					ChipmunkProWrapper.ucpSpaceRemoveCollisionHandler(_c.m_collisionDelegateTypes[i, 2], (ucpCollisionType)i);
				}
			}
		}
	}

	private static ucpCollisionPair ReverseCollisionPair(ucpCollisionPair _pair)
	{
		ucpCollisionPair result = _pair;
		result.shapeA = _pair.shapeB;
		result.shapeB = _pair.shapeA;
		result.ucpComponentIndexA = _pair.ucpComponentIndexB;
		result.ucpComponentIndexB = _pair.ucpComponentIndexA;
		result.normal = -_pair.normal;
		result.impulse = -_pair.impulse;
		result.friction = -_pair.friction;
		return result;
	}

	public static int HandleCollisionEvents()
	{
		int num = ChipmunkProWrapper.ucpGetBeginCollisionCount();
		if (num > 0)
		{
			ucpCollisionPair[] array = new ucpCollisionPair[num];
			ChipmunkProWrapper.ucpGetBeginCollisions(array, array.Length);
			for (int i = 0; i < num; i++)
			{
				ucpCollisionPair ucpCollisionPair2 = array[i];
				ChipmunkBodyC chipmunkBodyC = m_bodies.m_array[ucpCollisionPair2.ucpComponentIndexA];
				ChipmunkBodyC chipmunkBodyC2 = m_bodies.m_array[ucpCollisionPair2.ucpComponentIndexB];
				uint typeA = ucpCollisionPair2.typeA;
				uint typeB = ucpCollisionPair2.typeB;
				if (typeA <= m_collisionTypeCount && typeB <= m_collisionTypeCount)
				{
					if (m_globalCollisionDelegates[typeA, typeB, 0] != null)
					{
						m_globalCollisionDelegates[typeA, typeB, 0](ucpCollisionPair2, ucpCollisionPhase.Begin);
					}
					if (typeA != typeB && m_globalCollisionDelegates[typeB, typeA, 0] != null)
					{
						m_globalCollisionDelegates[typeB, typeA, 0](ucpCollisionPair2, ucpCollisionPhase.Begin);
					}
					if (chipmunkBodyC.m_collisionDelegates[typeB, 0] != null)
					{
						chipmunkBodyC.m_collisionDelegates[typeB, 0](ucpCollisionPair2, ucpCollisionPhase.Begin);
					}
					if (chipmunkBodyC2.m_collisionDelegates[typeA, 0] != null)
					{
						chipmunkBodyC2.m_collisionDelegates[typeA, 0](ReverseCollisionPair(ucpCollisionPair2), ucpCollisionPhase.Begin);
					}
				}
			}
		}
		int num2 = ChipmunkProWrapper.ucpGetPersistCollisionCount();
		if (num2 > 0)
		{
			ucpCollisionPair[] array2 = new ucpCollisionPair[num2];
			ChipmunkProWrapper.ucpGetPersistCollisions(array2, array2.Length);
			for (int j = 0; j < num2; j++)
			{
				ucpCollisionPair ucpCollisionPair3 = array2[j];
				ChipmunkBodyC chipmunkBodyC3 = m_bodies.m_array[ucpCollisionPair3.ucpComponentIndexA];
				ChipmunkBodyC chipmunkBodyC4 = m_bodies.m_array[ucpCollisionPair3.ucpComponentIndexB];
				uint typeA2 = ucpCollisionPair3.typeA;
				uint typeB2 = ucpCollisionPair3.typeB;
				if (typeA2 <= m_collisionTypeCount && typeB2 <= m_collisionTypeCount)
				{
					if (m_globalCollisionDelegates[typeA2, typeB2, 1] != null)
					{
						m_globalCollisionDelegates[typeA2, typeB2, 1](ucpCollisionPair3, ucpCollisionPhase.Persist);
					}
					if (typeA2 != typeB2 && m_globalCollisionDelegates[typeB2, typeA2, 1] != null)
					{
						m_globalCollisionDelegates[typeB2, typeA2, 1](ucpCollisionPair3, ucpCollisionPhase.Persist);
					}
					if (chipmunkBodyC3.m_collisionDelegates[typeB2, 1] != null)
					{
						chipmunkBodyC3.m_collisionDelegates[typeB2, 1](ucpCollisionPair3, ucpCollisionPhase.Persist);
					}
					if (chipmunkBodyC4.m_collisionDelegates[typeA2, 1] != null)
					{
						chipmunkBodyC4.m_collisionDelegates[typeA2, 1](ReverseCollisionPair(ucpCollisionPair3), ucpCollisionPhase.Persist);
					}
				}
			}
		}
		int num3 = ChipmunkProWrapper.ucpGetSeparateCollisionCount();
		if (num3 > 0)
		{
			ucpCollisionPair[] array3 = new ucpCollisionPair[num3];
			ChipmunkProWrapper.ucpGetSeparateCollisions(array3, array3.Length);
			for (int k = 0; k < num3; k++)
			{
				ucpCollisionPair ucpCollisionPair4 = array3[k];
				ChipmunkBodyC chipmunkBodyC5 = m_bodies.m_array[ucpCollisionPair4.ucpComponentIndexA];
				ChipmunkBodyC chipmunkBodyC6 = m_bodies.m_array[ucpCollisionPair4.ucpComponentIndexB];
				uint typeA3 = ucpCollisionPair4.typeA;
				uint typeB3 = ucpCollisionPair4.typeB;
				if (typeA3 <= m_collisionTypeCount && typeB3 <= m_collisionTypeCount)
				{
					if (m_globalCollisionDelegates[typeA3, typeB3, 2] != null)
					{
						m_globalCollisionDelegates[typeA3, typeB3, 2](ucpCollisionPair4, ucpCollisionPhase.Separate);
					}
					if (typeA3 != typeB3 && m_globalCollisionDelegates[typeB3, typeA3, 2] != null)
					{
						m_globalCollisionDelegates[typeB3, typeA3, 2](ucpCollisionPair4, ucpCollisionPhase.Separate);
					}
					if (chipmunkBodyC5.m_collisionDelegates[typeB3, 2] != null)
					{
						chipmunkBodyC5.m_collisionDelegates[typeB3, 2](ucpCollisionPair4, ucpCollisionPhase.Separate);
					}
					if (chipmunkBodyC6.m_collisionDelegates[typeA3, 2] != null)
					{
						chipmunkBodyC6.m_collisionDelegates[typeA3, 2](ReverseCollisionPair(ucpCollisionPair4), ucpCollisionPhase.Separate);
					}
				}
			}
		}
		return num + num3 + num2;
	}

	public static void RemoveAllQueuedBodies()
	{
		if (m_handleCollisionsRemoveBodyQueue.Count <= 0)
		{
			return;
		}
		foreach (ChipmunkBodyC item in m_handleCollisionsRemoveBodyQueue)
		{
			RemoveBody(item);
		}
		m_handleCollisionsRemoveBodyQueue.Clear();
	}

	public static void Update(float _dt)
	{
		if (CHIPMUNK_DEBUG_DRAW)
		{
			DebugDraw.Clear(CameraS.m_mainCamera, m_staticBodyTC);
		}
		ChipmunkProWrapper.ucpClearCollisionLists();
		ChipmunkProWrapper.ucpSpaceStep(_dt);
		m_handleCollisionsRemoveBodyQueue.Clear();
		m_isInsideHandleCollisions = true;
		HandleCollisionEvents();
		ChipmunkProWrapper.ucpClearCollisionLists();
		m_isInsideHandleCollisions = false;
		RemoveAllQueuedBodies();
		int aliveCount = m_bodies.m_aliveCount;
		int num = 0;
		for (int i = 0; i < aliveCount; i++)
		{
			ChipmunkBodyC chipmunkBodyC = m_bodies.m_array[m_bodies.m_aliveIndices[i]];
			if (!chipmunkBodyC.m_active)
			{
				continue;
			}
			if (ChipmunkProWrapper.ucpBodyGetType(chipmunkBodyC.body) != ucpBodyType.DYNAMIC)
			{
				bool flag = ChipmunkProWrapper.ucpBodyGetType(chipmunkBodyC.body) == ucpBodyType.KINEMATIC;
				if (!flag && !chipmunkBodyC.m_updateStaticVisuals)
				{
					continue;
				}
				bool flag2 = ChipmunkProWrapper.ucpBodyGetType(chipmunkBodyC.body) == ucpBodyType.STATIC;
				bool flag3 = false;
				bool flag4 = false;
				Vector3 vector = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body);
				float num2 = ChipmunkProWrapper.ucpBodyGetAngle(chipmunkBodyC.body);
				float num3 = ChipmunkProWrapper.ucpBodyGetScale(chipmunkBodyC.body);
				Vector2 vector2 = chipmunkBodyC.TC.transform.position - vector;
				float num4 = Mathf.DeltaAngle(chipmunkBodyC.TC.transform.rotation.eulerAngles.z, num2 * 57.29578f);
				float num5 = chipmunkBodyC.TC.transform.lossyScale.z - num3;
				if (num5 != 0f)
				{
					ChipmunkProWrapper.ucpBodySetScale(chipmunkBodyC.body, chipmunkBodyC.TC.transform.lossyScale.z);
				}
				if (Mathf.Abs(num4) > 0.0001f)
				{
					if (flag && _dt > 0f)
					{
						ChipmunkProWrapper.ucpBodySetAngVel(chipmunkBodyC.body, num4 * ((float)Math.PI / 180f) / _dt);
					}
					flag4 = true;
				}
				if (vector2 != Vector2.zero)
				{
					if (flag && _dt > 0f)
					{
						ChipmunkProWrapper.ucpBodySetVel(chipmunkBodyC.body, -vector2 / _dt * Main.m_timeScale);
					}
					flag3 = true;
				}
				if (flag2)
				{
					if (flag3 || flag4)
					{
						Debug.LogWarning("Moving or rotating static body. This is expensive!");
						ChipmunkProWrapper.ucpSpaceReindexShapesForBody(chipmunkBodyC.body);
						TransformS.SetGlobalTransform(chipmunkBodyC.TC, vector, Vector3.forward * num2 * 57.29578f);
					}
				}
				else
				{
					if (flag3 || flag4)
					{
						ChipmunkProWrapper.ucpBodyUpdatePosition(chipmunkBodyC.body, _dt);
						TransformS.SetGlobalTransform(chipmunkBodyC.TC, vector, Vector3.forward * num2 * 57.29578f);
					}
					if (!flag3)
					{
						ChipmunkProWrapper.ucpBodySetVel(chipmunkBodyC.body, Vector2.zero);
					}
					if (!flag4)
					{
						ChipmunkProWrapper.ucpBodySetAngVel(chipmunkBodyC.body, 0f);
					}
				}
				chipmunkBodyC.m_updateStaticVisuals = false;
				continue;
			}
			if (!ChipmunkProWrapper.ucpBodyIsSleeping(chipmunkBodyC.body))
			{
				chipmunkBodyC.m_isSleeping = false;
				num++;
				Vector3 vector = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body);
				vector.z = chipmunkBodyC.TC.transform.position.z;
				float num2 = ChipmunkProWrapper.ucpBodyGetAngle(chipmunkBodyC.body) * 57.29578f;
				float num3 = ChipmunkProWrapper.ucpBodyGetScale(chipmunkBodyC.body);
				float num6 = chipmunkBodyC.TC.transform.lossyScale.z - num3;
				if (num6 != 0f)
				{
					ChipmunkProWrapper.ucpBodySetScale(chipmunkBodyC.body, chipmunkBodyC.TC.transform.lossyScale.z);
					ChipmunkProWrapper.ucpSpaceReindexShapesForBody(chipmunkBodyC.body);
				}
				TransformS.SetGlobalTransform(chipmunkBodyC.TC, vector, Vector3.forward * num2);
			}
			else
			{
				chipmunkBodyC.m_isSleeping = true;
			}
			if (CHIPMUNK_DEBUG_DRAW)
			{
				if (chipmunkBodyC.m_isSleeping)
				{
					DebugDraw.defaultColor = new Color(0f, 1f, 0f, 1f);
				}
				else
				{
					DebugDraw.defaultColor = new Color(1f, 0f, 0f, 1f);
				}
				for (int j = 0; j < chipmunkBodyC.shapes.Count; j++)
				{
					IntPtr shapePtr = chipmunkBodyC.shapes[j].shapePtr;
					cpBB cpBB2 = ChipmunkProWrapper.ucpShapeGetBB(shapePtr);
					float num7 = cpBB2.r - cpBB2.l;
					float num8 = cpBB2.t - cpBB2.b;
					DebugDraw.CreateBox(_pos: new Vector2(cpBB2.l + num7 * 0.5f, cpBB2.b + num8 * 0.5f), _camera: CameraS.m_mainCamera, _transformComponent: m_staticBodyTC, _width: num7, _height: num8);
				}
			}
		}
		if (CHIPMUNK_DEBUG_DRAW)
		{
			DebugDraw.defaultColor = new Color(0f, 0f, 1f, 1f);
			aliveCount = m_constraints.m_aliveCount;
			for (int k = 0; k < aliveCount; k++)
			{
				ChipmunkConstraintC chipmunkConstraintC = m_constraints.m_array[m_constraints.m_aliveIndices[k]];
				if (!chipmunkConstraintC.m_active)
				{
					continue;
				}
				if (chipmunkConstraintC.type == ucpConstraintType.PivotJoint)
				{
					TransformS.SetPosition(chipmunkConstraintC.anchor1, ChipmunkProWrapper.ucpPivotJointGetAnchr1(chipmunkConstraintC.constraint));
					DebugDraw.CreateBox(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.anchor1.transform.position, 4f, 4f);
				}
				else if (chipmunkConstraintC.type != ucpConstraintType.PinJoint)
				{
					if (chipmunkConstraintC.type == ucpConstraintType.SlideJoint)
					{
						TransformS.SetPosition(chipmunkConstraintC.anchor1, ChipmunkProWrapper.ucpSlideJointGetAnchr1(chipmunkConstraintC.constraint));
						TransformS.SetPosition(chipmunkConstraintC.anchor2, ChipmunkProWrapper.ucpSlideJointGetAnchr2(chipmunkConstraintC.constraint));
						DebugDraw.CreateBox(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.anchor1.transform.position, 4f, 4f);
						DebugDraw.CreateBox(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.anchor2.transform.position, 4f, 4f);
						DebugDraw.CreateLine(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.anchor1.transform.position, chipmunkConstraintC.anchor2.transform.position);
					}
					else if (chipmunkConstraintC.type == ucpConstraintType.DampedSpring)
					{
						TransformS.SetPosition(chipmunkConstraintC.anchor1, ChipmunkProWrapper.ucpDampedSpringGetAnchr1(chipmunkConstraintC.constraint));
						TransformS.SetPosition(chipmunkConstraintC.anchor2, ChipmunkProWrapper.ucpDampedSpringGetAnchr2(chipmunkConstraintC.constraint));
						DebugDraw.CreateBox(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.anchor1.transform.position, 4f, 4f);
						DebugDraw.CreateBox(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.anchor2.transform.position, 4f, 4f);
						DebugDraw.CreateLine(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.anchor1.transform.position, chipmunkConstraintC.anchor2.transform.position);
					}
					else if (chipmunkConstraintC.type == ucpConstraintType.GrooveJoint)
					{
						TransformS.SetPosition(chipmunkConstraintC.anchor1, ChipmunkProWrapper.ucpGrooveJointGetAnchr2(chipmunkConstraintC.constraint));
						TransformS.SetPosition(chipmunkConstraintC.grooveA, ChipmunkProWrapper.ucpGrooveJointGetGrooveA(chipmunkConstraintC.constraint));
						TransformS.SetPosition(chipmunkConstraintC.grooveB, ChipmunkProWrapper.ucpGrooveJointGetGrooveB(chipmunkConstraintC.constraint));
						Vector3 vector3 = chipmunkConstraintC.grooveB.transform.localPosition - chipmunkConstraintC.grooveA.transform.localPosition;
						vector3 = ChipmunkProWrapper.ucpvperp(vector3).normalized;
						DebugDraw.CreateBox(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.anchor1.transform.position, 4f, 4f);
						DebugDraw.CreateLine(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.grooveA.transform.position - vector3 * 2f, chipmunkConstraintC.grooveB.transform.position - vector3 * 2f);
						DebugDraw.CreateLine(CameraS.m_mainCamera, m_staticBodyTC, chipmunkConstraintC.grooveA.transform.position + vector3 * 2f, chipmunkConstraintC.grooveB.transform.position + vector3 * 2f);
					}
				}
			}
			DebugDraw.defaultColor = new Color(1f, 1f, 1f, 1f);
		}
		m_bodies.Update();
		m_constraints.Update();
	}

	public static bool IsCrushing(ChipmunkBodyC _c, float _forceTolerance, float _crushTolerance = 0.8f)
	{
		float magnitude = ChipmunkProWrapper.ucpBodyGetForceVectorSum(_c.body).magnitude;
		float num = ChipmunkProWrapper.ucpBodyGetForceMagnitudeSum(_c.body);
		if (num > _forceTolerance)
		{
			float num2 = magnitude / num;
			if (!float.IsNaN(num2))
			{
				float num3 = 1f - num2;
				return num3 > _crushTolerance;
			}
		}
		return false;
	}

	public static Vector2[] GenerateCollisionVertexArrayFromGameObject(GameObject _gameObject, bool _zyOrientation = false, bool _flipX = false)
	{
		MeshFilter component = _gameObject.GetComponent<MeshFilter>();
		Mesh sharedMesh = component.sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		Vector2[] array = new Vector2[vertices.Length];
		int num = 0;
		for (int i = 0; i < vertices.Length; i++)
		{
			if (!_zyOrientation)
			{
				array[i].x = vertices[i].x;
			}
			else
			{
				array[i].x = vertices[i].z;
			}
			if (_flipX)
			{
				array[i].x *= -1f;
			}
			array[i].y = vertices[i].y;
			if (Mathf.Abs(array[i].x) < 0.1f)
			{
				num++;
			}
		}
		array = RemoveDuplicateVerts(array);
		if (num == vertices.Length)
		{
			Debug.LogError("Invalid vertex data. Wrong local orientation!");
			return null;
		}
		array = SortVerticeArrayCCW(array);
		int num2 = ChipmunkProWrapper.ucpConvexHull(array.Length, array, 0f);
		Vector2[] array2 = new Vector2[num2];
		Array.Copy(array, array2, array2.Length);
		return array2;
	}

	public static ucpPolyShape GeneratePolyShapeFromGameObject(GameObject _gameObject, Vector2 _offset, float _mass, float _elasticity, float _friction, ucpCollisionType _collisionType, uint _layers, bool _zyOrientation = false, bool _flipX = false)
	{
		Component component = _gameObject.GetComponent(typeof(MeshFilter));
		Mesh sharedMesh = (component as MeshFilter).sharedMesh;
		Vector3[] vertices = sharedMesh.vertices;
		Vector2[] array = new Vector2[vertices.Length];
		int num = 0;
		Vector2 vector = Vector2.zero;
		vector = (_zyOrientation ? new Vector2(component.gameObject.transform.localPosition.z, component.gameObject.transform.localPosition.y) : new Vector2(component.gameObject.transform.localPosition.x, component.gameObject.transform.localPosition.y));
		for (int i = 0; i < vertices.Length; i++)
		{
			if (!_zyOrientation)
			{
				array[i].x = vertices[i].x;
			}
			else
			{
				array[i].x = vertices[i].z;
			}
			if (_flipX)
			{
				array[i].x *= -1f;
			}
			array[i].y = vertices[i].y;
			if (Mathf.Abs(array[i].x) < 0.1f)
			{
				num++;
			}
		}
		array = RemoveDuplicateVerts(array);
		if (num == vertices.Length)
		{
			Debug.LogError("Invalid vertex data. Wrong local orientation!");
			return null;
		}
		array = SortVerticeArrayCCW(array);
		int num2 = ChipmunkProWrapper.ucpConvexHull(array.Length, array, 0f);
		Vector2[] array2 = new Vector2[num2];
		Array.Copy(array, array2, array2.Length);
		ucpPolyShape ucpPolyShape2 = new ucpPolyShape(array, _offset + vector, _layers, _mass, _elasticity, _friction, _collisionType);
		ucpPolyShape2.tag = _gameObject.name;
		return ucpPolyShape2;
	}

	public static ucpPolyShape[] GeneratePolyShapesFromChildren(GameObject _parentGameObject, Vector2 _offset, float _totalMass, float _elasticity, float _friction, ucpCollisionType _collisionType, uint _layers, bool _zyOrientation = false, bool _flipX = false)
	{
		Component[] componentsInChildren = _parentGameObject.GetComponentsInChildren(typeof(MeshFilter), true);
		if (componentsInChildren.Length == 0)
		{
			return null;
		}
		ucpPolyShape[] array = new ucpPolyShape[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Mesh sharedMesh = (componentsInChildren[i] as MeshFilter).sharedMesh;
			Vector3[] vertices = sharedMesh.vertices;
			Vector2[] array2 = new Vector2[vertices.Length];
			Vector2 vector = Vector2.zero;
			vector = (_zyOrientation ? new Vector2(componentsInChildren[i].gameObject.transform.localPosition.z, componentsInChildren[i].gameObject.transform.localPosition.y) : new Vector2(componentsInChildren[i].gameObject.transform.localPosition.x, componentsInChildren[i].gameObject.transform.localPosition.y));
			int num = 0;
			for (int j = 0; j < vertices.Length; j++)
			{
				if (!_zyOrientation)
				{
					array2[j].x = vertices[j].x;
				}
				else
				{
					array2[j].x = vertices[j].z;
				}
				if (_flipX)
				{
					array2[j].x *= -1f;
				}
				array2[j].y = vertices[j].y;
				if (Mathf.Abs(array2[j].x) < 0.1f)
				{
					num++;
				}
			}
			array2 = RemoveDuplicateVerts(array2);
			if (num == vertices.Length)
			{
				Debug.LogError("Invalid vertex data. Wrong local orientation!");
				continue;
			}
			array2 = SortVerticeArrayCCW(array2);
			int num2 = ChipmunkProWrapper.ucpConvexHull(array2.Length, array2, 0f);
			Vector2[] array3 = new Vector2[num2];
			Array.Copy(array2, array3, array3.Length);
			array[i] = new ucpPolyShape(array3, _offset + vector, _layers, 0f, _elasticity, _friction, _collisionType);
			array[i].tag = componentsInChildren[i].gameObject.name;
		}
		float num3 = 0f;
		for (int k = 0; k < array.Length; k++)
		{
			num3 += array[k].area;
		}
		for (int l = 0; l < array.Length; l++)
		{
			array[l].mass = _totalMass * array[l].area / num3;
		}
		return array;
	}

	public static ChipmunkBodyShape GetBodyShapeByTag(ChipmunkBodyC _c, string _tag)
	{
		for (int i = 0; i < _c.shapes.Count; i++)
		{
			if (_c.shapes[i].tag != null && _c.shapes[i].tag == _tag)
			{
				return _c.shapes[i];
			}
		}
		return null;
	}

	public static Vector2[] RemoveDuplicateVerts(Vector2[] _verts)
	{
		List<Vector2> list = new List<Vector2>();
		foreach (Vector2 item in _verts)
		{
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public static Vector2[] SortVerticeArrayCCW(Vector2[] _verts)
	{
		Vector2 center = Vector2.zero;
		for (int i = 0; i < _verts.Length; i++)
		{
			center += _verts[i];
		}
		center /= (float)_verts.Length;
		Array.Sort(_verts, (Vector2 v1, Vector2 v2) => (!vertSortIsLess(v1, v2, center)) ? 1 : (-1));
		return _verts;
	}

	private static bool vertSortIsLess(Vector2 a, Vector2 b, Vector2 center)
	{
		int num = (int)((a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y));
		if (num < 0)
		{
			return true;
		}
		if (num > 0)
		{
			return false;
		}
		int num2 = (int)((a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y));
		int num3 = (int)((b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y));
		return num2 < num3;
	}
}
