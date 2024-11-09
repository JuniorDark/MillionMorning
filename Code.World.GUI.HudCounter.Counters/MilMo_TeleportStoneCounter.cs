using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.ResourceSystem;
using Code.World.CharacterShop;
using Code.World.Player;
using UnityEngine;

namespace Code.World.GUI.HudCounter.Counters;

public class MilMo_TeleportStoneCounter : MilMo_HudCounter
{
	public MilMo_TeleportStoneCounter(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "TeleportStoneCounter";
		SpawnPosition = new Vector2(-180f, 80f);
		TargetPosition = new Vector2(30f, 80f);
		AllowPointerFocus = true;
		FlashColor = new Color(1f, 1f, 0f, 1f);
		LoadAndSetIconAsync();
		base.Number.SetTextNoLocalization("0");
		MilMo_Button milMo_Button = new MilMo_Button(ui);
		milMo_Button.SetScale(60f, 60f);
		milMo_Button.SetPosition(70f, 130f);
		milMo_Button.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("WorldMap_8823"));
		milMo_Button.AllowPointerFocus = true;
		milMo_Button.Function = delegate
		{
			MilMo_Player.Instance.RequestEnterShop();
			MilMo_CharacterShop.SelectItem("Shop:Batch01.TeleportStones.Telepod5");
		};
		UI.AddChild(milMo_Button);
	}

	private async void LoadAndSetIconAsync()
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/HUD/IconTeleportStoneCounter");
		base.Icon.SetTexture(new MilMo_Texture(texture));
	}
}
