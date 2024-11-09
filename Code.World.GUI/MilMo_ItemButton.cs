using System;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.GUI.Widget.SimpleWindow.Window.Popup;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI.ShopPopups;
using Code.World.Player;
using Code.World.Player.Skills;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_ItemButton : MilMo_Widget
{
	private int _mAmount = 1;

	public string Category = "Unknown";

	public string Section = "Misc";

	public MilMo_LocString Name = MilMo_Localization.GetNotLocalizedLocString("Item");

	private MilMo_LocString _mDescription = MilMo_LocString.Empty;

	public CustomFunc MCustomRightClickFunction;

	public bool AllowPopup = true;

	private Color _mPopupIconColor = Color.white;

	private readonly MilMo_Button _mIcon;

	public MilMo_PicturePopup Popup;

	private MilMo_ProgressBar _progressBar;

	private MilMo_Widget _progressCounter;

	public Color PopupColor = new Color(0.6f, 0.6f, 1f, 1f);

	public static float Size = 60f;

	private Vector2 _mTextAreaSize = new Vector2(150f, 20f);

	private readonly MilMo_AudioClip _mTickSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Tick");

	private readonly MilMo_Item _mItem;

	private readonly long _mItemId;

	private readonly MilMo_Skill _mSkill;

	private bool _mIconLoaded;

	private Vector2 _mDefaultScale = new Vector2(Size, Size);

	public MilMo_TimerEvent ShowPopupSchedule { get; private set; }

	public MilMo_Button Icon => _mIcon;

	public string IconTexture
	{
		set
		{
			LoadAndSetIconAsync("Content/GUI/" + value);
		}
	}

	public int Amount
	{
		get
		{
			return _mAmount;
		}
		set
		{
			_mAmount = value;
			_mIcon.SetText((Amount > 1) ? MilMo_LocString.Integer(_mAmount) : MilMo_LocString.Empty);
		}
	}

	public MilMo_LocString Description
	{
		get
		{
			return _mDescription;
		}
		set
		{
			_mDescription = value;
			_mTextAreaSize = new Vector2(_mTextAreaSize.x, GetTextHeight(Description.String, _mTextAreaSize.x));
		}
	}

	public long ItemId => _mItemId;

	public MilMo_Item Item => _mItem;

	public MilMo_Skill Skill => _mSkill;

	public bool IconLoaded => _mIconLoaded;

	public Vector2 DefaultScale
	{
		set
		{
			_mDefaultScale = value;
		}
	}

	private async void LoadAndSetIconAsync(string path)
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(path);
		_mIcon.SetAllTextures(new MilMo_Texture(texture));
	}

	public MilMo_ItemButton(MilMo_UserInterface ui, MilMo_Skill skill)
		: this(ui, 0L, null)
	{
		_mSkill = skill;
	}

	public MilMo_ItemButton(MilMo_UserInterface ui, long itemId, MilMo_Item item)
		: base(ui)
	{
		Identifier = "ItemButton" + MilMo_UserInterface.GetRandomID();
		UI = ui;
		_mItemId = itemId;
		_mItem = item;
		AllowPointerFocus = false;
		SetDefaultColor(1f, 1f, 1f, 1f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetPosPull(0.05f, 0.05f);
		SetPosDrag(0.6f, 0.6f);
		SetScalePull(0.05f, 0.05f);
		SetScaleDrag(0.6f, 0.6f);
		GoToNow(0f, 0f);
		ScaleNow(128f, 128f);
		SetFadeSpeed(0.2f);
		_mIcon = new MilMo_Button(UI);
		_mIcon.SetDefaultColor(1f, 1f, 1f, 1f);
		_mIcon.SetColor(1f, 1f, 1f, 0f);
		_mIcon.SetHoverColor(1f, 1f, 0.3f, 1f);
		_mIcon.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_mIcon.SetFadeSpeed(0.2f);
		_mIcon.SetFadeInSpeed(0.2f);
		_mIcon.SetFadeOutSpeed(0.2f);
		_mIcon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_mIcon.SetPosPull(0.05f, 0.05f);
		_mIcon.SetPosDrag(0.6f, 0.6f);
		_mIcon.SetScalePull(0.05f, 0.05f);
		_mIcon.SetScaleDrag(0.6f, 0.6f);
		_mIcon.Function = ClkSelect;
		_mIcon.RightClickFunction = RightClick;
		_mIcon.DoubleClickFunction = Use;
		_mIcon.GoToNow(Size / 2f, Size / 2f);
		_mIcon.ScaleNow(128f, 128f);
		_mIcon.PointerHoverFunction = ShowPopupSoon;
		_mIcon.PointerLeaveFunction = HidePopup;
		_mIcon.SetHoverSound(_mTickSound);
		_mIcon.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_mIcon.SetTextColor(1f, 1f, 1f, 0f);
		_mIcon.SetTextDropShadowPos(2f, 2f);
		_mIcon.SetFontScale(1f);
		_mIcon.SetTextOffset(-7f, -2f);
		_mIcon.SetTextAlignment(MilMo_GUI.Align.BottomRight);
		_mIcon.SetFont(MilMo_GUI.Font.EborgSmall);
		AddChild(_mIcon);
		UI.RemoveAllPopups();
		MCustomRightClickFunction = MilMo_Widget.Nothing;
		if (_mItem is MilMo_Wieldable)
		{
			_mIcon.ShowFavoriteStar = true;
			LoadAndSetStarIconAsync();
		}
	}

	private async void LoadAndSetStarIconAsync()
	{
		Texture2D starFilledTexture = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Voting/starFilled");
		_mIcon.StarFilledTexture = starFilledTexture;
		Texture2D starOutlineTexture = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Voting/starOutline");
		_mIcon.StarOutlineTexture = starOutlineTexture;
	}

	public override void Draw()
	{
		base.Draw();
		UnityEngine.GUI.skin = UI.Font0;
		DoPopupPosition();
		if (Popup != null && !Popup.DestroyFlag)
		{
			Popup.BringToFront();
		}
	}

	public void SetIcon(Texture2D icon)
	{
		if (!_mIconLoaded)
		{
			_mIconLoaded = true;
			_mIcon.SetAllTextures(icon);
		}
	}

	public void SetIconGrayedOut()
	{
		_mIcon.SetDefaultColor(1f, 1f, 1f, 0.45f);
	}

	public void SetIconIsFavorite(bool status)
	{
		_mIcon.IsFavorite = status;
	}

	public void SetIconNotGrayedOut()
	{
		_mIcon.SetDefaultColor(Color.white);
	}

	public override void SetScale(float x, float y)
	{
		if (_mIcon != null)
		{
			_mIcon.ScaleNow(x, y);
		}
		ScaleNow(x, y);
	}

	public void SetPopupIconColor(Color color)
	{
		_mPopupIconColor = color;
		if (Popup != null && Popup.Icon != null)
		{
			Popup.Icon.SetDefaultColor(color);
		}
	}

	private void ClkSelect(object o)
	{
		if (!MilMo_Player.Instance.Avatar.InCombat && (Item is MilMo_Consumable || Item is MilMo_Ability || _mSkill != null))
		{
			new MilMo_ItemButtonDragAndDrop(this);
		}
	}

	private void RightClick(object o)
	{
		if (MCustomRightClickFunction != null)
		{
			MCustomRightClickFunction(null);
		}
	}

	private void Use(object o)
	{
		MilMo_Window windowAncestor = GetWindowAncestor();
		if (windowAncestor == null || windowAncestor.IsActive)
		{
			UI.RemoveAllPopups();
			if (ShowPopupSchedule != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(ShowPopupSchedule);
			}
			if (CustomFunction != null)
			{
				CustomFunction(CustomArg);
			}
		}
	}

	private void DoPopupPosition()
	{
		if (Popup == null)
		{
			return;
		}
		MilMo_Window windowAncestor = GetWindowAncestor();
		if (windowAncestor != null)
		{
			Popup.IconAlignment = ((windowAncestor.Pos.x + windowAncestor.Scale.x <= UI.Center.x) ? 0f : 1f);
		}
		Popup.RefreshIconAlignment();
		MilMo_ScrollView scrollViewAncestor = GetScrollViewAncestor();
		Vector2 pos = Pos;
		Vector2 vector = scrollViewAncestor.Pos + Parent.Pos + scrollViewAncestor.Parent.Pos + scrollViewAncestor.Parent.Parent.Pos;
		Vector2 vector2 = new Vector2(Popup.ScaleMover.Target.x / 2f, 0f);
		Vector2 vector3 = _mIcon.ScaleMover.Target / 2f;
		float num = 0f - (_mIcon.PosMover.Target.y + 10f + Popup.CaptionOffset);
		Vector2 vector4 = new Vector2(-10f, 0f);
		Vector2 vector5 = new Vector2((0f - Size) / 2f, 0f);
		Vector2 vector6 = Vector2.zero;
		if (Popup.IconAlignment == 1f)
		{
			vector6 = new Vector2(0f - Popup.TextWidget.ScaleMover.Target.x, 0f);
		}
		Vector2 vector7 = new Vector2(0f, 0f - scrollViewAncestor.SoftScroll.Val.y);
		Vector2 vector8 = new Vector2(-9f, -15f);
		pos += vector;
		pos += vector2;
		pos += vector3;
		pos += vector4;
		pos += vector5;
		pos += vector6;
		pos += vector7;
		pos += vector8;
		pos.y += num;
		if (windowAncestor != null)
		{
			if (Popup.IconAlignment == 1f)
			{
				pos.x = windowAncestor.Pos.x - Popup.Scale.x / 2f;
			}
			else
			{
				pos.x = windowAncestor.Pos.x + windowAncestor.Scale.x + Popup.Scale.x / 2f;
			}
		}
		pos.x = Mathf.Max(pos.x, 0f);
		pos.x = Mathf.Min(pos.x, (float)Screen.width - Popup.ScaleMover.Target.x);
		pos.y = Mathf.Max(pos.y, 0f);
		pos.y = Mathf.Min(pos.y, (float)Screen.height - Popup.ScaleMover.Target.y);
		Popup.SetPosition(pos);
	}

	private void ShowPopup()
	{
		if (AllowPopup)
		{
			UI.BypassResolution();
			if (_mItem is MilMo_Weapon milMo_Weapon)
			{
				_mTextAreaSize.x = 242f;
				float normalDamage = milMo_Weapon.Template.NormalDamage;
				float magicDamage = milMo_Weapon.Template.MagicDamage;
				float cooldown = milMo_Weapon.Template.Cooldown;
				float range = milMo_Weapon.Template.Range;
				Popup = new MilMo_WeaponPopup(UI, Name, _mDescription, _mTextAreaSize, _mIcon.Texture, normalDamage, magicDamage, cooldown, range);
			}
			else if (_mItem is MilMo_Converter)
			{
				Popup = new MilMo_ConverterPopup(UI, _mItem as MilMo_Converter);
			}
			else
			{
				Popup = new MilMo_PicturePopup(UI, Name, _mDescription, _mTextAreaSize);
			}
			Popup.RequireWidgetEnabled = Parent;
			UI.AddChild(Popup);
			Popup.SetAlignment(MilMo_GUI.Align.TopLeft);
			if (!(_mItem is MilMo_Converter))
			{
				Popup.SetTextColor(0f, 0f, 0f, 0.85f);
				Popup.PopupArrow.IsInvisible = true;
				Popup.SetColor(PopupColor);
				Popup.Icon.SetTexture(_mIcon.Texture);
				Popup.Icon.SetDefaultColor(_mPopupIconColor);
			}
			if (Parent != null)
			{
				GetScrollViewAncestor()?.BringToFront(this);
			}
			UI.RestoreResolution();
		}
	}

	private void ShowPopupSoon()
	{
		MilMo_Window windowAncestor = GetWindowAncestor();
		if (windowAncestor == null || windowAncestor.IsActive)
		{
			if (ShowPopupSchedule != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(ShowPopupSchedule);
			}
			ShowPopupSchedule = MilMo_EventSystem.At(0.5f, ShowPopup);
		}
	}

	private void HidePopup()
	{
		if (ShowPopupSchedule != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(ShowPopupSchedule);
		}
		_mIcon.ScaleTo(_mDefaultScale);
		if (Popup != null)
		{
			Popup.FadeOut();
		}
		TargetTextColor = new Color(1f, 1f, 1f, 1f);
	}

	private float GetTextHeight(string msg, float width)
	{
		return GetTextHeight(UI, msg, width);
	}

	public static float GetTextHeight(MilMo_UserInterface ui, string msg, float width)
	{
		float num = ui.Font0.label.CalcHeight(new GUIContent(msg), width);
		if ((double)Math.Abs(ui.Font0.label.lineHeight - 23f) <= 0.0)
		{
			num *= 0.32f;
		}
		if ((double)Math.Abs(ui.Font0.label.lineHeight - 26f) <= 0.0)
		{
			num *= 0.31f;
		}
		return num + 15f;
	}
}
