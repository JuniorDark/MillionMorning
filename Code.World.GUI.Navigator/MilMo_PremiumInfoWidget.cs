using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.Player;

namespace Code.World.GUI.Navigator;

public sealed class MilMo_PremiumInfoWidget : MilMo_Widget
{
	private readonly MilMo_Widget _mIcon;

	private readonly MilMo_Widget _mTxt;

	private readonly MilMo_Widget _mCaption;

	private readonly MilMo_Button _mButton;

	public MilMo_PremiumInfoWidget(MilMo_UserInterface ui)
		: base(ui)
	{
		SetScale(200f, 48f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		_mIcon = new MilMo_Widget(UI);
		_mIcon.SetTexture("Batch01/Textures/HUD/IconPremium1Month");
		_mIcon.SetScale(45f, 45f);
		_mIcon.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(_mIcon);
		_mTxt = new MilMo_Widget(UI);
		_mTxt.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTxt.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mTxt.SetScale(200f, 25f);
		_mTxt.SetTextColor(1f, 1f, 1f, 1f);
		_mTxt.SetFont(MilMo_GUI.Font.EborgSmall);
		AddChild(_mTxt);
		_mCaption = new MilMo_Widget(UI);
		_mCaption.SetScale(200f, 45f);
		_mCaption.SetText(MilMo_Localization.GetLocString("WorldMap_4747"));
		_mCaption.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mCaption.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mCaption.SetFont(MilMo_GUI.Font.EborgLarge);
		_mCaption.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(_mCaption);
		_mButton = new MilMo_Button(UI);
		_mButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mButton.SetAllTextures(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/BecomeAMemberButton"));
		_mButton.SetFadeInSpeed(0.03f);
		_mButton.SetHoverTexture(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/BecomeAMemberButtonMO"));
		_mButton.Function = delegate
		{
			MilMo_GlobalUI.Instance.GetWindow("AboutMembership").Open();
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
		};
		MilMo_Localization.RegisterLanguageChangedCallback(delegate
		{
			if (_mButton != null)
			{
				_mButton.SetAllTextures(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/BecomeAMemberButton"));
				_mButton.SetHoverTexture(MilMo_Localization.GetLocTexturePath("Batch01/Textures/Shop/BecomeAMemberButtonMO"));
			}
		});
		_mButton.AllowPointerFocus = true;
		_mButton.SetScale(201f, 48f);
		AddChild(_mButton);
		Refresh();
	}

	public void Refresh()
	{
		bool isMember = MilMo_Player.Instance.IsMember;
		_mButton.Enabled = false;
		_mIcon.Enabled = false;
		_mTxt.Enabled = false;
		_mCaption.Enabled = false;
		if (isMember)
		{
			_mIcon.Enabled = true;
			_mTxt.Enabled = true;
			_mCaption.Enabled = true;
			_mCaption.SetPosition(65f, -8f);
			_mTxt.SetPosition(65f, 18f);
			_mIcon.SetPosition(15f, 0f);
			if (MilMo_Player.Instance.MembershipDaysLeft > 30000)
			{
				_mIcon.SetTexture("Batch01/Textures/HUD/IconPremiumRed64");
			}
			else if (MilMo_Player.Instance.MembershipDaysLeft > 93)
			{
				_mIcon.SetTexture("Batch01/Textures/HUD/IconPremiumYellow64");
			}
			else if (MilMo_Player.Instance.MembershipDaysLeft > 31)
			{
				_mIcon.SetTexture("Batch01/Textures/HUD/IconPremiumGreen64");
			}
			else
			{
				_mIcon.SetTexture("Batch01/Textures/HUD/IconPremiumBlue64");
			}
			MilMo_LocString locString;
			if (MilMo_Player.Instance.MembershipDaysLeft > 30000)
			{
				locString = MilMo_Localization.GetLocString("WorldMap_5536");
			}
			else
			{
				locString = MilMo_Localization.GetLocString("WorldMap_5535");
				locString.SetFormatArgs(MilMo_Player.Instance.MembershipDaysLeft);
			}
			_mTxt.SetText(locString);
		}
		else
		{
			_mButton.Enabled = true;
			_mButton.SetPosition(0f, 3f);
		}
	}
}
