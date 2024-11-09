using UnityEngine;

namespace Code.World.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/MilMo Color Tint")]
public class MilMo_ColorTintEffect : MilMo_ImageEffectBase
{
	public Color colorTint;

	private static int _colorTintID;

	private void Awake()
	{
		_colorTintID = Shader.PropertyToID("_ColorTint");
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.Material.SetColor(_colorTintID, colorTint);
		Graphics.Blit(source, destination, base.Material);
	}
}
