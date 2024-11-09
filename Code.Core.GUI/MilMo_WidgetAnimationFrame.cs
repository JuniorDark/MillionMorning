namespace Code.Core.GUI;

public sealed class MilMo_WidgetAnimationFrame
{
	public readonly string Texture;

	public bool TextureLoaded;

	public readonly float MinDelay;

	public readonly float MaxDelay;

	public MilMo_WidgetAnimationFrame(string texture, float minDelay, float maxDelay)
	{
		Texture = texture;
		MinDelay = minDelay;
		MaxDelay = maxDelay;
	}
}
