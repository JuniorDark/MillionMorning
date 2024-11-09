using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.ShopPopups;

public sealed class MilMo_WeaponPopup : MilMo_ShopItemPopup
{
	private MilMo_Widget _mAttack;

	private StatStarGenerator StatStarGenerator { get; }

	public MilMo_WeaponPopup(MilMo_UserInterface ui, MilMo_LocString caption, MilMo_LocString message, Vector2 textAreaSize, MilMo_Texture texture, float attack, float magic, float speed, float range, bool isInGameShop = false)
		: base(ui, caption, message, textAreaSize, texture)
	{
		Identifier = "WeaponPopup";
		StatStarGenerator = new StatStarGenerator(UI);
		if (isInGameShop)
		{
			SetTextColor(0f, 0f, 0f, 0.85f);
			base.PopupArrow.IsInvisible = true;
			SetAlignment(MilMo_GUI.Align.TopLeft);
			SetColor(0.6f, 0.6f, 1f, 1f);
		}
		WeaponStatBrackets weaponStatBrackets = new WeaponStatBrackets();
		base.Icon.SetTexture(texture);
		MDarkBack.SetTextureWhite();
		MDarkBack.SetDefaultColor(0.6f, 0.6f, 1f, 1f);
		MDarkBack.SetPosition(1f, 24f);
		MDarkBack.SetScale(256f, 84f);
		MDarkBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		MDarkBack.FadeToDefaultColor = false;
		MDarkBack.SetFadeSpeed(0.08f);
		MDarkBack.UseParentAlpha = false;
		MDarkBack.AllowPointerFocus = false;
		AddChild(MDarkBack);
		SendToBack(MDarkBack);
		SendToBack(Background);
		SendToBack(BlackOutline);
		SendToBack(BackShade);
		SetupAttackWidget(attack, weaponStatBrackets);
		SetupMagicWidget(magic, weaponStatBrackets);
		SetupSpeedWidget(speed, weaponStatBrackets);
		SetupRangeWidget(range, weaponStatBrackets);
		BottomPad = 58f;
		TextAreaSize.x = 242f;
	}

	private void SetupAttackWidget(float attack, WeaponStatBrackets weaponStatBrackets)
	{
		_mAttack = new MilMo_Widget(UI);
		StatStarGenerator.SetStars(_mAttack, weaponStatBrackets.GetAttackPoints(attack));
		_mAttack.SetPosition(70f, 29f);
		_mAttack.SetTextOffset(-53f, 1f);
		_mAttack.SetExtraDrawTextSize(0f, 5f);
		_mAttack.SetScale(96f, 16f);
		_mAttack.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mAttack.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mAttack.FadeToDefaultColor = false;
		_mAttack.SetFadeSpeed(0.08f);
		_mAttack.UseParentAlpha = false;
		_mAttack.AllowPointerFocus = false;
		_mAttack.SetText(MilMo_Localization.GetLocString("Bag_9343"));
		_mAttack.SetFont(MilMo_GUI.Font.ArialRounded);
		_mAttack.SetDefaultTextColor(0f, 0f, 0f, 1f);
		AddChild(_mAttack);
	}

	private void SetupMagicWidget(float magic, WeaponStatBrackets weaponStatBrackets)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		StatStarGenerator.SetStars(milMo_Widget, weaponStatBrackets.GetMagicPoints(magic));
		milMo_Widget.SetPosition(70f, 47f);
		milMo_Widget.SetTextOffset(-53f, 1f);
		milMo_Widget.SetExtraDrawTextSize(0f, 5f);
		milMo_Widget.SetScale(96f, 16f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetFadeSpeed(0.08f);
		milMo_Widget.UseParentAlpha = false;
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.SetText(MilMo_Localization.GetLocString("Bag_9344"));
		milMo_Widget.SetFont(MilMo_GUI.Font.ArialRounded);
		milMo_Widget.SetDefaultTextColor(0f, 0f, 0f, 1f);
		AddChild(milMo_Widget);
	}

	private void SetupSpeedWidget(float speed, WeaponStatBrackets weaponStatBrackets)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		StatStarGenerator.SetStars(milMo_Widget, weaponStatBrackets.GetSpeedPoints(speed));
		milMo_Widget.SetPosition(70f, 65f);
		milMo_Widget.SetTextOffset(-53f, 1f);
		milMo_Widget.SetExtraDrawTextSize(0f, 5f);
		milMo_Widget.SetScale(96f, 16f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetFadeSpeed(0.08f);
		milMo_Widget.UseParentAlpha = false;
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.SetText(MilMo_Localization.GetLocString("Bag_9345"));
		milMo_Widget.SetFont(MilMo_GUI.Font.ArialRounded);
		milMo_Widget.SetDefaultTextColor(0f, 0f, 0f, 1f);
		AddChild(milMo_Widget);
	}

	private void SetupRangeWidget(float range, WeaponStatBrackets weaponStatBrackets)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		StatStarGenerator.SetStars(milMo_Widget, weaponStatBrackets.GetRangePoints(range));
		milMo_Widget.SetPosition(70f, 83f);
		milMo_Widget.SetTextOffset(-53f, 1f);
		milMo_Widget.SetExtraDrawTextSize(0f, 5f);
		milMo_Widget.SetScale(96f, 16f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetFadeSpeed(0.08f);
		milMo_Widget.UseParentAlpha = false;
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.SetText(MilMo_Localization.GetLocString("Bag_9346"));
		milMo_Widget.SetFont(MilMo_GUI.Font.ArialRounded);
		milMo_Widget.SetDefaultTextColor(0f, 0f, 0f, 1f);
		AddChild(milMo_Widget);
	}

	protected override void RefreshWidgets()
	{
		base.RefreshWidgets();
		if (base.Icon != null)
		{
			base.Icon.SetScale(48f, 48f);
			base.Icon.ScaleTo(80f, 80f);
		}
	}

	public override void RefreshIconAlignment()
	{
		base.RefreshIconAlignment();
		base.TextWidget?.SetPosition(10f, 113f);
		base.Icon?.SetPosition(216f, 66f);
	}

	protected override void FadeIn()
	{
		MDarkBack.SetAlpha(0f);
		MDarkBack.AlphaTo(MDarkBack.DefaultColor.a);
		_mAttack.SetAlpha(0f);
		_mAttack.AlphaTo(_mAttack.DefaultColor.a);
		base.FadeIn();
	}

	public override void FadeOut()
	{
		MDarkBack.AlphaTo(0f);
		_mAttack.AlphaTo(0f);
		base.FadeOut();
	}

	protected override void InitWidgets()
	{
		RefreshIconAlignment();
		MTextWidget.SetPosition(10f + base.IconScale, 10f + base.CaptionOffset);
		SetScale(base.TextWidget.Scale.x + 20f, base.TextWidget.Scale.y + 20f + base.CaptionOffset + BottomPad + ((MPriceWidget != null) ? MPriceWidget.Scale.y : 0f));
		float num = base.IconScale + base.CaptionOffset + MIconPadding * 2f + BottomPad + 2f + ((MPriceWidget != null) ? MPriceWidget.Scale.y : 0f);
		if (ScaleMover.Target.y < num)
		{
			SetYScale(num);
		}
	}
}
