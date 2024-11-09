using UnityEngine;

namespace Code.Core.BodyPack.ColorSystem;

public static class MilMo_ColorShaderUtil
{
	public static void DrawQuad(Rect r)
	{
		GL.Begin(7);
		GL.TexCoord2(0f, 0f);
		GL.Vertex3(r.xMin, 1f - r.yMax, 0f);
		GL.TexCoord2(0f, 1f);
		GL.Vertex3(r.xMin, 1f - r.yMin, 0f);
		GL.TexCoord2(1f, 1f);
		GL.Vertex3(r.xMax, 1f - r.yMin, 0f);
		GL.TexCoord2(1f, 0f);
		GL.Vertex3(r.xMax, 1f - r.yMax, 0f);
		GL.End();
	}

	public static int NearestBiggerPowerOfTwo(int x)
	{
		if (IsPowerOfTwo(x))
		{
			return x;
		}
		for (int num = 1; num < 2147418112; num = (num << 1) + 1)
		{
			if ((num & x) == x)
			{
				return num + 1;
			}
		}
		return -1;
	}

	public static bool IsPowerOfTwo(int x)
	{
		return (x & (x - 1)) == 0;
	}
}
