using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.QuickInfoDialogs;
using Code.Core.ResourceSystem;
using Code.World.GUI;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_QuickInfoDialog : MilMo_Widget
{
	public delegate void ExtraWidgetRefreshFunction();

	private class ExtraWidget
	{
		public MilMo_Widget MWidget;

		public ExtraWidgetRefreshFunction RefreshFunc;
	}

	private MilMo_LocString _message;

	private readonly Vector2 _spawnPosition = Vector2.zero;

	private readonly Vector2 _endPosition = Vector2.zero;

	private readonly Vector2 _iconScale = new Vector2(64f, 64f);

	private readonly Vector2 _iconPosition = new Vector2(10f, 10f);

	private readonly Vector2 _captionScale = new Vector2(150f, 30f);

	private readonly Vector2 _captionPosition = new Vector2(84f, 10f);

	private Vector2 _textPosition = Vector2.zero;

	private readonly Vector2 _textScale;

	private readonly Vector2 _extraScale = Vector2.zero;

	private readonly MilMo_Widget _captionWidget;

	private readonly MilMo_Widget _textArea;

	private MilMo_Button _button;

	private MilMo_Widget _buttonText;

	private readonly List<ExtraWidget> _extraWidgets;

	private MilMo_TimerEvent _closeTimer;

	public bool StayOnMouseOver { get; set; }

	public MilMo_LocString Caption
	{
		set
		{
			_captionWidget.SetText(value);
		}
	}

	public MilMo_LocString Message
	{
		set
		{
			_message = value;
		}
	}

	public MilMo_Widget Icon { get; private set; }

	public bool IsActive { get; private set; }

	public float TimeInScreen { get; set; }

	public MilMo_QuickInfoDialog(MilMo_LocString caption, MilMo_LocString message, Vector2 textAreaSize, string iconTexture)
		: base(MilMo_GlobalUI.GetSystemUI)
	{
		TimeInScreen = 2f;
		if (textAreaSize.y < 40f)
		{
			textAreaSize.y = 40f;
		}
		base.FixedRes = true;
		_message = message;
		UseParentAlpha = false;
		Identifier = "QuickInfoDialog" + MilMo_UserInterface.GetRandomID();
		AllowPointerFocus = true;
		SetAlignment(MilMo_GUI.Align.CenterLeft);
		_textArea = new MilMo_Widget(UI);
		_textArea.SetTextureInvisible();
		_textArea.SetFont(MilMo_GUI.Font.ArialRounded);
		_textArea.SetFontPreset(MilMo_GUI.FontPreset.Normal);
		_textArea.SetAlignment(MilMo_GUI.Align.TopLeft);
		_textArea.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_textArea.SetWordWrap(w: true);
		_textScale = textAreaSize;
		_textArea.SetDefaultColor(1f, 1f, 1f, 1f);
		_textArea.SetScale(_textScale);
		_textArea.FixedRes = true;
		AddChild(_textArea);
		Icon = new MilMo_Widget(UI);
		Icon.SetAlignment(MilMo_GUI.Align.TopLeft);
		if (iconTexture != "")
		{
			Icon.SetTexture(iconTexture);
		}
		Icon.SetScale(_iconScale);
		Icon.FixedRes = true;
		AddChild(Icon);
		_captionWidget = new MilMo_Widget(UI);
		_captionWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_captionWidget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_captionWidget.SetScale(_captionScale);
		_captionWidget.FixedRes = true;
		_captionWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		_captionWidget.SetFontScale(0.7f);
		_captionWidget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_captionWidget.SetPosition(_captionPosition);
		_captionWidget.SetText(caption);
		AddChild(_captionWidget);
		_extraWidgets = new List<ExtraWidget>();
		Refresh();
		_textArea.SetPosition(_textPosition);
		SetScalePull(0.08f, 0.06f);
		SetScaleDrag(0.6f, 0.8f);
		SetPosPull(0.1f, 0.1f);
		SetPosDrag(0.7f, 0.7f);
		_spawnPosition.x = Scale.x + 90f;
		_spawnPosition.y = Scale.y * 0.5f + 13f;
		_endPosition.x = 15f;
		_endPosition.y = Scale.y * 0.5f + 13f;
		IsActive = false;
	}

	private void Refresh()
	{
		RefreshIconAndCaption();
		RefreshScaleAndPosition();
	}

	private void RefreshIconAndCaption()
	{
		Icon.SetScale(_iconScale);
		Icon.SetPosition(_iconPosition);
		_captionWidget.SetScale(_captionScale);
		_captionWidget.SetPosition(_captionPosition);
		foreach (ExtraWidget extraWidget in _extraWidgets)
		{
			extraWidget.RefreshFunc();
		}
	}

	private void RefreshScaleAndPosition()
	{
		Vector2 zero = Vector2.zero;
		zero.x = 94f + _textScale.x;
		if (_textArea.Scale.y + 50f > 84f)
		{
			zero.y = 50f + _textScale.y;
		}
		else
		{
			zero.y = 84f;
		}
		_textPosition.x = 84f;
		_textPosition.y = 35f;
		zero.x += _extraScale.x;
		zero.y += _extraScale.y;
		_captionWidget.SetFontScale(0.7f);
		_textArea.SetScale(_textScale);
		_textArea.SetFontScale(1f);
		SetScale(zero);
	}

	public void AddWidget(MilMo_Widget widget, ExtraWidgetRefreshFunction refreshFuc)
	{
		ExtraWidget extraWidget = new ExtraWidget
		{
			MWidget = widget,
			RefreshFunc = refreshFuc
		};
		extraWidget.MWidget.FixedRes = true;
		_extraWidgets.Add(extraWidget);
		AddChild(extraWidget.MWidget);
		BringToFront(extraWidget.MWidget);
	}

	public override void Draw()
	{
		Refresh();
		GUISkin skin = UnityEngine.GUI.skin;
		UnityEngine.GUI.skin = Skin;
		UnityEngine.GUI.color = CurrentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		Rect screenPosition = GetScreenPosition();
		screenPosition.width = Scale.x;
		screenPosition.height = Scale.y;
		UnityEngine.GUI.Box(screenPosition, "");
		UnityEngine.GUI.skin = skin;
		base.Draw();
	}

	internal void QueuedOpen()
	{
		if (!IsActive)
		{
			IsActive = true;
			_textArea.SetText(_message);
			_closeTimer = MilMo_EventSystem.At(TimeInScreen, Close);
			SetPosition(_spawnPosition);
			GoTo(_endPosition);
			SetEnabled(e: true);
		}
	}

	public void Open()
	{
		MilMo_QuickInfoDialogHandler.GetInstance().AddQuickInfoDialogToQueue(this);
	}

	internal void ForceClose()
	{
		MilMo_EventSystem.RemoveTimerEvent(_closeTimer);
		SetPosition(_spawnPosition);
		IsActive = false;
		Enabled = false;
	}

	public void Close()
	{
		if (Parent == null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_closeTimer);
			IsActive = false;
			Enabled = false;
			return;
		}
		if (Parent.Hover() && StayOnMouseOver)
		{
			MilMo_EventSystem.RemoveTimerEvent(_closeTimer);
			_closeTimer = MilMo_EventSystem.At(0.5f, Close);
			return;
		}
		GoTo(_spawnPosition);
		_closeTimer = MilMo_EventSystem.At(2.3f, delegate
		{
			if (Parent != null)
			{
				Parent.Enabled = false;
			}
			IsActive = false;
			Enabled = false;
			MilMo_EventSystem.RemoveTimerEvent(_closeTimer);
		});
	}

	public static void CreateOkQuickInfoDialog(MilMo_LocString caption, MilMo_LocString text)
	{
		float num = MilMo_ItemButton.GetTextHeight(MilMo_GlobalUI.GetSystemUI, text.String, 150f);
		if (num < 40f)
		{
			num = 40f;
		}
		new MilMo_QuickInfoDialog(caption, text, new Vector2(150f, num), "Batch01/Textures/Dialog/Confirm").Open();
	}
}
