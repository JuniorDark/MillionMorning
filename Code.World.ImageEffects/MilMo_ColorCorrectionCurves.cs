using UnityEngine;

namespace Code.World.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/MilMo Color Adjustments/Color Correction (Curves, Saturation)")]
public class MilMo_ColorCorrectionCurves : MilMo_ImageEffectBase
{
	public AnimationCurve redChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public AnimationCurve greenChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public AnimationCurve blueChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public float saturation = 1f;

	private Texture2D _rgbChannelTex;

	private static int _rgbTex;

	private static int _saturation;

	private Texture2D Texture
	{
		get
		{
			if (!_rgbChannelTex)
			{
				_rgbChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, mipChain: false, linear: true)
				{
					hideFlags = HideFlags.DontSave,
					wrapMode = TextureWrapMode.Clamp
				};
			}
			return _rgbChannelTex;
		}
	}

	private void Awake()
	{
		_rgbTex = Shader.PropertyToID("_RgbTex");
		_saturation = Shader.PropertyToID("_Saturation");
		UpdateParameters();
	}

	private void UpdateParameters()
	{
		if (redChannel != null && greenChannel != null && blueChannel != null)
		{
			for (float num = 0f; num <= 1f; num += 0.003921569f)
			{
				float num2 = Mathf.Clamp(redChannel.Evaluate(num), 0f, 1f);
				float num3 = Mathf.Clamp(greenChannel.Evaluate(num), 0f, 1f);
				float num4 = Mathf.Clamp(blueChannel.Evaluate(num), 0f, 1f);
				Texture.SetPixel((int)Mathf.Floor(num * 255f), 0, new Color(num2, num2, num2));
				Texture.SetPixel((int)Mathf.Floor(num * 255f), 1, new Color(num3, num3, num3));
				Texture.SetPixel((int)Mathf.Floor(num * 255f), 2, new Color(num4, num4, num4));
			}
			Texture.Apply();
		}
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.Material.SetTexture(_rgbTex, Texture);
		base.Material.SetFloat(_saturation, saturation);
		Graphics.Blit(source, destination, base.Material);
	}
}
