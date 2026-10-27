#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BBR249Server.Core;

namespace BBR249Server.Data;

// Thin SQLite layer written directly on the sqlite3 C API (P/Invoke), so it works the same on Mono and IL2CPP
// (Windows, Android, iOS). The SQL strings of the server are kept unchanged.
public static class Db
{
	// Library name used for DllImport: "sqlite3" everywhere except iOS, where it is linked statically ("__Internal").
	// See SqliteNative below.
	// iOS: the game passes its own implementation, bound to the sqlite3 compiled into the app
	public static SqliteApi NativeApi
	{
		get { return SqliteNative.Api; }
		set { SqliteNative.Api = value; }
	}

	// PRAGMAs run when the database is opened. WebGL (no shared memory, no threads) replaces them from the game.
	public static string StartupPragmas = "PRAGMA journal_mode=WAL; PRAGMA busy_timeout=5000; PRAGMA foreign_keys=OFF; PRAGMA synchronous=NORMAL;";

	private static readonly object Lock = new object();
	private static IntPtr _db = IntPtr.Zero;

	private static void Check(int rc, string what)
	{
		if (rc != SqliteNative.OK && rc != SqliteNative.ROW && rc != SqliteNative.DONE)
		{
			string msg = _db != IntPtr.Zero ? SqliteNative.Utf8(SqliteNative.sqlite3_errmsg(_db)) : "";
			throw new InvalidOperationException("SQLite error " + rc + " (" + what + "): " + msg);
		}
	}

	private static void EnsureOpen()
	{
		if (_db != IntPtr.Zero) return;
		System.IO.Directory.CreateDirectory(Settings.DataDir);
		IntPtr h;
		int rc = SqliteNative.sqlite3_open_v2(SqliteNative.Z(Settings.DbFile), out h, SqliteNative.OPEN_READWRITE | SqliteNative.OPEN_CREATE | SqliteNative.OPEN_FULLMUTEX, IntPtr.Zero);
		_db = h;
		Check(rc, "open " + Settings.DbFile);
		RunSimple(StartupPragmas);
	}

	public static void Close()
	{
		lock (Lock)
		{
			if (_db != IntPtr.Zero)
			{
				SqliteNative.sqlite3_close_v2(_db);
				_db = IntPtr.Zero;
			}
		}
	}

	private static void RunSimple(string sql)
	{
		IntPtr err;
		int rc = SqliteNative.sqlite3_exec(_db, SqliteNative.Z(sql), IntPtr.Zero, IntPtr.Zero, out err);
		Check(rc, sql);
	}

	private static IntPtr Prepare(string sql, object?[] args)
	{
		IntPtr stmt;
		IntPtr tail;
		int rc = SqliteNative.sqlite3_prepare_v2(_db, SqliteNative.Z(sql), -1, out stmt, out tail);
		Check(rc, sql);
		for (int i = 0; i < args.Length; i++)
		{
			// parameters are named $0, $1, ... in the SQL
			int idx = SqliteNative.sqlite3_bind_parameter_index(stmt, SqliteNative.Z("$" + i));
			if (idx == 0) continue;
			object? a = args[i];
			if (a == null || a is DBNull) rc = SqliteNative.sqlite3_bind_null(stmt, idx);
			else if (a is byte[] bytes)
			{
				if (bytes.Length == 0) rc = SqliteNative.sqlite3_bind_zeroblob(stmt, idx, 0);
				else rc = SqliteNative.sqlite3_bind_blob(stmt, idx, bytes, bytes.Length, SqliteNative.TRANSIENT);
			}
			else if (a is string s)
			{
				byte[] u = Encoding.UTF8.GetBytes(s);
				rc = SqliteNative.sqlite3_bind_text(stmt, idx, u, u.Length, SqliteNative.TRANSIENT);
			}
			else if (a is bool b) rc = SqliteNative.sqlite3_bind_int64(stmt, idx, b ? 1 : 0);
			else if (a is int || a is long || a is short || a is byte || a is uint) rc = SqliteNative.sqlite3_bind_int64(stmt, idx, Convert.ToInt64(a));
			else if (a is double || a is float || a is decimal) rc = SqliteNative.sqlite3_bind_double(stmt, idx, Convert.ToDouble(a));
			else
			{
				byte[] u = Encoding.UTF8.GetBytes(Convert.ToString(a, System.Globalization.CultureInfo.InvariantCulture) ?? "");
				rc = SqliteNative.sqlite3_bind_text(stmt, idx, u, u.Length, SqliteNative.TRANSIENT);
			}
			Check(rc, "bind " + i);
		}
		return stmt;
	}

	public static long Exec(string sql, params object?[] args)
	{
		lock (Lock)
		{
			EnsureOpen();
			// the SQL of Init() is made of single statements; Exec also accepts several statements when there are no arguments
			if (args.Length == 0 && sql.IndexOf(';') >= 0 && sql.TrimEnd().TrimEnd(';').IndexOf(';') >= 0)
			{
				RunSimple(sql);
				return SqliteNative.sqlite3_changes(_db);
			}
			IntPtr stmt = Prepare(sql, args);
			try
			{
				int rc = SqliteNative.sqlite3_step(stmt);
				if (rc != SqliteNative.DONE && rc != SqliteNative.ROW) Check(rc, sql);
				return SqliteNative.sqlite3_changes(_db);
			}
			finally
			{
				SqliteNative.sqlite3_finalize(stmt);
			}
		}
	}

	public static List<Row> Query(string sql, params object?[] args)
	{
		lock (Lock)
		{
			EnsureOpen();
			IntPtr stmt = Prepare(sql, args);
			try
			{
				List<Row> list = new List<Row>();
				int cols = SqliteNative.sqlite3_column_count(stmt);
				string[] names = new string[cols];
				for (int i = 0; i < cols; i++) names[i] = SqliteNative.Utf8(SqliteNative.sqlite3_column_name(stmt, i));
				while (true)
				{
					int rc = SqliteNative.sqlite3_step(stmt);
					if (rc == SqliteNative.DONE) break;
					if (rc != SqliteNative.ROW) { Check(rc, sql); break; }
					Dictionary<string, object?> d = new Dictionary<string, object?>(StringComparer.Ordinal);
					for (int i = 0; i < cols; i++)
					{
						switch (SqliteNative.sqlite3_column_type(stmt, i))
						{
							case 1: d[names[i]] = SqliteNative.sqlite3_column_int64(stmt, i); break;
							case 2: d[names[i]] = SqliteNative.sqlite3_column_double(stmt, i); break;
							case 3:
								{
									IntPtr p = SqliteNative.sqlite3_column_text(stmt, i);
									int n = SqliteNative.sqlite3_column_bytes(stmt, i);
									byte[] buf = new byte[n];
									if (n > 0) Marshal.Copy(p, buf, 0, n);
									d[names[i]] = Encoding.UTF8.GetString(buf);
									break;
								}
							case 4:
								{
									IntPtr p = SqliteNative.sqlite3_column_blob(stmt, i);
									int n = SqliteNative.sqlite3_column_bytes(stmt, i);
									byte[] buf = new byte[n];
									if (n > 0) Marshal.Copy(p, buf, 0, n);
									d[names[i]] = buf;
									break;
								}
							default: d[names[i]] = null; break;
						}
					}
					list.Add(new Row(d));
				}
				return list;
			}
			finally
			{
				SqliteNative.sqlite3_finalize(stmt);
			}
		}
	}

	public static Row? One(string sql, params object?[] args)
	{
		List<Row> list = Query(sql, args);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[0];
	}

	// Groups several statements in a single transaction (a lot faster on phones).
	public static void Transaction(Action body)
	{
		lock (Lock)
		{
			EnsureOpen();
			RunSimple("BEGIN");
			try
			{
				body();
				RunSimple("COMMIT");
			}
			catch
			{
				try { RunSimple("ROLLBACK"); } catch { }
				throw;
			}
		}
	}

	public static void Init()
	{
		EnsureOpen();
		DbSchema.Create();
	}
}

internal static class SqliteDll
{
	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_close_v2(IntPtr db);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr sqlite3_errmsg(IntPtr db);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_exec(IntPtr db, byte[] sql, IntPtr cb, IntPtr arg, out IntPtr err);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_prepare_v2(IntPtr db, byte[] sql, int nByte, out IntPtr stmt, out IntPtr tail);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_step(IntPtr stmt);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_finalize(IntPtr stmt);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_changes(IntPtr db);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_bind_parameter_index(IntPtr stmt, byte[] name);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_bind_null(IntPtr stmt, int idx);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_bind_int64(IntPtr stmt, int idx, long v);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_bind_double(IntPtr stmt, int idx, double v);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_bind_text(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_bind_blob(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_bind_zeroblob(IntPtr stmt, int idx, int n);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_column_count(IntPtr stmt);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr sqlite3_column_name(IntPtr stmt, int i);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_column_type(IntPtr stmt, int i);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern long sqlite3_column_int64(IntPtr stmt, int i);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern double sqlite3_column_double(IntPtr stmt, int i);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr sqlite3_column_text(IntPtr stmt, int i);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr sqlite3_column_blob(IntPtr stmt, int i);

	[DllImport("sqlite3", CallingConvention = CallingConvention.Cdecl)]
	public static extern int sqlite3_column_bytes(IntPtr stmt, int i);
}

// Everything the server needs from SQLite. The default implementation loads the shared library "sqlite3"
// (Windows, Android). On iOS sqlite3 is compiled into the app, and the game supplies its own implementation (Db.NativeApi).
public abstract class SqliteApi
{
	public abstract int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs);
	public abstract int sqlite3_close_v2(IntPtr db);
	public abstract IntPtr sqlite3_errmsg(IntPtr db);
	public abstract int sqlite3_exec(IntPtr db, byte[] sql, IntPtr cb, IntPtr arg, out IntPtr err);
	public abstract int sqlite3_prepare_v2(IntPtr db, byte[] sql, int nByte, out IntPtr stmt, out IntPtr tail);
	public abstract int sqlite3_step(IntPtr stmt);
	public abstract int sqlite3_finalize(IntPtr stmt);
	public abstract int sqlite3_changes(IntPtr db);
	public abstract int sqlite3_bind_parameter_index(IntPtr stmt, byte[] name);
	public abstract int sqlite3_bind_null(IntPtr stmt, int idx);
	public abstract int sqlite3_bind_int64(IntPtr stmt, int idx, long v);
	public abstract int sqlite3_bind_double(IntPtr stmt, int idx, double v);
	public abstract int sqlite3_bind_text(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor);
	public abstract int sqlite3_bind_blob(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor);
	public abstract int sqlite3_bind_zeroblob(IntPtr stmt, int idx, int n);
	public abstract int sqlite3_column_count(IntPtr stmt);
	public abstract IntPtr sqlite3_column_name(IntPtr stmt, int i);
	public abstract int sqlite3_column_type(IntPtr stmt, int i);
	public abstract long sqlite3_column_int64(IntPtr stmt, int i);
	public abstract double sqlite3_column_double(IntPtr stmt, int i);
	public abstract IntPtr sqlite3_column_text(IntPtr stmt, int i);
	public abstract IntPtr sqlite3_column_blob(IntPtr stmt, int i);
	public abstract int sqlite3_column_bytes(IntPtr stmt, int i);
}

internal sealed class SqliteDllApi : SqliteApi
{
	public override int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs)
	{
		return SqliteDll.sqlite3_open_v2(filename, out db, flags, vfs);
	}

	public override int sqlite3_close_v2(IntPtr db)
	{
		return SqliteDll.sqlite3_close_v2(db);
	}

	public override IntPtr sqlite3_errmsg(IntPtr db)
	{
		return SqliteDll.sqlite3_errmsg(db);
	}

	public override int sqlite3_exec(IntPtr db, byte[] sql, IntPtr cb, IntPtr arg, out IntPtr err)
	{
		return SqliteDll.sqlite3_exec(db, sql, cb, arg, out err);
	}

	public override int sqlite3_prepare_v2(IntPtr db, byte[] sql, int nByte, out IntPtr stmt, out IntPtr tail)
	{
		return SqliteDll.sqlite3_prepare_v2(db, sql, nByte, out stmt, out tail);
	}

	public override int sqlite3_step(IntPtr stmt)
	{
		return SqliteDll.sqlite3_step(stmt);
	}

	public override int sqlite3_finalize(IntPtr stmt)
	{
		return SqliteDll.sqlite3_finalize(stmt);
	}

	public override int sqlite3_changes(IntPtr db)
	{
		return SqliteDll.sqlite3_changes(db);
	}

	public override int sqlite3_bind_parameter_index(IntPtr stmt, byte[] name)
	{
		return SqliteDll.sqlite3_bind_parameter_index(stmt, name);
	}

	public override int sqlite3_bind_null(IntPtr stmt, int idx)
	{
		return SqliteDll.sqlite3_bind_null(stmt, idx);
	}

	public override int sqlite3_bind_int64(IntPtr stmt, int idx, long v)
	{
		return SqliteDll.sqlite3_bind_int64(stmt, idx, v);
	}

	public override int sqlite3_bind_double(IntPtr stmt, int idx, double v)
	{
		return SqliteDll.sqlite3_bind_double(stmt, idx, v);
	}

	public override int sqlite3_bind_text(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor)
	{
		return SqliteDll.sqlite3_bind_text(stmt, idx, v, n, destructor);
	}

	public override int sqlite3_bind_blob(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor)
	{
		return SqliteDll.sqlite3_bind_blob(stmt, idx, v, n, destructor);
	}

	public override int sqlite3_bind_zeroblob(IntPtr stmt, int idx, int n)
	{
		return SqliteDll.sqlite3_bind_zeroblob(stmt, idx, n);
	}

	public override int sqlite3_column_count(IntPtr stmt)
	{
		return SqliteDll.sqlite3_column_count(stmt);
	}

	public override IntPtr sqlite3_column_name(IntPtr stmt, int i)
	{
		return SqliteDll.sqlite3_column_name(stmt, i);
	}

	public override int sqlite3_column_type(IntPtr stmt, int i)
	{
		return SqliteDll.sqlite3_column_type(stmt, i);
	}

	public override long sqlite3_column_int64(IntPtr stmt, int i)
	{
		return SqliteDll.sqlite3_column_int64(stmt, i);
	}

	public override double sqlite3_column_double(IntPtr stmt, int i)
	{
		return SqliteDll.sqlite3_column_double(stmt, i);
	}

	public override IntPtr sqlite3_column_text(IntPtr stmt, int i)
	{
		return SqliteDll.sqlite3_column_text(stmt, i);
	}

	public override IntPtr sqlite3_column_blob(IntPtr stmt, int i)
	{
		return SqliteDll.sqlite3_column_blob(stmt, i);
	}

	public override int sqlite3_column_bytes(IntPtr stmt, int i)
	{
		return SqliteDll.sqlite3_column_bytes(stmt, i);
	}
}

internal static class SqliteNative
{
	public static SqliteApi Api = new SqliteDllApi();

	public const int OK = 0;
	public const int ROW = 100;
	public const int DONE = 101;
	public const int OPEN_READWRITE = 0x2;
	public const int OPEN_CREATE = 0x4;
	public const int OPEN_FULLMUTEX = 0x10000;
	public static readonly IntPtr TRANSIENT = new IntPtr(-1);

	public static byte[] Z(string s)
	{
		byte[] u = Encoding.UTF8.GetBytes(s);
		byte[] z = new byte[u.Length + 1];
		Buffer.BlockCopy(u, 0, z, 0, u.Length);
		return z;
	}

	public static string Utf8(IntPtr p)
	{
		if (p == IntPtr.Zero) return "";
		int n = 0;
		while (Marshal.ReadByte(p, n) != 0) n++;
		byte[] b = new byte[n];
		Marshal.Copy(p, b, 0, n);
		return Encoding.UTF8.GetString(b);
	}

	public static int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs)
	{
		return Api.sqlite3_open_v2(filename, out db, flags, vfs);
	}

	public static int sqlite3_close_v2(IntPtr db)
	{
		return Api.sqlite3_close_v2(db);
	}

	public static IntPtr sqlite3_errmsg(IntPtr db)
	{
		return Api.sqlite3_errmsg(db);
	}

	public static int sqlite3_exec(IntPtr db, byte[] sql, IntPtr cb, IntPtr arg, out IntPtr err)
	{
		return Api.sqlite3_exec(db, sql, cb, arg, out err);
	}

	public static int sqlite3_prepare_v2(IntPtr db, byte[] sql, int nByte, out IntPtr stmt, out IntPtr tail)
	{
		return Api.sqlite3_prepare_v2(db, sql, nByte, out stmt, out tail);
	}

	public static int sqlite3_step(IntPtr stmt)
	{
		return Api.sqlite3_step(stmt);
	}

	public static int sqlite3_finalize(IntPtr stmt)
	{
		return Api.sqlite3_finalize(stmt);
	}

	public static int sqlite3_changes(IntPtr db)
	{
		return Api.sqlite3_changes(db);
	}

	public static int sqlite3_bind_parameter_index(IntPtr stmt, byte[] name)
	{
		return Api.sqlite3_bind_parameter_index(stmt, name);
	}

	public static int sqlite3_bind_null(IntPtr stmt, int idx)
	{
		return Api.sqlite3_bind_null(stmt, idx);
	}

	public static int sqlite3_bind_int64(IntPtr stmt, int idx, long v)
	{
		return Api.sqlite3_bind_int64(stmt, idx, v);
	}

	public static int sqlite3_bind_double(IntPtr stmt, int idx, double v)
	{
		return Api.sqlite3_bind_double(stmt, idx, v);
	}

	public static int sqlite3_bind_text(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor)
	{
		return Api.sqlite3_bind_text(stmt, idx, v, n, destructor);
	}

	public static int sqlite3_bind_blob(IntPtr stmt, int idx, byte[] v, int n, IntPtr destructor)
	{
		return Api.sqlite3_bind_blob(stmt, idx, v, n, destructor);
	}

	public static int sqlite3_bind_zeroblob(IntPtr stmt, int idx, int n)
	{
		return Api.sqlite3_bind_zeroblob(stmt, idx, n);
	}

	public static int sqlite3_column_count(IntPtr stmt)
	{
		return Api.sqlite3_column_count(stmt);
	}

	public static IntPtr sqlite3_column_name(IntPtr stmt, int i)
	{
		return Api.sqlite3_column_name(stmt, i);
	}

	public static int sqlite3_column_type(IntPtr stmt, int i)
	{
		return Api.sqlite3_column_type(stmt, i);
	}

	public static long sqlite3_column_int64(IntPtr stmt, int i)
	{
		return Api.sqlite3_column_int64(stmt, i);
	}

	public static double sqlite3_column_double(IntPtr stmt, int i)
	{
		return Api.sqlite3_column_double(stmt, i);
	}

	public static IntPtr sqlite3_column_text(IntPtr stmt, int i)
	{
		return Api.sqlite3_column_text(stmt, i);
	}

	public static IntPtr sqlite3_column_blob(IntPtr stmt, int i)
	{
		return Api.sqlite3_column_blob(stmt, i);
	}

	public static int sqlite3_column_bytes(IntPtr stmt, int i)
	{
		return Api.sqlite3_column_bytes(stmt, i);
	}
}
