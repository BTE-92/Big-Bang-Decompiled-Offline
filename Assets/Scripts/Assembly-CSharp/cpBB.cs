using UnityEngine;

public struct cpBB
{
	public float l;

	public float b;

	public float r;

	public float t;

	public cpBB(float _value)
	{
		l = _value;
		r = _value;
		t = _value;
		b = _value;
	}

	public cpBB(float _l, float _b, float _r, float _t)
	{
		l = _l;
		b = _b;
		r = _r;
		t = _t;
	}

	public override string ToString()
	{
		return "(" + l + ", " + b + ", " + r + ", " + t + ")";
	}

	public bool isNull()
	{
		return l == 0f && r == 0f && b == 0f && t == 0f;
	}

	public Vector2 getCenter()
	{
		return new Vector2(l + (r - l) * 0.5f, t - (t - b) * 0.5f);
	}

	public Vector2 getDimensions()
	{
		return new Vector2(r - l, t - b);
	}
}
