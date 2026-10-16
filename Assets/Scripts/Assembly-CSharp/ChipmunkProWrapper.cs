using System;
using System.Runtime.InteropServices;
using UnityEngine;

internal static class ChipmunkProWrapper
{
	private const string lookFrom = "chipmunk";

	[DllImport("chipmunk")]
	public static extern void ucpInitialize();

	[DllImport("chipmunk")]
	public static extern void ucpFree();

	[DllImport("chipmunk")]
	public static extern int ucpGetTotalBodyCount();

	[DllImport("chipmunk")]
	public static extern int ucpGetTotalConstraintCount();

	[DllImport("chipmunk")]
	public static extern IntPtr ucpAddBody(float mass, float inertia, int componentIndex);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpAddKinematicBody(int componentIndex);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpAddStaticBody(int componentIndex);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpAddConstraint(IntPtr constraint, int componentIndex);

	[DllImport("chipmunk")]
	public static extern int ucpRemoveBody(IntPtr body, [In][Out] ucpConstraintData[] results, int maxResults);

	[DllImport("chipmunk")]
	public static extern void ucpRemoveShape(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern void ucpRemoveConstraint(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern int ucpRemoveConstraintsFromBody(IntPtr body, [In][Out] ucpConstraintData[] results, int maxResults);

	[DllImport("chipmunk")]
	public static extern bool ucpBodyHasCustomProperties(IntPtr body);

	[DllImport("chipmunk")]
	public static extern float ucpBodyGetAngularDamp(IntPtr body);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyGetGravity(IntPtr body);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyGetLinearDamp(IntPtr body);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetAngularDamp(IntPtr body, float angularDamp);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetGravity(IntPtr body, Vector2 gravity);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetLinearDamp(IntPtr body, Vector2 linearDamp);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceAddCollisionHandler(ucpCollisionType collisionTypeA, ucpCollisionType collisionTypeB, bool trackBegin, bool trackPersist, bool trackSeparate);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceRemoveCollisionHandler(ucpCollisionType collisionTypeA, ucpCollisionType collisionTypeB);

	[DllImport("chipmunk")]
	public static extern int ucpGetBeginCollisionCount();

	[DllImport("chipmunk")]
	public static extern int ucpGetPersistCollisionCount();

	[DllImport("chipmunk")]
	public static extern int ucpGetSeparateCollisionCount();

	[DllImport("chipmunk")]
	public static extern int ucpGetBeginCollisions([In][Out] ucpCollisionPair[] results, int maxResults);

	[DllImport("chipmunk")]
	public static extern int ucpGetPersistCollisions([In][Out] ucpCollisionPair[] results, int maxResults);

	[DllImport("chipmunk")]
	public static extern int ucpGetSeparateCollisions([In][Out] ucpCollisionPair[] results, int maxResults);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpGetSpace();

	[DllImport("chipmunk")]
	public static extern int ucpHastySpaceGetThreads();

	[DllImport("chipmunk")]
	public static extern void ucpHastySpaceSetThreads(int threads);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpSpaceGetStaticBody();

	[DllImport("chipmunk")]
	public static extern float ucpSpaceGetCollisionBias();

	[DllImport("chipmunk")]
	public static extern float ucpSpaceGetCollisionPersistence();

	[DllImport("chipmunk")]
	public static extern float ucpSpaceGetCollisionSlop();

	[DllImport("chipmunk")]
	public static extern float ucpSpaceGetDamping();

	[DllImport("chipmunk")]
	public static extern Vector2 ucpSpaceGetGravity();

	[DllImport("chipmunk")]
	public static extern float ucpSpaceGetIdleSpeedThreshold();

	[DllImport("chipmunk")]
	public static extern int ucpSpaceGetIterations();

	[DllImport("chipmunk")]
	public static extern float ucpSpaceGetSleepTimeThreshold();

	[DllImport("chipmunk")]
	public static extern void ucpSpaceSetCollisionBias(float value);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceSetCollisionPersistence(uint value);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceSetCollisionSlop(float value);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceSetDamping(float value);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceSetGravity(Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceSetIdleSpeedThreshold(float value);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceSetIterations(int value);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceSetSleepTimeThreshold(float value);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceStep(float dt);

	[DllImport("chipmunk")]
	public static extern void ucpClearCollisionLists();

	[DllImport("chipmunk")]
	public static extern void ucpBodyUpdatePosition(IntPtr body, float dt);

	[DllImport("chipmunk")]
	public static extern void ucpBodyUpdateVelocity(IntPtr body, Vector2 gravity, float damping, float dt);

	[DllImport("chipmunk")]
	public static extern cpBB ucpShapeUpdate(IntPtr shape, Vector2 pos, Vector2 rot);

	[DllImport("chipmunk")]
	public static extern bool ucpSpaceContainsBody(IntPtr body);

	[DllImport("chipmunk")]
	public static extern bool ucpSpaceContainsConstraint(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern bool ucpSpaceContainsShape(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceReindexShape(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceReindexShapesForBody(IntPtr body);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceReindexStatic();

	[DllImport("chipmunk")]
	public static extern float ucpAreaForCircle(float r1, float r2);

	[DllImport("chipmunk")]
	public static extern float ucpAreaForPoly(int numVerts, Vector2[] verts, float r);

	[DllImport("chipmunk")]
	public static extern float ucpAreaForSegment(Vector2 a, Vector2 b, float r);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpCenteroidForPoly(int numVerts, Vector2[] verts);

	[DllImport("chipmunk")]
	public static extern float ucpMomentForBox(float m, float width, float height);

	[DllImport("chipmunk")]
	public static extern float ucpMomentForBox2(float m, cpBB box);

	[DllImport("chipmunk")]
	public static extern float ucpMomentForCircle(float m, float r1, float r2, Vector2 offset);

	[DllImport("chipmunk")]
	public static extern float ucpMomentForSegment(float m, Vector2 a, Vector2 b, float r);

	[DllImport("chipmunk")]
	public static extern float ucpMomentForPoly(float m, int numVerts, Vector2[] verts, Vector2 offset, float r);

	[DllImport("chipmunk")]
	public static extern int ucpConvexHull(int count, [In][Out] Vector2[] verts, float tol);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpSpaceAddBody(IntPtr body);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpSpaceAddConstraint(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpSpaceAddShape(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceRemoveBody(IntPtr body);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceRemoveConstraint(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpSpaceRemoveShape(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern void ucpBodyActivate(IntPtr body);

	[DllImport("chipmunk")]
	public static extern void ucpBodySleep(IntPtr body);

	[DllImport("chipmunk")]
	public static extern void ucpBodySleepWithGroup(IntPtr body, IntPtr group);

	[DllImport("chipmunk")]
	public static extern void ucpBodyActivateStatic(IntPtr body, IntPtr filter);

	[DllImport("chipmunk")]
	public static extern void ucpBodyApplyForceAtLocalPoint(IntPtr body, Vector2 f, Vector2 r);

	[DllImport("chipmunk")]
	public static extern void ucpBodyApplyImpulseAtLocalPoint(IntPtr body, Vector2 f, Vector2 r);

	[DllImport("chipmunk")]
	public static extern void ucpBodyApplyForceAtWorldPoint(IntPtr body, Vector2 f, Vector2 r);

	[DllImport("chipmunk")]
	public static extern void ucpBodyApplyImpulseAtWorldPoint(IntPtr body, Vector2 f, Vector2 r);

	[DllImport("chipmunk")]
	public static extern float ucpBodyGetAngle(IntPtr body);

	[DllImport("chipmunk")]
	public static extern float ucpBodyGetAngVel(IntPtr body);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyGetForce(IntPtr body);

	[DllImport("chipmunk")]
	public static extern float ucpBodyGetMass(IntPtr body);

	[DllImport("chipmunk")]
	public static extern float ucpBodyGetMoment(IntPtr body);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyGetPos(IntPtr body);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyGetRot(IntPtr body);

	[DllImport("chipmunk")]
	public static extern float ucpBodyGetTorque(IntPtr body);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyGetVel(IntPtr body);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyGetVelAtLocalPoint(IntPtr body, Vector2 point);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyGetVelAtWorldPoint(IntPtr body, Vector2 point);

	[DllImport("chipmunk")]
	public static extern bool ucpBodyIsSleeping(IntPtr body);

	[DllImport("chipmunk")]
	public static extern ucpBodyType ucpBodyGetType(IntPtr body);

	[DllImport("chipmunk")]
	public static extern float ucpBodyKineticEnergy(IntPtr body);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyLocal2World(IntPtr body, Vector2 v);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyWorld2Local(IntPtr body, Vector2 v);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetAngle(IntPtr body, float a);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetAngVel(IntPtr body, float a);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetForce(IntPtr body, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetMass(IntPtr body, float m);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetMoment(IntPtr body, float i);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetPos(IntPtr body, Vector2 pos);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetTorque(IntPtr body, float value);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetVel(IntPtr body, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetType(IntPtr body, ucpBodyType type);

	[DllImport("chipmunk")]
	public static extern float ucpBodyGetScale(IntPtr body);

	[DllImport("chipmunk")]
	public static extern void ucpBodySetScale(IntPtr body, float value);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpBodyGetForceVectorSum(IntPtr body);

	[DllImport("chipmunk")]
	public static extern float ucpBodyGetForceMagnitudeSum(IntPtr body);

	[DllImport("chipmunk")]
	public static extern cpBB ucpShapeGetBB(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpShapeGetBody(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern uint ucpShapeGetCollisionType(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern float ucpShapeGetElasticity(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern float ucpShapeGetFriction(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern bool ucpShapeGetSensor(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpShapeGetSurfaceVelocity(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern void ucpShapeSetCollisionType(IntPtr shape, ucpCollisionType value);

	[DllImport("chipmunk")]
	public static extern void ucpShapeSetElasticity(IntPtr shape, float value);

	[DllImport("chipmunk")]
	public static extern void ucpShapeSetFriction(IntPtr shape, float value);

	[DllImport("chipmunk")]
	public static extern void ucpShapeSetFilter(IntPtr shape, ucpShapeFilter filter);

	[DllImport("chipmunk")]
	public static extern ucpShapeFilter ucpShapeGetFilter(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern void ucpShapeSetSensor(IntPtr shape, bool value);

	[DllImport("chipmunk")]
	public static extern void ucpShapeSetSurfaceVelocity(IntPtr shape, Vector2 value);

	[DllImport("chipmunk")]
	public static extern cpBB ucpShapeCacheBB(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpCircleShapeNew(IntPtr body, float radius, Vector2 offset, ucpCollisionType collisionType);

	[DllImport("chipmunk")]
	public static extern float ucpCircleShapeGetRadius(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpCircleShapeGetOffset(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern void ucpCircleShapeSetRadius(IntPtr shape, float radius);

	[DllImport("chipmunk")]
	public static extern void ucpCircleShapeSetOffset(IntPtr shape, Vector2 offset);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpSegmentShapeNew(IntPtr body, Vector2 a, Vector2 b, float radius, ucpCollisionType collisionType);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpSegmentShapeGetA(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpSegmentShapeGetB(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpSegmentShapeGetNormal(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern float ucpSegmentShapeGetRadius(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern void ucpSegmentShapeSetEndpoints(IntPtr shape, Vector2 a, Vector2 b);

	[DllImport("chipmunk")]
	public static extern void ucpSegmentShapeSetRadius(IntPtr shape, float radius);

	[DllImport("chipmunk")]
	public static extern void ucpSegmentShapeSetNeighbors(IntPtr shape, Vector2 prev, Vector2 next);

	[DllImport("chipmunk")]
	public static extern void ucpSegmentShapeSetPrevNeighborTangent(IntPtr shape, Vector2 tangent);

	[DllImport("chipmunk")]
	public static extern void ucpSegmentShapeSetNextNeighborTangent(IntPtr shape, Vector2 tangent);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpPolyShapeNew(IntPtr body, int numVerts, Vector2[] verts, ucpTransform transform, float radius, ucpCollisionType collisionType);

	[DllImport("chipmunk")]
	public static extern int ucpPolyShapeGetNumVerts(IntPtr shape);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpPolyShapeGetVert(IntPtr shape, int idx);

	[DllImport("chipmunk")]
	public static extern void ucpPolyShapeSetVerts(IntPtr shape, int numVerts, Vector2[] verts, ucpTransform transform);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpConstraintGetA(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpConstraintGetB(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpConstraintGetErrorBias(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpConstraintGetImpulse(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpConstraintGetMaxBias(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpConstraintGetMaxForce(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpConstraintSetErrorBias(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpConstraintSetMaxBias(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpConstraintSetMaxForce(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpDampedRotarySpringNew(IntPtr a, IntPtr b, float restAngle, float stiffness, float damping);

	[DllImport("chipmunk")]
	public static extern float ucpDampedRotarySpringGetDamping(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpDampedRotarySpringGetRestAngle(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpDampedRotarySpringGetStiffness(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpDampedRotarySpringSetDamping(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpDampedRotarySpringSetRestAngle(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpDampedRotarySpringSetStiffness(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpDampedSpringNew(IntPtr a, IntPtr b, Vector2 anchr1, Vector2 anchr2, float restLength, float stiffness, float damping);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpDampedSpringGetAnchr1(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpDampedSpringGetAnchr2(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpDampedSpringGetDamping(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpDampedSpringGetRestLength(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpDampedSpringGetStiffness(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpDampedSpringSetAnchr1(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpDampedSpringSetAnchr2(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpDampedSpringSetDamping(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpDampedSpringSetRestLength(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpDampedSpringSetStiffness(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpGearJointNew(IntPtr a, IntPtr b, float phase, float ratio);

	[DllImport("chipmunk")]
	public static extern float ucpGearJointGetPhase(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpGearJointGetRatio(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpGearJointSetPhase(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpGearJointSetRatio(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpGrooveJointNew(IntPtr a, IntPtr b, Vector2 groove_a, Vector2 groobe_b, Vector2 anchr2);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpGrooveJointGetAnchr2(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpGrooveJointGetGrooveA(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpGrooveJointGetGrooveB(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpGrooveJointSetAnchr2(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpGrooveJointSetGrooveA(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpGrooveJointSetGrooveB(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpPinJointNew(IntPtr a, IntPtr b, Vector2 anchr1, Vector2 anchr2);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpPinJointGetAnchr1(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpPinJointGetAnchr2(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpPinJointGetDist(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpPinJointSetAnchr1(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpPinJointSetAnchr2(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpPinJointSetDist(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpPivotJointNew(IntPtr a, IntPtr b, Vector2 pivot);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpPivotJointNew2(IntPtr a, IntPtr b, Vector2 anchr1, Vector2 anchr2);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpPivotJointGetAnchr1(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpPivotJointGetAnchr2(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpPivotJointSetAnchr1(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpPivotJointSetAnchr2(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpRatchetJointNew(IntPtr a, IntPtr b, float phase, float ratchet);

	[DllImport("chipmunk")]
	public static extern float ucpRatchetJointGetAngle(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpRatchetJointGetPhase(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpRatchetJointGetRatchet(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpRatchetJointSetAngle(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpRatchetJointSetPhase(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpRatchetJointSetRatchet(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpRotaryLimitJointNew(IntPtr a, IntPtr b, float min, float max);

	[DllImport("chipmunk")]
	public static extern float ucpRotaryLimitJointGetMax(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpRotaryLimitJointGetMin(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpRotaryLimitJointSetMax(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpRotaryLimitJointSetMin(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpSimpleMotorNew(IntPtr a, IntPtr b, float rate);

	[DllImport("chipmunk")]
	public static extern float ucpSimpleMotorGetRate(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpSimpleMotorSetRate(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpSlideJointNew(IntPtr a, IntPtr b, Vector2 anchr1, Vector2 anchr2, float min, float max);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpSlideJointGetAnchr1(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpSlideJointGetAnchr2(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpSlideJointGetMax(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern float ucpSlideJointGetMin(IntPtr constraint);

	[DllImport("chipmunk")]
	public static extern void ucpSlideJointSetAnchr1(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpSlideJointSetAnchr2(IntPtr constraint, Vector2 value);

	[DllImport("chipmunk")]
	public static extern void ucpSlideJointSetMax(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern void ucpSlideJointSetMin(IntPtr constraint, float value);

	[DllImport("chipmunk")]
	public static extern cpBB ucpBBNew(float l, float b, float r, float t);

	[DllImport("chipmunk")]
	public static extern cpBB ucpBBNewForCircle(Vector2 p, float r);

	[DllImport("chipmunk")]
	public static extern cpBB ucpBBExpand(cpBB bb, Vector2 v);

	[DllImport("chipmunk")]
	public static extern cpBB ucpBBMerge(cpBB a, cpBB b);

	[DllImport("chipmunk")]
	public static extern float ucpBBMergedArea(cpBB a, cpBB b);

	[DllImport("chipmunk")]
	public static extern bool ucpBBContainsBB(cpBB bb, cpBB other);

	[DllImport("chipmunk")]
	public static extern bool ucpBBContainsVect(cpBB bb, Vector2 v);

	[DllImport("chipmunk")]
	public static extern bool ucpBBIntersects(cpBB a, cpBB b);

	[DllImport("chipmunk")]
	public static extern bool ucpBBIntersectsSegment(cpBB bb, Vector2 a, Vector2 b);

	[DllImport("chipmunk")]
	public static extern float ucpBBSegmentQuery(cpBB bb, Vector2 a, Vector2 b);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvadd(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvclamp(Vector2 v, float len);

	[DllImport("chipmunk")]
	public static extern float ucpvcross(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern float ucpvdist(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern float ucpvdistsq(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern float ucpvdot(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern bool ucpveql(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvforangle(float a);

	[DllImport("chipmunk")]
	public static extern float ucpvlength(Vector2 v);

	[DllImport("chipmunk")]
	public static extern float ucpvlengthsq(Vector2 v);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvlerp(Vector2 v1, Vector2 v2, float t);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvlerpconst(Vector2 v1, Vector2 v2, float d);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvmult(Vector2 v, float s);

	[DllImport("chipmunk")]
	public static extern bool ucpvnear(Vector2 v1, Vector2 v2, float dist);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvneg(Vector2 v);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvnormalize_safe(Vector2 v);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvperp(Vector2 v);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvproject(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvrotate(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvrperp(Vector2 v);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvslerp(Vector2 v1, Vector2 v2, float t);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvslerpconst(Vector2 v1, Vector2 v2, float a);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvsub(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern float ucpvtoangle(Vector2 v);

	[DllImport("chipmunk")]
	public static extern Vector2 ucpvunrotate(Vector2 v1, Vector2 v2);

	[DllImport("chipmunk")]
	public static extern float ucpShapePointQuery(IntPtr shape, Vector2 p, ref ucpPointQueryInfo info);

	[DllImport("chipmunk")]
	public static extern bool ucpShapeSegmentQuery(IntPtr shape, Vector2 a, Vector2 b, float radius, ref ucpSegmentQueryInfo info);

	[DllImport("chipmunk")]
	public static extern int ucpSpacePointQuery(Vector2 point, float maxDistance, ucpShapeFilter filter, [In][Out] ucpPointQueryInfo[] results, int maxResults);

	[DllImport("chipmunk")]
	public static extern int ucpSpaceSegmentQuery(Vector2 start, Vector2 end, float radius, ucpShapeFilter filter, [In][Out] ucpSegmentQueryInfo[] results, int maxResults);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpSpacePointQueryNearest(Vector2 point, float maxDistance, ucpShapeFilter filter, ref ucpPointQueryInfo info);

	[DllImport("chipmunk")]
	public static extern IntPtr ucpSpaceSegmentQueryFirst(Vector2 start, Vector2 end, float radius, ucpShapeFilter filter, ref ucpSegmentQueryInfo info);

	[DllImport("chipmunk")]
	public static extern bool ucpSpaceShapeQuery(IntPtr shape, [In][Out] ucpPointQueryInfo[] results, int maxResults);

	public static void ucpShapeSetLayers(IntPtr shape, uint value)
	{
		ucpShapeFilter filter = ucpShapeGetFilter(shape);
		filter.categories = value;
		filter.mask = value;
		ucpShapeSetFilter(shape, filter);
	}

	public static void ucpShapeSetGroup(IntPtr shape, uint value)
	{
		ucpShapeFilter filter = ucpShapeGetFilter(shape);
		filter.group = value;
		ucpShapeSetFilter(shape, filter);
	}

	public static void ucpBodyResetForces(IntPtr body)
	{
		ucpBodySetForce(body, Vector2.zero);
		ucpBodySetTorque(body, 0f);
	}
}
