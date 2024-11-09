using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.HudCounter.Counters;

public class MilMo_GemCounter : MilMo_HudCounter
{
	private int _currentPrice;

	public MilMo_GemCounter(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "Gems";
		LoadAndSetGemIconAsync();
		base.Number.SetTextNoLocalization("0");
	}

	private async void LoadAndSetGemIconAsync()
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/HUD/IconGemCounter");
		base.Icon.SetTexture(new MilMo_Texture(texture));
	}

	public MilMo_GemCounter(MilMo_UserInterface ui, float posX, float posY)
		: this(ui)
	{
		SpawnPosition = new Vector2(1204f, posY);
		TargetPosition = new Vector2(posX, posY);
		NumberSpawnPos = new Vector2(50f, -6f);
		NumberTargetPos = new Vector2(50f, -6f);
	}

	public void Hide()
	{
		base.Pane.AlphaTo(0f);
		base.Icon.AlphaTo(0f);
		base.Number.AlphaTo(0f);
	}

	public void Show()
	{
		base.Pane.AlphaTo(0.25f);
		base.Icon.AlphaTo(1f);
		base.Number.AlphaTo(1f);
	}

	public void SetPrice(int price)
	{
		if (price != _currentPrice)
		{
			_currentPrice = price;
			SetNumber(price.ToString());
		}
	}
}
