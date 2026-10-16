internal struct ucpTransform
{
	public float a;

	public float b;

	public float c;

	public float d;

	public float tx;

	public float ty;

	public ucpTransform(float _tx, float _ty)
	{
		a = 1f;
		b = 0f;
		c = 0f;
		d = 1f;
		tx = _tx;
		ty = _ty;
	}

	public void SetIdentity()
	{
		a = 1f;
		b = 0f;
		c = 0f;
		d = 1f;
		tx = 0f;
		ty = 0f;
	}
}
