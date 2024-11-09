using UnityEngine;

namespace Core.Colors;

public readonly struct ColorPart
{
	public readonly Rect Region;

	public readonly ScriptableColor Color;

	public readonly Texture StitchTexture;

	public readonly Texture BlendTexture;

	public readonly string ShaderKeyword;

	public ColorPart(Rect region, ScriptableColor color, Texture stitchTexture, Texture blendTexture = null, string shaderKeyword = "")
	{
		Region = region;
		Color = color;
		StitchTexture = stitchTexture;
		BlendTexture = blendTexture;
		ShaderKeyword = shaderKeyword;
	}
}
