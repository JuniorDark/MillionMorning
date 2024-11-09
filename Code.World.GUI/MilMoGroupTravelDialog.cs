using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.ResourceSystem;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Core;
using Player;
using UnityEngine;

namespace Code.World.GUI;

public class MilMoGroupTravelDialog : MilMo_Window
{
	private float _timeToShow;

	private readonly string _destination;

	private readonly MilMo_LocString _destinationName;

	private string _initiatorName;

	private readonly string _initiatorId;

	private MilMo_Button _yesButton;

	private MilMo_Button _noButton;

	private MilMo_Widget _timeLeftWidget;

	private MilMo_Widget _textBlock;

	private MilMo_TimerEvent _tickEvent;

	private MilMo_LocString _timeLeftText;

	public MilMoGroupTravelDialog(MilMo_UserInterface ui, int timeInSeconds, string destination, string initiatorId)
		: base(ui)
	{
		_destination = destination;
		_destinationName = MilMo_LevelInfo.GetLevelDisplayName(destination);
		_timeToShow = timeInSeconds;
		_initiatorId = initiatorId;
		MilMo_ProfileManager.RequestPlayerName(initiatorId, delegate(string playerName, string playerId)
		{
			_initiatorName = playerName;
			Initialize();
		});
	}

	private void Initialize()
	{
		_timeLeftText = MilMo_Localization.GetLocString("World_11377");
		_yesButton = new MilMo_Button(UI);
		_yesButton.SetAllTextures("Batch01/Textures/Dialog/ButtonNormal");
		_yesButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_yesButton.SetText(MilMo_Localization.GetLocString("Generic_Yes"));
		_yesButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_yesButton.SetFontPreset(MilMo_GUI.FontPreset.Outline);
		_yesButton.Function = delegate
		{
			if (MilMo_Player.Instance.OkToTeleport())
			{
				Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupTravel(_destination, 1));
				ReadyToTravel();
			}
		};
		_noButton = new MilMo_Button(UI);
		_noButton.SetAllTextures("Batch01/Textures/Dialog/ButtonNormal");
		_noButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_noButton.SetText(MilMo_Localization.GetLocString("Generic_No"));
		_noButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_noButton.SetFontPreset(MilMo_GUI.FontPreset.Outline);
		_noButton.Function = delegate
		{
			MilMo_EventSystem.RemoveTimerEvent(_tickEvent);
			_tickEvent = null;
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupTravel(_destination, 0));
			Close(null);
		};
		AddChild(_noButton);
		AddChild(_yesButton);
		SetText(MilMo_Localization.GetLocString("Interact_Travel"));
		MilMo_LocString locString = MilMo_Localization.GetLocString("World_11376");
		locString.SetFormatArgs(_initiatorName, _destinationName);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetTexture("Batch01/Textures/FriendList/GroupIcon");
		milMo_Widget.SetScale(64f, 64f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetPosition(25f, 35f);
		AddChild(milMo_Widget);
		_textBlock = new MilMo_Widget(UI);
		_textBlock.SetAlignment(MilMo_GUI.Align.TopLeft);
		_textBlock.SetFont(MilMo_GUI.Font.ArialRounded);
		_textBlock.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_textBlock.SetWordWrap(w: true);
		_textBlock.SetText(locString);
		_textBlock.AllowPointerFocus = false;
		AddChild(_textBlock);
		_timeLeftWidget = new MilMo_Widget(UI);
		_timeLeftWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_timeLeftWidget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_timeLeftWidget.SetFont(MilMo_GUI.Font.ArialRounded);
		_timeLeftWidget.AllowPointerFocus = false;
		MilMo_LocString copy = _timeLeftText.GetCopy();
		copy.SetFormatArgs(_timeToShow);
		_timeLeftWidget.SetText(copy);
		AddChild(_timeLeftWidget);
		FixUI();
		if (MilMo_Player.Instance.Id == _initiatorId)
		{
			ReadyToTravel();
		}
		UI.AddChild(this);
		Open();
		_tickEvent = MilMo_EventSystem.At(1f, DoTick);
	}

	private void FixUI()
	{
		SpawnScale = new Vector2(350f, 180f);
		TargetScale = SpawnScale;
		_yesButton.SetScale(120f, 24f);
		_yesButton.SetPosition(SpawnScale.x * 0.5f + 70f, SpawnScale.y - 25f);
		_noButton.SetScale(120f, 24f);
		_noButton.SetPosition(SpawnScale.x * 0.5f - 70f, SpawnScale.y - 25f);
		_textBlock.SetScale(TargetScale.x - 125f, TargetScale.y - 50f);
		_textBlock.SetPosition(100f, 35f);
		_timeLeftWidget.SetScale(SpawnScale.x, 24f);
		_timeLeftWidget.SetPosition(0f, SpawnScale.y - 60f);
	}

	private void DoTick()
	{
		_timeToShow -= 1f;
		MilMo_LocString copy = _timeLeftText.GetCopy();
		copy.SetFormatArgs(_timeToShow);
		_timeLeftWidget.SetText(copy);
		if (_timeToShow == -1f)
		{
			MilMo_EventSystem.RemoveTimerEvent(_tickEvent);
			_tickEvent = null;
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupTravel(_destination, 0));
			Close(null);
		}
		else
		{
			_tickEvent = MilMo_EventSystem.At(1f, DoTick);
		}
	}

	private void ReadyToTravel()
	{
		GroupManager groupManager = GroupManager.Get();
		RemoveChild(_yesButton);
		RemoveChild(_noButton);
		MilMo_LocString locString = MilMo_Localization.GetLocString("World_11375");
		locString.SetFormatArgs(groupManager.HasAcceptedTravel + 1, groupManager.NrOfGroupMembers);
		_textBlock.SetText(locString);
	}
}
