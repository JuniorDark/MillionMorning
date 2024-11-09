using UnityEngine;

namespace Code.Core.Network.types;

public class color
{
	private readonly float r;

	private readonly float g;

	private readonly float b;

	private readonly float a;

	public color(MessageReader reader)
	{
		r = reader.ReadFloat();
		g = reader.ReadFloat();
		b = reader.ReadFloat();
		a = reader.ReadFloat();
	}

	public color(float r, float g, float b, float a)
	{
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
	}

	public float getR()
	{
		return r;
	}

	public float getG()
	{
		return g;
	}

	public float getB()
	{
		return b;
	}

	public float getA()
	{
		return a;
	}

	public Color getAsColor()
	{
		return new Color(r, g, b, a);
	}

	public int size()
	{
		return 16;
	}

	public void write(MessageWriter writer)
	{
		writer.WriteFloat(r);
		writer.WriteFloat(g);
		writer.WriteFloat(b);
		writer.WriteFloat(a);
	}
}
