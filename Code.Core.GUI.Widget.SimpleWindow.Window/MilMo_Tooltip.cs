using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using Code.World.GUI;
using UnityEngine;

namespace Code.Core.GUI.Widget.SimpleWindow.Window;

public sealed class MilMo_Tooltip : MilMo_Window
{
	private enum FrameEnum
	{
		Left,
		Right,
		Middle,
		NrOfWidgets
	}

	private MilMo_LocString _mMessage;

	private readonly int _nrOfRows;

	private Vector2 _mScale;

	private MilMo_Button _mParentButton;

	private readonly MilMo_Widget[] _mFrame;

	private readonly MilMo_Widget _mTextWidget;

	private readonly MilMo_Widget _mTipWidget;

	private readonly MilMo_Widget _mMotionWidget;

	private float _mTimeSinceLastClose = -1f;

	public MilMo_Tooltip(MilMo_LocString text)
		: base(MilMo_GlobalUI.GetSystemUI)
	{
		_mMotionWidget = new MilMo_Widget(UI);
		_mMotionWidget.SetScale(1f, 1f);
		_mMotionWidget.SetPosPull(0.01f, 0.01f);
		_mMotionWidget.SetPosDrag(0.8f, 0.8f);
		_mMotionWidget.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_mMotionWidget.AllowPointerFocus = false;
		Identifier = "Tooltip" + MilMo_UserInterface.GetRandomID();
		SetAlpha(0f);
		GUIStyle gUIStyle = new GUIStyle();
		GUIContent content = new GUIContent
		{
			text = text.String
		};
		float x = gUIStyle.CalcSize(content).x;
		_nrOfRows = 1 + (text.String.Length - text.String.Replace("\n", "").Length);
		_mScale = new Vector2(x + 12f, _nrOfRows * 26);
		base.FixedRes = true;
		_mMessage = text;
		SpawnScale = _mScale;
		TargetScale = SpawnScale;
		Scale = SpawnScale;
		SetPosPull(0.01f, 0.01f);
		SetPosDrag(0.8f, 0.8f);
		UseParentAlpha = false;
		FadeToDefaultColor = false;
		IgnoreGlobalFade = true;
		AllowPointerFocus = false;
		_mTipWidget = new MilMo_Widget(UI);
		_mTipWidget.SetScale(_mScale.x - 6f, _mScale.y - 6f);
		_mTipWidget.SetAlpha(1f);
		_mTipWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTipWidget.SetPosition(0f, 0f);
		_mTipWidget.UseParentAlpha = false;
		AddChild(_mTipWidget);
		_mFrame = new MilMo_Widget[3];
		for (int i = 0; i < 3; i++)
		{
			_mFrame[i] = new MilMo_Widget(UI);
			_mFrame[i].SetAlignment(MilMo_GUI.Align.TopLeft);
			_mFrame[i].SetScale(6f, _nrOfRows * 20);
			_mFrame[i].Enabled = false;
			_mFrame[i].FadeToDefaultColor = false;
			_mFrame[i].SetFadeSpeed(0.05f);
			_mTipWidget.AddChild(_mFrame[i]);
		}
		_mFrame[0].SetTexture("Batch01/Textures/Core/LeftToolTipBox");
		_mFrame[2].SetTexture("Batch01/Textures/Core/MiddleToolTipBox");
		_mFrame[1].SetTexture("Batch01/Textures/Core/RightToolTipBox");
		_mFrame[0].SetPosition(0f, 0f);
		_mFrame[2].SetPosition(6f, 0f);
		_mFrame[2].SetXScale(x - 8f);
		_mFrame[1].SetPosition(_mFrame[2].Pos.x + _mFrame[2].Scale.x, 0f);
		_mTextWidget = new MilMo_Widget(UI);
		_mTextWidget.SetFadeSpeed(0.05f);
		_mTextWidget.SetScale(x + 4f, _nrOfRows * 17);
		_mTextWidget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_mTextWidget.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_mTextWidget.SetFont(MilMo_GUI.Font.GothamSmall);
		_mTextWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTextWidget.FadeToDefaultColor = false;
		_mTextWidget.FadeToDefaultTextColor = false;
		_mTextWidget.SetPosition(4f, -1f);
		_mTipWidget.AddChild(_mTextWidget);
		foreach (MilMo_Widget child in base.Children)
		{
			child.AllowPointerFocus = false;
		}
		foreach (MilMo_Widget child2 in _mTipWidget.Children)
		{
			child2.AllowPointerFocus = false;
		}
		UI.AddChild(this);
		Close();
	}

	public override void SetText(MilMo_LocString text)
	{
		_mMessage = text;
	}

	internal void SetParentButton(MilMo_Button button)
	{
		_mParentButton = button;
	}

	public override void Open()
	{
		if (!(_mTimeSinceLastClose > 0f) || !(Time.time - _mTimeSinceLastClose < 0.1f))
		{
			_mTimeSinceLastClose = -1f;
			UI.AddChild(_mMotionWidget);
			_mMotionWidget.GoToNow(MilMo_Pointer.Position.x, MilMo_Pointer.Position.y - 20f);
			SetPosition(_mMotionWidget.Pos);
			base.Open();
			SetAlpha(0f);
			Enabled = true;
			_mTextWidget.SetText(_mMessage);
			GUIStyle gUIStyle = new GUIStyle();
			GUIContent content = new GUIContent
			{
				text = _mMessage.String
			};
			float x = gUIStyle.CalcSize(content).x;
			_mScale = new Vector2(x + 12f, _nrOfRows * 26);
			MAllowOffscreen = true;
			SpawnScale = _mScale;
			TargetScale = SpawnScale;
			SetScale(SpawnScale);
			_mTipWidget.SetScale(_mScale.x - 6f, _mScale.y - 6f);
			_mFrame[0].SetPosition(0f, 0f);
			_mFrame[2].SetPosition(6f, 0f);
			_mFrame[2].SetXScale(x);
			_mFrame[1].SetPosition(_mFrame[2].Pos.x + _mFrame[2].Scale.x, 0f);
			_mTextWidget.SetScale(x + 4f, _nrOfRows * 17);
			for (int i = 0; i < 3; i++)
			{
				_mFrame[i].SetAlpha(0f);
				_mFrame[i].Enabled = true;
				_mFrame[i].AlphaTo(1f);
			}
			_mTextWidget.SetTextColor(1f, 1f, 1f, 0f);
			_mTextWidget.Enabled = true;
			_mTextWidget.TextColorTo(1f, 1f, 1f, 1f);
		}
	}

	public override void Step()
	{
		if (Enabled && (_mParentButton == null || !_mParentButton.Enabled))
		{
			ClickClose();
		}
		if (Enabled)
		{
			base.Step();
			Vector2 pos = new Vector2(MilMo_Pointer.Position.x, MilMo_Pointer.Position.y - 20f);
			if (pos.x + _mScale.x + 12f > (float)Screen.width)
			{
				pos.x = (float)Screen.width - _mScale.x;
			}
			if (pos.x < 0f)
			{
				pos.x = 6f;
			}
			if (pos.y < 0f)
			{
				pos.y = 0f;
			}
			if (pos.y > (float)Screen.height)
			{
				pos.y = Screen.height;
			}
			_mMotionWidget.GoTo(pos);
			SetPosition(_mMotionWidget.Pos);
			if (!_mParentButton.Hover())
			{
				Close();
			}
			BringToFront();
		}
	}

	public void ClickClose()
	{
		if (_mParentButton != null && _mParentButton.TooltipTimerEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mParentButton.TooltipTimerEvent);
		}
		for (int i = 0; i < 3; i++)
		{
			_mFrame[i].Enabled = false;
		}
		_mTextWidget.Enabled = false;
		Enabled = false;
		UI.RemoveChild(_mMotionWidget);
		_mTimeSinceLastClose = Time.time;
	}

	public void Close()
	{
		for (int i = 0; i < 3; i++)
		{
			_mFrame[i].AlphaTo(0f);
		}
		_mTextWidget.TextColorTo(1f, 1f, 1f, 0f);
		MilMo_EventSystem.At(0.3f, delegate
		{
			if (_mParentButton != null && _mParentButton.TooltipTimerEvent != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(_mParentButton.TooltipTimerEvent);
			}
			for (int j = 0; j < 3; j++)
			{
				_mFrame[j].Enabled = false;
			}
			_mTextWidget.Enabled = false;
			Enabled = false;
			UI.RemoveChild(_mMotionWidget);
			_mTimeSinceLastClose = Time.time;
		});
	}
}
