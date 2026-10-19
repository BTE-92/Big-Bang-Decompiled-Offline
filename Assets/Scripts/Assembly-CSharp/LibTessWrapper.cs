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

#if (UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_WEBGL) && !UNITY_EDITOR
    private const string lookFrom = "__Internal";
#else
    private const string lookFrom = "chipmunk";
#endif

    [DllImport(lookFrom, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr tessNewTess(IntPtr alloc);

    [DllImport(lookFrom, CallingConvention = CallingConvention.Cdecl)]
    public static extern void tessDeleteTess(IntPtr tess);

    [DllImport(lookFrom, CallingConvention = CallingConvention.Cdecl)]
    public static extern void tessAddContour(IntPtr tess, int size, float[] pointer, int stride, int count);

    [DllImport(lookFrom, CallingConvention = CallingConvention.Cdecl)]
    public static extern int tessTesselate(IntPtr tess, int windingRule, int elementType, int polySize, int vertexSize, float[] normals);

    [DllImport(lookFrom, CallingConvention = CallingConvention.Cdecl)]
    public static extern int tessGetVertexCount(IntPtr tess);

    [DllImport(lookFrom, CallingConvention = CallingConvention.Cdecl)]
    public static extern void wTessGetVertices(IntPtr tess, float[] array, int vertexSize);

    [DllImport(lookFrom, CallingConvention = CallingConvention.Cdecl)]
    public static extern void wTessGetVertexIndices(IntPtr tess, int[] array);

    [DllImport(lookFrom, CallingConvention = CallingConvention.Cdecl)]
    public static extern int tessGetElementCount(IntPtr tess);

    [DllImport(lookFrom, CallingConvention = CallingConvention.Cdecl)]
    public static extern void wTessGetElements(IntPtr tess, int[] array, int elementSize);
}
