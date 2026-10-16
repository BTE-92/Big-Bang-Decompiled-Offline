using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Pool : GraphNode, ISerializable, IDeserializationCallback
{
	public Pool()
		: base(GraphNodeType.Normal)
	{
		m_name = "Pool";
	}

	public Pool(SerializationInfo info, StreamingContext ctxt)
		: base(info, ctxt)
	{
	}

	public override GraphElement DeepCopy()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Position = 0L;
			Pool pool = (Pool)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(pool, this);
			return pool;
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
	}
}
