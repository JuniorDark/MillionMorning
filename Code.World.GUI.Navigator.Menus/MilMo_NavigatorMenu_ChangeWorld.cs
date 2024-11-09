using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Code.World.WorldMap;
using Core.Analytics;
using UnityEngine;

namespace Code.World.GUI.Navigator.Menus;

public sealed class MilMo_NavigatorMenu_ChangeWorld : MilMo_NavigatorMenuItem
{
	private List<MilMo_Button> _mWorldList;

	public MilMo_NavigatorMenu_ChangeWorld(MilMo_UserInterface ui)
		: base(ui)
	{
	}

	public override MilMo_Widget CreateMenu()
	{
		Menu = new MilMo_Widget(UI);
		Menu.AllowPointerFocus = true;
		MScrollView = new MilMo_ScrollView(UI);
		MScrollView.SetAlignment(MilMo_GUI.Align.TopLeft);
		MScrollView.SetPosition(0f, 10f);
		Menu.AddChild(MScrollView);
		Menu.SetScale(0f, 0f);
		MScrollView.ShowHorizontalBar(h: false);
		MScrollView.SetScale(0f, 0f);
		MScrollView.SetViewSize(0f, 0f);
		_mWorldList = new List<MilMo_Button>();
		return Menu;
	}

	private MilMo_Button CreateWorldButton(MilMo_WorldInfoData world, bool isCurrentWorld = false)
	{
		bool isLocked = MilMo_LevelInfo.IsWorldLocked(world.World);
		string worldIconPath = "Content/Worlds/" + world.World.Replace("World", "W") + "/WorldMapImages/WorldIcon";
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetScale(270f, 60f);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.AllowPointerFocus = true;
		milMo_Button.Function = delegate
		{
			if (isLocked)
			{
				MilMo_Dialog lockedDialog = new MilMo_Dialog(UI);
				MilMo_LocString locString = MilMo_Localization.GetLocString("WorldMap_362");
				MilMo_LocString locString2 = MilMo_Localization.GetLocString("WorldMap_8797");
				locString2.SetFormatArgs(world.WorldDisplayName);
				lockedDialog.DoOK("Batch01/Textures/Dialog/Warning", locString, locString2, delegate
				{
					lockedDialog.CloseAndRemove(null);
				});
				UI.AddChild(lockedDialog);
				LoadAndSetLockedIconAsync(lockedDialog, worldIconPath);
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			}
			else
			{
				Analytics.ChangeWorld(world.World);
				MilMo_WorldMap.Activate(delegate
				{
					MilMo_EventSystem.At(2f, delegate
					{
						MilMo_Player.Instance.InNavigator = true;
						MilMo_Player.Instance.EnteringNavigator = false;
					});
				}, world.World);
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
			}
		};
		milMo_Button.SetHoverSound(new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/CharBuilderTick"));
		MilMo_Button icon = new MilMo_Button(UI);
		icon.SetAllTextures(worldIconPath, prefixStandardGuiPath: false);
		icon.SetScale(60f, 60f);
		icon.UseParentAlpha = false;
		icon.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		icon.SetExtraScaleOnHover(3f, 3f);
		icon.SetAlignment(MilMo_GUI.Align.CenterLeft);
		icon.SetPosPull(0.08f, 0.08f);
		icon.SetPosDrag(0.6f, 0.6f);
		icon.SetScalePull(0.05f, 0.05f);
		icon.SetScaleDrag(0.6f, 0.7f);
		icon.AllowPointerFocus = false;
		if (isLocked)
		{
			icon.SetDefaultColor(1f, 1f, 1f, 0.5f);
		}
		icon.SetPosition(0f, milMo_Button.Scale.y * 0.5f);
		milMo_Button.AddChild(icon);
		MilMo_Widget text = new MilMo_Widget(UI);
		text.SetFont(MilMo_GUI.Font.EborgMedium);
		text.SetAlignment(MilMo_GUI.Align.CenterLeft);
		text.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		text.SetText(world.WorldDisplayName);
		text.SetScale(world.WorldDisplayName.Length * 16, 60f);
		text.SetFontPreset(MilMo_GUI.FontPreset.DropShadow);
		text.UseParentAlpha = false;
		text.SetPosition(74f, milMo_Button.Scale.y * 0.5f);
		text.AllowPointerFocus = false;
		text.FadeToDefaultTextColor = false;
		if (isLocked)
		{
			text.SetTextColor(0.6f, 0.6f, 0.6f, 0.5f);
			text.SetDefaultTextColor(0.6f, 0.6f, 0.6f, 0.5f);
		}
		else if (isCurrentWorld)
		{
			text.SetTextColor(0.85f, 0.85f, 0.85f, 1f);
			text.SetDefaultTextColor(0.85f, 0.85f, 0.85f, 1f);
		}
		else
		{
			text.SetTextColor(0.65f, 0.65f, 0.65f, 1f);
			text.SetDefaultTextColor(0.65f, 0.65f, 0.65f, 1f);
		}
		milMo_Button.AddChild(text);
		if (isLocked)
		{
			return milMo_Button;
		}
		milMo_Button.PointerHoverFunction = delegate
		{
			icon.HoverScaleImpulse();
			text.SetTextColor(1f, 1f, 1f, 1f);
		};
		if (isCurrentWorld)
		{
			milMo_Button.PointerLeaveFunction = delegate
			{
				text.SetTextColor(0.85f, 0.85f, 0.85f, 1f);
			};
		}
		else
		{
			milMo_Button.PointerLeaveFunction = delegate
			{
				text.SetTextColor(0.65f, 0.65f, 0.65f, 1f);
			};
		}
		return milMo_Button;
	}

	private async void LoadAndSetLockedIconAsync(MilMo_Dialog lockedDialog, string worldIconPath)
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(worldIconPath, "WorldIcon");
		lockedDialog.Icon.SetTexture(new MilMo_Texture(texture));
		lockedDialog.Icon.SetPosition(65f, lockedDialog.Icon.PosMover.Target.y / UI.Res.y + 10f);
		lockedDialog.Icon.SetScale(0f, 0f);
		lockedDialog.Icon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		lockedDialog.Icon.SetScale(0f, 0f);
		lockedDialog.Icon.SetScale(100f, 100f);
	}

	public override void UpdateMenu()
	{
		IEnumerable<MilMo_WorldInfoData> worldInfoDataArray = MilMo_LevelInfo.GetWorldInfoDataArray();
		_mWorldList.Clear();
		Vector2 position = new Vector2(5f, 0f);
		MScrollView.RemoveAllChildren();
		float num = 300f;
		foreach (MilMo_WorldInfoData item in worldInfoDataArray)
		{
			if (item.VisibleInGUILists)
			{
				bool isCurrentWorld = item.World == MilMo_World.CurrentWorld;
				MilMo_Button milMo_Button = CreateWorldButton(item, isCurrentWorld);
				milMo_Button.SetPosition(position);
				milMo_Button.SetDefaultColor(1f, 1f, 1f, 0.5f);
				position.y += 62f;
				_mWorldList.Add(milMo_Button);
				MScrollView.AddChild(milMo_Button);
				if (milMo_Button.Scale.x > num)
				{
					num = milMo_Button.Scale.x;
				}
			}
		}
		if (position.y <= 248f)
		{
			MScrollView.SetScale(num + 30f + 5f, position.y);
		}
		else
		{
			MScrollView.SetScale(num + 30f + 5f, 248f);
			MScrollView.ShowVerticalBar(v: true);
		}
		MScrollView.RefreshViewSize();
		Menu.SetScale(MScrollView.Scale.x + 10f, MScrollView.Scale.y + 10f);
	}
}
