using UnityEngine;

namespace Code.World.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/MilMo Bloom")]
public class MilMo_BloomEffect : MilMo_ImageEffectBase
{
	public enum Resolution
	{
		Low,
		High
	}

	public enum BlurType
	{
		Standard,
		Sgx
	}

	public const float DEFAULT_THRESHOLD = 0.85f;

	public const float DEFAULT_INTENSITY = 1.5f;

	public const float DEFAULT_BLUR_SIZE = 1f;

	public const int DEFAULT_BLUR_ITERATIONS = 2;

	[Range(0f, 1.5f)]
	public float threshold = 0.85f;

	[Range(0f, 2.5f)]
	public float intensity = 1.5f;

	[Range(0.25f, 5.5f)]
	public float blurSize = 1f;

	[Range(1f, 4f)]
	public int blurIterations = 2;

	public Resolution resolution;

	public BlurType blurType;

	private static int _parameterID;

	private static int _bloomID;

	public void Reset()
	{
		threshold = 0.85f;
		intensity = 1.5f;
		blurSize = 1f;
		blurIterations = 2;
	}

	private void Awake()
	{
		_parameterID = Shader.PropertyToID("_Parameter");
		_bloomID = Shader.PropertyToID("_Bloom");
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		int num = ((resolution == Resolution.Low) ? 4 : 2);
		float num2 = ((resolution == Resolution.Low) ? 0.5f : 1f);
		base.Material.SetVector(_parameterID, new Vector4(blurSize * num2, 0f, threshold, intensity));
		source.filterMode = FilterMode.Bilinear;
		int width = source.width / num;
		int height = source.height / num;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;
		Graphics.Blit(source, renderTexture, base.Material, 1);
		int num3 = ((blurType != 0) ? 2 : 0);
		for (int i = 0; i < blurIterations; i++)
		{
			base.Material.SetVector(_parameterID, new Vector4(blurSize * num2 + (float)i * 1f, 0f, threshold, intensity));
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, base.Material, 2 + num3);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
			temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, base.Material, 3 + num3);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		base.Material.SetTexture(_bloomID, renderTexture);
		Graphics.Blit(source, destination, base.Material, 0);
		RenderTexture.ReleaseTemporary(renderTexture);
	}
}
