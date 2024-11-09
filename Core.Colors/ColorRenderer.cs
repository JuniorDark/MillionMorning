using System.Collections.Generic;
using UnityEngine;

namespace Core.Colors;

public class ColorRenderer
{
	private const string COLOR_SHADER_PATH = "Shaders/BodyBuilder/MilMoColor";

	private Shader _colorShader;

	private Material _colorMaterial;

	private readonly int _saturation = Shader.PropertyToID("_Saturation");

	private readonly int _stitch = Shader.PropertyToID("_Stitch");

	private readonly int _blendTex1 = Shader.PropertyToID("_BlendTex1");

	private readonly int _color1 = Shader.PropertyToID("_Color1");

	private readonly int _color2 = Shader.PropertyToID("_Color2");

	private readonly int _color3 = Shader.PropertyToID("_Color3");

	private void InitMaterial()
	{
		if (!_colorMaterial)
		{
			_colorShader = Resources.Load<Shader>("Shaders/BodyBuilder/MilMoColor");
			_colorMaterial = new Material(_colorShader)
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
	}

	private void ResetMaterial()
	{
		if ((bool)_colorMaterial)
		{
			_colorMaterial.DisableKeyword("OVERLAY1");
			_colorMaterial.DisableKeyword("OVERLAY2");
			_colorMaterial.DisableKeyword("OVERLAY3");
			_colorMaterial.DisableKeyword("SOFTLIGHT1");
			_colorMaterial.DisableKeyword("SOFTLIGHT2");
			_colorMaterial.DisableKeyword("SOFTLIGHT3");
			_colorMaterial.DisableKeyword("SATURATION");
			_colorMaterial.DisableKeyword("EYE");
			_colorMaterial.DisableKeyword("NOBLEND");
			_colorMaterial.DisableKeyword("STITCHING");
			_colorMaterial.DisableKeyword("COPY");
			_colorMaterial.SetColor(_color1, Color.white);
			_colorMaterial.SetColor(_color2, Color.white);
			_colorMaterial.SetColor(_color3, Color.white);
			_colorMaterial.SetFloat(_saturation, 0f);
			_colorMaterial.SetTexture(_stitch, Texture2D.whiteTexture);
			_colorMaterial.SetTexture(_blendTex1, Texture2D.whiteTexture);
		}
	}

	public void Render(IEnumerable<ColorPart> parts)
	{
		InitMaterial();
		foreach (ColorPart part in parts)
		{
			ResetMaterial();
			DrawPart(part.Region, part.Color, part.StitchTexture, part.BlendTexture, part.ShaderKeyword);
		}
	}

	private void DrawPart(Rect region, ScriptableColor color, Texture stitchTexture, Texture blendTexture = null, string shaderKeyword = "")
	{
		_colorMaterial.SetTexture(_stitch, stitchTexture);
		if ((bool)color)
		{
			UseColor(color);
		}
		if (shaderKeyword == "COPY")
		{
			_colorMaterial.EnableKeyword("COPY");
		}
		if (shaderKeyword == "EYE")
		{
			_colorMaterial.EnableKeyword("EYE");
		}
		if (shaderKeyword == "STITCHING")
		{
			_colorMaterial.EnableKeyword("STITCHING");
		}
		if ((bool)blendTexture)
		{
			_colorMaterial.SetTexture(_blendTex1, blendTexture);
		}
		_colorMaterial.SetPass(0);
		DrawQuad(region);
	}

	private void UseColor(ScriptableColor color)
	{
		int num = 1;
		foreach (Color overlay in color.GetOverlays())
		{
			_colorMaterial.EnableKeyword("OVERLAY" + num);
			_colorMaterial.SetColor("_Color" + num, overlay);
			num++;
		}
		foreach (Color softLight in color.GetSoftLights())
		{
			_colorMaterial.EnableKeyword("SOFTLIGHT" + num);
			_colorMaterial.SetColor("_Color" + num, softLight);
			num++;
		}
		int saturation = color.GetSaturation();
		if (saturation != 0)
		{
			_colorMaterial.EnableKeyword("SATURATION");
			_colorMaterial.SetFloat(_saturation, (float)saturation / 100f);
		}
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
