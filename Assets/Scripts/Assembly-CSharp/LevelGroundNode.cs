using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class LevelGroundNode : GraphNode
{
	public const int LAYERS = 6;

	public AutoGeometryLayer[] m_AGLayer = new AutoGeometryLayer[6];

	public byte[][] m_AGLayerData = new byte[6][];

	public static int m_AGLayerCount;

	public Entity m_entity;

	public LevelGroundNode()
		: base(GraphNodeType.Invisible, null, "LevelGround", Vector3.zero, Vector3.zero, Vector3.one)
	{
		m_assembleClassType = typeof(BasicAssembledClass);
		m_AGLayerCount = m_AGLayerData.Length;
	}

	public LevelGroundNode(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
		try
		{
			m_AGLayerCount = (int)info.GetValue("layerCount", typeof(int));
			if (m_AGLayerCount > 6)
			{
				m_AGLayerCount = 6;
			}
		}
		catch
		{
			m_AGLayerCount = 6;
		}
		m_AGLayer = new AutoGeometryLayer[6];
		m_AGLayerData = new byte[6][];
		for (int i = 0; i < m_AGLayerCount; i++)
		{
			m_AGLayerData[i] = (byte[])info.GetValue("layer" + i + "Data", typeof(byte[]));
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
		info.AddValue("layerCount", m_AGLayerCount);
		for (int i = 0; i < m_AGLayerCount; i++)
		{
			info.AddValue("layer" + i + "Data", m_AGLayer[i].m_bytes);
		}
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			LevelGroundNode levelGroundNode = (LevelGroundNode)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(levelGroundNode, this);
			return levelGroundNode;
		}
	}

	public override void Assemble()
	{
		Debug.LogWarning("assemble ground");
		if (!m_assembled)
		{
			AutoGeometryManager.Free();
			AutoGeometryManager.Initialize(LevelManager.m_currentLevel.m_currentLayer.m_layerWidth, LevelManager.m_currentLevel.m_currentLayer.m_layerHeight, true);
			Ground[] array = new Ground[6]
			{
				new SandGround(this),
				new IceGround(this),
				new MudGround(this),
				new MetalGround(this),
				new DangerousGround(this),
				new LavaGround(this)
			};
			bool flag = m_AGLayerData[0] != null;
			for (int i = 0; i < array.Length; i++)
			{
				m_AGLayer[i] = AutoGeometryManager.AddLayer(array[i], 16, 17, 3f);
				m_AGLayer[i].m_index = i;
				if (flag && i < m_AGLayerCount)
				{
					m_AGLayerData[i].CopyTo(m_AGLayer[i].m_bytes, 0);
					m_AGLayer[i].MarchTiles(new cpBB(0f, 0f, AutoGeometryManager.m_width, AutoGeometryManager.m_height));
					m_AGLayer[i].TakeSnapshot();
					m_AGLayer[i].CopyByteArrayToMaskTexture(m_AGLayer[i].m_maskTexture, m_AGLayer[i].m_bytes);
					if (!m_AGLayer[i].m_groundC.m_ground.m_destructible)
					{
						m_AGLayerData[i] = null;
						GC.Collect();
					}
					Debug.Log("Marching tiles...");
				}
				else if (m_AGLayer[i].m_groundC.m_ground.m_destructible)
				{
					m_AGLayerData[i] = new byte[m_AGLayer[i].m_bytes.Length];
				}
				else
				{
					m_AGLayerData[i] = null;
				}
			}
			AutoGeometryManager.ClearTileDirtyFlags();
			AutoGeometryManager.SetLayerShaders(PsState.m_activeGameLoop.m_context != PsMinigameContext.Editor && PsState.m_activeGameLoop.m_context != PsMinigameContext.EditorPuzzle && PsState.m_activeGameLoop.m_context != PsMinigameContext.EditorTemplate);
			m_assembled = true;
		}
		else
		{
			AutoGeometryManager.SetLayerShaders(PsState.m_activeGameLoop.m_context != PsMinigameContext.Editor && PsState.m_activeGameLoop.m_context != PsMinigameContext.EditorPuzzle && PsState.m_activeGameLoop.m_context != PsMinigameContext.EditorTemplate);
		}
	}

	public void SaveGroundBeforePlay()
	{
		for (int i = 0; i < m_AGLayerCount; i++)
		{
			if (m_AGLayer[i].m_groundC.m_ground.m_destructible)
			{
				Array.Copy(m_AGLayer[i].m_bytes, m_AGLayerData[i], m_AGLayer[i].m_bytes.Length);
			}
		}
		AutoGeometryManager.ClearTileDirtyFlags();
	}

	public void RevertGroundFromPlay()
	{
		for (int i = 0; i < m_AGLayerCount; i++)
		{
			if (m_AGLayer[i].m_groundC.m_ground.m_destructible)
			{
				m_AGLayer[i].RevertAllDirtyTiles(m_AGLayerData[i]);
			}
		}
		AutoGeometryManager.ClearTileDirtyFlags();
	}

	public override void Clear(bool _isReset)
	{
		if (!_isReset)
		{
			Debug.Log("REMOVING ALL LEVEL GROUNDS");
			AutoGeometryManager.DestroyAllLayers();
			(LevelManager.m_currentLevel as Minigame).m_groundNode = null;
			for (int i = 0; i < m_AGLayerCount; i++)
			{
				m_AGLayer[i] = null;
				m_AGLayerData[i] = null;
			}
			m_AGLayerData = null;
			m_AGLayer = null;
			base.Clear(_isReset);
		}
	}
}
