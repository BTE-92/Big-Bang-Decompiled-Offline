using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class StateConnection : GraphConnection, ISerializable, IDeserializationCallback
{
	public StateConnection()
		: base("StateConnection")
	{
		m_connectionType = ConnectionType.State;
	}

	public StateConnection(SerializationInfo info, StreamingContext ctxt)
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
			StateConnection stateConnection = (StateConnection)binaryFormatter.Deserialize(memoryStream);
			CopyAttributes(stateConnection, this);
			return stateConnection;
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
		base.GetObjectData(info, ctxt);
	}
}
