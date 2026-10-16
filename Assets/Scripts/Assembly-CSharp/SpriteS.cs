using System;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteS
{
	public static int m_resizeTolerance = 64;

	public static GenericArray<SpriteSheet> m_sheets;

	private static SpriteC[] p_sprites;

	private static Vector3[] p_vertices;

	private static int[] p_aliveIndices;

	public static Vector3 m_outOfScreen = new Vector3(99999f, 99999f, -99999f);

	public static Color m_defaultColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	private static int debug_rotationUpdates;

	private static int debug_scaleUpdates;

	private static int debug_positionUpdates;

	public static void Initialize()
	{
		m_sheets = new GenericArray<SpriteSheet>(50);
	}

	public static SpriteSheet AddSpriteSheet(Camera _camera, Material _material, float _globalSpriteScale)
	{
		return AddSpriteSheet(_camera, _material, null, _globalSpriteScale);
	}

	public static SpriteSheet AddSpriteSheet(Camera _camera, Material _material, TextAsset _atlas, float _globalSpriteScale)
	{
		SpriteSheet spriteSheet = new SpriteSheet(_camera, _material, _atlas, _globalSpriteScale);
		int num = m_sheets.AddItem(spriteSheet);
		m_sheets.m_array[num].m_index = num;
		return spriteSheet;
	}

	public static SpriteSheet AddSpriteSheet(Camera _camera, Texture _texture, Shader _shader, float _globalSpriteScale)
	{
		SpriteSheet spriteSheet = new SpriteSheet(_camera, _texture, _shader, _globalSpriteScale);
		int num = m_sheets.AddItem(spriteSheet);
		m_sheets.m_array[num].m_index = num;
		return spriteSheet;
	}

	public static void RemoveSpriteSheet(SpriteSheet _sheet)
	{
		RemoveAllComponentsFromSheet(_sheet);
		_sheet.Destroy();
		UnityEngine.Object.Destroy(_sheet.m_gameObject);
		_sheet.m_gameObject = null;
		_sheet.m_atlas = null;
		UnityEngine.Object.Destroy(_sheet.m_mesh);
		_sheet.m_mesh = null;
		UnityEngine.Object.Destroy(_sheet.m_meshFilter);
		UnityEngine.Object.Destroy(_sheet.m_meshRenderer);
		_sheet.m_meshFilter = null;
		_sheet.m_meshRenderer = null;
		m_sheets.RemoveItem(_sheet.m_index);
		m_sheets.m_array[_sheet.m_index] = null;
	}

	public static SpriteC AddComponent(TransformC _transformComponent, string _frameName, SpriteSheet _sheet)
	{
		Frame frame = _sheet.m_atlas.GetFrame(_frameName);
		return AddComponent(_transformComponent, frame, _sheet);
	}

	public static SpriteC AddComponent(TransformC _transformComponent, Frame _frame, SpriteSheet _sheet)
	{
		if (_sheet.m_components.m_aliveCount == 0)
		{
			_sheet.m_gameObject.SetActive(true);
		}
		SpriteC spriteC = _sheet.m_components.AddItem();
		if (_sheet.m_components.m_aliveCount < _sheet.m_currentMeshCapacity)
		{
			spriteC.vertDataIndex = _sheet.m_freeVertexIndices[_sheet.m_freeVertexIndicesCount - 1];
			_sheet.m_freeVertexIndicesCount--;
		}
		else
		{
			spriteC.vertDataIndex = -1;
		}
		spriteC.p_TC = _transformComponent;
		spriteC.p_spriteSheet = _sheet;
		SetFrame(_sheet, spriteC, _frame);
		EntityManager.AddComponentToEntity(_transformComponent.p_entity, spriteC);
		return spriteC;
	}

	public static void RemoveComponent(SpriteC _c)
	{
		if (_c.p_entity == null)
		{
			Debug.LogWarning("Trying to remove component that has already been removed");
			return;
		}
		SpriteSheet p_spriteSheet = _c.p_spriteSheet;
		if (_c.vertDataIndex >= 0)
		{
			int num = _c.vertDataIndex * 4;
			for (int i = 0; i < 4; i++)
			{
				p_spriteSheet.m_vertices[num + i] = m_outOfScreen;
			}
			p_spriteSheet.m_vertsChanged = true;
			p_spriteSheet.m_freeVertexIndices[p_spriteSheet.m_freeVertexIndicesCount] = _c.vertDataIndex;
			p_spriteSheet.m_freeVertexIndicesCount++;
		}
		_c.p_TC = null;
		_c.p_spriteSheet = null;
		_c.sortValue = 0f;
		EntityManager.RemoveComponentFromEntity(_c);
		p_spriteSheet.m_components.RemoveItem(_c);
		if (p_spriteSheet.m_components.m_aliveCount == 0)
		{
			p_spriteSheet.m_gameObject.SetActive(false);
		}
	}

	public static void RemoveComponentsByEntity(Entity _e)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Sprite, _e);
		while (componentsByEntity.Count > 0)
		{
			int index = componentsByEntity.Count - 1;
			RemoveComponent(componentsByEntity[index] as SpriteC);
			componentsByEntity.RemoveAt(index);
		}
	}

	public static void RemoveAllComponentsFromSheet(SpriteSheet _sheet)
	{
		while (_sheet.m_components.m_aliveCount > 0)
		{
			SpriteC c = _sheet.m_components.m_array[_sheet.m_components.m_aliveIndices[0]];
			RemoveComponent(c);
		}
	}

	public static void RemoveComponentsByTransformComponent(TransformC _tc)
	{
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Sprite, _tc.p_entity);
		for (int num = componentsByEntity.Count - 1; num > -1; num--)
		{
			SpriteC spriteC = componentsByEntity[num] as SpriteC;
			if (spriteC.p_TC == _tc)
			{
				RemoveComponent(spriteC);
			}
		}
	}

	public static void SetDimensions(SpriteC _c, float _width, float _height)
	{
		_c.wDimension = _width;
		_c.hDimension = _height;
		_c.width = _width * _c.p_spriteSheet.m_globalSpriteScale;
		_c.height = _height * _c.p_spriteSheet.m_globalSpriteScale;
		_c.updateScale = true;
	}

	public static void SetDimensionScale(SpriteC _c, float _scale)
	{
		_c.dimensionScale = _scale;
		_c.updateScale = true;
	}

	public static void SetVisibility(SpriteC _s, bool _visible)
	{
		SetVisibility(_s, _visible, true);
	}

	public static void SetVisibility(SpriteC _s, bool _visible, bool _markVisibility)
	{
		_s.visible = _visible;
		if (_markVisibility)
		{
			_s.wasVisible = _visible;
		}
		_s.updatePosition = true;
	}

	public static void SetVisibilityByTransformComponent(TransformC _tc, bool _visible, bool _affectChildren = false, bool _affectWholeHierarchy = false)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				SetVisibilityByTransformComponent(_tc.childs[i], _visible, true);
			}
		}
		for (int j = 0; j < m_sheets.m_aliveCount; j++)
		{
			SpriteSheet spriteSheet = m_sheets.m_array[m_sheets.m_aliveIndices[j]];
			p_sprites = spriteSheet.m_components.m_array;
			p_aliveIndices = spriteSheet.m_components.m_aliveIndices;
			int aliveCount = spriteSheet.m_components.m_aliveCount;
			for (int k = 0; k < aliveCount; k++)
			{
				SpriteC spriteC = p_sprites[p_aliveIndices[k]];
				if (spriteC.p_TC == _tc)
				{
					SetVisibility(spriteC, _visible);
				}
			}
		}
	}

	public static void SetClip(SpriteC _c, float _left, float _right, float _bottom, float _top)
	{
		_c.clip = true;
		_c.clipLeft = _left;
		_c.clipRight = _right;
		_c.clipBottom = _bottom;
		_c.clipTop = _top;
		_c.updateScale = true;
		_c.updateUVs = true;
	}

	public static void SetAlignment(int _sheetIndex, int _spriteIndex, Align _horizontal, Align _vertical)
	{
		float horizontal = 0f;
		switch (_horizontal)
		{
		case Align.Left:
			horizontal = -0.5f;
			break;
		case Align.Right:
			horizontal = 0.5f;
			break;
		}
		float vertical = 0f;
		switch (_horizontal)
		{
		case Align.Top:
			vertical = 0.5f;
			break;
		case Align.Bottom:
			vertical = -0.5f;
			break;
		}
		SetAlignment(_sheetIndex, _spriteIndex, horizontal, vertical);
	}

	public static void SetAlignment(int _sheetIndex, int _spriteIndex, float _horizontal, float _vertical)
	{
		SpriteC spriteC = m_sheets.m_array[_sheetIndex].m_components.m_array[_spriteIndex];
		spriteC.align.x = (0f - _horizontal) * spriteC.width;
		spriteC.align.y = _vertical * spriteC.height;
		spriteC.updatePosition = true;
	}

	public static void SetOffset(SpriteC _s, Vector3 _pos, float _rot)
	{
		_s.offset = _pos;
		float num = Mathf.Cos(_rot * ((float)Math.PI / 180f));
		float num2 = Mathf.Sin(_rot * ((float)Math.PI / 180f));
		_s.offsetRight = new Vector3(num, num2, 0f);
		_s.offsetUp = new Vector3(0f - num2, num, 0f);
		_s.updateRotation = true;
	}

	public static void SetColor(SpriteC _sprite, Color _color)
	{
		_sprite.color = _color;
		_sprite.updateColors = true;
	}

	public static void SetColorByTransformComponent(TransformC _tc, Color _color, bool _affectChildren = false, bool _affectWholeHierarchy = false)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				SetColorByTransformComponent(_tc.childs[i], _color, true);
			}
		}
		for (int j = 0; j < m_sheets.m_aliveCount; j++)
		{
			SpriteSheet spriteSheet = m_sheets.m_array[m_sheets.m_aliveIndices[j]];
			p_sprites = spriteSheet.m_components.m_array;
			p_aliveIndices = spriteSheet.m_components.m_aliveIndices;
			int aliveCount = spriteSheet.m_components.m_aliveCount;
			for (int k = 0; k < aliveCount; k++)
			{
				SpriteC spriteC = p_sprites[p_aliveIndices[k]];
				if (spriteC.p_TC == _tc)
				{
					SetColor(spriteC, _color);
				}
			}
		}
	}

	public static void SetAlphaByTransformComponent(TransformC _tc, float _alpha, bool _affectChildren = false, bool _affectWholeHierarchy = false)
	{
		if (_affectWholeHierarchy)
		{
			_tc = TransformS.GetRootTransformComponent(_tc);
		}
		if (_affectChildren || _affectWholeHierarchy)
		{
			for (int i = 0; i < _tc.childs.Count; i++)
			{
				SetAlphaByTransformComponent(_tc.childs[i], _alpha, true);
			}
		}
		for (int j = 0; j < m_sheets.m_aliveCount; j++)
		{
			SpriteSheet spriteSheet = m_sheets.m_array[m_sheets.m_aliveIndices[j]];
			p_sprites = spriteSheet.m_components.m_array;
			p_aliveIndices = spriteSheet.m_components.m_aliveIndices;
			int aliveCount = spriteSheet.m_components.m_aliveCount;
			for (int k = 0; k < aliveCount; k++)
			{
				SpriteC spriteC = p_sprites[p_aliveIndices[k]];
				Color color = spriteC.color;
				color.a = _alpha;
				if (spriteC.p_TC == _tc)
				{
					SetColor(spriteC, color);
				}
			}
		}
	}

	public static void SetFlip(SpriteC _c, bool _x, bool _y)
	{
		_c.frame.flipX = _x;
		_c.frame.flipY = _y;
		SetFrame(_c.p_spriteSheet, _c, _c.frame);
	}

	public static void SetSortValue(TransformC _tc, float _sortValue)
	{
		for (int i = 0; i < m_sheets.m_aliveCount; i++)
		{
			SpriteSheet spriteSheet = m_sheets.m_array[m_sheets.m_aliveIndices[i]];
			p_sprites = spriteSheet.m_components.m_array;
			p_aliveIndices = spriteSheet.m_components.m_aliveIndices;
			int aliveCount = spriteSheet.m_components.m_aliveCount;
			for (int j = 0; j < aliveCount; j++)
			{
				SpriteC spriteC = p_sprites[p_aliveIndices[j]];
				if (spriteC.p_TC == _tc)
				{
					SetSortValue(spriteC, _sortValue);
				}
			}
		}
	}

	public static void SetSortValue(SpriteC _s, float _sortValue)
	{
		_s.sortValue = _sortValue;
		_s.p_spriteSheet.m_sortMesh = true;
	}

	public static void SortMesh(SpriteSheet _sheet)
	{
		_sheet.resetVertexIndices();
		int[] array = new int[_sheet.m_components.m_aliveCount];
		float[] array2 = new float[_sheet.m_components.m_aliveCount];
		for (int i = 0; i < _sheet.m_components.m_aliveCount; i++)
		{
			int num = _sheet.m_components.m_aliveIndices[i];
			SpriteC spriteC = _sheet.m_components.m_array[num];
			if (spriteC.vertDataIndex >= 0)
			{
				int num2 = spriteC.vertDataIndex * 4;
				for (int j = 0; j < 4; j++)
				{
					_sheet.m_vertices[num2 + j] = m_outOfScreen;
				}
			}
			array[i] = num;
			array2[i] = spriteC.sortValue;
		}
		array = ToolBox.sortTable(array, array2);
		for (int k = 0; k < _sheet.m_components.m_aliveCount; k++)
		{
			SpriteC spriteC2 = _sheet.m_components.m_array[array[k]];
			spriteC2.vertDataIndex = _sheet.m_freeVertexIndices[_sheet.m_freeVertexIndicesCount - 1];
			_sheet.m_freeVertexIndicesCount--;
		}
	}

	public static void SetFrame(SpriteSheet _sheet, SpriteC _sprite, Frame _frame)
	{
		_sprite.frame = _frame;
		if (_sprite.wDimension > 0f)
		{
			_sprite.width = _sprite.wDimension * _sheet.m_globalSpriteScale;
		}
		else
		{
			_sprite.wDimension = _frame.width;
			_sprite.width = _frame.width * _sheet.m_globalSpriteScale;
		}
		if (_sprite.hDimension > 0f)
		{
			_sprite.height = _sprite.hDimension * _sheet.m_globalSpriteScale;
		}
		else
		{
			_sprite.hDimension = _frame.height;
			_sprite.height = _frame.height * _sheet.m_globalSpriteScale;
		}
		_sprite.updateUVs = true;
	}

	public static void Update()
	{
		for (int i = 0; i < m_sheets.m_aliveCount; i++)
		{
			int num = m_sheets.m_aliveIndices[i];
			SpriteSheet sheet = m_sheets.m_array[num];
			UpdateSheet(sheet);
		}
		debug_rotationUpdates = 0;
		debug_scaleUpdates = 0;
		debug_positionUpdates = 0;
	}

	public static void UpdateSheet(SpriteSheet _sheet)
	{
		if (_sheet.m_currentMeshCapacity <= _sheet.m_components.m_aliveCount || Mathf.Abs(_sheet.m_components.m_aliveCount - _sheet.m_currentMeshCapacity) >= m_resizeTolerance * 2)
		{
			_sheet.setMeshData(_sheet.m_components.m_aliveCount + m_resizeTolerance);
			_sheet.assignVertexDataIndices();
			if (_sheet.m_sortingEnabled)
			{
				_sheet.m_sortMesh = true;
			}
		}
		if (_sheet.m_sortingEnabled && _sheet.m_sortMesh)
		{
			SortMesh(_sheet);
			_sheet.m_sortMesh = false;
			_sheet.m_meshCapacityChanged = true;
		}
		if (_sheet.m_components.m_aliveCount == 0)
		{
			if (_sheet.m_gameObject.activeSelf)
			{
				_sheet.m_gameObject.SetActive(false);
			}
		}
		else
		{
			if (!_sheet.m_gameObject.activeSelf)
			{
				_sheet.m_gameObject.SetActive(true);
			}
			p_sprites = _sheet.m_components.m_array;
			p_vertices = _sheet.m_vertices;
			for (int i = 0; i < _sheet.m_components.m_aliveCount; i++)
			{
				int num = _sheet.m_components.m_aliveIndices[i];
				SpriteC spriteC = _sheet.m_components.m_array[num];
				TransformC p_TC = spriteC.p_TC;
				Transform transform = spriteC.p_TC.transform;
				if (p_TC.updatedPosition)
				{
					spriteC.updatePosition = true;
				}
				if (p_TC.updatedRotation)
				{
					spriteC.updateRotation = true;
				}
				if (p_TC.updatedScale)
				{
					spriteC.updateScale = true;
				}
				if (spriteC.vertDataIndex < 0)
				{
					spriteC.vertDataIndex = _sheet.m_freeVertexIndices[_sheet.m_freeVertexIndicesCount - 1];
					_sheet.m_freeVertexIndicesCount--;
				}
				bool flag = false;
				if (spriteC.updateScale || _sheet.m_meshCapacityChanged)
				{
					if (p_TC.forceScale)
					{
						spriteC.wScale = spriteC.width * spriteC.dimensionScale * 0.5f * p_TC.forcedScale.x;
						spriteC.hScale = spriteC.height * spriteC.dimensionScale * 0.5f * p_TC.forcedScale.y;
					}
					else
					{
						Vector3 lossyScale = transform.lossyScale;
						spriteC.wScale = spriteC.width * spriteC.dimensionScale * 0.5f * lossyScale.x;
						spriteC.hScale = spriteC.height * spriteC.dimensionScale * 0.5f * lossyScale.y;
					}
					spriteC.updatePosition = true;
					spriteC.updateScale = false;
					flag = true;
					debug_scaleUpdates++;
				}
				if (spriteC.updateRotation || _sheet.m_meshCapacityChanged)
				{
					if (p_TC.forceRotation)
					{
						spriteC.relRight = p_TC.forcedRotation * spriteC.offsetRight;
						spriteC.relUp = p_TC.forcedRotation * spriteC.offsetUp;
						spriteC.relOffset = p_TC.forcedRotation * spriteC.offset;
						if (!p_TC.forceScale)
						{
							spriteC.relOffset.Scale(transform.lossyScale);
						}
					}
					else
					{
						Quaternion rotation = transform.rotation;
						spriteC.relRight = rotation * spriteC.offsetRight;
						spriteC.relUp = rotation * spriteC.offsetUp;
						spriteC.relOffset = rotation * spriteC.offset;
						if (!p_TC.forceScale)
						{
							spriteC.relOffset.Scale(transform.lossyScale);
						}
					}
					spriteC.updatePosition = true;
					spriteC.updateRotation = false;
					flag = true;
					debug_rotationUpdates++;
				}
				if (flag)
				{
					spriteC.scaledRelRight = spriteC.relRight * spriteC.wScale;
					spriteC.scaledRelUp = spriteC.relUp * spriteC.hScale;
				}
				if (spriteC.updatePosition)
				{
					int num2 = spriteC.vertDataIndex * 4;
					if (spriteC.visible)
					{
						Vector3 vector = transform.position + spriteC.relOffset;
						if (spriteC.clip)
						{
							p_vertices[num2] = vector + spriteC.scaledRelUp * (2f * (1f - spriteC.clipTop) - 1f) - spriteC.scaledRelRight * (2f * (1f - spriteC.clipLeft) - 1f);
							p_vertices[num2 + 1] = vector - spriteC.scaledRelUp * (2f * (1f - spriteC.clipBottom) - 1f) - spriteC.scaledRelRight * (2f * (1f - spriteC.clipLeft) - 1f);
							p_vertices[num2 + 2] = vector - spriteC.scaledRelUp * (2f * (1f - spriteC.clipBottom) - 1f) + spriteC.scaledRelRight * (2f * (1f - spriteC.clipRight) - 1f);
							p_vertices[num2 + 3] = vector + spriteC.scaledRelUp * (2f * (1f - spriteC.clipTop) - 1f) + spriteC.scaledRelRight * (2f * (1f - spriteC.clipRight) - 1f);
						}
						else
						{
							p_vertices[num2] = vector + spriteC.scaledRelUp - spriteC.scaledRelRight;
							p_vertices[num2 + 1] = vector - spriteC.scaledRelUp - spriteC.scaledRelRight;
							p_vertices[num2 + 2] = vector - spriteC.scaledRelUp + spriteC.scaledRelRight;
							p_vertices[num2 + 3] = vector + spriteC.scaledRelUp + spriteC.scaledRelRight;
						}
					}
					else
					{
						for (int j = 0; j < 4; j++)
						{
							p_vertices[num2 + j] = m_outOfScreen;
						}
					}
					spriteC.updatePosition = false;
					_sheet.m_vertsChanged = true;
					debug_positionUpdates++;
				}
				if (spriteC.updateUVs || _sheet.m_meshCapacityChanged)
				{
					float num3 = spriteC.frame.x / (float)_sheet.m_textureWidth;
					float num4 = (spriteC.frame.x + spriteC.frame.width) / (float)_sheet.m_textureWidth;
					float num5 = 1f - spriteC.frame.y / (float)_sheet.m_textureHeight;
					float num6 = 1f - (spriteC.frame.y + spriteC.frame.height) / (float)_sheet.m_textureHeight;
					if (spriteC.clip)
					{
						float num7 = num4 - num3;
						float num8 = num6 - num5;
						num3 += num7 * spriteC.clipLeft;
						num4 -= num7 * (spriteC.clipLeft + spriteC.clipRight);
						num5 += num8 * spriteC.clipBottom;
						num6 -= num8 * (spriteC.clipBottom + spriteC.clipTop);
					}
					if (spriteC.frame.flipX)
					{
						float num9 = num3;
						num3 = num4;
						num4 = num9;
					}
					if (spriteC.frame.flipY)
					{
						float num10 = num5;
						num5 = num6;
						num6 = num10;
					}
					int num11 = spriteC.vertDataIndex * 4;
					Vector2[] uVs = _sheet.m_UVs;
					uVs[num11 + 3].x = num4;
					uVs[num11 + 3].y = num5;
					uVs[num11 + 2].x = num4;
					uVs[num11 + 2].y = num6;
					uVs[num11 + 1].x = num3;
					uVs[num11 + 1].y = num6;
					uVs[num11].x = num3;
					uVs[num11].y = num5;
					spriteC.updateUVs = false;
					_sheet.m_uvsChanged = true;
				}
				if (spriteC.updateColors || _sheet.m_meshCapacityChanged)
				{
					int num12 = spriteC.vertDataIndex * 4;
					Color[] colors = _sheet.m_colors;
					colors[num12] = spriteC.color;
					colors[num12 + 1] = spriteC.color;
					colors[num12 + 2] = spriteC.color;
					colors[num12 + 3] = spriteC.color;
					spriteC.updateColors = false;
					_sheet.m_colorsChanged = true;
				}
			}
		}
		if (_sheet.m_vertsChanged)
		{
			_sheet.m_mesh.vertices = _sheet.m_vertices;
			_sheet.m_vertsChanged = false;
		}
		if (_sheet.m_colorsChanged)
		{
			_sheet.m_mesh.colors = _sheet.m_colors;
			_sheet.m_colorsChanged = false;
		}
		if (_sheet.m_uvsChanged)
		{
			_sheet.m_mesh.uv = _sheet.m_UVs;
			_sheet.m_uvsChanged = false;
		}
		_sheet.m_meshCapacityChanged = false;
		_sheet.m_components.Update();
	}

	public static List<SpriteC> GetSpritesByTransform(TransformC _tc)
	{
		List<SpriteC> list = new List<SpriteC>();
		Entity p_entity = _tc.p_entity;
		if (p_entity == null)
		{
			return list;
		}
		for (int i = 0; i < p_entity.m_components.Count; i++)
		{
			if (p_entity.m_components[i].m_componentType == ComponentType.Sprite && (p_entity.m_components[i] as SpriteC).p_TC == _tc)
			{
				list.Add(p_entity.m_components[i] as SpriteC);
			}
		}
		return list;
	}

	public static List<PrefabC> ConvertSpritesToPrefabComponent(TransformC _tc, bool _removeSprites)
	{
		return ConvertSpritesToPrefabComponent(_tc, null, _removeSprites);
	}

	public static List<PrefabC> ConvertSpritesToPrefabComponent(TransformC _tc, Camera _camera, bool _removeSprites, Shader _shader = null)
	{
		Update();
		List<PrefabC> list = new List<PrefabC>();
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Sprite, _tc.p_entity);
		List<GameObject> list2 = new List<GameObject>();
		List<SpriteSheet> list3 = new List<SpriteSheet>();
		List<List<int>> list4 = new List<List<int>>();
		if (componentsByEntity.Count > 0)
		{
			for (int i = 0; i < componentsByEntity.Count; i++)
			{
				SpriteC spriteC = componentsByEntity[i] as SpriteC;
				SpriteSheet p_spriteSheet = spriteC.p_spriteSheet;
				bool flag = false;
				int num = 0;
				if (list3.Count > 0)
				{
					for (int j = 0; j < list3.Count; j++)
					{
						if (p_spriteSheet == list3[j])
						{
							flag = true;
							num = j;
							list4[num].Add(i);
							break;
						}
					}
				}
				if (!flag || list3.Count == 0)
				{
					num = list3.Count;
					list3.Add(p_spriteSheet);
					list4.Add(new List<int>());
					list4[num].Add(i);
					list2.Add(new GameObject("ConvertedSprites"));
					if (_camera == null)
					{
						list2[num].layer = p_spriteSheet.m_camera.gameObject.layer;
					}
					else
					{
						list2[num].layer = _camera.gameObject.layer;
					}
					list2[num].AddComponent<MeshFilter>();
					MeshRenderer meshRenderer = list2[num].AddComponent<MeshRenderer>();
					meshRenderer.material = p_spriteSheet.m_material;
					if (_shader != null)
					{
						meshRenderer.material.shader = _shader;
					}
				}
			}
			for (int k = 0; k < list3.Count; k++)
			{
				Vector3[] array = new Vector3[list4[k].Count * 4];
				Vector3[] normals = new Vector3[list4[k].Count * 4];
				Vector2[] array2 = new Vector2[list4[k].Count * 4];
				Color[] array3 = new Color[list4[k].Count * 4];
				int[] array4 = new int[list4[k].Count * 6];
				for (int l = 0; l < list4[k].Count; l++)
				{
					array4[l * 6 + 5] = l * 4;
					array4[l * 6 + 4] = l * 4 + 1;
					array4[l * 6 + 3] = l * 4 + 3;
					array4[l * 6 + 2] = l * 4 + 3;
					array4[l * 6 + 1] = l * 4 + 1;
					array4[l * 6] = l * 4 + 2;
				}
				int[] array5 = new int[list4[k].Count];
				float[] array6 = new float[list4[k].Count];
				for (int m = 0; m < list4[k].Count; m++)
				{
					array5[m] = list4[k][m];
					array6[m] = (componentsByEntity[list4[k][m]] as SpriteC).sortValue;
				}
				int[] array7 = ToolBox.sortTable(array5, array6);
				for (int n = 0; n < list4[k].Count; n++)
				{
					SpriteC spriteC2 = componentsByEntity[array7[n]] as SpriteC;
					int num2 = spriteC2.vertDataIndex * 4;
					int num3 = n * 4;
					Vector3[] vertices = list3[k].m_vertices;
					array[num3] = vertices[num2];
					array[num3 + 1] = vertices[num2 + 1];
					array[num3 + 2] = vertices[num2 + 2];
					array[num3 + 3] = vertices[num2 + 3];
					Vector2[] uVs = list3[k].m_UVs;
					array2[num3 + 3].x = uVs[num2 + 3].x;
					array2[num3 + 3].y = uVs[num2 + 3].y;
					array2[num3 + 2].x = uVs[num2 + 2].x;
					array2[num3 + 2].y = uVs[num2 + 2].y;
					array2[num3 + 1].x = uVs[num2 + 1].x;
					array2[num3 + 1].y = uVs[num2 + 1].y;
					array2[num3].x = uVs[num2].x;
					array2[num3].y = uVs[num2].y;
					Color[] colors = list3[k].m_colors;
					array3[num3] = colors[num2];
					array3[num3 + 1] = colors[num2 + 1];
					array3[num3 + 2] = colors[num2 + 2];
					array3[num3 + 3] = colors[num2 + 3];
					if (_removeSprites)
					{
						RemoveComponent(spriteC2);
					}
				}
				MeshFilter meshFilter = list2[k].GetComponent("MeshFilter") as MeshFilter;
				Mesh mesh = meshFilter.mesh;
				mesh.vertices = array;
				mesh.triangles = array4;
				mesh.uv = array2;
				mesh.colors = array3;
				mesh.normals = normals;
				mesh.RecalculateBounds();
				PrefabC prefabC = PrefabS.AddComponent(_tc, -_tc.transform.position, list2[k], "ConvertedSprites");
				if ((bool)_shader)
				{
					prefabC.p_gameObject.GetComponent<Renderer>().sharedMaterial = new Material(meshFilter.GetComponent<Renderer>().material);
					UnityEngine.Object.DestroyImmediate(meshFilter.GetComponent<Renderer>().material);
				}
				list.Add(prefabC);
				UnityEngine.Object.Destroy(mesh);
				UnityEngine.Object.Destroy(list2[k]);
			}
		}
		return list;
	}

	public static GameObject CloneAsGameObject(SpriteSheet _sheet)
	{
		return UnityEngine.Object.Instantiate(_sheet.m_gameObject);
	}
}
