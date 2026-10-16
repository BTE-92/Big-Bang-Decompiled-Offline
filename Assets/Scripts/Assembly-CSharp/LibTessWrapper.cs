using System;
using System.Runtime.InteropServices;

internal static class LibTessWrapper
{
	public enum WindingRule
	{
		EvenOdd = 0,
		NonZero = 1,
		Positive = 2,
		Negative = 3,
		AbsGeqTwo = 4
	}

	public enum ElementType
	{
		Polygons = 0,
		ConnectedPolygons = 1,
		BoundaryContours = 2
	}

	private const string lookFrom = "chipmunk";

	[DllImport("chipmunk")]
	public static extern IntPtr tessNewTess(IntPtr alloc);

	[DllImport("chipmunk")]
	public static extern void tessDeleteTess(IntPtr tess);

	[DllImport("chipmunk")]
	public static extern void tessAddContour(IntPtr tess, int size, float[] pointer, int stride, int count);

	[DllImport("chipmunk")]
	public static extern int tessTesselate(IntPtr tess, int windingRule, int elementType, int polySize, int vertexSize, float[] normals);

	[DllImport("chipmunk")]
	public static extern int tessGetVertexCount(IntPtr tess);

	[DllImport("chipmunk")]
	public static extern void wTessGetVertices(IntPtr tess, float[] array, int vertexSize);

	[DllImport("chipmunk")]
	public static extern void wTessGetVertexIndices(IntPtr tess, int[] array);

	[DllImport("chipmunk")]
	public static extern int tessGetElementCount(IntPtr tess);

	[DllImport("chipmunk")]
	public static extern void wTessGetElements(IntPtr tess, int[] array, int elementSize);
}
