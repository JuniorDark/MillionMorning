using UnityEngine;

namespace Code.World.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/MilMo Blur")]
public class MilMo_BlurEffect : MilMo_ImageEffectBase
{
	[Range(0f, 10f)]
	public int iterations = 2;

	[Range(0f, 1f)]
	public float blurSpread = 0.3f;

	private void FourTapCone(Texture source, RenderTexture dest, int iteration)
	{
		float num = 0.5f + (float)iteration * blurSpread;
		Graphics.BlitMultiTap(source, dest, base.Material, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}

	private void DownSample4X(Texture source, RenderTexture dest)
	{
		Graphics.BlitMultiTap(source, dest, base.Material, new Vector2(-1f, -1f), new Vector2(-1f, 1f), new Vector2(1f, 1f), new Vector2(1f, -1f));
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		int width = source.width / 4;
		int height = source.height / 4;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0);
		DownSample4X(source, renderTexture);
		for (int i = 0; i < iterations; i++)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
			FourTapCone(renderTexture, temporary, i);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Graphics.Blit(renderTexture, destination);
		RenderTexture.ReleaseTemporary(renderTexture);
	}
}
