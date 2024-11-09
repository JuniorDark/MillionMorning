using System;
using System.Collections.Generic;
using Code.Core.Global;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.GUI.Widget.SimpleWindow.Window.Popup;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.World.GUI.ShopPopups;
using Core.GameEvent;
using UnityEngine;

namespace Code.Core.GUI.Core;

public sealed class MilMo_UserInterface
{
	public struct MilMo_Rect
	{
		public float Left;

		public float Right;

		public float Top;

		public float Bottom;
	}

	public Vector2 Res;

	private Vector2 _resBak = new Vector2(1f, 1f);

	private bool _windowRes;

	public bool OffsetMode;

	public Vector2 GlobalPosOffset;

	public Vector2 GlobalInputOffset;

	private static bool _enabledGlobal = true;

	public bool Enabled = true;

	public MilMo_Widget ForceCameraWidget;

	private bool _unFocusAllWindows;

	public MilMo_Widget AncestorOfFocused;

	public bool PrintMouseFocus;

	public static bool KeyboardFocus = false;

	private Vector2 _mLastScreenSize = new Vector2(0f, 0f);

	public bool ScreenSizeDirty;

	public int Depth;

	public MilMo_Dialog ModalDialog;

	private MilMo_Widget _borderTop;

	private MilMo_Widget _borderBottom;

	private MilMo_Widget _borderLeft;

	private MilMo_Widget _borderRight;

	public Vector2 Padding = new Vector2(10f, 10f);

	public Vector2 Next;

	public Vector2 Prev;

	public Vector2 Same;

	public Vector2 Center;

	public MilMo_Rect Align;

	public float Width;

	public float Height;

	public readonly AudioSourceWrapper SoundFx;

	public static string CurrentRoom = "";

	public static MilMo_Widget PointerFocus
	{
		get
		{
			return MilMo_UserInterfaceManager.MouseFocus;
		}
		set
		{
			MilMo_UserInterfaceManager.MouseFocus = value;
		}
	}

	public static MilMo_Widget SelectedWidget { get; set; }

	public List<MilMo_Widget> Children { get; private set; }

	public string Identifier { get; private set; }

	public int NumSkins { get; private set; }

	public static MilMo_Widget FinalMouseFocus => MilMo_UserInterfaceManager.FinalMouseFocus;

	public GUISkin Skin { get; private set; }

	public GUISkin[] Skins { get; private set; }

	public GUISkin Font0 { get; private set; }

	public GUISkin Font11 { get; private set; }

	public GUISkin Font4 { get; private set; }

	public GUISkin Font5 { get; private set; }

	public GUISkin Font6 { get; private set; }

	public GUISkin Font7 { get; private set; }

	public GUISkin Font8 { get; private set; }

	public GUISkin Font9 { get; private set; }

	public GUISkin Font10 { get; private set; }

	public static void EnabledGlobal(bool shouldEnable)
	{
		_enabledGlobal = shouldEnable;
	}

	public MilMo_UserInterface(string identifier)
	{
		Children = new List<MilMo_Widget>();
		Identifier = identifier;
		Skin = MilMo_GUISkins.GetSkin("Junebug");
		Skins = new GUISkin[5];
		LoadSkin("Junebug");
		LoadSkin("Chat");
		LoadSkin("JunebugDarker");
		Font0 = MilMo_GUISkins.GetSkin("FontArialRounded");
		if ((bool)Font0)
		{
			Font0.label.wordWrap = true;
		}
		Font11 = MilMo_GUISkins.GetSkin("FontArialRoundedMedium");
		Font4 = MilMo_GUISkins.GetSkin("FontEborgSmall");
		Font5 = MilMo_GUISkins.GetSkin("FontEborgMedium");
		Font6 = MilMo_GUISkins.GetSkin("FontEborgLarge");
		Font7 = MilMo_GUISkins.GetSkin("FontEborgXL");
		Font8 = MilMo_GUISkins.GetSkin("FontGothamSmall");
		Font9 = MilMo_GUISkins.GetSkin("FontGothamMedium");
		Font10 = MilMo_GUISkins.GetSkin("FontGothamLarge");
		Res.x = 1f;
		Res.y = 1f;
		GlobalPosOffset.x = 0f;
		GlobalPosOffset.y = 0f;
		Next.x = 0f;
		Next.y = 0f;
		Same.x = 0f;
		Same.y = 0f;
		SetPadding(2f, 2f);
		if ((bool)MilMo_Global.AudioListener)
		{
			SoundFx = MilMo_Global.AudioListener.AddComponent<AudioSourceWrapper>();
		}
		CreateLetterbox();
	}

	private void LoadSkin(string name)
	{
		Skins[NumSkins] = MilMo_GUISkins.GetSkin(name);
		NumSkins++;
	}

	public void Cleanup()
	{
		UnityEngine.Object.Destroy(SoundFx.Source);
		UnityEngine.Object.Destroy(SoundFx);
	}

	public void AddChild(MilMo_Widget w)
	{
		if (w.Parent != null)
		{
			w.Parent.RemoveChild(w);
			w.Parent = null;
		}
		Children.Add(w);
	}

	public void RemoveChild(MilMo_Widget w)
	{
		if (w != null)
		{
			w.Parent = null;
			Children.Remove(w);
		}
	}

	public void RemoveAllChildrenByType(Type childType)
	{
		for (int num = Children.Count - 1; num >= 0; num--)
		{
			MilMo_Widget milMo_Widget = Children[num];
			if (!(milMo_Widget.GetType() != childType))
			{
				milMo_Widget.Parent = null;
				Children.Remove(milMo_Widget);
			}
		}
	}

	public static int NextWindowId()
	{
		return MilMo_UserInterfaceManager.NextWindowID();
	}

	public void FixedUpdate()
	{
		if (!_enabledGlobal)
		{
			return;
		}
		for (int i = 0; i < Children.Count; i++)
		{
			MilMo_Widget milMo_Widget = Children[i];
			if (milMo_Widget.IgnoreNextStepDueToBringToFront)
			{
				milMo_Widget.IgnoreNextStepDueToBringToFront = false;
			}
			else
			{
				milMo_Widget.Step();
			}
		}
		if (PrintMouseFocus)
		{
			DoPrintMouseFocus();
		}
	}

	public void OnGUI()
	{
		if (_enabledGlobal)
		{
			AncestorOfFocused = null;
			if (_unFocusAllWindows)
			{
				UnityEngine.GUI.UnfocusWindow();
				_unFocusAllWindows = false;
			}
			UnityEngine.GUI.skin = Skin;
			for (int i = 0; i < Children.Count; i++)
			{
				Children[i].Draw();
			}
			if (OffsetMode)
			{
				Res.x = Mathf.Min((float)Screen.width / 1024f, 1f);
				Res.y = Mathf.Min((float)Screen.height / 720f, 1f);
				UpdateGlobalOffset();
				UpdateLetterbox();
			}
			else
			{
				Res.x = 1f;
				Res.y = 1f;
			}
			if (_mLastScreenSize != new Vector2(Screen.width, Screen.height))
			{
				_mLastScreenSize = new Vector2(Screen.width, Screen.height);
				ScreenSizeDirty = true;
				RefreshResolution();
			}
			if (ForceCameraWidget != null)
			{
				ForceCameraToWidget();
			}
		}
	}

	public void BypassResolution()
	{
		if (!_windowRes)
		{
			_windowRes = true;
			_resBak = Res;
			Res = new Vector2(1f, 1f);
		}
	}

	public void RestoreResolution()
	{
		if (_windowRes)
		{
			_windowRes = false;
			Res = _resBak;
		}
	}

	private void RefreshResolution()
	{
		for (int i = 0; i < Children.Count; i++)
		{
			Children[i].RefreshResolution();
		}
	}

	private void ForceCameraToWidget()
	{
		Rect screenPosition = ForceCameraWidget.GetScreenPosition();
		screenPosition.y = (float)Screen.height - screenPosition.height - screenPosition.y;
		screenPosition.x /= Screen.width;
		screenPosition.y /= Screen.height;
		screenPosition.width /= Screen.width;
		screenPosition.height /= Screen.height;
		MilMo_Global.Camera.rect = screenPosition;
	}

	public static void ResetCameraRect()
	{
		MilMo_Global.Camera.rect = new Rect(0f, 0f, 1f, 1f);
	}

	public void BringToFront(MilMo_Widget w)
	{
		if (w != null && Children.Contains(w))
		{
			Children.Remove(w);
			Children.Add(w);
		}
	}

	public void ResetLayout(MilMo_Widget parent)
	{
		ResetLayout(0f, 0f, parent);
	}

	public void ResetLayout(float padX = 0f, float padY = 0f)
	{
		Padding.x = padX;
		Padding.y = padY;
		Next.x = padX;
		Next.y = padY;
		Prev.x = Next.x;
		Prev.y = Next.y;
		Same.x = Next.x;
		Same.y = Next.y;
		Center = GetCenter();
		Align.Left = padX;
		Align.Top = padY;
		Align.Right = Mathf.Min(1024f, Screen.width) - padX;
		Align.Bottom = Mathf.Min(720f, Screen.height) - padY;
		Width = Mathf.Min(1024f, Screen.width) - padX * 2f;
		Height = Mathf.Min(720f, Screen.height) - padY * 2f;
	}

	public void ResetLayout(float padX, float padY, MilMo_Widget parent)
	{
		if (Res.x <= 0f && Res.y <= 0f)
		{
			Res.x = 1f;
			Res.y = 1f;
		}
		Padding.x = padX;
		Padding.y = padY;
		Next.x = padX;
		Next.y = padY;
		Prev.x = Next.x;
		Prev.y = Next.y;
		Same.x = Next.x;
		Same.y = Next.y;
		Center.x = parent.ScaleMover.Target.x * 0.5f;
		Center.y = parent.ScaleMover.Target.y * 0.5f;
		Align.Left = padX;
		Align.Top = padY;
		Align.Right = parent.ScaleMover.Target.x / Res.x - padX;
		Align.Bottom = parent.ScaleMover.Target.y / Res.y - padY;
		Width = parent.ScaleMover.Target.x / Res.x - padX * 2f;
		Height = parent.ScaleMover.Target.y / Res.y - padY * 2f;
	}

	public void UpdateAutoLayout(MilMo_Widget w)
	{
		if (Res.x <= 0f && Res.y <= 0f)
		{
			Res.x = 1f;
			Res.y = 1f;
		}
		Same.x = w.PosMover.Target.x / Res.x;
		Next.x = w.PosMover.Target.x / Res.x + w.ScaleMover.Target.x / Res.x + Padding.x;
		Prev.x = w.PosMover.Target.x / Res.x - w.ScaleMover.Target.x / Res.x - Padding.x;
		Same.y = w.PosMover.Target.y / Res.y;
		Next.y = w.PosMover.Target.y / Res.y + w.ScaleMover.Target.y / Res.y + Padding.y;
		Prev.y = w.PosMover.Target.y / Res.y - w.ScaleMover.Target.y / Res.y - Padding.y;
	}

	public void SetNext(float x, float y)
	{
		Same.x = x;
		Same.y = y;
		Next.x = x;
		Next.y = y;
		Prev.x = x;
		Prev.y = y;
	}

	public void UnFocusAllWindows()
	{
		_unFocusAllWindows = true;
	}

	private void SetPadding(float x, float y)
	{
		Padding.x = x;
		Padding.y = y;
	}

	public static string Debug_GUI(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage: GUI {0|1}";
		}
		try
		{
			bool flag = Convert.ToInt32(args[1]) != 0;
			EnabledGlobal(flag);
			GameEvent.ShowHUDEvent.RaiseEvent(flag);
			return "User interface was " + (flag ? "enabled" : "disabled");
		}
		catch (FormatException ex)
		{
			return ex?.ToString() + " usage: GUI {0|1}";
		}
	}

	private void DoPrintMouseFocus()
	{
		string text = "UI: '" + Identifier + "'";
		if (PointerFocus != null)
		{
			text += "      Focus: ";
			text = ((PointerFocus.Identifier == null) ? (text + "'Unknown widget'   ") : (text + "'" + PointerFocus.Identifier + "'   "));
		}
		else
		{
			text += "   ";
		}
		if (PointerFocus != null && PointerFocus.Parent != null)
		{
			text += "      Parent: ";
			text = ((PointerFocus.Parent.Identifier == null) ? (text + "'Unknown widget'   ") : (text + "'" + PointerFocus.Parent.Identifier + "'   "));
		}
		else
		{
			text += "   ";
		}
		if (AncestorOfFocused != null)
		{
			text += "   Ancestor: ";
			text = ((AncestorOfFocused.Identifier == null) ? (text + "'Unknown widget'") : (text + "'" + AncestorOfFocused.Identifier + "'"));
		}
		else
		{
			text = text ?? "";
		}
		Debug.Log(text);
	}

	public static string GetRandomID()
	{
		return MilMo_Utility.RandomID().ToString();
	}

	public void UpdateGlobalOffset()
	{
		GlobalPosOffset = ((Screen.width <= 1024) ? new Vector2(0f, GlobalPosOffset.y) : new Vector2(((float)Screen.width - 1024f) * 0.5f, GlobalPosOffset.y));
		GlobalPosOffset = ((Screen.height <= 720) ? new Vector2(GlobalPosOffset.x, 0f) : new Vector2(GlobalPosOffset.x, (float)(Screen.height - 720) * 0.5f));
		GlobalInputOffset = GlobalPosOffset;
	}

	public void SetGlobalOffset(Vector2 offset)
	{
		GlobalPosOffset = offset;
		GlobalInputOffset = GlobalPosOffset;
	}

	private Vector2 GetCenter()
	{
		return new Vector2((float)Screen.width / 2f - GlobalPosOffset.x, (float)Screen.height / 2f - GlobalPosOffset.y);
	}

	public float GetLowestUIRes()
	{
		if (Res.x < 1f || Res.y < 1f)
		{
			if (!(Res.x <= Res.y))
			{
				return Res.y;
			}
			return Res.x;
		}
		return 1f;
	}

	public Vector2 ScaleToLowestUIRes(Vector2 vector)
	{
		return new Vector2(vector.x * GetLowestUIRes(), vector.y * GetLowestUIRes());
	}

	public Vector2 ScaleToLowestUIRes(float x, float y)
	{
		return ScaleToLowestUIRes(new Vector2(x, y));
	}

	private void CreateLetterbox()
	{
		_borderTop = new MilMo_Widget(this)
		{
			Identifier = "B1"
		};
		_borderTop.SetAlignment(MilMo_GUI.Align.TopLeft);
		_borderTop.SetTexture("Batch01/Textures/Core/Black");
		_borderTop.AllowPointerFocus = false;
		_borderTop.SetEnabled(e: false);
		_borderTop.IgnoreGlobalFade = true;
		_borderBottom = new MilMo_Widget(this)
		{
			Identifier = "B2"
		};
		_borderBottom.SetAlignment(MilMo_GUI.Align.TopLeft);
		_borderBottom.SetTexture("Batch01/Textures/Core/Black");
		_borderBottom.AllowPointerFocus = false;
		_borderBottom.SetEnabled(e: false);
		_borderBottom.IgnoreGlobalFade = true;
		_borderLeft = new MilMo_Widget(this)
		{
			Identifier = "B3"
		};
		_borderLeft.SetAlignment(MilMo_GUI.Align.TopLeft);
		_borderLeft.SetTexture("Batch01/Textures/Core/Black");
		_borderLeft.AllowPointerFocus = false;
		_borderLeft.SetEnabled(e: false);
		_borderLeft.IgnoreGlobalFade = true;
		_borderRight = new MilMo_Widget(this)
		{
			Identifier = "B4"
		};
		_borderRight.SetAlignment(MilMo_GUI.Align.TopLeft);
		_borderRight.SetTexture("Batch01/Textures/Core/Black");
		_borderRight.AllowPointerFocus = false;
		_borderRight.SetEnabled(e: false);
		_borderRight.IgnoreGlobalFade = true;
	}

	private void UpdateLetterbox()
	{
		_borderTop.SetEnabled(e: true);
		_borderBottom.SetEnabled(e: true);
		_borderLeft.SetEnabled(e: true);
		_borderRight.SetEnabled(e: true);
		if (Res.x < 1f)
		{
			_borderLeft.SetEnabled(e: false);
			_borderRight.SetEnabled(e: false);
		}
		if (Res.y < 1f)
		{
			_borderTop.SetEnabled(e: false);
			_borderBottom.SetEnabled(e: false);
		}
		_borderTop.SetPosition(0f - GlobalPosOffset.x, 0f - GlobalPosOffset.y);
		_borderTop.SetScale((float)Screen.width / Res.x, GlobalPosOffset.y);
		_borderTop.Draw();
		_borderBottom.SetPosition(0f - GlobalPosOffset.x, (float)Screen.height - GlobalPosOffset.y - GlobalPosOffset.y);
		_borderBottom.SetScale((float)Screen.width / Res.x, GlobalPosOffset.y);
		_borderBottom.Draw();
		_borderLeft.SetPosition(0f - GlobalPosOffset.x, 0f);
		_borderLeft.SetScale(GlobalPosOffset.x, (float)Screen.height / Res.y - GlobalPosOffset.y * 2f);
		_borderLeft.Draw();
		_borderRight.SetPosition((float)Screen.width / Res.x - GlobalPosOffset.x - GlobalPosOffset.x, 0f);
		_borderRight.SetScale(GlobalPosOffset.x, (float)Screen.height / Res.y - GlobalPosOffset.y * 2f);
		_borderRight.Draw();
	}

	public void FlashModalDialog()
	{
		ModalDialog.SetColor(1f, 0f, 0f, 1f);
		ModalDialog.ColorTo(ModalDialog.DefaultColor);
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
	}

	public void RemoveAllPopups()
	{
		for (int i = 0; i < Children.Count; i++)
		{
			MilMo_Widget milMo_Widget = Children[i];
			Type type = milMo_Widget.GetType();
			if (!(type != typeof(MilMo_Popup)) || !(type != typeof(MilMo_PicturePopup)) || !(type != typeof(MilMo_WeaponPopup)) || !(type != typeof(MilMo_ConverterPopup)))
			{
				((MilMo_Popup)milMo_Widget).Remove();
			}
		}
	}
}
