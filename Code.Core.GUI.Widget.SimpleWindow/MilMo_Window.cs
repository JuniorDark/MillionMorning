using System.Linq;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.Button;
using Code.Core.ResourceSystem;
using Code.World.GUI;
using UnityEngine;

namespace Code.Core.GUI.Widget.SimpleWindow;

public class MilMo_Window : MilMo_SimpleWindow
{
	private MilMo_Widget _mFadeLeft;

	private MilMo_Widget _mFadeRight;

	private MilMo_Widget _mTopDiv;

	private MilMo_Widget _mBottomDiv;

	private MilMo_Widget _mFadeAbove;

	private MilMo_Widget _mFadeBelow;

	public Vector2 SpawnPos;

	public Vector2 TargetPos;

	public Vector2 SpawnScale;

	public Vector2 TargetScale;

	public Vector2 ExitScale;

	protected Vector2 CloseButtonOffset = new Vector2(-5f, 5f);

	private const float BOTTOM_DIV_TWEAK = -32f;

	private int _mLastTab;

	private bool _mHasPropertyPageGfx;

	private bool _bringToFront;

	public bool IsActive = true;

	protected bool HasPropertyPage;

	protected bool HasCloseButton;

	public bool DestroyFlag;

	protected bool ReadyToDraw;

	protected const int NO_PROPERTY_PAGE = 0;

	private const int NO_CLOSE_BUTTON = 1;

	public MilMo_PropertyPage PropertyPage { get; private set; }

	protected MilMo_Widget FadeTop { get; private set; }

	protected MilMo_Widget FadeBottom { get; private set; }

	protected MilMo_Button CloseButton { get; private set; }

	public override bool IgnoreGlobalFade
	{
		protected get
		{
			return base.IgnoreGlobalFade;
		}
		set
		{
			base.IgnoreGlobalFade = value;
			if (PropertyPage != null)
			{
				PropertyPage.IgnoreGlobalFade = value;
			}
			if (FadeTop != null)
			{
				FadeTop.IgnoreGlobalFade = value;
			}
			if (FadeBottom != null)
			{
				FadeBottom.IgnoreGlobalFade = value;
			}
			if (_mFadeLeft != null)
			{
				_mFadeLeft.IgnoreGlobalFade = value;
			}
			if (_mFadeRight != null)
			{
				_mFadeRight.IgnoreGlobalFade = value;
			}
			if (_mTopDiv != null)
			{
				_mTopDiv.IgnoreGlobalFade = value;
			}
			if (_mBottomDiv != null)
			{
				_mBottomDiv.IgnoreGlobalFade = value;
			}
			if (_mFadeAbove != null)
			{
				_mFadeAbove.IgnoreGlobalFade = value;
			}
			if (_mFadeBelow != null)
			{
				_mFadeBelow.IgnoreGlobalFade = value;
			}
			if (CloseButton != null)
			{
				CloseButton.IgnoreGlobalFade = value;
			}
		}
	}

	public MilMo_Window(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "Window " + MilMo_UserInterface.GetRandomID();
		UI = ui;
		IsWindow = true;
		UI.ResetLayout(10f, 10f);
		SetText(MilMo_Localization.GetLocString("Generic_91"));
		SetTexture("Batch01/Textures/Core/Invisible", prefixStandardGuiPath: true);
		SetFont(MilMo_GUI.Font.EborgMedium);
		SetTextAlignment(MilMo_GUI.Align.TopCenter);
		SetTextDropShadowPos(3f, 3f);
		SetTextOffset(0f, 0f);
		SetAlignment(MilMo_GUI.Align.TopCenter);
		ScaleNow(282f, 400f);
		SetScalePull(0.08f, 0.08f);
		SetScaleDrag(0.7f, 0.7f);
		Enabled = false;
		SetPosPull(0.08f, 0.08f);
		SetPosDrag(0.7f, 0.7f);
		SetAlpha(0f);
		SetFadeSpeed(0.1f);
		FadeToDefaultColor = false;
		UI.ResetLayout(5f, 5f);
		SpawnScale = new Vector2(0f, 0f);
		TargetScale = new Vector2(282f, 400f);
		ExitScale = new Vector2(0f, 0f);
		SpawnPos = new Vector2(UI.Center.x - TargetScale.x / 2f, UI.Center.y - TargetScale.y / 2f);
		TargetPos = SpawnPos;
		UI.ResetLayout(5f, 5f);
		CloseButton = new MilMo_Button(UI);
		CloseButton.SetScale(16f, 16f);
		CloseButton.SetAllTextures("Batch01/Textures/Core/Black");
		CloseButton.SetAlignment(MilMo_GUI.Align.TopRight);
		CloseButton.SetPosition(ScaleMover.Target.x + CloseButtonOffset.x, CloseButtonOffset.y);
		CloseButton.SetTexture("Batch01/Textures/World/CloseButton");
		CloseButton.SetHoverTexture("Batch01/Textures/World/CloseButtonMO");
		CloseButton.SetPressedTexture("Batch01/Textures/World/CloseButton");
		CloseButton.Function = Close;
		AddChild(CloseButton);
		Step();
	}

	public override void Draw()
	{
		if (!ReadyToDraw)
		{
			return;
		}
		if (HasPropertyPage)
		{
			if (FadeTop != null && FadeBottom != null)
			{
				UI.ResetLayout(5f, 5f);
				FadeTop.SetPosition(0f, 68f);
				FadeTop.SetScale(Scale.x / base.Res.x, 37.4992f);
				FadeBottom.SetPosition(0f, Scale.y / base.Res.y - UI.Padding.y * 1f - 31f);
				FadeBottom.SetScale(Scale.x / base.Res.x, 75f);
				_mFadeLeft.SetPosition(UI.Padding.x + 1f, 68f);
				_mFadeLeft.SetScale(37.4992f, Scale.y - UI.Padding.y * 1f - 31f - 68f);
				_mFadeRight.SetPosition(Scale.x - 20f, 68f);
				_mFadeRight.SetScale(37.4992f, Scale.y - UI.Padding.y * 1f - 31f - 68f);
			}
			if (_mHasPropertyPageGfx)
			{
				_mFadeAbove.SetScale(Scale.x / base.Res.x, 35f);
				_mFadeBelow.SetPosition(0f, Scale.y / base.Res.y - UI.Padding.y * 1f - 7f);
				_mFadeBelow.SetScale(Scale.x / base.Res.x, -16f);
			}
			_mTopDiv.ScaleNow(Scale.x / base.Res.x, 9.3748f);
			_mBottomDiv.SetPosition(0f, Scale.y / base.Res.y - UI.Padding.y * 1f + -32f);
			_mBottomDiv.ScaleNow(Scale.x / base.Res.x, 9.3748f);
			PropertyPage.ScaleNow(Scale.x / base.Res.x - UI.Padding.x * 1f + PropertyPage.ScaleTweak.x, Scale.y / base.Res.y + PropertyPage.ScaleTweak.y);
		}
		if (!HasCloseButton)
		{
			CloseButton.SetScale(0f, 0f);
		}
		else
		{
			CloseButton.SetPosition(Scale.x + CloseButtonOffset.x, CloseButtonOffset.y);
		}
		KeepOnScreen();
		SnapToEdges();
		base.Draw();
		if (_bringToFront)
		{
			UnityEngine.GUI.BringWindowToFront(base.WindowId);
			_bringToFront = false;
		}
	}

	private static void KeepOnScreen()
	{
	}

	private static void SnapToEdges()
	{
	}

	public override void Step()
	{
		if (HasPropertyPage)
		{
			int currentTab = PropertyPage.CurrentTab;
			if (currentTab != _mLastTab)
			{
				ClearPopups();
				_mLastTab = currentTab;
			}
		}
		base.Step();
		if (DestroyFlag)
		{
			Destroy();
		}
	}

	protected MilMo_Tab CreateTab(MilMo_LocString name)
	{
		if (!HasPropertyPage)
		{
			Warning(0);
			return null;
		}
		return PropertyPage.CreateTab(name);
	}

	public virtual void ClearPopups()
	{
	}

	public virtual void Refresh()
	{
		if (!HasPropertyPage)
		{
			return;
		}
		foreach (MilMo_Tab item in PropertyPage.TabScroller.Children.Cast<MilMo_Tab>())
		{
			item.ScrollView.RemoveAllChildren();
			UI.ResetLayout(5f, 5f, item.ScrollView);
			UI.Next.x += 5f;
			item.NextPos = UI.Next;
			item.SamePos = UI.Same;
		}
	}

	protected MilMo_Tab GetTab(string tabName)
	{
		if (!HasPropertyPage)
		{
			Warning(0);
			return null;
		}
		MilMo_Tab result = null;
		foreach (MilMo_Tab item in from MilMo_Tab t in PropertyPage.TabScroller.Children
			where tabName == t.Identifier
			select t)
		{
			result = item;
		}
		return result;
	}

	protected void AddWidget(MilMo_Widget widget, string tabName)
	{
		if (!HasPropertyPage || PropertyPage == null)
		{
			Warning(0);
			return;
		}
		MilMo_Tab milMo_Tab = null;
		foreach (MilMo_Tab item in from MilMo_Tab t in PropertyPage.TabScroller.Children
			where tabName == t.Identifier
			select t)
		{
			milMo_Tab = item;
		}
		if (milMo_Tab == null)
		{
			Debug.LogWarning("Error: 'MilMo_Window:AddWidget' failed");
			return;
		}
		float num = milMo_Tab.NextPos.x - UI.Padding.x;
		float y = milMo_Tab.SamePos.y;
		if (num * base.Res.x + widget.Scale.x > ScaleMover.Target.x - 27f)
		{
			num = UI.Align.Left;
			y = milMo_Tab.NextPos.y;
		}
		widget.SetPosition(num, y);
		milMo_Tab.NextPos = UI.Next;
		milMo_Tab.SamePos = UI.Same;
		milMo_Tab.AddWidget(widget);
		milMo_Tab.ScrollView.RefreshViewSize();
	}

	private void DisableWindow()
	{
		foreach (MilMo_WindowAddon item in from addon in UI.Children.OfType<MilMo_WindowAddon>()
			where addon.ParentWindow == this
			select addon)
		{
			item.SetEnabled(e: false);
		}
		SetEnabled(e: false);
	}

	public void Toggle()
	{
		if (!IsActive)
		{
			Open();
		}
		else
		{
			Close(null);
		}
	}

	public virtual void Open()
	{
		Refresh();
		IsActive = true;
		foreach (MilMo_WindowAddon item in from addon in UI.Children.OfType<MilMo_WindowAddon>()
			where addon.ParentWindow == this
			select addon)
		{
			item.SetEnabled(e: true);
		}
		SetEnabled(e: true);
		UI.ResetLayout(10f, 10f);
		SetScale(SpawnScale);
		ScaleTo(TargetScale);
		SetPosition(TargetPos);
		SetAlpha(1f);
		ReadyToDraw = true;
	}

	public virtual void Close(object obj)
	{
		IsActive = false;
		UI.ResetLayout(10f, 10f);
		UI.RemoveAllPopups();
		ScaleTo(ExitScale);
		DisableWindow();
	}

	protected void SetTabOffset(float x, float y)
	{
		if (!HasPropertyPage)
		{
			Warning(0);
		}
		else
		{
			PropertyPage.TabOffset = new Vector2(x, y);
		}
	}

	protected void SetTabSize(float x, float y)
	{
		if (!HasPropertyPage)
		{
			Warning(0);
		}
		else
		{
			PropertyPage.TabSize = new Vector2(x, y);
		}
	}

	public void UpdateTabs()
	{
		if (!HasPropertyPage)
		{
			Warning(0);
		}
		else
		{
			PropertyPage.UpdateTabs();
		}
	}

	protected void SetTabFont(MilMo_GUI.Font font, float scale)
	{
		if (!HasPropertyPage)
		{
			Warning(0);
			return;
		}
		PropertyPage.TabFont = font;
		PropertyPage.TabFontScale = scale;
	}

	protected void SetBackgroundTexture(string tex)
	{
		if (!HasPropertyPage)
		{
			Warning(0);
		}
		else
		{
			PropertyPage.Background.SetTexture(tex);
		}
	}

	protected void SetDividerTexture(string tex)
	{
		if (!HasPropertyPage)
		{
			Warning(0);
			return;
		}
		_mTopDiv.SetTexture(tex);
		_mBottomDiv.SetTexture(tex);
	}

	protected void SetActiveTabTexture(string tex)
	{
		if (!HasPropertyPage)
		{
			Warning(0);
		}
		else
		{
			PropertyPage.ActiveTabTexture = tex;
		}
	}

	protected void SetInactiveTabTexture(string tex)
	{
		if (!HasPropertyPage)
		{
			Warning(0);
		}
		else
		{
			PropertyPage.InactiveTabTexture = tex;
		}
	}

	public void BringToFront()
	{
		_bringToFront = true;
	}

	protected void AddPropertyPage()
	{
		HasPropertyPage = true;
		UI.ResetLayout(5f, 5f);
		PropertyPage = new MilMo_PropertyPage(UI);
		PropertyPage.GoToNow(UI.Padding.x, UI.Padding.y);
		PropertyPage.ScaleNow(ScaleMover.Target.x / base.Res.x - UI.Padding.x * 2f, ScaleMover.Target.y / base.Res.y);
		AddChild(PropertyPage);
		_mTopDiv = new MilMo_Widget(UI);
		_mTopDiv.SetTexture("Batch01/Textures/Core/Black");
		_mTopDiv.SetPosition(0f, 60f);
		_mTopDiv.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTopDiv.SetScale(281f, 9.3748f);
		_mTopDiv.SetCropMode(MilMo_GUI.CropMode.Crop);
		_mTopDiv.AllowPointerFocus = false;
		AddChild(_mTopDiv);
		_mBottomDiv = new MilMo_Widget(UI);
		_mBottomDiv.SetTexture("Batch01/Textures/Core/Black");
		_mBottomDiv.SetPosition(0f, 358f);
		_mBottomDiv.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mBottomDiv.SetScale(281f, 9.3748f);
		_mBottomDiv.SetCropMode(MilMo_GUI.CropMode.Crop);
		_mBottomDiv.AllowPointerFocus = false;
		AddChild(_mBottomDiv);
	}

	protected void AddPropertyPageWithGfx()
	{
		HasPropertyPage = true;
		_mHasPropertyPageGfx = true;
		UI.ResetLayout(5f, 5f);
		_mFadeAbove = new MilMo_Widget(UI);
		_mFadeAbove.SetTexture("Batch01/Textures/Shop/FadeBottom");
		_mFadeAbove.SetDefaultColor(0f, 0f, 0f, 1f);
		_mFadeAbove.SetPosition(0f, 63f);
		_mFadeAbove.SetScale(281f, 35f);
		_mFadeAbove.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_mFadeAbove.AllowPointerFocus = false;
		AddChild(_mFadeAbove);
		_mFadeBelow = new MilMo_Widget(UI);
		_mFadeBelow.SetTexture("Batch01/Textures/Shop/FadeBottom");
		_mFadeBelow.SetDefaultColor(0f, 0f, 0f, 1f);
		_mFadeBelow.SetPosition(0f, 383f);
		_mFadeBelow.SetScale(281f, -16f);
		_mFadeBelow.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mFadeBelow.AllowPointerFocus = false;
		AddChild(_mFadeBelow);
		PropertyPage = new MilMo_PropertyPage(UI);
		PropertyPage.GoToNow(UI.Padding.x, UI.Padding.y);
		PropertyPage.ScaleNow(ScaleMover.Target.x / base.Res.x - UI.Padding.x * 2f, ScaleMover.Target.y / base.Res.y);
		AddChild(PropertyPage);
		FadeTop = new MilMo_Widget(UI);
		FadeTop.SetTexture("Batch01/Textures/Shop/FadeTop");
		FadeTop.SetAlignment(MilMo_GUI.Align.TopLeft);
		FadeTop.AllowPointerFocus = false;
		AddChild(FadeTop);
		FadeBottom = new MilMo_Widget(UI);
		FadeBottom.SetTexture("Batch01/Textures/Shop/FadeBottom");
		FadeBottom.SetDefaultColor(0f, 0f, 0f, 1f);
		FadeBottom.SetAlignment(MilMo_GUI.Align.BottomLeft);
		FadeBottom.AllowPointerFocus = false;
		AddChild(FadeBottom);
		_mFadeLeft = new MilMo_Widget(UI);
		_mFadeLeft.SetTexture("Batch01/Textures/Shop/FadeLeft2");
		_mFadeLeft.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mFadeLeft.AllowPointerFocus = false;
		AddChild(_mFadeLeft);
		_mFadeRight = new MilMo_Widget(UI);
		_mFadeRight.SetTexture("Batch01/Textures/Shop/FadeRight2");
		_mFadeRight.SetAlignment(MilMo_GUI.Align.TopRight);
		_mFadeRight.AllowPointerFocus = false;
		AddChild(_mFadeRight);
		_mTopDiv = new MilMo_Widget(UI);
		_mTopDiv.SetTexture("Batch01/Textures/Core/Black");
		_mTopDiv.SetPosition(0f, 60f);
		_mTopDiv.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTopDiv.SetScale(281f, 9.3748f);
		_mTopDiv.SetCropMode(MilMo_GUI.CropMode.Crop);
		_mTopDiv.AllowPointerFocus = false;
		AddChild(_mTopDiv);
		_mBottomDiv = new MilMo_Widget(UI);
		_mBottomDiv.SetTexture("Batch01/Textures/Core/Black");
		_mBottomDiv.SetPosition(0f, 358f);
		_mBottomDiv.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mBottomDiv.SetScale(281f, 9.3748f);
		_mBottomDiv.SetCropMode(MilMo_GUI.CropMode.Crop);
		_mBottomDiv.AllowPointerFocus = false;
		AddChild(_mBottomDiv);
	}

	protected void SetPropertyPageGfxEnabled(bool enabled)
	{
		_mFadeAbove.SetEnabled(enabled);
		_mFadeBelow.SetEnabled(enabled);
		FadeTop.SetEnabled(enabled);
		FadeBottom.SetEnabled(enabled);
		_mFadeLeft.SetEnabled(enabled);
		_mFadeRight.SetEnabled(enabled);
		_mTopDiv.SetEnabled(enabled);
		_mBottomDiv.SetEnabled(enabled);
	}

	protected static void Warning(int error)
	{
		string text = "'MilMo_Window': ";
		switch (error)
		{
		case 0:
			text += "trying to use non-existing property page";
			break;
		case 1:
			text += "trying to use non-existing minimize button";
			break;
		}
		Debug.LogWarning(text);
	}

	private void Destroy()
	{
		foreach (MilMo_WindowAddon item in from addon in UI.Children.OfType<MilMo_WindowAddon>()
			where addon.ParentWindow == this
			select addon)
		{
			item.Destroy();
		}
		RemoveAllChildren();
		UI.RemoveChild(this);
	}
}
