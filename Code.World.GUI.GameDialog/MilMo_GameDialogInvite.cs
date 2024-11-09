using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public sealed class MilMo_GameDialogInvite : MilMo_GameDialogGenericTextBox
{
	private static MilMo_TimerEvent _timeToReEnable;

	private static int _currentInviteCount;

	private static int _nextRewardInviteCount;

	private static MilMo_Item _nextReward;

	private static int _nextRewardAmount;

	private readonly MilMo_TutorialImage _rewardImage;

	private readonly MilMo_Widget _rewardName;

	public MilMo_GameDialogInvite(MilMo_UserInterface ui, MilMo_Button.ButtonFunc inviteFunction, MilMo_Button.ButtonFunc skipFunction)
		: base(ui, ButtonMode.SkipInvite, inviteFunction, skipFunction)
	{
		Vector2 res = ui.Res;
		ui.Res = new Vector2(1f, 1f);
		base.CustomJinglePath = "Content/Sounds/Batch01/GUI/GamePlayDialog/InviteJingle";
		Identifier = "GameDialogInvite";
		SetHeadlineText(MilMo_LocString.Empty);
		SetEventDescription(MilMo_LocString.Empty);
		MilMo_LocString milMo_LocString;
		if (_nextReward == null)
		{
			milMo_LocString = MilMo_Localization.GetLocString("World_387");
		}
		else
		{
			milMo_LocString = MilMo_Localization.GetLocString("Generic_4997").GetCopy();
			milMo_LocString.SetFormatArgs(_nextRewardInviteCount - _currentInviteCount);
		}
		SetText(MilMo_Localization.GetLocString("World_386"), milMo_LocString);
		if (_nextReward != null)
		{
			_rewardImage = new MilMo_TutorialImage(ui, "", new Vector2(20f, 0f));
			AddChild(_rewardImage);
			_nextReward.AsyncGetIcon(_rewardImage.SetTexture);
			_rewardName = new MilMo_Widget(ui);
			_rewardName.SetPosition(14f, MTextBody.PosMover.Target.y + MTextBody.ScaleMover.Target.y - 18f);
			_rewardName.SetScale(210f, 46f);
			_rewardName.SetAlignment(MilMo_GUI.Align.TopLeft);
			_rewardName.SetTextAlignment(MilMo_GUI.Align.TopCenter);
			_rewardName.SetWordWrap(w: false);
			_rewardName.SetFont(MilMo_GUI.Font.EborgSmall);
			_rewardName.SetFontScale(0.9f);
			MilMo_LocString milMo_LocString2;
			if (_nextRewardAmount > 1)
			{
				milMo_LocString2 = MilMo_Localization.GetLocString("World_8003").GetCopy();
				milMo_LocString2.SetFormatArgs(_nextRewardAmount, _nextReward.Template.DisplayName);
			}
			else
			{
				milMo_LocString2 = _nextReward.Template.DisplayName;
			}
			_rewardName.SetText(milMo_LocString2);
			_rewardName.SetDefaultTextColor(ItemTextColor);
			_rewardName.SetAlpha(0f);
			_rewardName.FadeToDefaultColor = false;
			_rewardName.SetFadeSpeed(0.01f);
			_rewardName.SetEnabled(e: true);
			_rewardName.AllowPointerFocus = false;
			TextBox.AddChild(_rewardName);
		}
		IconScale = new Vector2(140f, 181f);
		IconPos = new Vector2(90f, 60f);
		Icon.SetAlignment(MilMo_GUI.Align.BottomCenter);
		ui.Res = res;
	}

	protected override void ShowIcon()
	{
		base.ShowIcon();
		Icon.GoToNow(IconPos);
		Icon.ScaleNow(0f, 0f);
		if (ShowIconEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(ShowIconEvent);
		}
		ShowIconEvent = MilMo_EventSystem.At(0f, delegate
		{
			Icon.SetAlpha(0f);
			Icon.AlphaTo(1f);
			Icon.SetScalePull(0.1f, 0.1f);
			Icon.SetScaleDrag(0.4f, 0.4f);
			Icon.ScaleNow(0f, 0f);
			Icon.ScaleTo(IconScale);
		});
	}

	public override void Hide()
	{
		base.Hide();
		if (_rewardImage != null)
		{
			_rewardImage.Close();
		}
	}

	protected override void ShowSecondaryWidgets()
	{
		base.ShowSecondaryWidgets();
		if (_rewardName != null)
		{
			_rewardName.AlphaTo(1f);
		}
	}

	protected override void SetHeight()
	{
		float num = MTextBody.PosMover.Target.y + MTextBody.ScaleMover.Target.y + 15f;
		if (_rewardName != null)
		{
			num += _rewardName.ScaleMover.Target.y;
		}
		TextBox.SetYScale(num);
		Height = TextBox.PosMover.Target.y + TextBox.ScaleMover.Target.y + 15f;
		Height += ButtonRight.ScaleMover.Target.y + 30f;
	}

	public static void SetNextRewardInfo(int currentInviteCount, int nextRewardInviteCount, MilMo_Item nextReward, int nextRewardAmount)
	{
		_currentInviteCount = currentInviteCount;
		_nextRewardInviteCount = nextRewardInviteCount;
		_nextReward = nextReward;
		_nextRewardAmount = nextRewardAmount;
	}

	public static void DisableFor(float seconds)
	{
		MilMo_EventSystem.RemoveTimerEvent(_timeToReEnable);
		_timeToReEnable = MilMo_EventSystem.At(seconds, delegate
		{
		});
	}
}
