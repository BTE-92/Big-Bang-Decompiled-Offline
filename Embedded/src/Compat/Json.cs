#nullable enable
// Minimal stand-in for System.Text.Json.Nodes (JsonNode / JsonObject / JsonArray / JsonValue), enough for the
// server code. It exists so the server code runs unchanged inside Unity (Mono / IL2CPP), where System.Text.Json
// is not available. It keeps the insertion order of object properties, like the real one.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Text.Json.Nodes
{
	public abstract class JsonNode
	{
		public static JsonNode? Parse(string json)
		{
			JsonReader r = new JsonReader(json);
			r.SkipWs();
			JsonNode? n = r.ReadValue();
			r.SkipWs();
			if (!r.End) throw new FormatException("Unexpected data after the JSON value");
			return n;
		}

		public static JsonNode? Parse(byte[] utf8)
		{
			int off = (utf8.Length >= 3 && utf8[0] == 0xEF && utf8[1] == 0xBB && utf8[2] == 0xBF) ? 3 : 0;
			return Parse(Encoding.UTF8.GetString(utf8, off, utf8.Length - off));
		}

		public string ToJsonString()
		{
			StringBuilder sb = new StringBuilder();
			Write(sb);
			return sb.ToString();
		}

		public override string ToString()
		{
			return ToJsonString();
		}

		internal abstract void Write(StringBuilder sb);

		public abstract JsonNode DeepClone();

		public JsonObject AsObject()
		{
			return this as JsonObject ?? throw new InvalidOperationException("The node is not a JsonObject");
		}

		public JsonArray AsArray()
		{
			return this as JsonArray ?? throw new InvalidOperationException("The node is not a JsonArray");
		}

		public JsonValue AsValue()
		{
			return this as JsonValue ?? throw new InvalidOperationException("The node is not a JsonValue");
		}

		public virtual T GetValue<T>()
		{
			throw new InvalidOperationException("The node is not a JsonValue");
		}

		public virtual JsonNode? this[string key]
		{
			get { throw new InvalidOperationException("The node is not a JsonObject"); }
			set { throw new InvalidOperationException("The node is not a JsonObject"); }
		}

		public virtual JsonNode? this[int index]
		{
			get { throw new InvalidOperationException("The node is not a JsonArray"); }
			set { throw new InvalidOperationException("The node is not a JsonArray"); }
		}

		// ---- implicit conversions (the server writes `["x"] = 5` or `["x"] = "text"`)
		public static implicit operator JsonNode?(string? v) { return v == null ? null : new JsonValue(v); }
		public static implicit operator JsonNode(bool v) { return new JsonValue(v); }
		public static implicit operator JsonNode(int v) { return new JsonValue((long)v); }
		public static implicit operator JsonNode(long v) { return new JsonValue(v); }
		public static implicit operator JsonNode(uint v) { return new JsonValue((long)v); }
		public static implicit operator JsonNode(ulong v) { return new JsonValue(v); }
		public static implicit operator JsonNode(short v) { return new JsonValue((long)v); }
		public static implicit operator JsonNode(byte v) { return new JsonValue((long)v); }
		public static implicit operator JsonNode(double v) { return new JsonValue(v); }
		public static implicit operator JsonNode(float v) { return new JsonValue((double)v); }
		public static implicit operator JsonNode(decimal v) { return new JsonValue(v); }
		public static implicit operator JsonNode?(int? v) { return v.HasValue ? new JsonValue((long)v.Value) : null; }
		public static implicit operator JsonNode?(long? v) { return v.HasValue ? new JsonValue(v.Value) : null; }
		public static implicit operator JsonNode?(bool? v) { return v.HasValue ? new JsonValue(v.Value) : null; }
		public static implicit operator JsonNode?(double? v) { return v.HasValue ? new JsonValue(v.Value) : null; }

		// ---- explicit conversions ((string?)node["x"], (int)node["x"], ...)
		public static explicit operator string?(JsonNode? n) { return n == null ? null : n.GetValue<string>(); }
		public static explicit operator int(JsonNode n) { return n.GetValue<int>(); }
		public static explicit operator long(JsonNode n) { return n.GetValue<long>(); }
		public static explicit operator bool(JsonNode n) { return n.GetValue<bool>(); }
		public static explicit operator double(JsonNode n) { return n.GetValue<double>(); }
		public static explicit operator int?(JsonNode? n) { return n == null ? (int?)null : n.GetValue<int>(); }
		public static explicit operator long?(JsonNode? n) { return n == null ? (long?)null : n.GetValue<long>(); }
		public static explicit operator bool?(JsonNode? n) { return n == null ? (bool?)null : n.GetValue<bool>(); }
		public static explicit operator double?(JsonNode? n) { return n == null ? (double?)null : n.GetValue<double>(); }

		internal static void WriteString(StringBuilder sb, string s)
		{
			sb.Append('"');
			foreach (char c in s)
			{
				switch (c)
				{
					case '"': sb.Append("\\\""); break;
					case '\\': sb.Append("\\\\"); break;
					case '\n': sb.Append("\\n"); break;
					case '\r': sb.Append("\\r"); break;
					case '\t': sb.Append("\\t"); break;
					case '\b': sb.Append("\\b"); break;
					case '\f': sb.Append("\\f"); break;
					default:
						// like System.Text.Json by default: control characters, non-ASCII and HTML-sensitive characters are escaped
						if (c < 0x20 || c > 0x7E || c == '<' || c == '>' || c == '&' || c == '\'' || c == '+')
							sb.Append("\\u").Append(((int)c).ToString("X4", CultureInfo.InvariantCulture));
						else
							sb.Append(c);
						break;
				}
			}
			sb.Append('"');
		}
	}

	public sealed class JsonValue : JsonNode
	{
		internal readonly object Value;

		internal JsonValue(object value)
		{
			Value = value;
		}

		public static JsonValue? Create(string? v) { return v == null ? null : new JsonValue(v); }
		public static JsonValue Create(bool v) { return new JsonValue(v); }
		public static JsonValue Create(int v) { return new JsonValue((long)v); }
		public static JsonValue Create(long v) { return new JsonValue(v); }
		public static JsonValue Create(double v) { return new JsonValue(v); }
		public static JsonValue Create(decimal v) { return new JsonValue(v); }
		public static JsonValue? Create<T>(T value)
		{
			if (value == null) return null;
			object o = value;
			if (o is int i) return new JsonValue((long)i);
			if (o is short sh) return new JsonValue((long)sh);
			if (o is byte b) return new JsonValue((long)b);
			if (o is uint ui) return new JsonValue((long)ui);
			if (o is float f) return new JsonValue((double)f);
			if (o is long || o is ulong || o is double || o is decimal || o is bool || o is string) return new JsonValue(o);
			return new JsonValue(Convert.ToString(o, CultureInfo.InvariantCulture)!);
		}

		public override JsonNode DeepClone() { return new JsonValue(Value); }

		// like System.Text.Json: a string value gives its text without quotes
		public override string ToString()
		{
			string? s = Value as string;
			return s != null ? s : ToJsonString();
		}

		public override T GetValue<T>()
		{
			Type t = typeof(T);
			Type u = Nullable.GetUnderlyingType(t) ?? t;
			if (u == typeof(object)) return (T)Value;
			if (u == typeof(string))
			{
				if (Value is string s) return (T)(object)s;
				if (Value is bool bb) return (T)(object)(bb ? "true" : "false");
				return (T)(object)Convert.ToString(Value, CultureInfo.InvariantCulture)!;
			}
			if (u == typeof(bool))
			{
				if (Value is bool b) return (T)(object)b;
				if (Value is string sb && bool.TryParse(sb, out bool pb)) return (T)(object)pb;
				throw new InvalidOperationException("The value is not a boolean");
			}
			if (Value is string str)
			{
				if (u == typeof(Guid)) return (T)(object)Guid.Parse(str);
				if (u == typeof(DateTime)) return (T)(object)DateTime.Parse(str, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
				throw new InvalidOperationException("The value is a string, not a number");
			}
			if (Value is bool) throw new InvalidOperationException("The value is a boolean, not a number");
			return (T)Convert.ChangeType(Value, u, CultureInfo.InvariantCulture);
		}

		public bool TryGetValue<T>(out T value)
		{
			try
			{
				value = GetValue<T>();
				return true;
			}
			catch (Exception)
			{
				value = default(T)!;
				return false;
			}
		}

		internal bool IsString { get { return Value is string; } }
		internal object Raw { get { return Value; } }

		internal override void Write(StringBuilder sb)
		{
			if (Value is string s) WriteString(sb, s);
			else if (Value is bool b) sb.Append(b ? "true" : "false");
			else if (Value is double d)
			{
				if (double.IsNaN(d) || double.IsInfinity(d)) throw new ArgumentException("NaN/Infinity is not a valid JSON number");
				sb.Append(d.ToString("R", CultureInfo.InvariantCulture));
			}
			else if (Value is float f) sb.Append(f.ToString("R", CultureInfo.InvariantCulture));
			else if (Value is decimal m) sb.Append(m.ToString(CultureInfo.InvariantCulture));
			else sb.Append(Convert.ToString(Value, CultureInfo.InvariantCulture));
		}
	}

	public sealed class JsonObject : JsonNode, IEnumerable<KeyValuePair<string, JsonNode?>>
	{
		private readonly List<string> _keys = new List<string>();
		private readonly Dictionary<string, JsonNode?> _map = new Dictionary<string, JsonNode?>(StringComparer.Ordinal);

		public JsonObject() { }

		public int Count { get { return _keys.Count; } }

		public ICollection<string> Keys { get { return _keys; } }

		public override JsonNode? this[string key]
		{
			get { JsonNode? v; return _map.TryGetValue(key, out v) ? v : null; }
			set
			{
				if (!_map.ContainsKey(key)) _keys.Add(key);
				_map[key] = value;
			}
		}

		public void Add(string key, JsonNode? value)
		{
			if (_map.ContainsKey(key)) throw new ArgumentException("An item with the same key has already been added: " + key);
			_keys.Add(key);
			_map[key] = value;
		}

		public bool ContainsKey(string key) { return _map.ContainsKey(key); }

		public bool TryGetPropertyValue(string key, out JsonNode? value) { return _map.TryGetValue(key, out value); }

		public bool Remove(string key)
		{
			if (!_map.Remove(key)) return false;
			_keys.Remove(key);
			return true;
		}

		public void Clear() { _keys.Clear(); _map.Clear(); }

		public IEnumerator<KeyValuePair<string, JsonNode?>> GetEnumerator()
		{
			foreach (string k in _keys.ToArray())
				yield return new KeyValuePair<string, JsonNode?>(k, _map[k]);
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		public override JsonNode DeepClone()
		{
			JsonObject o = new JsonObject();
			foreach (string k in _keys) o.Add(k, _map[k]?.DeepClone());
			return o;
		}

		internal override void Write(StringBuilder sb)
		{
			sb.Append('{');
			for (int i = 0; i < _keys.Count; i++)
			{
				if (i > 0) sb.Append(',');
				WriteString(sb, _keys[i]);
				sb.Append(':');
				JsonNode? v = _map[_keys[i]];
				if (v == null) sb.Append("null"); else v.Write(sb);
			}
			sb.Append('}');
		}
	}

	public sealed class JsonArray : JsonNode, IEnumerable<JsonNode?>
	{
		private readonly List<JsonNode?> _items = new List<JsonNode?>();

		public JsonArray() { }

		public JsonArray(params JsonNode?[] items)
		{
			if (items != null) _items.AddRange(items);
		}

		public int Count { get { return _items.Count; } }

		public override JsonNode? this[int index]
		{
			get { return _items[index]; }
			set { _items[index] = value; }
		}

		public void Add(JsonNode? item) { _items.Add(item); }
		public void Insert(int index, JsonNode? item) { _items.Insert(index, item); }
		public void RemoveAt(int index) { _items.RemoveAt(index); }
		public bool Remove(JsonNode? item) { return _items.Remove(item); }
		public void Clear() { _items.Clear(); }

		public IEnumerator<JsonNode?> GetEnumerator() { return _items.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return _items.GetEnumerator(); }

		public override JsonNode DeepClone()
		{
			JsonArray a = new JsonArray();
			foreach (JsonNode? n in _items) a.Add(n?.DeepClone());
			return a;
		}

		internal override void Write(StringBuilder sb)
		{
			sb.Append('[');
			for (int i = 0; i < _items.Count; i++)
			{
				if (i > 0) sb.Append(',');
				JsonNode? v = _items[i];
				if (v == null) sb.Append("null"); else v.Write(sb);
			}
			sb.Append(']');
		}
	}

	internal sealed class JsonReader
	{
		private readonly string _s;
		private int _p;

		public JsonReader(string s) { _s = s; }

		public bool End { get { return _p >= _s.Length; } }

		public void SkipWs()
		{
			while (_p < _s.Length && (_s[_p] == ' ' || _s[_p] == '\t' || _s[_p] == '\r' || _s[_p] == '\n' || _s[_p] == '﻿')) _p++;
		}

		private FormatException Err(string m) { return new FormatException(m + " at position " + _p); }

		public JsonNode? ReadValue()
		{
			SkipWs();
			if (_p >= _s.Length) throw Err("Unexpected end of JSON");
			char c = _s[_p];
			if (c == '{') return ReadObject();
			if (c == '[') return ReadArray();
			if (c == '"') return new JsonValue(ReadString());
			if (c == 't') { Expect("true"); return new JsonValue(true); }
			if (c == 'f') { Expect("false"); return new JsonValue(false); }
			if (c == 'n') { Expect("null"); return null; }
			return ReadNumber();
		}

		private void Expect(string lit)
		{
			if (string.CompareOrdinal(_s, _p, lit, 0, lit.Length) != 0) throw Err("Invalid literal");
			_p += lit.Length;
		}

		private JsonNode ReadNumber()
		{
			int start = _p;
			if (_p < _s.Length && (_s[_p] == '-' || _s[_p] == '+')) _p++;
			bool isFloat = false;
			while (_p < _s.Length)
			{
				char c = _s[_p];
				if (c >= '0' && c <= '9') _p++;
				else if (c == '.' || c == 'e' || c == 'E' || c == '-' || c == '+') { isFloat = true; _p++; }
				else break;
			}
			if (_p == start) throw Err("Invalid JSON value");
			string t = _s.Substring(start, _p - start);
			if (!isFloat)
			{
				long l;
				if (long.TryParse(t, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out l)) return new JsonValue(l);
				ulong ul;
				if (ulong.TryParse(t, NumberStyles.None, CultureInfo.InvariantCulture, out ul)) return new JsonValue(ul);
			}
			double d;
			if (!double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out d)) throw Err("Invalid number");
			return new JsonValue(d);
		}

		private string ReadString()
		{
			_p++; // opening quote
			StringBuilder sb = new StringBuilder();
			while (true)
			{
				if (_p >= _s.Length) throw Err("Unterminated string");
				char c = _s[_p++];
				if (c == '"') break;
				if (c != '\\') { sb.Append(c); continue; }
				if (_p >= _s.Length) throw Err("Unterminated escape");
				char e = _s[_p++];
				switch (e)
				{
					case '"': sb.Append('"'); break;
					case '\\': sb.Append('\\'); break;
					case '/': sb.Append('/'); break;
					case 'b': sb.Append('\b'); break;
					case 'f': sb.Append('\f'); break;
					case 'n': sb.Append('\n'); break;
					case 'r': sb.Append('\r'); break;
					case 't': sb.Append('\t'); break;
					case 'u':
						if (_p + 4 > _s.Length) throw Err("Bad unicode escape");
						sb.Append((char)int.Parse(_s.Substring(_p, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
						_p += 4;
						break;
					default: throw Err("Bad escape");
				}
			}
			return sb.ToString();
		}

		private JsonObject ReadObject()
		{
			_p++; // {
			JsonObject o = new JsonObject();
			SkipWs();
			if (_p < _s.Length && _s[_p] == '}') { _p++; return o; }
			while (true)
			{
				SkipWs();
				if (_p >= _s.Length || _s[_p] != '"') throw Err("Property name expected");
				string k = ReadString();
				SkipWs();
				if (_p >= _s.Length || _s[_p] != ':') throw Err("':' expected");
				_p++;
				o[k] = ReadValue();
				SkipWs();
				if (_p >= _s.Length) throw Err("Unterminated object");
				char c = _s[_p++];
				if (c == '}') break;
				if (c != ',') throw Err("',' or '}' expected");
			}
			return o;
		}

		private JsonArray ReadArray()
		{
			_p++; // [
			JsonArray a = new JsonArray();
			SkipWs();
			if (_p < _s.Length && _s[_p] == ']') { _p++; return a; }
			while (true)
			{
				a.Add(ReadValue());
				SkipWs();
				if (_p >= _s.Length) throw Err("Unterminated array");
				char c = _s[_p++];
				if (c == ']') break;
				if (c != ',') throw Err("',' or ']' expected");
			}
			return a;
		}
	}
}
