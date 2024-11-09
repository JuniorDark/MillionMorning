using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.World.GUI.Navigator.Menus;
using UnityEngine;

namespace Code.World.GUI.Navigator;

public sealed class MilMo_NavigatorMenuTabWindow : MilMo_Widget
{
	public enum ButtonAlignment
	{
		TopLeftUpper,
		BottomLeft,
		TopLeftLower
	}

	private enum FrameParts
	{
		Top,
		Left,
		Right,
		Bot,
		NrOfParts
	}

	private enum FrameBendParts
	{
		TopLeft,
		TopRight,
		BotLeft,
		BotRight,
		NrOfParts
	}

	public delegate void CloseWindowCallback();

	private readonly MilMo_Widget _mInsideWidget;

	private readonly MilMo_Widget[] _mFrame;

	public readonly MilMo_Button MButtonToExpandFrom;

	private readonly MilMo_Widget[] _mFrameBends;

	private const float M_ALL_FADE_SPEED = 0.05f;

	public ButtonAlignment MAlign;

	public Vector2 ForcedButtonPosition = Vector2.zero;

	private readonly List<CloseWindowCallback> _mCloseWindowCallback = new List<CloseWindowCallback>();

	private readonly string _mTexturePathExtenstion = "";

	private float _clickBlock;

	public MilMo_NavigatorMenuItem ChildMenuItem { get; set; }

	public MilMo_NavigatorMenuTabWindow(MilMo_Widget insideWidget, MilMo_Button buttonToExpandFrom)
		: this(insideWidget, buttonToExpandFrom, MilMo_GlobalUI.GetSystemUI, useDarkTextures: false)
	{
	}

	public MilMo_NavigatorMenuTabWindow(MilMo_Widget insideWidget, MilMo_Button buttonToExpandFrom, MilMo_UserInterface ui, bool useDarkTextures)
		: base(ui)
	{
		_mInsideWidget = null;
		base.FixedRes = true;
		SetTextureInvisible();
		SetScale(0f, 0f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		_mFrame = new MilMo_Widget[4];
		for (int i = 0; i < 4; i++)
		{
			_mFrame[i] = new MilMo_Widget(UI);
			_mFrame[i].SetAlignment(MilMo_GUI.Align.CenterCenter);
			_mFrame[i].FixedRes = true;
			UI.AddChild(_mFrame[i]);
			_mFrame[i].Enabled = false;
			_mFrame[i].SetFadeSpeed(0.05f);
		}
		_mFrameBends = new MilMo_Widget[4];
		for (int j = 0; j < 4; j++)
		{
			_mFrameBends[j] = new MilMo_Widget(UI);
			_mFrameBends[j].FixedRes = true;
			UI.AddChild(_mFrameBends[j]);
			_mFrameBends[j].Enabled = false;
			_mFrameBends[j].SetFadeSpeed(0.05f);
		}
		if (useDarkTextures)
		{
			_mTexturePathExtenstion = "Dark";
		}
		_mFrameBends[0].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerTopLeft" + _mTexturePathExtenstion);
		_mFrameBends[1].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerTopRight" + _mTexturePathExtenstion);
		_mFrameBends[2].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerBottomLeft" + _mTexturePathExtenstion);
		_mFrameBends[3].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerBottomRight" + _mTexturePathExtenstion);
		_mFrameBends[0].SetAlignment(MilMo_GUI.Align.BottomRight);
		_mFrameBends[1].SetAlignment(MilMo_GUI.Align.BottomLeft);
		_mFrameBends[2].SetAlignment(MilMo_GUI.Align.TopRight);
		_mFrameBends[3].SetAlignment(MilMo_GUI.Align.TopLeft);
		_mFrame[0].SetAlignment(MilMo_GUI.Align.CenterLeft);
		_mFrame[0].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_Top" + _mTexturePathExtenstion);
		_mFrame[2].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_Right" + _mTexturePathExtenstion);
		_mFrame[1].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_Left" + _mTexturePathExtenstion);
		_mFrame[3].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_Bottom" + _mTexturePathExtenstion);
		MButtonToExpandFrom = buttonToExpandFrom;
		_mInsideWidget = insideWidget;
		_mInsideWidget.FixedRes = true;
		_mInsideWidget.SetTextureBlack();
		_mInsideWidget.FadeToDefaultColor = false;
		_mInsideWidget.SetAlpha(0f);
		_mInsideWidget.Enabled = false;
		_mInsideWidget.UseParentAlpha = false;
		_mInsideWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mInsideWidget.SetFadeSpeed(0.025f);
		UI.AddChild(_mInsideWidget);
		Enabled = false;
	}

	public bool MouseOverAll()
	{
		for (int i = 0; i < 4; i++)
		{
			if (_mFrameBends[i].Hover())
			{
				return true;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (_mFrame[j].Hover())
			{
				return true;
			}
		}
		if (MButtonToExpandFrom != null && MButtonToExpandFrom.Hover())
		{
			return true;
		}
		if (_clickBlock < Time.time)
		{
			return true;
		}
		if (MouseOverChildRecursive(_mInsideWidget))
		{
			return true;
		}
		return false;
	}

	private bool MouseOverChildRecursive(MilMo_Widget widget)
	{
		if (widget.Hover())
		{
			return true;
		}
		for (int i = 0; i < widget.Children.Count; i++)
		{
			if (MouseOverChildRecursive(widget.Children[i]))
			{
				return true;
			}
		}
		return false;
	}

	public void Open()
	{
		_clickBlock = Time.time + 0.5f;
		UI.BringToFront(_mInsideWidget);
		if (ForcedButtonPosition != Vector2.zero)
		{
			SetPosition(ForcedButtonPosition);
			UI.AddChild(this);
		}
		else
		{
			if (MButtonToExpandFrom != null)
			{
				SetPosition(MButtonToExpandFrom.Pos);
			}
			else
			{
				SetPosition(-400f, 0f);
			}
			if (MButtonToExpandFrom != null && MButtonToExpandFrom.Parent != null)
			{
				MButtonToExpandFrom.Parent.AddChild(this);
				SetPosition(MButtonToExpandFrom.Pos + MButtonToExpandFrom.Parent.Pos);
			}
			else
			{
				UI.AddChild(this);
			}
		}
		UpdatePosAndScale();
		_mInsideWidget.Enabled = true;
		if (ChildMenuItem != null)
		{
			ChildMenuItem.Open();
		}
		Enabled = true;
		for (int i = 0; i < 4; i++)
		{
			_mFrame[i].SetAlpha(0f);
			_mFrame[i].Enabled = true;
			_mFrame[i].AlphaTo(1f);
		}
		for (int j = 0; j < 4; j++)
		{
			_mFrameBends[j].SetAlpha(0f);
			_mFrameBends[j].Enabled = true;
			_mFrameBends[j].AlphaTo(1f);
		}
	}

	public void RefreshUI()
	{
		UI.BringToFront(_mInsideWidget);
		if (ForcedButtonPosition != Vector2.zero)
		{
			SetPosition(ForcedButtonPosition);
		}
		else
		{
			SetPosition(MilMo_HudHandler.GetHudElementPosition("FriendList"));
			if (MButtonToExpandFrom != null)
			{
				SetPosition(MButtonToExpandFrom.Pos);
			}
			if (MButtonToExpandFrom != null && MButtonToExpandFrom.Parent != null)
			{
				MButtonToExpandFrom.Parent.AddChild(this);
				SetPosition(MButtonToExpandFrom.Pos + MButtonToExpandFrom.Parent.Pos);
			}
		}
		UpdatePosAndScale();
	}

	public void Close()
	{
		if (Parent != null)
		{
			Parent.RemoveChild(this);
			for (int i = 0; i < 4; i++)
			{
				_mFrame[i].Enabled = false;
			}
			for (int j = 0; j < 4; j++)
			{
				_mFrameBends[j].Enabled = false;
			}
		}
		else
		{
			UI.RemoveChild(this);
			for (int k = 0; k < 4; k++)
			{
				_mFrame[k].Enabled = false;
			}
			for (int l = 0; l < 4; l++)
			{
				_mFrameBends[l].Enabled = false;
			}
		}
		_mInsideWidget.Enabled = false;
		Enabled = false;
		for (int num = _mCloseWindowCallback.Count - 1; num >= 0; num--)
		{
			_mCloseWindowCallback[num]();
			RemoveCloseWindowCallback(num);
		}
	}

	public void AddCloseWindowCallback(CloseWindowCallback callback)
	{
		_mCloseWindowCallback.Add(callback);
	}

	private void RemoveCloseWindowCallback(int id)
	{
		_mCloseWindowCallback.RemoveAt(id);
	}

	public void BringAllToFront()
	{
		UI.BringToFront(this);
		UI.BringToFront(_mInsideWidget);
		MilMo_Widget[] mFrame = _mFrame;
		foreach (MilMo_Widget w in mFrame)
		{
			UI.BringToFront(w);
		}
		mFrame = _mFrameBends;
		foreach (MilMo_Widget w2 in mFrame)
		{
			UI.BringToFront(w2);
		}
		UI.BringToFront(_mInsideWidget);
	}

	private void UpdatePosAndScale()
	{
		for (int i = 0; i < 4; i++)
		{
			_mFrameBends[i].SetScale(14f, 14f);
		}
		switch (MAlign)
		{
		case ButtonAlignment.TopLeftUpper:
			_mInsideWidget.SetAlpha(0f);
			if (_mTexturePathExtenstion == "")
			{
				_mInsideWidget.AlphaTo(0.5f);
			}
			else if (_mTexturePathExtenstion.ToUpper() == "DARK")
			{
				_mInsideWidget.AlphaTo(0.74f);
			}
			_mInsideWidget.RefreshResolution();
			_mInsideWidget.SetPosition(Pos.x + 6f, Pos.y + 60f + 2f);
			_mFrameBends[1].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x, _mInsideWidget.Pos.y);
			_mFrameBends[0].SetPosition(Pos.x + 6f, _mInsideWidget.Pos.y);
			_mFrameBends[2].SetPosition(_mInsideWidget.Pos.x, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y);
			_mFrameBends[3].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y);
			_mFrame[0].SetPosition(Pos.x + 6f, Pos.y + 60f - 5f);
			_mFrame[3].SetPosition(Pos.x + 6f + _mInsideWidget.Scale.x / 2f, Pos.y + _mInsideWidget.Scale.y + 60f + 9f);
			_mFrame[2].SetPosition(Pos.x + _mInsideWidget.Scale.x + 13f, 60f + Pos.y + _mInsideWidget.Scale.y / 2f + 2f);
			_mFrame[1].SetPosition(Pos.x - 1f, 60f + Pos.y + _mInsideWidget.Scale.y / 2f + 2f);
			_mFrame[0].SetScale(_mInsideWidget.Scale.x, 14f);
			_mFrame[3].SetScale(_mInsideWidget.Scale.x, 14f);
			_mFrame[2].SetScale(14f, _mInsideWidget.Scale.y);
			_mFrame[1].SetScale(14f, _mInsideWidget.Scale.y);
			break;
		case ButtonAlignment.BottomLeft:
		{
			for (int l = 0; l < 4; l++)
			{
				_mFrameBends[l].SetAlignment(MilMo_GUI.Align.CenterCenter);
			}
			for (int m = 0; m < 4; m++)
			{
				_mFrame[m].SetAlignment(MilMo_GUI.Align.CenterCenter);
			}
			_mInsideWidget.SetAlignment(MilMo_GUI.Align.BottomLeft);
			_mInsideWidget.SetAlpha(0f);
			if (_mTexturePathExtenstion == "")
			{
				_mInsideWidget.AlphaTo(0.5f);
			}
			else if (_mTexturePathExtenstion.ToUpper() == "DARK")
			{
				_mInsideWidget.AlphaTo(0.74f);
			}
			_mInsideWidget.RefreshResolution();
			_mInsideWidget.SetPosition(Pos.x + 6f, Pos.y - 2f);
			_mFrame[0].SetScale(_mInsideWidget.Scale.x, 14f);
			_mFrame[1].SetScale(14f, _mInsideWidget.Scale.y);
			_mFrame[2].SetScale(14f, _mInsideWidget.Scale.y);
			_mFrame[3].SetScale(_mInsideWidget.Scale.x, 14f);
			_mFrame[0].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x * 0.5f, _mInsideWidget.Pos.y - 7f);
			_mFrame[1].SetPosition(_mInsideWidget.Pos.x - 7f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y * 0.5f);
			_mFrame[2].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x + 7f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y * 0.5f);
			_mFrame[3].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x * 0.5f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y + 7f);
			_mFrameBends[0].SetPosition(_mInsideWidget.Pos.x - 7f, _mInsideWidget.Pos.y - 7f);
			_mFrameBends[1].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x + 7f, _mInsideWidget.Pos.y - 7f);
			_mFrameBends[3].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x + 7f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y + 7f);
			_mFrameBends[2].SetPosition(_mInsideWidget.Pos.x - 7f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y + 7f);
			break;
		}
		case ButtonAlignment.TopLeftLower:
		{
			for (int j = 0; j < 4; j++)
			{
				_mFrameBends[j].SetAlignment(MilMo_GUI.Align.CenterCenter);
			}
			for (int k = 0; k < 4; k++)
			{
				_mFrame[k].SetAlignment(MilMo_GUI.Align.CenterCenter);
			}
			_mInsideWidget.SetAlpha(0f);
			if (_mTexturePathExtenstion == "")
			{
				_mInsideWidget.AlphaTo(0.5f);
			}
			else if (_mTexturePathExtenstion.ToUpper() == "DARK")
			{
				_mInsideWidget.AlphaTo(0.74f);
			}
			_mInsideWidget.RefreshResolution();
			_mInsideWidget.SetPosition(Pos.x + 60f + 2f, Pos.y + 5f);
			_mFrame[0].SetScale(_mInsideWidget.Scale.x + 46f + 14f, 14f);
			_mFrame[0].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x * 0.5f - 30f, _mInsideWidget.Pos.y - 7f);
			_mFrame[1].SetScale(14f, _mInsideWidget.Scale.y - 60f);
			_mFrame[1].SetPosition(_mInsideWidget.Pos.x - 7f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y * 0.5f + 30f);
			_mFrame[3].SetScale(_mInsideWidget.Scale.x, 14f);
			_mFrame[3].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x * 0.5f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y + 7f);
			_mFrame[2].SetScale(14f, _mInsideWidget.Scale.y);
			_mFrame[2].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x + 7f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y * 0.5f);
			_mFrameBends[0].SetPosition(_mInsideWidget.Pos.x - 67f, _mInsideWidget.Pos.y - 7f);
			_mFrameBends[3].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x + 7f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y + 7f);
			_mFrameBends[2].SetPosition(_mInsideWidget.Pos.x - 7f, _mInsideWidget.Pos.y + _mInsideWidget.Scale.y + 7f);
			_mFrameBends[1].SetPosition(_mInsideWidget.Pos.x + _mInsideWidget.Scale.x + 7f, _mInsideWidget.Pos.y - 7f);
			break;
		}
		}
	}
}
