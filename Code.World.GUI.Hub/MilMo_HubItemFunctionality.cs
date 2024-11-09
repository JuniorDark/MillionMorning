using System.Collections.Generic;
using System.Runtime.InteropServices;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Network;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI.Ladder;
using Code.World.GUI.PVP;
using Code.World.GUI.StartScreen;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Code.World.WorldMap;
using Core;
using Localization;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.Modal;
using UI.Sprites;
using UnityEngine;

namespace Code.World.GUI.Hub;

internal sealed class MilMo_HubItemFunctionality
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct StateTypes
	{
		public const string NEW_NEWS = "NEWNEWS";

		public const string SHOP_ENOUGH_CASH = "SHOPENOUCHCASH";

		public const string SHOP_HOT_ITEMS = "SHOPHOTITEMS";

		public const string NEW_LEVEL_IN_WORLD = "NEWLEVELINWORLD";

		public const string NEW_LEVEL_IN_CHAT_ROOMS = "NEWLEVELINCHATROOMS";

		public const string HOME_DELIVERY_BOX_ACTIVE = "HOMEDELIVERYBOXACTIVE";

		public const string HOME_GUEST = "HOMEGUEST";

		public const string UNKNOWN = "UNKNOWN";

		public const string FAKE_STATE = "FAKESTATE";
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct HubItemIdentifiers
	{
		public const string SHOP = "SHOP";

		public const string HOME = "HOME";

		public const string MAKEOVER_STUDIO = "MAKEOVERSTUDIO";

		public const string MAP_ROOM = "MAPROOM";

		public const string CHAT_ROOMS = "CHATROOMS";

		public const string NEWS = "NEWS";

		public const string ARENA = "COLOSSEUM";

		public const string WORLD = "WORLD";
	}

	private class ChatroomButton
	{
		public MilMo_ChatRoomInfoPopup InfoPopup;

		public MilMo_Button Chatroom;

		public MilMo_Widget Tag;
	}

	private static float _timeSinceClickHome = Time.time;

	private static float _timeSinceRun = Time.time;

	private static MilMo_HubInfoWindow _window;

	private static MilMo_StartScreenWindow _startScreenWindow;

	private static MilMo_UserInterface _ui;

	private List<ChatroomButton> _chatroomButtons;

	internal static readonly List<Texture2D> HotItemsTextures = new List<Texture2D>();

	private static readonly List<Texture2D> WorldLevelsTextures = new List<Texture2D>();

	private static readonly List<Texture2D> ChatroomLevelsTextures = new List<Texture2D>();

	private static readonly List<Texture2D> TempUnknownTextures = new List<Texture2D>();

	internal static Texture2D CurrentHomeDeliveryBoxTexture = null;

	internal static readonly List<Texture2D> GuestInHomeTextures = new List<Texture2D>();

	public static void OpenStartScreen()
	{
		if (_startScreenWindow == null)
		{
			_startScreenWindow = new MilMo_StartScreenWindow(_ui);
			_ui.AddChild(_startScreenWindow);
		}
		_startScreenWindow.Open((float)Screen.width * 0.5f - 285f, (float)Screen.height * 0.5f - 210f);
	}

	internal MilMo_HubItemFunctionality(MilMo_UserInterface ui)
	{
		_ui = ui;
		if (TempUnknownTextures.Count == 0)
		{
			AddTempUnknownTexturesAsync();
		}
	}

	private static async void AddTempUnknownTexturesAsync()
	{
		Texture2D black = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Core/Black");
		TempUnknownTextures.Add(black);
		Texture2D item = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Core/BlackTransparent");
		TempUnknownTextures.Add(item);
		TempUnknownTextures.Add(black);
		TempUnknownTextures.Add(item);
	}

	internal void Run(string function)
	{
		_timeSinceRun = Time.time;
		MilMo_HubInfoWindow window = _window;
		if (window != null && window.Enabled)
		{
			_window.Close();
		}
		if (MilMo_Player.Instance.EnteringOrLeaving || MilMo_Player.Instance.HasPendingRequests || Time.time - _timeSinceClickHome < 0.5f)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			return;
		}
		string[] array = function.Split('#');
		switch (array[0].ToUpper())
		{
		case "SHOP":
			if (MilMo_Player.Instance.OkToEnterShop())
			{
				MilMo_Player.Instance.RequestEnterShop();
			}
			break;
		case "HOME":
			ShowHomeMenu();
			break;
		case "MAKEOVERSTUDIO":
			if (MilMo_Player.Instance.OkToEnterCharBuilder())
			{
				MilMo_Player.Instance.RequestEnterCharBuilder();
			}
			break;
		case "MAPROOM":
			if (MilMo_Player.Instance.OkToEnterNavigator())
			{
				MilMo_WorldMap.UserOpen();
			}
			break;
		case "CHATROOMS":
			ShowChatRoomsMenu();
			break;
		case "NEWS":
			ShowNewsMenu();
			break;
		case "COLOSSEUM":
			ShowArenaMenu();
			break;
		case "WORLD":
			EnterWorld("World" + array[1]);
			break;
		default:
			ShowComingSoonPopup();
			break;
		}
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
	}

	private static void EnterWorld(string world)
	{
		MilMo_WorldInfoData worldInfoData = MilMo_LevelInfo.GetWorldInfoData(world);
		if (MilMo_LevelInfo.IsWorldLocked(world))
		{
			string worldIconPath = "Content/Worlds/" + world.Replace("World", "W") + "/WorldMapImages/WorldIcon";
			MilMo_Dialog lockedDialog = new MilMo_Dialog(_ui);
			MilMo_LocString locString = MilMo_Localization.GetLocString("WorldMap_362");
			MilMo_LocString locString2 = MilMo_Localization.GetLocString("WorldMap_8797");
			locString2.SetFormatArgs(worldInfoData.WorldDisplayName);
			lockedDialog.DoOK("Batch01/Textures/Dialog/Warning", locString, locString2, delegate
			{
				lockedDialog.CloseAndRemove(null);
			});
			_ui.AddChild(lockedDialog);
			LoadAndSetIconAsync(worldIconPath, lockedDialog);
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
		}
		else if (!string.IsNullOrEmpty(MilMo_Level.LastAdventureLevel) && world == MilMo_Level.LastAdventureLevel.Split(':')[0])
		{
			if (MilMo_Player.Instance.OkToEnterNavigator())
			{
				MilMo_Player.Instance.RequestEnterNavigator();
			}
		}
		else
		{
			MilMo_Hub.TravelWorld(world);
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
		}
	}

	private static async void LoadAndSetIconAsync(string worldIconPath, MilMo_Dialog lockedDialog)
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(worldIconPath, "WorldIcon");
		lockedDialog.Icon.SetTexture(new MilMo_Texture(texture));
		lockedDialog.Icon.SetPosition(65f, lockedDialog.Icon.PosMover.Target.y / _ui.Res.y + 10f);
		lockedDialog.Icon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		lockedDialog.Icon.SetScale(100f, 100f);
	}

	private void ShowArenaMenu()
	{
		_window = new MilMo_HubInfoWindow(_ui, new Vector2(140f, 85f));
		MilMo_Button milMo_Button = new MilMo_Button(_ui);
		milMo_Button.SetAllTextures("Batch01/Textures/HUD/IconLadderPVP");
		milMo_Button.SetScalePull(0.05f, 0.05f);
		milMo_Button.SetScaleDrag(0.6f, 0.7f);
		milMo_Button.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		milMo_Button.SetExtraScaleOnHover(5f, 5f);
		milMo_Button.SetScale(60f, 60f);
		milMo_Button.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("PVP_9334"));
		milMo_Button.UseParentAlpha = false;
		milMo_Button.SetPosition(0f, 10f);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.Function = delegate
		{
			if (CanUseMenuButton())
			{
				((MilMoPvpLadderWindow)MilMo_GlobalUI.Instance.GetItem("PvpLadderWindow")).Open();
				_window?.Close();
			}
		};
		_window.AddChild(milMo_Button);
		MilMo_Button milMo_Button2 = new MilMo_Button(_ui);
		milMo_Button2.SetAllTextures("Content/Bodypacks/Batch01/Generic/Icons/IconPVP", prefixStandardGuiPath: false);
		milMo_Button2.SetScalePull(0.05f, 0.05f);
		milMo_Button2.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button2.SetScaleDrag(0.6f, 0.7f);
		milMo_Button2.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		milMo_Button2.SetExtraScaleOnHover(5f, 5f);
		milMo_Button2.SetScale(60f, 60f);
		milMo_Button2.SetPosition(65f, 10f);
		milMo_Button2.UseParentAlpha = false;
		_window.AddChild(milMo_Button2);
		ChatroomButton t = new ChatroomButton
		{
			Chatroom = milMo_Button2,
			InfoPopup = new MilMo_ChatRoomInfoPopup(_ui, MilMo_Localization.GetNotLocalizedLocString("PVP"), MilMo_Localization.GetLocString("PVP_9391"), 240f, new List<bool>(), new List<bool>(), null)
		};
		_ui.AddChild(t.InfoPopup);
		t.Chatroom.PointerHoverFunction = delegate
		{
			t.InfoPopup.Open(new Vector2(_window.Pos.x - (t.InfoPopup.Scale.x + 15f), _window.Pos.y));
		};
		t.Chatroom.PointerLeaveFunction = delegate
		{
			t.InfoPopup.Close();
		};
		milMo_Button2.Function = delegate
		{
			if (CanUseMenuButton())
			{
				Singleton<GameNetwork>.Instance.RequestPvPQueues();
				_window.Close();
				t.InfoPopup.Close();
			}
		};
		_window.AddCloseButton();
		_ui.AddChild(_window);
		Vector2 screenPosition = MilMo_Hub.Instance.GetScreenPosition("COLOSSEUM");
		_window.Open(screenPosition.x - _window.Scale.x * 0.5f, screenPosition.y - _window.Scale.y);
	}

	public static void ShowComingSoonPopup()
	{
		if (_window != null)
		{
			_window.Close();
			_window = null;
		}
		_window = new MilMo_HubInfoWindow(_ui, new Vector2(150f, 170f));
		MilMo_Widget milMo_Widget = new MilMo_Widget(_ui);
		milMo_Widget.SetTexture("Batch01/Textures/Shop/ComingSoon");
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Widget.SetDefaultColor(1f, 1f, 1f, 1f);
		_window.AddChild(milMo_Widget);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(_ui);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Widget2.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget2.SetFont(MilMo_GUI.Font.EborgLarge);
		milMo_Widget2.SetText(MilMo_Localization.GetLocString("CharacterShop_265"));
		milMo_Widget2.SetDefaultTextColor(1f, 1f, 1f, 0.75f);
		milMo_Widget2.SetFontScale(0.6f);
		milMo_Widget2.TextOutlineColor = new Color(0f, 0f, 0f, 1f);
		_window.AddChild(milMo_Widget2);
		MilMo_Widget milMo_Widget3 = new MilMo_Widget(_ui);
		milMo_Widget3.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Widget3.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget3.SetFont(MilMo_GUI.Font.ArialRounded);
		milMo_Widget3.SetFontScale(0.8f);
		milMo_Widget3.SetText(MilMo_Localization.GetLocString("CharacterShop_266"));
		milMo_Widget3.SetTextDropShadowPos(2f, 2f);
		milMo_Widget3.SetDefaultTextColor(1f, 0.75f, 0.2f, 0.75f);
		_window.AddChild(milMo_Widget3);
		milMo_Widget.SetScale(75f, 118f);
		milMo_Widget.SetPosition(75f, 40f);
		milMo_Widget2.SetPosition(75f, -5f);
		milMo_Widget2.SetScale(225f, 43f);
		milMo_Widget2.SetTextOutline(1f, 1f);
		milMo_Widget2.SetTextDropShadowPos(3f, 3f);
		milMo_Widget3.SetPosition(75f, 23f);
		milMo_Widget3.SetScale(225f, 15f);
		_ui.AddChild(_window);
		_window.AddCloseButton();
		_window.Open(new Vector2(Screen.width / 2 - 75, Screen.height / 2 - 85));
	}

	private static void ShowNewsMenu()
	{
		if (_window != null)
		{
			_window.Close();
			_window = null;
		}
		OpenStartScreen();
	}

	private void ShowHomeMenu()
	{
		_window = new MilMo_HubInfoWindow(_ui, new Vector2(135f, 85f));
		MilMo_Button milMo_Button = new MilMo_Button(_ui);
		milMo_Button.SetAllTextures("Batch01/Textures/Homes/IconEnterHome");
		milMo_Button.SetScalePull(0.05f, 0.05f);
		milMo_Button.SetScaleDrag(0.6f, 0.7f);
		milMo_Button.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		milMo_Button.SetExtraScaleOnHover(5f, 5f);
		milMo_Button.SetScale(60f, 60f);
		milMo_Button.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("Homes_6807"));
		milMo_Button.UseParentAlpha = false;
		milMo_Button.SetPosition(65f, 10f);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.Function = delegate
		{
			if (CanUseMenuButton())
			{
				_timeSinceClickHome = Time.time;
				if (MilMo_Player.Instance.OkToLeaveHub())
				{
					MilMo_World.Instance.GoToHome(MilMo_Player.Instance.Id, MilMo_Player.Instance.Avatar.Name);
				}
				_window?.Close();
			}
		};
		_window.AddChild(milMo_Button);
		((MilMoLadderWindow)MilMo_GlobalUI.Instance.GetItem("HomeLadderWindow")).SetButton(delegate(int id)
		{
			MilMo_ProfileManager.RequestPlayerName(id.ToString(), delegate(string name, string playerId)
			{
				if (MilMo_Player.Instance.OkToLeaveHub())
				{
					MilMo_World.Instance.GoToHome(playerId, name);
				}
				_window?.Close();
				MilMo_GlobalUI.Instance.CloseItem("HomeLadderWindow");
			});
		}, "Batch01/Textures/Homes/IconEnterHome", MilMo_Localization.GetLocString("Messenger_FriendCard_7865"));
		MilMo_Button milMo_Button2 = new MilMo_Button(_ui);
		milMo_Button2.SetAllTextures("Batch01/Textures/HUD/IconLadderHome");
		milMo_Button2.SetScalePull(0.05f, 0.05f);
		milMo_Button2.SetScaleDrag(0.6f, 0.7f);
		milMo_Button2.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		milMo_Button2.SetExtraScaleOnHover(5f, 5f);
		milMo_Button2.SetScale(60f, 60f);
		milMo_Button2.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("Homes_13301"));
		milMo_Button2.UseParentAlpha = false;
		milMo_Button2.SetPosition(0f, 10f);
		milMo_Button2.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button2.Function = delegate
		{
			if (CanUseMenuButton())
			{
				((MilMoLadderWindow)MilMo_GlobalUI.Instance.GetItem("HomeLadderWindow")).Open();
				_window?.Close();
			}
		};
		_window.AddChild(milMo_Button2);
		_ui.AddChild(_window);
		_window.AddCloseButton();
		Vector2 screenPosition = MilMo_Hub.Instance.GetScreenPosition("HOME");
		_window.Open(screenPosition.x - _window.Scale.x * 0.5f, screenPosition.y - _window.Scale.y - 100f);
	}

	private void ShowChatRoomsMenu()
	{
		if (_chatroomButtons == null)
		{
			_chatroomButtons = new List<ChatroomButton>();
		}
		_chatroomButtons.Clear();
		IList<MilMo_LevelInfoData> chatRooms = MilMo_LevelInfo.GetChatRooms();
		if (chatRooms == null)
		{
			Debug.LogWarning("Creating chat rooms menu, But chat rooms are null!");
			return;
		}
		float x = ((chatRooms.Count > 4) ? 260 : (chatRooms.Count * 65 + 5));
		_window = new MilMo_HubInfoWindow(_ui, new Vector2(x, 85f));
		for (int i = 0; i < chatRooms.Count; i++)
		{
			CreateChatroomItem(chatRooms, i);
		}
		Vector2 screenPosition = MilMo_Hub.Instance.GetScreenPosition("CHATROOMS");
		_window.Open(screenPosition.x - _window.Scale.x * 0.5f, screenPosition.y - _window.Scale.y);
	}

	private void CreateChatroomItem(IList<MilMo_LevelInfoData> chatRooms, int index)
	{
		MilMo_Button milMo_Button = new MilMo_Button(_ui)
		{
			AllowPointerFocus = false
		};
		string text = chatRooms[index].World.Replace("orld", "");
		string text2 = chatRooms[index].Level.Replace("evel", "");
		string filename = "Content/Worlds/" + text + "/LevelIcons/LevelIcon" + text + text2;
		milMo_Button.SetScale(60f, 60f);
		milMo_Button.UseParentAlpha = true;
		milMo_Button.SetScalePull(0.05f, 0.05f);
		milMo_Button.SetScaleDrag(0.6f, 0.7f);
		milMo_Button.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		milMo_Button.SetExtraScaleOnHover(5f, 5f);
		milMo_Button.SetAllTextures(filename, prefixStandardGuiPath: false);
		milMo_Button.SetAlpha(0f);
		milMo_Button.AllowPointerFocus = true;
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.UseParentAlpha = false;
		if (chatRooms[index].IsMembersOnlyArea)
		{
			MilMo_Widget milMo_Widget = new MilMo_Widget(_ui);
			milMo_Button.AddChild(milMo_Widget);
			milMo_Widget.SetTexture("Batch01/Textures/HUD/TagPremium");
			milMo_Widget.AllowPointerFocus = false;
			milMo_Widget.SetScale(20f, 20f);
			milMo_Widget.SetAlignment(MilMo_GUI.Align.BottomRight);
			milMo_Widget.SetPosition(milMo_Button.Scale);
		}
		string fullLevelName = chatRooms[index].FullLevelName;
		milMo_Button.SetPosition(index * 65, 10f);
		ChatroomButton t = new ChatroomButton
		{
			Chatroom = milMo_Button,
			Tag = new MilMo_PlayerCountTag(_ui)
			{
				FixedRes = true
			}
		};
		t.Tag.SetAlignment(MilMo_GUI.Align.TopRight);
		t.Tag.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		t.Tag.SetTexture("Batch01/Textures/WorldMap/SplineDot");
		t.Tag.SetFont(MilMo_GUI.Font.EborgSmall);
		t.Tag.SetDefaultColor(0f, 0f, 0f, 0.8f);
		t.Tag.SetFontScale(0.8f);
		t.Tag.Enabled = false;
		t.Tag.SetDefaultTextColor(1f, 1f, 1f, 1f);
		t.Tag.SetScale(28f, 28f);
		t.Tag.SetPosition(t.Chatroom.Scale.x, 0f);
		t.InfoPopup = new MilMo_ChatRoomInfoPopup(_ui, chatRooms[index].DisplayName, chatRooms[index].WorldMapDescription, 240f, chatRooms[index].ExplorationTokens, chatRooms[index].CoinTokens, chatRooms[index].PremiumToken);
		_ui.AddChild(t.InfoPopup);
		t.Chatroom.PointerHoverFunction = delegate
		{
			t.InfoPopup.Open(new Vector2(_window.Pos.x + _window.Scale.x + 15f, _window.Pos.y));
		};
		t.Chatroom.PointerLeaveFunction = delegate
		{
			t.InfoPopup.Close();
		};
		if (!RoomIsMemberOnly(chatRooms, index) || (MilMo_Player.Instance != null && MilMo_Player.Instance.IsMember))
		{
			if (TargetRoomIsDifferentThanCurrent(chatRooms, index))
			{
				SetupTravelToDestinationCallback(milMo_Button, t, fullLevelName);
			}
			else
			{
				SetupCloseTownCallback(milMo_Button, t);
			}
		}
		else
		{
			SetupVipRequiredCallback(milMo_Button);
		}
		milMo_Button.AddChild(t.Tag);
		_chatroomButtons.Add(t);
		_window.AddChild(milMo_Button);
		_window.AddCloseButton();
		_ui.AddChild(_window);
	}

	private void SetupCloseTownCallback(MilMo_Button button, ChatroomButton t)
	{
		button.Function = delegate
		{
			if (CanUseMenuButton())
			{
				if (MilMo_Player.Instance.OkToLeaveHub())
				{
					MilMo_Hub.WasTravelClosed = false;
					_window.Close();
					t.InfoPopup.Close();
					MilMo_Player.Instance.RequestLeaveHub();
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
				}
				else
				{
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
				}
				button.IgnoreNextStepDueToBringToFront = true;
			}
		};
	}

	private void SetupTravelToDestinationCallback(MilMo_Button button, ChatroomButton t, string levelName)
	{
		button.Function = delegate
		{
			if (CanUseMenuButton())
			{
				if (MilMo_Player.Instance.OkToLeaveHub())
				{
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
					_window.Close();
					t.InfoPopup.Close();
					MilMo_Hub.WasTravelClosed = true;
					MilMo_Hub.TravelClosedFullLevelName = levelName;
					MilMo_Player.Instance.RequestLeaveHub();
				}
				else
				{
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
				}
				button.IgnoreNextStepDueToBringToFront = true;
			}
		};
	}

	private static bool TargetRoomIsDifferentThanCurrent(IList<MilMo_LevelInfoData> chatRooms, int index)
	{
		if (MilMo_Level.CurrentLevel != null)
		{
			return chatRooms[index].FullLevelName != MilMo_Level.CurrentLevel.VerboseName;
		}
		return true;
	}

	private static bool RoomIsMemberOnly(IList<MilMo_LevelInfoData> chatRooms, int index)
	{
		return chatRooms[index].IsMembersOnlyArea;
	}

	private void SetupVipRequiredCallback(MilMo_Button button)
	{
		button.Function = delegate
		{
			if (CanUseMenuButton())
			{
				DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("World_7628"), new LocalizedStringWithArgument("World_7629"), "Shop:Batch01.Subscriptions.SubscriptionSixMonths", new AddressableSpriteLoader("IconPremium")));
				button.IgnoreNextStepDueToBringToFront = true;
				button.IgnoreNextClick = true;
			}
		};
	}

	private bool CanUseMenuButton()
	{
		return Time.time - _timeSinceRun > 0.2f;
	}

	internal MilMo_HubBubble GetBubble(string stateType, Vector2 position, MilMo_HubBubble.OnClose onCloseCallback)
	{
		MilMo_HubBubble milMo_HubBubble = new MilMo_HubBubble(_ui, new Vector2(60f, 60f), onCloseCallback);
		switch (stateType)
		{
		case "NEWNEWS":
			milMo_HubBubble.SetIconTexture("Batch01/Textures/SmartPhone/IconNews");
			break;
		case "SHOPENOUCHCASH":
			milMo_HubBubble.SetIconTexture("Batch01/Textures/HUD/IconVoucherPoint");
			break;
		case "HOMEGUEST":
			if (GuestInHomeTextures.Count > 1)
			{
				milMo_HubBubble.SetIconTextureList(GuestInHomeTextures);
			}
			else
			{
				milMo_HubBubble.SetIconTexture(GuestInHomeTextures[0]);
			}
			break;
		case "HOMEDELIVERYBOXACTIVE":
			milMo_HubBubble.SetIconTexture(CurrentHomeDeliveryBoxTexture);
			break;
		case "SHOPHOTITEMS":
			milMo_HubBubble.SetIconTextureList(HotItemsTextures);
			break;
		case "NEWLEVELINWORLD":
			milMo_HubBubble.SetIconTextureList(WorldLevelsTextures);
			break;
		case "NEWLEVELINCHATROOMS":
			milMo_HubBubble.SetIconTextureList(ChatroomLevelsTextures);
			break;
		case "FAKESTATE":
		case "UNKNOWN":
			milMo_HubBubble.SetIconTexture("Batch01/Textures/Core/BlackTransparent");
			break;
		}
		milMo_HubBubble.SetPosition(position);
		milMo_HubBubble.Enabled = false;
		_ui.AddChild(milMo_HubBubble);
		return milMo_HubBubble;
	}
}
