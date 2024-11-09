using System;
using System.Threading.Tasks;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Input;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.Player;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public abstract class MilMo_GameDialog : MilMo_Widget
{
	protected enum ButtonMode
	{
		Okay,
		SkipShare,
		SkipInvite,
		LaterYes
	}

	public enum HudDestination
	{
		None,
		Map,
		MedalBoard,
		Bag,
		Storage,
		Home,
		HomeInNav
	}

	private static int _debugCounter;

	protected readonly int DebugId;

	protected bool IsActive;

	protected bool ShouldShow;

	private const float PADDING = 0.1f;

	private const float SHADE_PADDING = 10f;

	protected const float WIDTH = 265f;

	protected float Height = 330f;

	protected const float MARGIN = 15f;

	protected Color ItemFrameColor = new Color(0f, 1f, 0f, 0.8f);

	protected Color ButtonColor = new Color(1f, 1f, 1f, 1f);

	protected Color TextColor1 = new Color(0.35f, 0.35f, 0.35f, 1f);

	protected Color ItemTextColor = new Color(0f, 0f, 0f, 0.65f);

	protected Vector2 IconPos = new Vector2(60f, 0f);

	protected Vector2 IconScale = new Vector2(95f, 95f);

	private ButtonMode _buttonMode;

	private HudDestination _hudDestination;

	protected MilMo_Widget Icon;

	protected MilMo_Widget PreviousIcon;

	private readonly MilMo_Button _buttonLeft;

	protected readonly MilMo_Button ButtonRight;

	protected readonly MilMo_Widget Headline;

	protected readonly MilMo_Widget EventDescription;

	private MilMo_TimerEvent _showButtonsEvent;

	private MilMo_TimerEvent _showSecondaryWidgetsEvent;

	protected MilMo_TimerEvent ShowIconEvent;

	protected float ShowSecondaryWidgetsTime = 1f;

	protected readonly MilMo_AudioClip TickSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Tick");

	private readonly AudioSourceWrapper _dialogAudioSource;

	private readonly AudioClip _selectClip;

	private AudioClip _swooshClip;

	protected AudioClip JingleClip;

	private Rect _textRect;

	private Rect _frameRect;

	private Rect _shadowRect;

	private readonly MilMo_KeyListener _returnListener;

	public string CustomJinglePath { get; set; }

	protected MilMo_GameDialog(MilMo_UserInterface ui, ButtonMode buttonMode, MilMo_Button.ButtonFunc rightButtonFunction, MilMo_Button.ButtonFunc leftButtonFunction)
		: base(ui)
	{
		CustomJinglePath = "Content/Sounds/Batch01/GUI/GamePlayDialog/ReceivedItemJingle";
		DebugId = _debugCounter++;
		Identifier = "GameDialog";
		IsGameDialog = true;
		Debug.Log("Creating game dialog " + DebugId + " of type " + ToString());
		_selectClip = null;
		LoadSwoshSound();
		Vector2 res = UI.Res;
		UI.Res = new Vector2(1f, 1f);
		ui.ResetLayout();
		ScaleNow(0f, 0f);
		GoToNow((float)Screen.width / 2f, (float)Screen.height / 2f);
		SetAlignment(MilMo_GUI.Align.BottomLeft);
		SetSkin(1);
		SetScalePull(0.09f, 0.09f);
		SetScaleDrag(0.7f, 0.7f);
		FadeToDefaultColor = false;
		SetFadeSpeed(0.08f);
		ScaleMover.MinVel = new Vector2(0.01f, 0.01f);
		SetTextAlignment(MilMo_GUI.Align.TopLeft);
		base.FixedRes = true;
		Icon = new MilMo_Widget(UI);
		Icon.SetPosPull(0.05f, 0.05f);
		Icon.SetPosDrag(0.6f, 0.6f);
		Icon.SetAnglePull(0.09f);
		Icon.SetAngleDrag(0.4f);
		Icon.ScaleNow(IconScale);
		Icon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		Icon.FadeToDefaultColor = false;
		Icon.SetFadeSpeed(0.08f);
		Icon.SetMinScaleVel(0.01f, 0.01f);
		Icon.SetMinScale(0f, 0f);
		Icon.AllowPointerFocus = false;
		AddChild(Icon);
		Headline = new MilMo_Widget(UI);
		Headline.GoToNow(110f, 11f);
		Headline.ScaleNow(225f, 50f);
		Headline.SetAlignment(MilMo_GUI.Align.TopLeft);
		Headline.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		Headline.SetWordWrap(w: true);
		Headline.SetFont(MilMo_GUI.Font.EborgSmall);
		Headline.SetDefaultTextColor(TextColor1);
		Headline.FadeToDefaultColor = false;
		Headline.SetFadeSpeed(0.08f);
		Headline.AllowPointerFocus = false;
		AddChild(Headline);
		EventDescription = new MilMo_Widget(UI);
		EventDescription.GoToNow(112f, 32f);
		EventDescription.ScaleNow(138f, 35f);
		EventDescription.SetAlignment(MilMo_GUI.Align.TopLeft);
		EventDescription.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		EventDescription.SetWordWrap(w: true);
		EventDescription.SetFont(MilMo_GUI.Font.ArialRounded);
		EventDescription.SetText(MilMo_LocString.Empty);
		EventDescription.SetDefaultTextColor(TextColor1);
		EventDescription.FadeToDefaultColor = false;
		EventDescription.SetFadeSpeed(0.08f);
		EventDescription.AllowPointerFocus = false;
		AddChild(EventDescription);
		_buttonLeft = new MilMo_Button(UI);
		_buttonLeft.SetAlignment(MilMo_GUI.Align.BottomCenter);
		_buttonLeft.SetPosition(133f, 315f);
		_buttonLeft.SetScale(70f, 28f);
		_buttonLeft.SetTexture("Batch01/Textures/GameDialog/GameDialogButtonGreen");
		_buttonLeft.SetHoverTexture("Batch01/Textures/GameDialog/GameDialogButtonGreenMO");
		_buttonLeft.SetPressedTexture("Batch01/Textures/GameDialog/GameDialogButtonGreenPressed");
		_buttonLeft.FadeToDefaultColor = false;
		_buttonLeft.SetFadeOutSpeed(0.08f);
		_buttonLeft.SetDefaultTextColor(new Color(1f, 1f, 1f, 1f));
		_buttonLeft.SetDefaultColor(ButtonColor);
		_buttonLeft.SetFont(MilMo_GUI.Font.EborgSmall);
		_buttonLeft.SetTextOffset(0f, -3f);
		_buttonLeft.SetHoverSound(null);
		_buttonLeft.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(_buttonLeft);
		ButtonRight = new MilMo_Button(UI);
		ButtonRight.SetAlignment(MilMo_GUI.Align.BottomRight);
		ButtonRight.SetPosition(244f, 315f);
		ButtonRight.SetScale(70f, 28f);
		ButtonRight.SetTexture("Batch01/Textures/GameDialog/GameDialogButtonGreen");
		ButtonRight.SetHoverTexture("Batch01/Textures/GameDialog/GameDialogButtonGreenMO");
		ButtonRight.SetPressedTexture("Batch01/Textures/GameDialog/GameDialogButtonGreenPressed");
		ButtonRight.FadeToDefaultColor = false;
		ButtonRight.SetFadeOutSpeed(0.08f);
		ButtonRight.SetDefaultTextColor(new Color(1f, 1f, 1f, 1f));
		ButtonRight.SetDefaultColor(ButtonColor);
		ButtonRight.SetFont(MilMo_GUI.Font.EborgSmall);
		ButtonRight.SetTextOffset(0f, -3f);
		ButtonRight.SetHoverSound(null);
		ButtonRight.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(ButtonRight);
		SetupButtons(buttonMode, rightButtonFunction, leftButtonFunction);
		Step();
		DisableAll();
		_dialogAudioSource = MilMo_Global.AudioListener.AddComponent<AudioSourceWrapper>();
		UI.Res = res;
		_returnListener = new MilMo_KeyListener(KeyCode.Return, ConfirmCallback, ConfirmEnabled);
	}

	private async void LoadSwoshSound()
	{
		_swooshClip = await MilMo_ResourceManager.Instance.LoadAudioAsync("Content/Sounds/Batch01/GUI/Generic/Swap");
	}

	public override void Step()
	{
		base.FixedRes = true;
		base.Step();
	}

	public override void Draw()
	{
		if (Enabled)
		{
			Vector2 res = UI.Res;
			UI.Res = new Vector2(1f, 1f);
			base.FixedRes = true;
			SetPosition(15f, (float)Screen.height - UI.GlobalPosOffset.y * 2f - 50f);
			if (MilMo_Player.InMyHome && Screen.height < 670 && MilMo_Player.InHome && !MilMo_Player.Instance.InNavigator && !MilMo_Player.Instance.InShop && !MilMo_Player.Instance.InCharBuilderWorld)
			{
				SetPosition(65f, (float)Screen.height - UI.GlobalPosOffset.y * 2f - 50f);
			}
			Vector2 vector = new Vector2(0.1f, 0.1f);
			_textRect = GetScreenPosition();
			_frameRect = _textRect;
			_frameRect.x -= vector.x;
			_frameRect.y -= vector.y;
			_frameRect.width += vector.x * 2f;
			_frameRect.height += vector.y * 2f;
			_shadowRect = _frameRect;
			_shadowRect.x -= 10f;
			_shadowRect.y -= 10f;
			_shadowRect.width += 20f;
			_shadowRect.height += 20f;
			GUISkin skin = UnityEngine.GUI.skin;
			UnityEngine.GUI.skin = Skin;
			Color itemFrameColor = ItemFrameColor;
			itemFrameColor.a = CurrentColor.a * 0.3f;
			UnityEngine.GUI.color = itemFrameColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			UnityEngine.GUI.Box(_shadowRect, "");
			itemFrameColor = CurrentColor;
			UnityEngine.GUI.color = itemFrameColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			UnityEngine.GUI.Box(_frameRect, "");
			UnityEngine.GUI.skin = skin;
			if (UI.Identifier == "WorldMap")
			{
				CheckPointerFocus();
			}
			base.Children.ForEach(delegate(MilMo_Widget w)
			{
				w.Draw();
			});
			UI.Res = res;
		}
	}

	protected void ScheduleShowSecondaryWidgets()
	{
		Debug.Log("Schedule show secondary widgets at " + ShowSecondaryWidgetsTime + " for game dialog " + DebugId + " of type " + ToString());
		if (_showSecondaryWidgetsEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_showSecondaryWidgetsEvent);
		}
		_showSecondaryWidgetsEvent = MilMo_EventSystem.At(ShowSecondaryWidgetsTime, ShowSecondaryWidgets);
	}

	protected virtual void ShowSecondaryWidgets()
	{
		Debug.Log("Show secondary widgets for game dialog " + DebugId + " of type " + ToString());
	}

	protected void ScheduleShowButtons()
	{
		Debug.Log("Schedule show buttons at 2.0 for game dialog " + DebugId + " of type " + ToString());
		_buttonLeft.AllowPointerFocus = false;
		ButtonRight.AllowPointerFocus = false;
		if (_showButtonsEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_showButtonsEvent);
		}
		_showButtonsEvent = MilMo_EventSystem.At(2f, ShowButtons);
	}

	protected virtual void ShowInternal(Texture2D iconTexture, AudioClip audioClip)
	{
		Debug.Log("ShowInternal with icon " + iconTexture.name + " for game dialog " + DebugId + " of type " + ToString());
		if (iconTexture != null)
		{
			Icon.SetTexture(new MilMo_Texture(iconTexture));
		}
		JingleClip = audioClip;
		ScaleMover.Arrive = null;
		IsActive = true;
		SetEnabled(e: true);
		EventDescription.SetEnabled(e: true);
		EventDescription.AlphaTo(1f);
		Headline.SetEnabled(e: true);
		Headline.AlphaTo(1f);
		Icon.SetEnabled(e: true);
		_buttonLeft.SetYPos(Height - 15f);
		ButtonRight.SetYPos(Height - 15f);
		_buttonLeft.SetEnabled(e: false);
		ButtonRight.SetEnabled(e: false);
		UI.BringToFront(this);
		ShowIcon();
		ScheduleShowCustomContent();
		GrowIn();
		PlayDialogSound(_swooshClip);
	}

	protected virtual void GrowIn()
	{
		SetScale(265f, 0f);
		ScaleTo(265f, Height);
		ColorTo(1f, 1f, 1f, 1f);
	}

	protected virtual void ScheduleShowCustomContent()
	{
		ScheduleShowButtons();
		ScheduleShowSecondaryWidgets();
	}

	public void Show(string icon)
	{
		Debug.Log("Show (icon path " + icon + ") game dialog " + DebugId + " of type " + ToString());
		ShouldShow = true;
		LoadStuffAndShow(icon);
	}

	private async Task<AudioClip> GetJingleSound()
	{
		if (string.IsNullOrEmpty(CustomJinglePath))
		{
			return null;
		}
		return await MilMo_ResourceManager.Instance.LoadAudioAsync(CustomJinglePath);
	}

	private async void LoadStuffAndShow(MilMo_Item item)
	{
		AudioClip audioClip = await GetJingleSound();
		item.AsyncGetIcon(delegate(Texture2D tex)
		{
			if (ShouldShow)
			{
				ShowInternal(tex, audioClip);
			}
		});
	}

	private async void LoadStuffAndShow(string iconPath)
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(iconPath);
		AudioClip audioClip = await GetJingleSound();
		if (ShouldShow)
		{
			ShowInternal(texture, audioClip);
		}
	}

	public virtual void Show(MilMo_Item item)
	{
		Debug.Log("Show (item " + item.Template.DisplayName?.ToString() + ") game dialog " + DebugId + " of type " + ToString());
		ShouldShow = true;
		if (!(item is MilMo_Gem))
		{
			if (item is MilMo_Coin)
			{
				Show("Content/GUI/Batch01/Textures/HUD/IconVoucherPoint");
			}
			else
			{
				LoadStuffAndShow(item);
			}
		}
		else
		{
			Show("Content/GUI/Batch01/Textures/GameDialog/IconGem");
		}
	}

	protected virtual void ShowIcon()
	{
		BringToFront(Icon);
	}

	public virtual void Hide()
	{
		Debug.Log("Hide game dialog " + DebugId + " of type " + ToString());
		ShouldShow = false;
		MilMo_Input.RemoveKeyListener(_returnListener);
		IsActive = false;
		ScaleMover.Arrive = HideFast;
		_buttonLeft.SetEnabled(e: false);
		ButtonRight.SetEnabled(e: false);
		_buttonLeft.Function = null;
		ButtonRight.Function = null;
		HideIcon();
		ScaleTo(265f, Height * 1.1f);
		if (_selectClip != null)
		{
			PlayDialogSound(_selectClip);
		}
	}

	private void HideFast()
	{
		Debug.Log("HideFast game dialog " + DebugId + " of type " + ToString());
		ScaleTo(265f, 0f);
		AlphaTo(0f);
		ScaleMover.Arrive = DisableAll;
		if (_hudDestination == HudDestination.None)
		{
			PlayDialogSound(_swooshClip, ignoreGUIDisabled: true);
		}
	}

	protected virtual void HideIcon()
	{
		Debug.Log("Hide Icon for game dialog " + DebugId + " of type " + ToString());
		MilMo_HudHandler hudHandler = MilMo_World.HudHandler;
		switch (_hudDestination)
		{
		case HudDestination.Bag:
			hudHandler.SendWidgetToHud(Icon, HudDestination.Bag);
			break;
		case HudDestination.MedalBoard:
			hudHandler.SendWidgetToHud(Icon, HudDestination.MedalBoard);
			break;
		case HudDestination.Map:
			hudHandler.SendWidgetToHud(Icon, HudDestination.Map);
			break;
		case HudDestination.Home:
			hudHandler.SendWidgetToHud(Icon, HudDestination.Home);
			break;
		case HudDestination.Storage:
			hudHandler.SendWidgetToHud(Icon, HudDestination.Storage);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case HudDestination.None:
		case HudDestination.HomeInNav:
			break;
		}
	}

	protected virtual void DisableAll()
	{
		Debug.Log("Disable All in game dialog " + DebugId + " of type " + ToString());
		SetEnabled(e: false);
		_buttonLeft.SetEnabled(e: false);
		ButtonRight.SetEnabled(e: false);
		EventDescription.SetEnabled(e: false);
		Headline.SetEnabled(e: false);
		if (Icon.Parent == this)
		{
			Icon.SetEnabled(e: false);
		}
		if (PreviousIcon != null && PreviousIcon.Parent == this)
		{
			PreviousIcon.SetEnabled(e: false);
		}
		ScaleNow(0f, 0f);
		SetAlpha(0f);
		_buttonLeft.SetAlpha(0f);
		ButtonRight.SetAlpha(0f);
		EventDescription?.SetAlpha(0f);
		Headline.SetAlpha(0f);
		ScaleMover.ClearArriveFunc();
		_buttonLeft.SetHoverSound(null);
		ButtonRight.SetHoverSound(null);
		if (Parent != null)
		{
			Parent.RemoveChild(this);
		}
		else
		{
			UI.RemoveChild(this);
		}
		if (_dialogAudioSource != null)
		{
			UnityEngine.Object.Destroy(_dialogAudioSource);
		}
	}

	protected void SetHudDestination(HudDestination destination)
	{
		_hudDestination = destination;
	}

	protected void SetHeadlineText(MilMo_LocString headline)
	{
		Headline.SetText(headline);
	}

	protected void SetEventDescription(MilMo_LocString description)
	{
		EventDescription.SetText(description);
	}

	private void SetupButtons(ButtonMode mode, MilMo_Button.ButtonFunc rightButtonFunction, MilMo_Button.ButtonFunc leftButtonFunction)
	{
		_buttonMode = mode;
		_buttonLeft.Function = null;
		ButtonRight.Function = null;
		switch (_buttonMode)
		{
		case ButtonMode.Okay:
			_buttonLeft.SetText(MilMo_LocString.Empty);
			ButtonRight.SetText(MilMo_Localization.GetLocString("World_367"));
			ButtonRight.Function = rightButtonFunction;
			break;
		case ButtonMode.SkipShare:
			_buttonLeft.SetText(MilMo_Localization.GetLocString("World_368"));
			ButtonRight.SetText(MilMo_Localization.GetLocString("World_369"));
			_buttonLeft.Function = leftButtonFunction;
			ButtonRight.Function = rightButtonFunction;
			break;
		case ButtonMode.SkipInvite:
			_buttonLeft.SetText(MilMo_Localization.GetLocString("World_368"));
			ButtonRight.SetText(MilMo_Localization.GetLocString("World_370"));
			_buttonLeft.Function = leftButtonFunction;
			ButtonRight.Function = rightButtonFunction;
			break;
		case ButtonMode.LaterYes:
			_buttonLeft.SetText(MilMo_Localization.GetLocString("World_371"));
			ButtonRight.SetText(MilMo_Localization.GetLocString("Generic_Yes"));
			_buttonLeft.Function = leftButtonFunction;
			ButtonRight.Function = rightButtonFunction;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	protected virtual void ShowButtons()
	{
		switch (_buttonMode)
		{
		case ButtonMode.Okay:
			_buttonLeft.SetEnabled(e: false);
			ButtonRight.SetEnabled(e: true);
			break;
		case ButtonMode.SkipShare:
			_buttonLeft.SetEnabled(e: true);
			ButtonRight.SetEnabled(e: true);
			break;
		case ButtonMode.SkipInvite:
			_buttonLeft.SetEnabled(e: true);
			ButtonRight.SetEnabled(e: true);
			break;
		case ButtonMode.LaterYes:
			_buttonLeft.SetEnabled(e: true);
			ButtonRight.SetEnabled(e: true);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (_buttonLeft.Enabled)
		{
			_buttonLeft.SetDefaultColor(1f, 1f, 1f, 1f);
			_buttonLeft.SetColor(1f, 1f, 1f, 0f);
			_buttonLeft.SetTexture("Batch01/Textures/GameDialog/GameDialogButtonGrey");
			_buttonLeft.SetHoverTexture("Batch01/Textures/GameDialog/GameDialogButtonGreyMO");
			_buttonLeft.SetPressedTexture("Batch01/Textures/GameDialog/GameDialogButtonGreyPressed");
			_buttonLeft.SetHoverSound(TickSound);
			_buttonLeft.AlphaTo(1f);
			_buttonLeft.AllowPointerFocus = true;
		}
		if (ButtonRight.Enabled)
		{
			ButtonRight.SetDefaultColor(1f, 1f, 1f, 1f);
			ButtonRight.SetColor(1f, 1f, 1f, 0f);
			ButtonRight.SetTexture("Batch01/Textures/GameDialog/GameDialogButtonGreen");
			ButtonRight.SetHoverTexture("Batch01/Textures/GameDialog/GameDialogButtonGreenMO");
			ButtonRight.SetPressedTexture("Batch01/Textures/GameDialog/GameDialogButtonGreenPressed");
			ButtonRight.AlphaTo(1f);
			ButtonRight.SetHoverSound(TickSound);
			ButtonRight.AllowPointerFocus = true;
		}
		MilMo_Input.AddKeyListener(_returnListener);
	}

	protected static float GetTextHeight(MilMo_UserInterface ui, string msg, float width)
	{
		float num = ui.Font0.label.CalcHeight(new GUIContent(msg), width);
		float lineHeight = ui.Font0.label.lineHeight;
		if (lineHeight != 23f)
		{
			if (lineHeight == 26f)
			{
				num *= 0.31f;
			}
		}
		else
		{
			num *= 0.32f;
		}
		return num + 15f;
	}

	private void ConfirmCallback(object o)
	{
		if (ButtonRight.Function != null)
		{
			ButtonRight.Function(null);
		}
	}

	private bool ConfirmEnabled()
	{
		if (!MilMo_UserInterface.KeyboardFocus)
		{
			return UI.Enabled;
		}
		return false;
	}

	protected void PlayDialogSound(AudioClip clip, bool ignoreGUIDisabled = false)
	{
		if (!(clip == null) && !(_dialogAudioSource == null) && (ignoreGUIDisabled || UI.Enabled))
		{
			_dialogAudioSource.Clip = clip;
			_dialogAudioSource.Loop = false;
			_dialogAudioSource.Play();
		}
	}
}
