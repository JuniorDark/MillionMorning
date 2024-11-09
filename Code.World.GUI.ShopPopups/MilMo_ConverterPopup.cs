using System.Collections.Generic;
using System.Linq;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.GUI.Converters;
using Code.World.Player;
using Core;
using Core.State;
using UnityEngine;

namespace Code.World.GUI.ShopPopups;

public sealed class MilMo_ConverterPopup : MilMo_ShopItemPopup
{
	private float _mIngredientsHeight;

	private const float M_PADDING = 10f;

	private readonly float _mToolScale = 24f;

	private float _mUpperScale;

	private MilMo_Widget _mResultIcon;

	private readonly bool _mIsInitialized;

	private MilMo_SimpleLabel _mResultCaption;

	private MilMo_SimpleLabel _mResultDescription;

	private readonly List<MilMo_Widget> _mIngredients;

	private MilMo_Widget _mTool;

	private MilMo_SimpleLabel _mToolText;

	private readonly MilMo_Converter _mConverter;

	public MilMo_ConverterPopup(MilMo_UserInterface ui, MilMo_Converter item)
		: base(ui, item.Template.DisplayName, MilMo_LocString.Empty, new Vector2(150f, 50f), null)
	{
		_mConverter = item;
		SetColor(0.496f, 0.495f, 0.89f, 1f);
		base.PopupArrow.IsInvisible = true;
		SetupBackdrop();
		SendToBack(MDarkBack);
		SendToBack(Background);
		SendToBack(BlackOutline);
		SendToBack(BackShade);
		SetupResultCaption();
		SetupResultDescription();
		SetupResultIcon();
		if (_mConverter.Template.RequiredTool != null)
		{
			AddToolPreview();
		}
		else
		{
			_mToolScale = 0f;
		}
		TemplateReference reference = null;
		MilMo_Player instance = MilMo_Player.Instance;
		if (instance != null && instance.Avatar != null)
		{
			reference = ((MilMo_Player.Instance.Avatar.Gender == 1) ? _mConverter.Template.GirlReward.GetTemplate() : _mConverter.Template.BoyReward.GetTemplate());
		}
		else
		{
			Debug.LogWarning("Player was null (MilMo_ConverterPopup).");
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(reference, delegate(MilMo_Template rewardTemplate, bool timeout)
		{
			MilMo_Item.AsyncGetItem(rewardTemplate.Identifier, delegate(MilMo_Item rewardItem)
			{
				rewardItem.AsyncGetIcon(delegate(Texture2D rewardTexture)
				{
					_mResultIcon.SetTexture(rewardTexture);
				});
				_mResultDescription.SetText(rewardItem.Template.Description);
				_mResultCaption.SetText(rewardItem.Template.DisplayName);
			});
		});
		base.Icon.SetTextureInvisible();
		_mIsInitialized = true;
		_mIngredients = new List<MilMo_Widget>();
		RefreshWidgets();
		InitWidgets();
	}

	private void SetupBackdrop()
	{
		MDarkBack.SetTextureWhite();
		MDarkBack.SetDefaultColor(0.6f, 0.6f, 1f, 1f);
		MDarkBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		MDarkBack.FadeToDefaultColor = false;
		MDarkBack.SetFadeSpeed(0.08f);
		MDarkBack.UseParentAlpha = false;
		MDarkBack.AllowPointerFocus = false;
		AddChild(MDarkBack);
	}

	private void SetupResultCaption()
	{
		_mResultCaption = new MilMo_SimpleLabel(UI);
		_mResultCaption.SetFont(MilMo_GUI.Font.EborgSmall);
		_mResultCaption.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mResultCaption.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mResultCaption.SetDefaultTextColor(Color.black);
		_mResultCaption.UseParentAlpha = false;
		_mResultCaption.AllowPointerFocus = false;
		_mResultCaption.SetFadeSpeed(0.08f);
		AddChild(_mResultCaption);
	}

	private void SetupResultDescription()
	{
		_mResultDescription = new MilMo_SimpleLabel(UI);
		_mResultDescription.SetWordWrap(w: true);
		_mResultDescription.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mResultDescription.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mResultDescription.SetDefaultTextColor(Color.black);
		_mResultDescription.SetFadeSpeed(0.08f);
		_mResultDescription.AllowPointerFocus = false;
		_mResultDescription.UseParentAlpha = false;
		AddChild(_mResultDescription);
	}

	private void SetupResultIcon()
	{
		_mResultIcon = new MilMo_Widget(UI);
		_mResultIcon.SetAlignment(MilMo_GUI.Align.TopRight);
		_mResultIcon.SetFadeSpeed(0.08f);
		_mResultIcon.AllowPointerFocus = false;
		_mResultIcon.FadeToDefaultColor = false;
		_mResultIcon.UseParentAlpha = false;
		AddChild(_mResultIcon);
	}

	private void AddToolPreview()
	{
		_mTool = new MilMo_Widget(UI);
		_mTool.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTool.AllowPointerFocus = false;
		_mTool.FadeToDefaultColor = false;
		_mTool.UseParentAlpha = false;
		_mTool.SetFadeSpeed(0.08f);
		AddChild(_mTool);
		_mToolText = new MilMo_SimpleLabel(UI);
		_mToolText.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mToolText.AllowPointerFocus = false;
		_mToolText.SetDefaultTextColor(Color.black);
		_mToolText.UseParentAlpha = false;
		_mToolText.SetFadeSpeed(0.08f);
		AddChild(_mToolText);
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(_mConverter.Template.RequiredTool, delegate(MilMo_Template toolTemplate, bool timeout)
		{
			MilMo_Item.AsyncGetItem(toolTemplate.Identifier, delegate(MilMo_Item toolItem)
			{
				toolItem.AsyncGetIcon(delegate(Texture2D toolTexture)
				{
					_mTool.SetTexture(toolTexture);
				});
				MilMo_LocString copy = MilMo_Localization.GetLocString("World_10261").GetCopy();
				copy.SetFormatArgs(toolItem.Template.DisplayName);
				_mToolText.SetText(copy);
			});
		});
	}

	private void RefreshIngredients()
	{
		foreach (MilMo_Widget mIngredient in _mIngredients)
		{
			RemoveChild(mIngredient);
		}
		Vector2 position = new Vector2(1f, _mUpperScale + 5f);
		if (_mConverter.Template.RequiredGems > 0)
		{
			int amountGot = GlobalStates.Instance.playerState.gems.Get();
			MilMo_ConverterPopupIngredient milMo_ConverterPopupIngredient = new MilMo_ConverterPopupIngredient(UI, "Content/GUI/Batch01/Textures/GameDialog/IconGem", MilMo_Localization.GetLocString("World_5541"), _mConverter.Template.RequiredGems, amountGot, base.TextWidget.Scale.x + 20f + base.IconScale);
			_mIngredients.Add(milMo_ConverterPopupIngredient);
			milMo_ConverterPopupIngredient.SetPosition(position);
			position.y += 28f;
			AddChild(milMo_ConverterPopupIngredient);
		}
		foreach (TemplateCountPair ingredient in _mConverter.Template.Ingredients)
		{
			MilMo_ConverterPopupIngredient milMo_ConverterPopupIngredient2 = new MilMo_ConverterPopupIngredient(UI, ingredient.GetTemplate(), ingredient.GetCount(), base.TextWidget.Scale.x + 20f + base.IconScale);
			milMo_ConverterPopupIngredient2.SetPosition(position);
			position.y += 28f;
			_mIngredients.Add(milMo_ConverterPopupIngredient2);
			AddChild(milMo_ConverterPopupIngredient2);
		}
		_mIngredientsHeight = position.y + 10f - (_mUpperScale + 5f);
	}

	public override void RefreshIconAlignment()
	{
	}

	protected override void InitWidgets()
	{
		if (_mIsInitialized)
		{
			if (_mIngredients != null)
			{
				RefreshIngredients();
			}
			_mUpperScale = 25f + _mToolScale + 10f;
			SetYScale(_mUpperScale + _mIngredientsHeight + 10f + 90f + ((MPriceWidget != null) ? MPriceWidget.Scale.y : 0f));
			Vector2 scale = Scale;
			scale.x /= base.Res.x;
			scale.y /= base.Res.y;
			MDarkBack.SetScale(scale.x - ShadePadding - 2f, _mIngredientsHeight);
			MDarkBack.SetPosition(1f, _mUpperScale);
			_mResultIcon.SetScale(base.IconScale, base.IconScale);
			_mResultIcon.SetPosition(Scale.x - 10f, _mUpperScale + _mIngredientsHeight + 10f - 25f);
			_mResultCaption.SetPosition(10f, _mUpperScale + _mIngredientsHeight + 10f - 5f);
			_mResultCaption.SetScale(scale.x, 27f);
			_mResultDescription.SetPosition(10f, _mUpperScale + _mIngredientsHeight + 10f + 14f);
			_mResultDescription.SetScale(scale.x - base.IconScale, 80f);
			if (_mToolText != null && _mTool != null)
			{
				_mToolText.SetScale(scale.x - ShadePadding - 2f, 20f);
				_mToolText.SetPosition(38f, 28f);
				_mTool.SetPosition(10f, 28f);
				_mTool.SetScale(24f, 24f);
			}
		}
	}

	protected override void RefreshWidgets()
	{
		InitWidgets();
		base.RefreshWidgets();
	}

	public override void FadeOut()
	{
		_mResultCaption.SetDefaultTextColor(0f, 0f, 0f, 0f);
		_mResultDescription.SetDefaultTextColor(0f, 0f, 0f, 0f);
		_mResultIcon.AlphaTo(0f);
		foreach (MilMo_ConverterPopupIngredient item in _mIngredients.Cast<MilMo_ConverterPopupIngredient>())
		{
			item.AlphaTo(0f);
		}
		if (_mToolText != null && _mTool != null)
		{
			_mTool.AlphaTo(0f);
			_mToolText.SetDefaultTextColor(0f, 0f, 0f, 0f);
		}
		base.FadeOut();
	}

	protected override void FadeIn()
	{
		base.FadeIn();
		_mResultCaption.SetDefaultTextColor(0f, 0f, 0f, 1f);
		_mResultDescription.SetDefaultTextColor(0f, 0f, 0f, 1f);
		_mResultIcon.AlphaTo(1f);
		foreach (MilMo_ConverterPopupIngredient item in _mIngredients.Cast<MilMo_ConverterPopupIngredient>())
		{
			item.AlphaTo(1f);
		}
		if (_mToolText != null && _mTool != null)
		{
			_mTool.AlphaTo(1f);
			_mToolText.SetDefaultTextColor(0f, 0f, 0f, 1f);
		}
	}
}
