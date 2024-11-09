using System.Collections.Generic;
using UnityEngine;

namespace Core.BodyShapes;

public class ShapeRenderer
{
	private const string SKIN_PART_SHADER_PATH = "Shaders/skinPartShader";

	private Shader _skinPartShader;

	private Material _skinPartMaterial;

	private void InitMaterial()
	{
		if (!_skinPartMaterial)
		{
			_skinPartShader = Resources.Load<Shader>("Shaders/skinPartShader");
			_skinPartMaterial = new Material(_skinPartShader)
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
	}

	public void Render(IEnumerable<ShapePart> parts)
	{
		InitMaterial();
		foreach (ShapePart part in parts)
		{
			DrawPart(part.Region, part.Texture);
		}
	}

	private void DrawPart(Rect region, Texture texture)
	{
		_skinPartMaterial.mainTexture = texture;
		_skinPartMaterial.SetPass(0);
		DrawQuad(region);
	}

	private void DrawQuad(Rect r)
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
}
