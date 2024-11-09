using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public sealed class MilMo_GameDialogInviteReward : MilMo_GameDialogGenericTextBox
{
	private readonly Color _mNextRewardBoxColor = new Color(0.85f, 0.85f, 0.85f);

	private readonly Color _mDarkTextColor = new Color(0.2f, 0.2f, 0.2f, 1f);

	private readonly Color _mBrightTextColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	private readonly bool _mHaveNextReward;

	private readonly MilMo_Widget _mNextRewardName;

	private readonly MilMo_Widget _mNextRewardIcon;

	private readonly MilMo_Widget _mNextRewardCriteria;

	public MilMo_GameDialogInviteReward(MilMo_UserInterface ui, MilMo_Button.ButtonFunc okayFunction, int invitedCount, MilMo_Item reward, int rewardAmount, int nextRewardInviteCount, MilMo_Item nextReward, int nextRewardAmount, bool isAcceptedRewards)
		: base(ui, ButtonMode.Okay, okayFunction, null)
	{
		Vector2 res = ui.Res;
		ui.Res = new Vector2(1f, 1f);
		SetHeadlineText(MilMo_Localization.GetLocString("Generic_4992"));
		SetEventDescription(MilMo_Localization.GetLocString("Generic_4993"));
		SetHudDestination(HudDestination.Bag);
		Identifier = "GameDialogInviteReward_" + reward.Template.Path;
		base.CustomJinglePath = "Content/Sounds/Batch01/GUI/GamePlayDialog/ReceivedItemJingle";
		ShowSecondaryWidgetsTime = 2f;
		MilMo_LocString milMo_LocString;
		if (rewardAmount > 1)
		{
			milMo_LocString = MilMo_Localization.GetLocString("World_8003").GetCopy();
			milMo_LocString.SetFormatArgs(rewardAmount, reward.Template.DisplayName);
		}
		else
		{
			milMo_LocString = reward.Template.DisplayName;
		}
		MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_4994");
		if (isAcceptedRewards)
		{
			locString = MilMo_Localization.GetLocString("Messenger_FriendInvites_12598");
		}
		locString.SetFormatArgs(invitedCount);
		SetText(milMo_LocString, locString);
		_mHaveNextReward = nextReward != null;
		bool flag = _mHaveNextReward && !(nextReward is MilMo_Gem) && !(nextReward is MilMo_Coin);
		if (_mHaveNextReward)
		{
			MilMo_Widget milMo_Widget = new MilMo_Widget(ui);
			milMo_Widget.SetPosition(14f, TextBox.PosMover.Target.y + TextBox.ScaleMover.Target.y + 15f);
			milMo_Widget.SetScale(236f, flag ? 124 : 104);
			milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
			milMo_Widget.SetTexture("Batch01/Textures/GameDialog/GameDialogBoxBig");
			milMo_Widget.SetDefaultColor(_mNextRewardBoxColor);
			milMo_Widget.SetEnabled(e: true);
			milMo_Widget.AllowPointerFocus = false;
			AddChild(milMo_Widget);
			MilMo_Widget milMo_Widget2 = new MilMo_Widget(ui);
			milMo_Widget2.SetPosition(14f, 5f);
			milMo_Widget2.SetScale(236f, 50f);
			milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
			milMo_Widget2.SetTextAlignment(MilMo_GUI.Align.TopLeft);
			milMo_Widget2.SetFont(MilMo_GUI.Font.EborgSmall);
			milMo_Widget2.SetFontScale(0.8f);
			milMo_Widget2.SetText(MilMo_Localization.GetLocString("Generic_4996"));
			milMo_Widget2.SetDefaultTextColor(_mDarkTextColor);
			milMo_Widget2.SetAlpha(1f);
			milMo_Widget2.FadeToDefaultColor = false;
			milMo_Widget2.SetFadeSpeed(0.01f);
			milMo_Widget2.SetEnabled(e: true);
			milMo_Widget2.AllowPointerFocus = false;
			milMo_Widget.AddChild(milMo_Widget2);
			_mNextRewardCriteria = new MilMo_Widget(ui);
			_mNextRewardCriteria.SetPosition(20f, 33f);
			_mNextRewardCriteria.SetScale(204f, 33f);
			_mNextRewardCriteria.SetAlignment(MilMo_GUI.Align.TopLeft);
			_mNextRewardCriteria.SetTextAlignment(MilMo_GUI.Align.TopLeft);
			_mNextRewardCriteria.SetWordWrap(w: true);
			_mNextRewardCriteria.SetFont(MilMo_GUI.Font.ArialRounded);
			MilMo_LocString copy = MilMo_Localization.GetLocString("Generic_4995").GetCopy();
			if (isAcceptedRewards)
			{
				copy = MilMo_Localization.GetLocString("Messenger_FriendInvites_12597").GetCopy();
			}
			copy.SetFormatArgs(nextRewardInviteCount);
			_mNextRewardCriteria.SetText(copy);
			_mNextRewardCriteria.SetDefaultTextColor(_mBrightTextColor);
			_mNextRewardCriteria.SetAlpha(0f);
			_mNextRewardCriteria.FadeToDefaultColor = false;
			_mNextRewardCriteria.SetFadeSpeed(0.01f);
			_mNextRewardCriteria.SetEnabled(e: true);
			_mNextRewardCriteria.AllowPointerFocus = false;
			milMo_Widget.AddChild(_mNextRewardCriteria);
			MilMo_Widget milMo_Widget3 = new MilMo_Widget(ui);
			milMo_Widget3.SetPosition(14f, 73f);
			milMo_Widget3.SetScale(236f, 50f);
			milMo_Widget3.SetAlignment(MilMo_GUI.Align.TopLeft);
			milMo_Widget3.SetTextAlignment(MilMo_GUI.Align.TopLeft);
			milMo_Widget3.SetFont(MilMo_GUI.Font.EborgSmall);
			milMo_Widget3.SetFontScale(0.8f);
			milMo_Widget3.SetText(MilMo_Localization.GetLocString("World_365"));
			milMo_Widget3.SetDefaultTextColor(_mDarkTextColor);
			milMo_Widget3.SetAlpha(1f);
			milMo_Widget3.FadeToDefaultColor = false;
			milMo_Widget3.SetFadeSpeed(0.01f);
			milMo_Widget3.SetEnabled(e: true);
			milMo_Widget3.AllowPointerFocus = false;
			milMo_Widget.AddChild(milMo_Widget3);
			_mNextRewardIcon = new MilMo_Widget(ui);
			_mNextRewardIcon.SetAlignment(MilMo_GUI.Align.CenterCenter);
			_mNextRewardIcon.SetFont(MilMo_GUI.Font.EborgSmall);
			_mNextRewardIcon.SetFontScale(0.9f);
			if (flag)
			{
				_mNextRewardIcon.SetPosition(212f, 98f);
				_mNextRewardIcon.SetScale(32f, 32f);
				_mNextRewardIcon.SetTextOffset(-156f, 5f);
				if (nextRewardAmount > 1)
				{
					_mNextRewardIcon.SetTextNoLocalization("x  " + nextRewardAmount);
				}
				else
				{
					_mNextRewardIcon.SetText(MilMo_LocString.Empty);
				}
				nextReward?.AsyncGetIcon(_mNextRewardIcon.SetTexture);
			}
			else
			{
				_mNextRewardIcon.SetPosition(212f, 84f);
				_mNextRewardIcon.SetScale(45f, 45f);
				_mNextRewardIcon.SetTextOffset(-156f, 0f);
				_mNextRewardIcon.SetTextNoLocalization(nextRewardAmount.ToString());
				if (!(nextReward is MilMo_Gem))
				{
					if (nextReward is MilMo_Coin)
					{
						_mNextRewardIcon.SetTexture("Batch01/Textures/HUD/IconVoucherPointCounter");
					}
				}
				else
				{
					_mNextRewardIcon.SetTexture("Batch01/Textures/HUD/IconGemCounter");
				}
			}
			_mNextRewardIcon.SetTextAlignment(MilMo_GUI.Align.CenterRight);
			_mNextRewardIcon.SetExtraDrawTextSize(200f, 0f);
			_mNextRewardIcon.SetDefaultTextColor(TextColor1);
			_mNextRewardIcon.FadeToDefaultColor = false;
			_mNextRewardIcon.SetAlpha(0f);
			_mNextRewardIcon.SetFadeSpeed(0.01f);
			_mNextRewardIcon.SetEnabled(e: true);
			_mNextRewardIcon.AllowPointerFocus = false;
			milMo_Widget.AddChild(_mNextRewardIcon);
			_mNextRewardName = new MilMo_Widget(ui);
			_mNextRewardName.SetPosition(20f, 91f);
			_mNextRewardName.SetScale(236f, 50f);
			_mNextRewardName.SetAlignment(MilMo_GUI.Align.TopLeft);
			_mNextRewardName.SetTextAlignment(MilMo_GUI.Align.TopLeft);
			_mNextRewardName.SetFont(MilMo_GUI.Font.EborgSmall);
			_mNextRewardName.SetFontScale(0.8f);
			_mNextRewardName.SetDefaultTextColor(_mBrightTextColor);
			_mNextRewardName.FadeToDefaultColor = false;
			_mNextRewardName.SetAlpha(0f);
			_mNextRewardName.SetFadeSpeed(0.01f);
			if (_mHaveNextReward && flag && nextReward != null)
			{
				_mNextRewardName.SetText(nextReward.Template.DisplayName);
			}
			_mNextRewardName.SetEnabled(flag);
			_mNextRewardName.AllowPointerFocus = false;
			milMo_Widget.AddChild(_mNextRewardName);
			Height = milMo_Widget.PosMover.Target.y + milMo_Widget.ScaleMover.Target.y;
			Height += ButtonRight.ScaleMover.Target.y + 30f;
		}
		ui.Res = res;
	}

	protected override void ShowSecondaryWidgets()
	{
		base.ShowSecondaryWidgets();
		if (_mHaveNextReward)
		{
			_mNextRewardName.AlphaTo(1f);
			_mNextRewardIcon.AlphaTo(0.6f);
			_mNextRewardCriteria.AlphaTo(1f);
		}
	}

	protected override void ScheduleShowCustomContent()
	{
		base.ScheduleShowCustomContent();
		if (_mHaveNextReward)
		{
			_mNextRewardName.SetAlpha(0f);
			_mNextRewardIcon.SetAlpha(0f);
			_mNextRewardCriteria.SetAlpha(0f);
		}
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
			Icon.SetAngle(0f - 3000f * MilMo_Utility.Random() - 12.5f);
			Icon.Angle(0f);
			Icon.SetAlpha(0f);
			Icon.AlphaTo(1f);
			Icon.SetScalePull(0.1f, 0.1f);
			Icon.SetScaleDrag(0.4f, 0.4f);
			Icon.ScaleNow(0f, 0f);
			Icon.ScaleTo(IconScale);
			PlayDialogSound(JingleClip);
		});
	}
}
