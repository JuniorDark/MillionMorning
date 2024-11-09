using Code.Core.BodyPack.ColorSystem;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack.SkinPartSystem;

public class MilMo_SkinPart
{
	public readonly string Name;

	private readonly Material _material;

	private readonly Rect _region;

	private const string SHADER = "Shaders/skinPartShader";

	public MilMo_SkinPart(Texture texture, Rect region)
	{
		Name = texture.name;
		_region = region;
		Shader shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/skinPartShader");
		_material = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave,
			mainTexture = texture
		};
	}

	public void Apply()
	{
		_material.SetPass(0);
		MilMo_ColorShaderUtil.DrawQuad(_region);
	}
}
