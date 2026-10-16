using System;

public class ChipmunkBodyShape
{
	public IntPtr shapePtr;

	public uint layerMask;

	public string tag;

	public ChipmunkBodyShape(IntPtr _shapePtr, uint _layers, string _tag)
	{
		shapePtr = _shapePtr;
		layerMask = _layers;
		tag = _tag;
	}
}
