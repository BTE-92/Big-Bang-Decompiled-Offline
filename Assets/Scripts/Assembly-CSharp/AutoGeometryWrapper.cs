using System;
using System.Runtime.InteropServices;
using UnityEngine;

internal static class AutoGeometryWrapper
{
	private const string lookFrom = "chipmunk";

	[DllImport("chipmunk")]
	public static extern IntPtr agSimpleSamplerNew(int width, int height, cpBB outputRect, [In][Out] byte[] data);

	[DllImport("chipmunk")]
	public static extern void agSimpleSamplerFree(IntPtr sampler);

	[DllImport("chipmunk")]
	public static extern void agSimpleSamplerSetProperties(IntPtr sampler, float marchTreshold);

	[DllImport("chipmunk")]
	public static extern IntPtr agCachedTileGetPolylineSet(IntPtr tile);

	[DllImport("chipmunk")]
	public static extern cpBB agCachedTileGetBB(IntPtr tile);

	[DllImport("chipmunk")]
	public static extern void agCachedTileSetDirty(IntPtr tile, bool dirty);

	[DllImport("chipmunk")]
	public static extern int agPolylineSetGetLineCount(IntPtr set);

	[DllImport("chipmunk")]
	public static extern void agPolylineSetGetLines(IntPtr set, IntPtr[] polylineArray);

	[DllImport("chipmunk")]
	public static extern int agPolylineGetVertCount(IntPtr polyline);

	[DllImport("chipmunk")]
	public static extern void agPolyLineGetVertices(IntPtr polyline, [In][Out] Vector2[] vertexArray);

	[DllImport("chipmunk")]
	public static extern bool agPolylineIsLooped(IntPtr polyline);

	[DllImport("chipmunk")]
	public static extern IntPtr agBasicTileCacheNew(IntPtr sampler, IntPtr space, float tileSize, int maxTiles, float samplesPerTile, bool extendedDirtyList);

	[DllImport("chipmunk")]
	public static extern void agBasicTileCacheFree(IntPtr cache);

	[DllImport("chipmunk")]
	public static extern void agBasicTileCacheResetCache(IntPtr cache);

	[DllImport("chipmunk")]
	public static extern void agBasicTileCacheMarkDirtyRect(IntPtr cache, cpBB bounds);

	[DllImport("chipmunk")]
	public static extern void agBasicTileCacheEnsureRect(IntPtr cache, cpBB bounds);

	[DllImport("chipmunk")]
	public static extern int agBasicTileCacheGetTileCountInRect(IntPtr cache, cpBB bounds);

	[DllImport("chipmunk")]
	public static extern void agBasicTileCacheGetTilesInRect(IntPtr cache, cpBB bounds, IntPtr[] tileListArray);

	[DllImport("chipmunk")]
	public static extern void agBasicTileCacheSetOffsets(IntPtr cache, Vector2 worldOffset, Vector2 tileOffset);

	[DllImport("chipmunk")]
	public static extern void agBasicTileCacheSetMarchProperties(IntPtr cache, bool marchHard, float simplifyTreshold);

	[DllImport("chipmunk")]
	public static extern void agBasicTileCacheSetSegmentProperties(IntPtr cache, float segmentRadius, float segmentFriction, float segmentElasticity, ucpShapeFilter segmentFilter, uint segmentCollisionType);

	[DllImport("chipmunk")]
	public static extern int agBasicTileCacheGetDirtyTileCount(IntPtr cache);

	[DllImport("chipmunk")]
	public static extern void agBasicTileCacheGetDirtyTileList(IntPtr cache, IntPtr[] tileListArray);
}
