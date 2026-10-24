#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using BBR249Server.Data;

// On iOS the sqlite3 library is compiled into the app (Assets/Plugins/iOS/sqlite/sqlite3.c), so it is reached through "__Internal".
// Used by OfflineServer (Db.NativeApi). Not compiled on the other platforms, where "__Internal" would not link.
public sealed class IosSqliteApi : SqliteApi
{
	[DllImport("__Internal", EntryPoint = "sqlite3_open_v2")]
	private static extern int n_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs);

	[DllImport("__Internal", EntryPoint = "sqlite3_close_v2")]
	private static extern int n_close_v2(IntPtr db);

	[DllImport("__Internal", EntryPoint = "sqlite3_errmsg")]
	private static extern IntPtr n_errmsg(IntPtr db);

	[DllImport("__Internal", EntryPoint = "sqlite3_exec")]
	private static extern int n_exec(IntPtr db, byte[] sql, IntPtr cb, IntPtr arg, out IntPtr err);

	[DllImport("__Internal", EntryPoint = "sqlite3_prepare_v2")]
	private static extern int n_prepare_v2(IntPtr db, byte[] sql, int nByte, out IntPtr stmt, out IntPtr tail);

	[DllImport("__Internal", EntryPoint = "sqlite3_step")]
	private static extern int n_step(IntPtr stmt);

	[DllImport("__Internal", EntryPoint = "sqlite3_finalize")]
	private static extern int n_finalize(IntPtr stmt);

	[DllImport("__Internal", EntryPoint = "sqlite3_changes")]
	private static extern int n_changes(IntPtr db);

	[DllImport("__Internal", EntryPoint = "sqlite3_bind_parameter_index")]
	private static extern int n_bind_parameter_index(IntPtr stmt, byte[] name);

	[DllImport("__Internal", EntryPoint = "sqlite3_bind_null")]
	private static extern int n_bind_null(IntPtr stmt, int idx);

	[DllImport("__Internal", EntryPoint = "sqlite3_bind_int64")]
	private static extern int n_bind_int64(IntPtr stmt, int idx, long v);

	[DllImport("__Internal", EntryPoint = "sqlite3_bind_double")]
	private static extern int n_bind_double(IntPtr stmt, int idx, double v);

	[DllImport("__Internal", EntryPoint = "sqlite3_bind_text")]
	private static extern int n_bind_text(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor);

	[DllImport("__Internal", EntryPoint = "sqlite3_bind_blob")]
	private static extern int n_bind_blob(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor);

	[DllImport("__Internal", EntryPoint = "sqlite3_bind_zeroblob")]
	private static extern int n_bind_zeroblob(IntPtr stmt, int idx, int n);

	[DllImport("__Internal", EntryPoint = "sqlite3_column_count")]
	private static extern int n_column_count(IntPtr stmt);

	[DllImport("__Internal", EntryPoint = "sqlite3_column_name")]
	private static extern IntPtr n_column_name(IntPtr stmt, int i);

	[DllImport("__Internal", EntryPoint = "sqlite3_column_type")]
	private static extern int n_column_type(IntPtr stmt, int i);

	[DllImport("__Internal", EntryPoint = "sqlite3_column_int64")]
	private static extern long n_column_int64(IntPtr stmt, int i);

	[DllImport("__Internal", EntryPoint = "sqlite3_column_double")]
	private static extern double n_column_double(IntPtr stmt, int i);

	[DllImport("__Internal", EntryPoint = "sqlite3_column_text")]
	private static extern IntPtr n_column_text(IntPtr stmt, int i);

	[DllImport("__Internal", EntryPoint = "sqlite3_column_blob")]
	private static extern IntPtr n_column_blob(IntPtr stmt, int i);

	[DllImport("__Internal", EntryPoint = "sqlite3_column_bytes")]
	private static extern int n_column_bytes(IntPtr stmt, int i);

	public override int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs)
	{
		return n_open_v2(filename, out db, flags, vfs);
	}

	public override int sqlite3_close_v2(IntPtr db)
	{
		return n_close_v2(db);
	}

	public override IntPtr sqlite3_errmsg(IntPtr db)
	{
		return n_errmsg(db);
	}

	public override int sqlite3_exec(IntPtr db, byte[] sql, IntPtr cb, IntPtr arg, out IntPtr err)
	{
		return n_exec(db, sql, cb, arg, out err);
	}

	public override int sqlite3_prepare_v2(IntPtr db, byte[] sql, int nByte, out IntPtr stmt, out IntPtr tail)
	{
		return n_prepare_v2(db, sql, nByte, out stmt, out tail);
	}

	public override int sqlite3_step(IntPtr stmt)
	{
		return n_step(stmt);
	}

	public override int sqlite3_finalize(IntPtr stmt)
	{
		return n_finalize(stmt);
	}

	public override int sqlite3_changes(IntPtr db)
	{
		return n_changes(db);
	}

	public override int sqlite3_bind_parameter_index(IntPtr stmt, byte[] name)
	{
		return n_bind_parameter_index(stmt, name);
	}

	public override int sqlite3_bind_null(IntPtr stmt, int idx)
	{
		return n_bind_null(stmt, idx);
	}

	public override int sqlite3_bind_int64(IntPtr stmt, int idx, long v)
	{
		return n_bind_int64(stmt, idx, v);
	}

	public override int sqlite3_bind_double(IntPtr stmt, int idx, double v)
	{
		return n_bind_double(stmt, idx, v);
	}

	public override int sqlite3_bind_text(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor)
	{
		return n_bind_text(stmt, idx, v, n, destructor);
	}

	public override int sqlite3_bind_blob(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor)
	{
		return n_bind_blob(stmt, idx, v, n, destructor);
	}

	public override int sqlite3_bind_zeroblob(IntPtr stmt, int idx, int n)
	{
		return n_bind_zeroblob(stmt, idx, n);
	}

	public override int sqlite3_column_count(IntPtr stmt)
	{
		return n_column_count(stmt);
	}

	public override IntPtr sqlite3_column_name(IntPtr stmt, int i)
	{
		return n_column_name(stmt, i);
	}

	public override int sqlite3_column_type(IntPtr stmt, int i)
	{
		return n_column_type(stmt, i);
	}

	public override long sqlite3_column_int64(IntPtr stmt, int i)
	{
		return n_column_int64(stmt, i);
	}

	public override double sqlite3_column_double(IntPtr stmt, int i)
	{
		return n_column_double(stmt, i);
	}

	public override IntPtr sqlite3_column_text(IntPtr stmt, int i)
	{
		return n_column_text(stmt, i);
	}

	public override IntPtr sqlite3_column_blob(IntPtr stmt, int i)
	{
		return n_column_blob(stmt, i);
	}

	public override int sqlite3_column_bytes(IntPtr stmt, int i)
	{
		return n_column_bytes(stmt, i);
	}
}
#endif
