using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class TouchAreaS
{
    public static DynamicArray<TLTouch> m_touches;

    private static List<TLTouch> m_touchRemoveList;

    public static DynamicArray<TouchAreaC> m_areas;

    public static bool m_abort;

    private static Vector2 m_prevMousePosition;

    private static Touch2[] m_mouseTouches;

    private static TransformC m_colliderHelperTC;

    private static bool m_colliderMeshesCreated;

    private static Mesh m_rectMesh;

    private static Mesh m_circleMesh;

    private static Camera m_touchCameraFilter;

    private static TEvent[] m_touchEvents;

    private static int m_touchEventCount;

    public static bool m_disabled = false;

    public static float m_fingerRadius = 12f;

    private static int[] m_mouseButtonIds = new int[2] { -1, -1 };

    private static bool[] m_mouseButtonDowns = new bool[2];

    private static int m_tick;

    public static void Initialize()
    {
        m_touches = new DynamicArray<TLTouch>(10);
        m_touchRemoveList = new List<TLTouch>();
        m_areas = new DynamicArray<TouchAreaC>();
        m_mouseTouches = new Touch2[10];
        for (int i = 0; i < 10; i++)
        {
            m_mouseTouches[i] = new Touch2();
        }
        m_touchEvents = new TEvent[50];
        for (int j = 0; j < m_touchEvents.Length; j++)
        {
            m_touchEvents[j] = new TEvent();
            m_touchEvents[j].m_touches = new TLTouch[10];
            m_touchEvents[j].m_touchSecondaries = new bool[10];
            m_touchEvents[j].m_touchPhases = new TouchAreaPhase[10];
            m_touchEvents[j].m_touchCount = 0;
        }
    }

    public static List<TLTouch> GetTouchesOnTAC(TouchAreaC _c)
    {
        List<TLTouch> list = new List<TLTouch>();
        for (int i = 0; i < m_touches.m_array.Length; i++)
        {
            if (m_touches.m_array[i].m_primaryArea == _c || m_touches.m_array[i].m_secondaryArea == _c)
            {
                list.Add(m_touches.m_array[i]);
            }
        }
        return list;
    }

    private static void CreateColliderMeshes()
    {
        Entity entity = EntityManager.AddEntity();
        entity.m_persistent = true;
        m_colliderHelperTC = TransformS.AddComponent(entity, "Touch Area Helper Transform");
        Vector2[] circle = DebugDraw.GetCircle(1f, 16, Vector2.zero);
        m_circleMesh = GeometryGenerator.GenerateFlatMesh(new GGData(circle));
        Vector2[] rect = DebugDraw.GetRect(1f, 1f, Vector2.zero);
        m_rectMesh = GeometryGenerator.GenerateFlatMesh(new GGData(rect));
        m_colliderMeshesCreated = true;
    }

    private static Mesh GetCircleMesh(float _radius)
    {
        if (!m_colliderMeshesCreated)
        {
            CreateColliderMeshes();
        }
        Mesh mesh = UnityEngine.Object.Instantiate(m_circleMesh);
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= _radius;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }

    private static Mesh GetRectMesh(float _width, float _height)
    {
        if (!m_colliderMeshesCreated)
        {
            CreateColliderMeshes();
        }
        Mesh mesh = UnityEngine.Object.Instantiate(m_rectMesh);
        Vector3 scale = new Vector3(_width, _height, 1f);
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].Scale(scale);
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }

    public static void ResizeRectCollider(TouchAreaC _c, float _width, float _height)
    {
        if (_c.m_colliderShape != ColliderShape.Rect)
        {
            Debug.LogWarning("Trying to resize non-rect collider");
            return;
        }
        UnityEngine.Object.DestroyImmediate(_c.m_collider.sharedMesh);
        _c.m_collider.sharedMesh = GetRectMesh(_width, _height);
    }

    public static void ResizeCircleCollider(TouchAreaC _c, float _radius)
    {
        if (_c.m_colliderShape != ColliderShape.Circle)
        {
            Debug.LogWarning("Trying to resize non-circle collider");
            return;
        }
        UnityEngine.Object.DestroyImmediate(_c.m_collider.sharedMesh);
        _c.m_collider.sharedMesh = GetCircleMesh(_radius);
    }

    public static void AddTouchEventListener(TouchAreaC _c, TouchEventDelegate _touchEventHandler)
    {
        if (_c.m_delegatedCount == 0)
        {
            _c.d_TouchEventDelegate = _touchEventHandler;
        }
        else
        {
            _c.d_TouchEventDelegate = (TouchEventDelegate)Delegate.Combine(_c.d_TouchEventDelegate, _touchEventHandler);
        }
        _c.m_delegatedCount++;
    }

    public static void RemoveTouchEventListener(TouchAreaC _c, TouchEventDelegate _touchEventHandler)
    {
        if (_c.m_delegatedCount > 0)
        {
            _c.d_TouchEventDelegate = (TouchEventDelegate)Delegate.Remove(_c.d_TouchEventDelegate, _touchEventHandler);
            _c.m_delegatedCount--;
        }
    }

    public static void RemoveAllTouchEventListeners(TouchAreaC _c)
    {
        if (_c.m_delegatedCount > 0)
        {
            Delegate[] invocationList = _c.d_TouchEventDelegate.GetInvocationList();
            Delegate[] array = invocationList;
            foreach (Delegate obj in array)
            {
                _c.d_TouchEventDelegate = (TouchEventDelegate)Delegate.Remove(_c.d_TouchEventDelegate, (TouchEventDelegate)obj);
            }
            _c.m_delegatedCount = 0;
        }
    }

    private static TLTouch AddTouch(Vector2 _pos, int _fingerId, TouchPhase _phase)
    {
        TLTouch tLTouch = m_touches.AddItem();
        tLTouch.m_currentPosition = _pos;
        tLTouch.m_startPosition = _pos;
        tLTouch.m_fingerId = _fingerId;
        tLTouch.m_phase = _phase;
        tLTouch.m_cancelled = false;
        return tLTouch;
    }

    private static void RemoveTouch(TLTouch _t)
    {
        _t.m_primaryArea = null;
        _t.m_secondaryArea = null;
        _t.m_consumed = false;
        m_touches.RemoveItem(_t);
    }

    public static TouchAreaC AddRectArea(TransformC _tc, string _name, float _width, float _height, Camera _camera, IComponent _customComponent = null)
    {
        TouchAreaC touchAreaC = m_areas.AddItem();
        touchAreaC.m_TC = _tc;
        touchAreaC.m_TC.transform.gameObject.layer = _camera.gameObject.layer;
        touchAreaC.m_customComponent = _customComponent;
        touchAreaC.m_camera = _camera;
        touchAreaC.m_name = _name;
        GameObject gameObject = touchAreaC.m_TC.transform.gameObject;
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = GetRectMesh(_width, _height);
        touchAreaC.m_collider = meshCollider;
        touchAreaC.m_colliderShape = ColliderShape.Rect;
        TouchAreaBootstrap touchAreaBootstrap = gameObject.AddComponent<TouchAreaBootstrap>();
        touchAreaBootstrap.m_TAC = touchAreaC;
        EntityManager.AddComponentToEntity(_tc.p_entity, touchAreaC);
        return touchAreaC;
    }

    public static TouchAreaC AddCircleArea(TransformC _tc, string _name, float _radius, Camera _camera, IComponent _customComponent = null)
    {
        TouchAreaC touchAreaC = m_areas.AddItem();
        touchAreaC.m_TC = _tc;
        touchAreaC.m_TC.transform.gameObject.layer = _camera.gameObject.layer;
        touchAreaC.m_customComponent = _customComponent;
        touchAreaC.m_camera = _camera;
        touchAreaC.m_name = _name;
        GameObject gameObject = touchAreaC.m_TC.transform.gameObject;
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = GetCircleMesh(_radius);
        touchAreaC.m_collider = meshCollider;
        touchAreaC.m_colliderShape = ColliderShape.Circle;
        TouchAreaBootstrap touchAreaBootstrap = gameObject.AddComponent<TouchAreaBootstrap>();
        touchAreaBootstrap.m_TAC = touchAreaC;
        EntityManager.AddComponentToEntity(_tc.p_entity, touchAreaC);
        return touchAreaC;
    }

    public static TouchAreaC AddMeshArea(TransformC _tc, string _name, Mesh _mesh, Camera _camera, IComponent _customComponent = null)
    {
        if (!m_colliderMeshesCreated)
        {
            CreateColliderMeshes();
        }
        TouchAreaC touchAreaC = m_areas.AddItem();
        touchAreaC.m_TC = _tc;
        touchAreaC.m_TC.transform.gameObject.layer = _camera.gameObject.layer;
        touchAreaC.m_customComponent = _customComponent;
        touchAreaC.m_camera = _camera;
        touchAreaC.m_name = _name;
        GameObject gameObject = touchAreaC.m_TC.transform.gameObject;
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = _mesh;
        touchAreaC.m_collider = meshCollider;
        touchAreaC.m_colliderShape = ColliderShape.Mesh;
        TouchAreaBootstrap touchAreaBootstrap = gameObject.AddComponent<TouchAreaBootstrap>();
        touchAreaBootstrap.m_TAC = touchAreaC;
        EntityManager.AddComponentToEntity(_tc.p_entity, touchAreaC);
        return touchAreaC;
    }

    public static void RemoveArea(TouchAreaC _c)
    {
        GameObject gameObject = _c.m_TC.transform.gameObject;
        TouchAreaBootstrap component = gameObject.GetComponent<TouchAreaBootstrap>();
        MeshCollider component2 = gameObject.GetComponent<MeshCollider>();
        UnityEngine.Object.DestroyImmediate(component2.sharedMesh);
        UnityEngine.Object.DestroyImmediate(component2);
        UnityEngine.Object.DestroyImmediate(component);
        if (_c.m_delegatedCount > 0)
        {
            Delegate[] invocationList = _c.d_TouchEventDelegate.GetInvocationList();
            Delegate[] array = invocationList;
            foreach (Delegate obj in array)
            {
                _c.d_TouchEventDelegate = (TouchEventDelegate)Delegate.Remove(_c.d_TouchEventDelegate, (TouchEventDelegate)obj);
            }
            _c.m_delegatedCount = 0;
        }
        EntityManager.RemoveComponentFromEntity(_c);
        m_areas.RemoveItem(_c);
    }

    public static void SetCamera(TouchAreaC _c, Camera _camera)
    {
        _c.m_camera = _camera;
        _c.m_TC.transform.gameObject.layer = _camera.gameObject.layer;
    }

    public static void SetTouchCameraFilter(Camera _camera)
    {
        m_touchCameraFilter = _camera;
    }

    public static void RemoveTouchCameraFilter()
    {
        m_touchCameraFilter = null;
    }

    public static void SetClip(TouchAreaC _c, float _left, float _right, float _bottom, float _top)
    {
        _c.m_clip = true;
        _c.m_clipBB.r = _right;
        _c.m_clipBB.l = _left;
        _c.m_clipBB.t = _top;
        _c.m_clipBB.b = _bottom;
    }

    public static bool IsTouchInside(Vector2 touchPos, Vector2 position, float radius)
    {
        Vector2 vector = touchPos - position;
        float num = vector.x * vector.x + vector.y * vector.y;
        if (num < radius * radius)
        {
            return true;
        }
        return false;
    }

    public static bool IsTouchInside(Vector2 touchPos, Vector2 position, Vector2 dimensions, float rotAngle)
    {
        float num = touchPos.x;
        float num2 = touchPos.y;
        if (rotAngle != 0f)
        {
            Vector2 vec = new Vector2(num, num2) - position;
            float num3 = ToolBox.getAngleFromVector2(vec) * 57.29578f;
            float num4 = rotAngle - num3;
            num = position.x + Mathf.Cos(num4 * ((float)Math.PI / 180f)) * vec.magnitude;
            num2 = position.y + Mathf.Sin(num4 * ((float)Math.PI / 180f)) * vec.magnitude;
        }
        position.x -= dimensions.x * 0.5f;
        position.y -= dimensions.y * 0.5f;
        if (num > position.x && num < position.x + dimensions.x && num2 > position.y && num2 < position.y + dimensions.y)
        {
            return true;
        }
        return false;
    }

    public static Vector3 GetTouchWorldPos(Camera _camera, Vector2 _screenPos, float _zOffset = 0f)
    {
        if (_camera.rect.width < 0.01f || _camera.rect.height < 0.01f)
        {
            return Vector3.one * float.MaxValue;
        }
        Vector3 result = -_camera.ScreenToWorldPoint(new Vector3(_screenPos.x, _screenPos.y, _camera.transform.position.z + _zOffset));
        result += _camera.transform.position * 2f;
        result.z = 0f;
        return result;
    }

    public static void Disable()
    {
        m_disabled = true;
        Debug.Log("Touches DISABLED");
    }

    public static void Enable()
    {
        m_disabled = false;
        Debug.Log("Touches ENABLED");
    }

    public static void ConvertSecondaryAreaToPrimary(TLTouch _touch)
    {
        TouchAreaC touchAreaC = null;
        TouchAreaPhase touchAreaPhase = TouchAreaPhase.Began;
        if (_touch.m_primaryArea != null)
        {
            touchAreaC = _touch.m_primaryArea;
            touchAreaPhase = _touch.m_primaryPhase;
            touchAreaC.m_touchCount--;
            touchAreaC.m_wasDragged = false;
            touchAreaC.m_isDragged = false;
        }
        _touch.m_primaryArea = _touch.m_secondaryArea;
        _touch.m_primaryPhase = _touch.m_secondaryPhase;
        _touch.m_secondaryArea = null;
        _touch.m_secondaryPhase = TouchAreaPhase.ReleaseOut;
    }

    public static void LockSecondaryTouchArea(TLTouch _touch)
    {
        _touch.m_lockedSecondary = _touch.m_secondaryArea;
    }

    public static void CancelAllTouches(TLTouch _touch = null)
    {
        for (int i = 0; i < m_areas.m_aliveCount; i++)
        {
            TouchAreaC touchAreaC = m_areas.m_array[m_areas.m_aliveIndices[i]];
            for (int j = 0; j < m_touches.m_aliveCount; j++)
            {
                TLTouch tLTouch = m_touches.m_array[m_touches.m_aliveIndices[j]];
                if ((_touch == null || tLTouch.m_fingerId != _touch.m_fingerId) && (tLTouch.m_primaryArea == touchAreaC || tLTouch.m_secondaryArea == touchAreaC))
                {
                    CancelTouch(tLTouch);
                }
            }
        }
    }

    public static void CancelTouch(TLTouch _touch)
    {
        if (_touch.m_phase == TouchPhase.Canceled || _touch.m_cancelled)
        {
            return;
        }
        if (_touch.m_primaryArea != null && _touch.m_primaryPhase != TouchAreaPhase.ReleaseOut)
        {
            _touch.m_primaryPhase = TouchAreaPhase.ReleaseOut;
            if (_touch.m_primaryArea.m_touchCount > 0)
            {
                _touch.m_primaryArea.m_touchCount--;
            }
            HandleTouch(_touch.m_primaryArea, _touch, false);
        }
        if (_touch.m_secondaryArea != null && _touch.m_primaryArea != _touch.m_secondaryArea && _touch.m_secondaryPhase != TouchAreaPhase.RollOut)
        {
            _touch.m_secondaryPhase = TouchAreaPhase.RollOut;
            if (_touch.m_secondaryArea.m_touchCount > 0)
            {
                _touch.m_secondaryArea.m_touchCount--;
            }
            HandleTouch(_touch.m_secondaryArea, _touch, true);
        }
        _touch.m_primaryArea = null;
        _touch.m_secondaryArea = null;
        _touch.m_lockedSecondary = null;
        _touch.m_phase = TouchPhase.Canceled;
        _touch.m_cancelled = true;
    }

    public static void Update()
    {
        if (m_disabled)
        {
            return;
        }
        m_tick++;
        int touchCount = Input.touchCount;
        List<Touch2> list = new List<Touch2>();
        for (int i = 0; i < touchCount; i++)
        {
            Touch2 touch = new Touch2();
            Touch touch2 = Input.GetTouch(i);
            touch.deltaPosition = touch2.deltaPosition;
            touch.fingerId = touch2.fingerId;
            touch.phase = touch2.phase;
            if (touch.phase == TouchPhase.Canceled)
            {
                touch.phase = TouchPhase.Ended;
            }
            touch.position = touch2.position;
            touch.tapCount = touch2.tapCount;
            touch.deltaTime = touch2.deltaTime;
            list.Add(touch);
        }

        if (touchCount == 0)
        {
            Vector2 currentMousePos = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                Touch2 touch = new Touch2();
                touch.deltaPosition = Vector2.zero;
                touch.fingerId = -1;
                touch.phase = TouchPhase.Began;
                touch.position = currentMousePos;
                touch.tapCount = 1;
                touch.deltaTime = Time.deltaTime;
                list.Add(touch);
                touchCount++;

                m_prevMousePosition = currentMousePos;
                m_mouseButtonDowns[0] = true;
                m_mouseButtonIds[0] = -1;
            }
            else if (Input.GetMouseButton(0) && m_mouseButtonDowns[0])
            {
                Touch2 touch = new Touch2();
                Vector2 delta = currentMousePos - m_prevMousePosition;
                touch.deltaPosition = delta;
                touch.fingerId = -1;
                touch.phase = ((delta.sqrMagnitude > 0.01f) ? TouchPhase.Moved : TouchPhase.Stationary);
                touch.position = currentMousePos;
                touch.tapCount = 1;
                touch.deltaTime = Time.deltaTime;
                list.Add(touch);
                touchCount++;

                m_prevMousePosition = currentMousePos;
            }
            else if (Input.GetMouseButtonUp(0) && m_mouseButtonDowns[0])
            {
                Touch2 touch = new Touch2();
                touch.deltaPosition = currentMousePos - m_prevMousePosition;
                touch.fingerId = -1;
                touch.phase = TouchPhase.Ended;
                touch.position = currentMousePos;
                touch.tapCount = 1;
                touch.deltaTime = Time.deltaTime;
                list.Add(touch);
                touchCount++;

                m_mouseButtonDowns[0] = false;
                m_mouseButtonIds[0] = -1;
            }
        }

        while (m_touchRemoveList.Count > 0)
        {
            int index = m_touchRemoveList.Count - 1;
            RemoveTouch(m_touchRemoveList[index]);
            m_touchRemoveList.RemoveAt(index);
        }
        m_touches.Update();
        for (int j = 0; j < touchCount; j++)
        {
            TLTouch tLTouch = null;
            Touch2 touch3 = list[j];
            bool flag = true;
            int aliveCount = m_touches.m_aliveCount;
            for (int k = 0; k < aliveCount; k++)
            {
                tLTouch = m_touches.m_array[m_touches.m_aliveIndices[k]];
                if (tLTouch.m_fingerId == touch3.fingerId)
                {
                    flag = false;
                    tLTouch.m_deltaPosition = touch3.position - tLTouch.m_currentPosition;
                    tLTouch.m_currentPosition = touch3.position;
                    tLTouch.m_phase = touch3.phase;
                    tLTouch.m_tapCount = touch3.tapCount;
                    break;
                }
            }
            if (flag)
            {
                tLTouch = AddTouch(touch3.position, touch3.fingerId, TouchPhase.Began);
            }
            int count = CameraS.m_cameras.Count;
            for (int num = count - 1; num > -1; num--)
            {
                Camera camera = CameraS.m_cameras[num];
                if ((!(EventSystem.current != null) || !EventSystem.current.IsPointerOverGameObject() || !(UnityUIManager.m_canvas != null) || !(UnityUIManager.m_canvas.worldCamera.depth >= camera.depth)) && (!(m_touchCameraFilter != null) || !(camera != m_touchCameraFilter)) && (!tLTouch.m_consumed || !(tLTouch.m_consumingCamera != camera)) && (tLTouch.m_primaryArea == null || !(tLTouch.m_primaryArea.m_camera != camera)) && (tLTouch.m_secondaryArea == null || !(tLTouch.m_secondaryArea.m_camera != camera)))
                {
                    Vector3 touchWorldPos = GetTouchWorldPos(camera, touch3.position);
                    Vector3 position = camera.transform.position;
                    Vector3 direction = touchWorldPos - position;
                    RaycastHit[] array = null;
                    Vector3 vector = position;
                    if (camera.orthographic)
                    {
                        vector = -touchWorldPos + position * 2f;
                        vector.z = position.z;
                        array = Physics.SphereCastAll(vector, m_fingerRadius, Vector3.forward, Math.Abs(position.z) + 500f, 1 << camera.gameObject.layer);
                    }
                    else
                    {
                        array = Physics.SphereCastAll(vector, m_fingerRadius, direction, Math.Abs(position.z) + 500f, 1 << camera.gameObject.layer);
                    }
                    List<RaycastHit> list2 = new List<RaycastHit>(array);
                    list2.Sort((RaycastHit a, RaycastHit b) => a.transform.position.z.CompareTo(b.transform.position.z));
                    TouchAreaC touchAreaC = null;
                    float num2 = 99999f;
                    TouchAreaC touchAreaC2 = null;
                    float num3 = 99999f;
                    bool flag2 = false;
                    bool flag3 = false;
                    for (int num4 = 0; num4 < list2.Count; num4++)
                    {
                        RaycastHit raycastHit = list2[num4];
                        TouchAreaBootstrap touchAreaBootstrap = raycastHit.transform.GetComponent("TouchAreaBootstrap") as TouchAreaBootstrap;
                        if (!(touchAreaBootstrap != null))
                        {
                            continue;
                        }
                        TouchAreaC tAC = touchAreaBootstrap.m_TAC;
                        if ((camera.orthographic && tAC.m_clip && !ChipmunkProWrapper.ucpBBIntersects(b: new cpBB(touch3.position.x - m_fingerRadius, touch3.position.y - m_fingerRadius, touch3.position.x + m_fingerRadius, touch3.position.y + m_fingerRadius), a: tAC.m_clipBB)) || !tAC.m_active)
                        {
                            continue;
                        }
                        if (tAC.m_allowPrimary)
                        {
                            if (touchAreaC2 == null)
                            {
                                touchAreaC2 = tAC;
                                continue;
                            }
                            float num5 = Mathf.Abs(raycastHit.transform.position.z - touchAreaC2.m_TC.transform.position.z);
                            if (num5 <= 1f || tAC.m_ignoreDepth)
                            {
                                float num6 = Vector2.Distance(touchAreaC2.m_TC.transform.position, vector);
                                float num7 = Vector2.Distance(tAC.m_TC.transform.position, vector);
                                if (num7 < num6)
                                {
                                    if (touchAreaC2.m_allowSecondary && (touchAreaC == null || (touchAreaC != null && touchAreaC2.m_TC.transform.position.z < touchAreaC.m_TC.transform.position.z)) && touchAreaC2.m_letTouchesThrough)
                                    {
                                        touchAreaC = touchAreaC2;
                                    }
                                    touchAreaC2 = tAC;
                                    continue;
                                }
                            }
                        }
                        if (!tAC.m_allowSecondary)
                        {
                            continue;
                        }
                        if (touchAreaC == null)
                        {
                            if (touchAreaC2 == null || touchAreaC2.m_letTouchesThrough)
                            {
                                touchAreaC = tAC;
                            }
                            continue;
                        }
                        float num8 = Mathf.Abs(raycastHit.transform.position.z - touchAreaC.m_TC.transform.position.z);
                        if (num8 <= 1f || tAC.m_ignoreDepth)
                        {
                            float num9 = Vector2.Distance(touchAreaC.m_TC.transform.position, vector);
                            float num10 = Vector2.Distance(tAC.m_TC.transform.position, vector);
                            if (num10 < num9 && (touchAreaC2 == null || touchAreaC2.m_letTouchesThrough))
                            {
                                touchAreaC = tAC;
                            }
                        }
                    }
                    if ((touchAreaC2 != null && !touchAreaC2.m_letTouchesThrough) || (touchAreaC != null && !touchAreaC.m_letTouchesThrough))
                    {
                        tLTouch.m_consumed = true;
                        tLTouch.m_consumingCamera = camera;
                    }
                    if (tLTouch.m_primaryArea != null && touchAreaC2 == tLTouch.m_primaryArea)
                    {
                        flag2 = true;
                    }
                    if (tLTouch.m_secondaryArea != null && touchAreaC2 == tLTouch.m_secondaryArea)
                    {
                        flag3 = true;
                    }
                    else if (tLTouch.m_secondaryArea != null && touchAreaC == tLTouch.m_secondaryArea)
                    {
                        flag3 = true;
                    }
                    if (flag3 && touchAreaC2 != null && touchAreaC2.m_allowSecondary && touchAreaC2 != tLTouch.m_secondaryArea && touchAreaC2 != tLTouch.m_primaryArea)
                    {
                        flag3 = false;
                    }
                    if (tLTouch.m_primaryArea == null && touchAreaC2 != null && tLTouch.m_phase == TouchPhase.Began && touchAreaC2.m_touchCount < touchAreaC2.m_maxTouches)
                    {
                        if (touchAreaC2.m_cancelOtherTouches)
                        {
                            CancelAllTouches(tLTouch);
                        }
                        if (touchAreaC2.m_touchCount == 0)
                        {
                            touchAreaC2.m_wasDragged = false;
                        }
                        tLTouch.m_primaryArea = touchAreaC2;
                        tLTouch.m_primaryPhase = TouchAreaPhase.Began;
                        tLTouch.m_primaryArea.m_touchCount++;
                        HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                        flag2 = true;
                    }
                    if (!flag2 && tLTouch.m_primaryArea != null)
                    {
                        if (tLTouch.m_phase == TouchPhase.Ended || tLTouch.m_phase == TouchPhase.Canceled)
                        {
                            if (tLTouch.m_primaryArea.m_wasDragged)
                            {
                                tLTouch.m_primaryPhase = TouchAreaPhase.DragEnd;
                                HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                            }
                            tLTouch.m_primaryPhase = TouchAreaPhase.ReleaseOut;
                            tLTouch.m_primaryArea.m_touchCount--;
                            HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                        }
                        else
                        {
                            if (tLTouch.m_primaryPhase == TouchAreaPhase.Began || tLTouch.m_primaryPhase == TouchAreaPhase.RollIn || tLTouch.m_primaryPhase == TouchAreaPhase.MoveIn || tLTouch.m_primaryPhase == TouchAreaPhase.StationaryIn)
                            {
                                tLTouch.m_primaryPhase = TouchAreaPhase.RollOut;
                                HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                            }
                            if (tLTouch.m_phase == TouchPhase.Moved)
                            {
                                if (!tLTouch.m_dragged && Vector2.Distance(tLTouch.m_startPosition, tLTouch.m_currentPosition) > tLTouch.m_primaryArea.m_dragThreshold)
                                {
                                    tLTouch.m_dragged = true;
                                    tLTouch.m_primaryArea.m_wasDragged = true;
                                    tLTouch.m_primaryPhase = TouchAreaPhase.DragStart;
                                    HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                                    if (tLTouch.m_secondaryArea != null)
                                    {
                                        tLTouch.m_secondaryArea.m_wasDragged = true;
                                        TouchAreaPhase secondaryPhase = tLTouch.m_secondaryPhase;
                                        tLTouch.m_secondaryPhase = TouchAreaPhase.DragStart;
                                        HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                        tLTouch.m_secondaryPhase = secondaryPhase;
                                    }
                                }
                                if (tLTouch.m_primaryArea.m_wasDragged)
                                {
                                    tLTouch.m_primaryArea.m_isDragged = true;
                                }
                                tLTouch.m_primaryPhase = TouchAreaPhase.MoveOut;
                                HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                            }
                            else if (tLTouch.m_phase == TouchPhase.Stationary)
                            {
                                tLTouch.m_primaryPhase = TouchAreaPhase.StationaryOut;
                                HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                                if (tLTouch.m_primaryArea.m_wasDragged)
                                {
                                    tLTouch.m_primaryArea.m_isDragged = false;
                                }
                            }
                        }
                    }
                    else if (tLTouch.m_primaryArea != null)
                    {
                        if (tLTouch.m_phase == TouchPhase.Ended || tLTouch.m_phase == TouchPhase.Canceled)
                        {
                            if (tLTouch.m_primaryArea.m_wasDragged)
                            {
                                tLTouch.m_primaryPhase = TouchAreaPhase.DragEnd;
                                HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                            }
                            tLTouch.m_primaryPhase = TouchAreaPhase.ReleaseIn;
                            tLTouch.m_primaryArea.m_touchCount--;
                            HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                        }
                        else
                        {
                            if (tLTouch.m_primaryPhase == TouchAreaPhase.RollOut || tLTouch.m_primaryPhase == TouchAreaPhase.MoveOut || tLTouch.m_primaryPhase == TouchAreaPhase.StationaryOut)
                            {
                                tLTouch.m_primaryPhase = TouchAreaPhase.RollIn;
                                HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                            }
                            if (tLTouch.m_phase == TouchPhase.Moved)
                            {
                                if (!tLTouch.m_dragged && Vector2.Distance(tLTouch.m_startPosition, tLTouch.m_currentPosition) > tLTouch.m_primaryArea.m_dragThreshold)
                                {
                                    tLTouch.m_dragged = true;
                                    tLTouch.m_primaryArea.m_wasDragged = true;
                                    tLTouch.m_primaryPhase = TouchAreaPhase.DragStart;
                                    HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                                    if (tLTouch.m_secondaryArea != null)
                                    {
                                        tLTouch.m_secondaryArea.m_wasDragged = true;
                                        TouchAreaPhase secondaryPhase2 = tLTouch.m_secondaryPhase;
                                        tLTouch.m_secondaryPhase = TouchAreaPhase.DragStart;
                                        HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                        tLTouch.m_secondaryPhase = secondaryPhase2;
                                    }
                                }
                                if (tLTouch.m_primaryArea.m_wasDragged)
                                {
                                    tLTouch.m_primaryArea.m_isDragged = true;
                                }
                                tLTouch.m_primaryPhase = TouchAreaPhase.MoveIn;
                                HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                            }
                            else if (tLTouch.m_phase == TouchPhase.Stationary)
                            {
                                tLTouch.m_primaryPhase = TouchAreaPhase.StationaryIn;
                                HandleTouch(tLTouch.m_primaryArea, tLTouch, false);
                                if (tLTouch.m_primaryArea.m_wasDragged)
                                {
                                    tLTouch.m_primaryArea.m_isDragged = false;
                                }
                            }
                        }
                    }
                    if (!flag3)
                    {
                        if (tLTouch.m_secondaryArea != null)
                        {
                            if (tLTouch.m_secondaryPhase != TouchAreaPhase.RollOut)
                            {
                                tLTouch.m_secondaryPhase = TouchAreaPhase.RollOut;
                                HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                if (tLTouch.m_secondaryArea != tLTouch.m_lockedSecondary)
                                {
                                    tLTouch.m_secondaryArea.m_touchCount--;
                                    tLTouch.m_secondaryArea = null;
                                }
                            }
                            else if (tLTouch.m_phase == TouchPhase.Moved)
                            {
                                TouchAreaPhase secondaryPhase3;
                                if (!tLTouch.m_dragged && Vector2.Distance(tLTouch.m_startPosition, tLTouch.m_currentPosition) > tLTouch.m_secondaryArea.m_dragThreshold)
                                {
                                    tLTouch.m_dragged = true;
                                    tLTouch.m_secondaryArea.m_wasDragged = true;
                                    secondaryPhase3 = tLTouch.m_secondaryPhase;
                                    tLTouch.m_secondaryPhase = TouchAreaPhase.DragStart;
                                    HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                    tLTouch.m_secondaryPhase = secondaryPhase3;
                                }
                                if (tLTouch.m_secondaryArea.m_wasDragged)
                                {
                                    tLTouch.m_secondaryArea.m_isDragged = true;
                                }
                                secondaryPhase3 = tLTouch.m_secondaryPhase;
                                tLTouch.m_secondaryPhase = TouchAreaPhase.MoveOut;
                                HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                tLTouch.m_secondaryPhase = secondaryPhase3;
                            }
                            else if (tLTouch.m_phase == TouchPhase.Stationary)
                            {
                                TouchAreaPhase secondaryPhase4 = tLTouch.m_secondaryPhase;
                                tLTouch.m_secondaryPhase = TouchAreaPhase.StationaryOut;
                                HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                tLTouch.m_secondaryPhase = secondaryPhase4;
                                if (tLTouch.m_secondaryArea.m_wasDragged)
                                {
                                    tLTouch.m_secondaryArea.m_isDragged = false;
                                }
                            }
                        }
                        if (tLTouch.m_lockedSecondary == null)
                        {
                            if (touchAreaC2 != null && touchAreaC2.m_allowSecondary && touchAreaC2 != tLTouch.m_primaryArea)
                            {
                                if (touchAreaC2.m_touchCount < touchAreaC2.m_maxTouches)
                                {
                                    if (touchAreaC2.m_cancelOtherTouches)
                                    {
                                        CancelAllTouches(tLTouch);
                                    }
                                    if (touchAreaC2.m_touchCount == 0)
                                    {
                                        touchAreaC2.m_wasDragged = false;
                                    }
                                    tLTouch.m_secondaryArea = touchAreaC2;
                                    tLTouch.m_secondaryArea.m_touchCount++;
                                    tLTouch.m_secondaryPhase = TouchAreaPhase.RollIn;
                                    HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                }
                            }
                            else if (touchAreaC != null && touchAreaC.m_allowSecondary && touchAreaC != tLTouch.m_primaryArea && touchAreaC.m_touchCount < touchAreaC.m_maxTouches)
                            {
                                if (touchAreaC.m_cancelOtherTouches)
                                {
                                    CancelAllTouches(tLTouch);
                                }
                                if (touchAreaC.m_touchCount == 0)
                                {
                                    touchAreaC.m_wasDragged = false;
                                }
                                tLTouch.m_secondaryArea = touchAreaC;
                                tLTouch.m_secondaryArea.m_touchCount++;
                                tLTouch.m_secondaryPhase = TouchAreaPhase.RollIn;
                                HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                            }
                        }
                        if ((tLTouch.m_phase == TouchPhase.Ended || tLTouch.m_phase == TouchPhase.Canceled) && tLTouch.m_secondaryArea != null)
                        {
                            if (tLTouch.m_dragged)
                            {
                                TouchAreaPhase secondaryPhase5 = tLTouch.m_secondaryPhase;
                                tLTouch.m_secondaryPhase = TouchAreaPhase.DragEnd;
                                HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                tLTouch.m_secondaryPhase = secondaryPhase5;
                            }
                            tLTouch.m_secondaryArea.m_touchCount--;
                            if (tLTouch.m_lockedSecondary == tLTouch.m_secondaryArea)
                            {
                                tLTouch.m_secondaryPhase = TouchAreaPhase.ReleaseOut;
                            }
                            else
                            {
                                tLTouch.m_secondaryPhase = TouchAreaPhase.RollOut;
                            }
                            HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                            tLTouch.m_lockedSecondary = null;
                            tLTouch.m_secondaryArea = null;
                        }
                    }
                    else if (tLTouch.m_secondaryArea != null)
                    {
                        if (tLTouch.m_phase == TouchPhase.Ended || tLTouch.m_phase == TouchPhase.Canceled)
                        {
                            if (tLTouch.m_dragged)
                            {
                                TouchAreaPhase secondaryPhase6 = tLTouch.m_secondaryPhase;
                                tLTouch.m_secondaryPhase = TouchAreaPhase.DragEnd;
                                HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                tLTouch.m_secondaryPhase = secondaryPhase6;
                            }
                            tLTouch.m_secondaryArea.m_touchCount--;
                            tLTouch.m_secondaryPhase = TouchAreaPhase.RollOut;
                            HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                            tLTouch.m_lockedSecondary = null;
                            tLTouch.m_secondaryArea = null;
                        }
                        else
                        {
                            if (tLTouch.m_secondaryPhase == TouchAreaPhase.RollOut && (tLTouch.m_secondaryArea.m_touchCount < tLTouch.m_secondaryArea.m_maxTouches || (tLTouch.m_secondaryArea.m_touchCount == tLTouch.m_secondaryArea.m_touchCount && tLTouch.m_lockedSecondary == tLTouch.m_secondaryArea)))
                            {
                                if (tLTouch.m_secondaryArea != tLTouch.m_lockedSecondary)
                                {
                                    tLTouch.m_secondaryArea.m_touchCount++;
                                }
                                tLTouch.m_secondaryPhase = TouchAreaPhase.RollIn;
                                HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                            }
                            if (tLTouch.m_phase == TouchPhase.Moved)
                            {
                                if (!tLTouch.m_dragged && Vector2.Distance(tLTouch.m_startPosition, tLTouch.m_currentPosition) > tLTouch.m_secondaryArea.m_dragThreshold)
                                {
                                    tLTouch.m_dragged = true;
                                    tLTouch.m_secondaryArea.m_wasDragged = true;
                                    TouchAreaPhase secondaryPhase7 = tLTouch.m_secondaryPhase;
                                    tLTouch.m_secondaryPhase = TouchAreaPhase.DragStart;
                                    HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                    tLTouch.m_secondaryPhase = secondaryPhase7;
                                }
                                if (tLTouch.m_secondaryArea.m_wasDragged)
                                {
                                    tLTouch.m_secondaryArea.m_isDragged = true;
                                }
                                tLTouch.m_secondaryPhase = TouchAreaPhase.MoveIn;
                                HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                            }
                            else if (tLTouch.m_phase == TouchPhase.Stationary)
                            {
                                tLTouch.m_secondaryPhase = TouchAreaPhase.StationaryIn;
                                HandleTouch(tLTouch.m_secondaryArea, tLTouch, true);
                                if (tLTouch.m_secondaryArea.m_wasDragged)
                                {
                                    tLTouch.m_secondaryArea.m_isDragged = false;
                                }
                            }
                        }
                    }
                }
            }
            if (tLTouch.m_phase == TouchPhase.Ended)
            {
                m_touchRemoveList.Add(tLTouch);
            }
        }
        for (int num11 = 0; num11 < m_touchEventCount; num11++)
        {
            TEvent tEvent = m_touchEvents[num11];
            if (tEvent.m_touchArea.m_delegatedCount > 0 && !m_disabled)
            {
                tEvent.m_touchArea.d_TouchEventDelegate(tEvent.m_touchArea, tEvent.m_touchPhases[0], tEvent.m_touchSecondaries[0], tEvent.m_touchCount, tEvent.m_touches);
            }
            tEvent.m_touchCount = 0;
        }
        m_touchEventCount = 0;
    }

    private static void HandleTouch(TouchAreaC _touchArea, TLTouch _touch, bool _secondary)
    {
        if (m_disabled || _touchArea.m_delegatedCount == 0)
        {
            return;
        }
        TEvent tEvent = null;
        for (int i = 0; i < m_touchEventCount; i++)
        {
            bool flag = false;
            for (int j = 0; j < m_touchEvents[i].m_touchCount; j++)
            {
                if (m_touchEvents[i].m_touches[j] == _touch)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag && _touchArea == m_touchEvents[i].m_touchArea)
            {
                tEvent = m_touchEvents[i];
                tEvent.m_touches[tEvent.m_touchCount] = _touch;
                tEvent.m_touchPhases[tEvent.m_touchCount] = ((!_secondary) ? _touch.m_primaryPhase : _touch.m_secondaryPhase);
                tEvent.m_touchSecondaries[tEvent.m_touchCount] = _secondary;
                tEvent.m_touchCount++;
            }
        }
        if (tEvent == null)
        {
            TEvent tEvent2 = m_touchEvents[m_touchEventCount];
            tEvent2.m_touchArea = _touchArea;
            tEvent2.m_touches[tEvent2.m_touchCount] = _touch;
            tEvent2.m_touchPhases[tEvent2.m_touchCount] = ((!_secondary) ? _touch.m_primaryPhase : _touch.m_secondaryPhase);
            tEvent2.m_touchSecondaries[tEvent2.m_touchCount] = _secondary;
            tEvent2.m_touchCount++;
            m_touchEventCount++;
        }
    }
}