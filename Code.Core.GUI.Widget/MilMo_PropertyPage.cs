using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.Button;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_PropertyPage : MilMo_Widget
{
	private int _tabCount;

	public int CurrentTab;

	public int CoverFlowMargin;

	private Vector2 _currentWindowSize = new Vector2(282f, 400f);

	public bool ScaleParent;

	private readonly MilMo_UserInterface _ui;

	public readonly MilMo_ScrollView TabScroller;

	public readonly MilMo_Button LeftArrow;

	public readonly MilMo_Button RightArrow;

	public readonly MilMo_Widget Background;

	public Vector2 ScaleTweak = Vector2.zero;

	public Vector2 TabOffset = new Vector2(16f, 55f);

	public Vector2 TabSize = new Vector2(80f, 26f);

	private readonly Color _tabSelectedTextColor = new Color(1f, 1f, 1f, 1f);

	private readonly Color _tabSelectedMouseOverTextColor = new Color(1f, 1f, 1f, 1f);

	public Color TabNonSelectedTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);

	public Color TabNonSelectedMouseOverTextColor = new Color(0.9f, 0.9f, 0.9f, 1f);

	public MilMo_GUI.Font TabFont = MilMo_GUI.Font.EborgSmall;

	public float TabFontScale = 1f;

	public string InactiveTabTexture = "Batch01/Textures/Core/Default";

	public string ActiveTabTexture = "Batch01/Textures/Core/Black";

	public bool AlwaysFadeInContents;

	public MilMo_Tab CurrentTabWidget
	{
		get
		{
			if (CurrentTab < 0 || CurrentTab > TabScroller.Children.Count)
			{
				return null;
			}
			return TabScroller.Children[CurrentTab] as MilMo_Tab;
		}
	}

	public MilMo_PropertyPage(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "PropertyPage " + MilMo_UserInterface.GetRandomID();
		AllowPointerFocus = false;
		_ui = ui;
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetTexture("Batch01/Textures/Core/Invisible");
		TabScroller = new MilMo_ScrollView(_ui);
		TabScroller.SetText(MilMo_LocString.Empty);
		TabScroller.SetPosition(2f, 0f);
		TabScroller.SetAlignment(MilMo_GUI.Align.TopLeft);
		TabScroller.ColorNow(1f, 1f, 1f, 1f);
		TabScroller.SetViewSize(1000f, 10f);
		TabScroller.HasBackground(b: false);
		TabScroller.IsUserControlled(b: false);
		TabScroller.SetScalePull(0.1f, 0.1f);
		TabScroller.SetScaleDrag(0.55f, 0.55f);
		TabScroller.SetScrollPull(0.15f, 0.15f);
		TabScroller.SetScrollDrag(0.65f, 0.65f);
		TabScroller.AllowPointerFocus = false;
		TabScroller.MouseWheelScrollable = false;
		AddChild(TabScroller);
		LeftArrow = new MilMo_Button(_ui);
		LeftArrow.SetTexture("Batch01/Textures/Shop/TabArrowLeft");
		LeftArrow.SetHoverTexture("Batch01/Textures/Shop/TabArrowLeftMO");
		LeftArrow.SetPressedTexture("Batch01/Textures/Shop/TabArrowLeftMO");
		LeftArrow.SetHoverColor(1f, 1f, 1f, 1f);
		LeftArrow.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		LeftArrow.SetFadeInSpeed(0.05f);
		LeftArrow.SetFadeOutSpeed(1f);
		LeftArrow.SetDefaultColor(1f, 1f, 1f, 0.6f);
		LeftArrow.SetPosition(0f, TabOffset.y);
		LeftArrow.SetDefaultScale(TabOffset.x, TabOffset.x * 2f);
		LeftArrow.ScaleMover.Vel.x = 0f;
		LeftArrow.ScaleMover.Vel.y = 0f;
		LeftArrow.SetAlignment(MilMo_GUI.Align.BottomLeft);
		LeftArrow.Function = ClkPrevCategory;
		LeftArrow.SetScalePull(0.1f, 0.1f);
		LeftArrow.SetScaleDrag(0.55f, 0.55f);
		AddChild(LeftArrow);
		RightArrow = new MilMo_Button(_ui);
		RightArrow.SetTexture("Batch01/Textures/Shop/TabArrowRight");
		RightArrow.SetHoverTexture("Batch01/Textures/Shop/TabArrowRightMO");
		RightArrow.SetPressedTexture("Batch01/Textures/Shop/TabArrowRightMO");
		RightArrow.SetHoverColor(1f, 1f, 1f, 1f);
		RightArrow.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		RightArrow.SetFadeInSpeed(0.05f);
		RightArrow.SetFadeOutSpeed(1f);
		RightArrow.SetDefaultColor(1f, 1f, 1f, 1f);
		RightArrow.SetDefaultScale(TabOffset.x, TabOffset.x * 2f);
		RightArrow.SetAlignment(MilMo_GUI.Align.BottomLeft);
		RightArrow.Function = ClkNextCategory;
		RightArrow.SetScalePull(0.1f, 0.1f);
		RightArrow.SetScaleDrag(0.55f, 0.55f);
		AddChild(RightArrow);
		Background = new MilMo_Widget(UI);
		Background.SetTexture("Batch01/Textures/Core/Black");
		Background.SetPosition(0f, 56f);
		Background.SetScale(281f, 300f);
		Background.SetAlignment(MilMo_GUI.Align.TopLeft);
		Background.SetCropMode(MilMo_GUI.CropMode.Crop);
		Background.AllowPointerFocus = false;
		AddChild(Background);
	}

	public override void Draw()
	{
		Background.SetPosition(3f, TabOffset.y);
		if (Parent != null)
		{
			if (ScaleParent)
			{
				Parent.ScaleTo(_currentWindowSize);
			}
			Background.SetScale(Parent.Scale.x / base.Res.x + _ui.Padding.x - 36f, Parent.Scale.y / base.Res.y - TabOffset.y - _ui.Padding.y * 2f - 22f);
		}
		ScaleTo(_currentWindowSize);
		TabScroller.ScaleNow(ScaleMover.Target.x - _ui.Padding.x * 2f / 2f, (float)Screen.height / base.Res.y + 50f);
		RightArrow.SetPosition(Scale.x / base.Res.x - TabOffset.x + 1f, TabOffset.y);
		DoTabScroll();
		DoTabArrows();
		foreach (MilMo_Tab child in TabScroller.Children)
		{
			DoTabVisibility(child);
		}
		base.Draw();
	}

	public MilMo_Tab CreateTab(MilMo_LocString name)
	{
		MilMo_Tab milMo_Tab = new MilMo_Tab(_ui, name);
		AttachTab(milMo_Tab);
		return milMo_Tab;
	}

	private void CurrentTabSelected()
	{
		if (CurrentTab < 0 || CurrentTab > TabScroller.Children.Count)
		{
			Debug.LogWarning("CurrentTab is out of bounds in MilMo_PropertyPage when fetching current widget tab");
		}
		else if (!(TabScroller.Children[CurrentTab] is MilMo_Tab milMo_Tab))
		{
			Debug.LogWarning("Tab scroller in MilMo_PropertyPage has a child widget that is not a MilMo_Tab (" + TabScroller.Children[CurrentTab].GetType()?.ToString() + ")");
		}
		else
		{
			milMo_Tab.Selected();
		}
	}

	private void AttachTab(MilMo_Tab tab)
	{
		Vector2 vector = new Vector2(Parent.Scale.x / base.Res.x - _ui.Padding.x * 2f, Parent.Scale.y / base.Res.y - _ui.Padding.y * 2f);
		if (Parent != null)
		{
			ScaleNow(vector.x, vector.y);
			TabScroller.SetScale(vector.x, (float)Screen.height / base.Res.y + 50f);
		}
		if (tab != null)
		{
			TabScroller.AddChild(tab);
			tab.Info = _tabCount;
			tab.SetPosition((float)tab.Info * TabSize.x + TabOffset.x, TabOffset.y);
			tab.SetFont(TabFont);
			tab.SetFontScale(TabFontScale);
			tab.ScrollView.SetPosition(0f, TabOffset.y + _ui.Padding.y * 2f);
			tab.ScrollView.SetScale(vector.x, vector.y - TabOffset.y - (_ui.Padding.y * 2f + 5f) - 33f);
			tab.ScrollView.ShowHorizontalBar(h: false);
			tab.ScrollView.RefreshViewSize();
			AddChild(tab.ScrollView);
			_tabCount++;
		}
		else
		{
			Debug.LogWarning("Error: Failed to attach tab. (MilMo_PropertyPage:AttachTab)");
		}
		UpdateTabs();
	}

	protected override void DrawText()
	{
		UpdateLabelStyle();
		foreach (MilMo_Tab child in TabScroller.Children)
		{
			float x = LabelStyle.CalcSize(new GUIContent(child.GetText())).x;
			float num = 1f - (x - 40f) / TabSize.x;
			if (num < 1f)
			{
				child.SetFontScale(num, 1f - (1f - num) / 2f);
			}
		}
		base.DrawText();
	}

	public void ReAttachTab(MilMo_Tab tab)
	{
		AttachTab(tab);
	}

	public void DetachTabs()
	{
		foreach (MilMo_Tab child in TabScroller.Children)
		{
			RemoveChild(child.ScrollView);
		}
		TabScroller.RemoveAllChildren();
		_tabCount = 0;
		UpdateTabs();
	}

	public void UpdateTabs()
	{
		foreach (MilMo_Tab child in TabScroller.Children)
		{
			DoTabHighlight(child);
			DoTabVisibility(child);
			if (!AlwaysFadeInContents)
			{
				continue;
			}
			foreach (MilMo_Widget child2 in child.ScrollView.Children)
			{
				child2.SetAlpha(0f);
				child2.AlphaTo(1f);
			}
		}
	}

	private void DoTabScroll()
	{
		float num = (float)CurrentTab * (TabSize.x * base.Res.x);
		float num2 = TabScroller.SoftScroll.Target.x + ScaleMover.Target.x - TabOffset.x * base.Res.x * 2f;
		float x = TabScroller.SoftScroll.Target.x;
		Vector2 scrollTarget = TabScroller.GetScrollTarget();
		if (CoverFlowMargin != 0)
		{
			DoCoverFlow();
		}
		else if (num - TabSize.x * base.Res.x <= x)
		{
			TabScroller.ScrollTo(scrollTarget.x - TabSize.x, scrollTarget.y);
		}
		else if (num + TabSize.x * base.Res.x >= num2)
		{
			TabScroller.ScrollTo(scrollTarget.x + TabSize.x, scrollTarget.y);
		}
		if ((float)TabScroller.Children.Count <= (ScaleMover.Target.x / base.Res.x - TabOffset.x * 2f) / TabSize.x)
		{
			TabScroller.ScrollTo(0f, scrollTarget.y);
		}
	}

	public void DoTabHighlight(MilMo_Tab tab)
	{
		if (tab.Info == CurrentTab)
		{
			tab.SetAllTextures(ActiveTabTexture);
			tab.SetHoverColor(1f, 1f, 1f, 1f);
			tab.GoTo((float)tab.Info * TabSize.x + TabOffset.x, TabOffset.y);
			tab.ScaleTo(TabSize.x, TabSize.y * 1.2f);
			tab.SetTextOffset(0f, -5f);
			tab.ColorNow(tab.HoverColor);
			tab.SetDefaultTextColor(_tabSelectedTextColor);
			tab.SetHoverTextColor(_tabSelectedMouseOverTextColor);
			tab.ScrollView.SetEnabled(e: true);
			_currentWindowSize = tab.WindowSize;
		}
		else
		{
			tab.SetAllTextures(InactiveTabTexture);
			tab.SetHoverColor(1f, 1f, 0.7f, 1f);
			tab.GoTo((float)tab.Info * TabSize.x + TabOffset.x, TabOffset.y);
			tab.SetScale(TabSize);
			tab.SetTextOffset(0f, -5f);
			tab.SetDefaultTextColor(TabNonSelectedTextColor);
			tab.SetHoverTextColor(TabNonSelectedMouseOverTextColor);
			tab.ScrollView.SetEnabled(e: false);
		}
	}

	private void DoTabArrows()
	{
		int num = 0;
		int num2 = 0;
		RightArrow.ScaleTo(32f, 0f);
		LeftArrow.ScaleTo(32f, 0f);
		foreach (MilMo_Tab child in TabScroller.Children)
		{
			num++;
			if (child.AllowPointerFocus)
			{
				num2++;
			}
		}
		if (num2 < num)
		{
			RightArrow.ScaleTo(TabOffset.x, TabOffset.x * 2f);
			LeftArrow.ScaleTo(TabOffset.x, TabOffset.x * 2f);
		}
	}

	private void DoTabVisibility(MilMo_Tab tab)
	{
		tab.SetEnabled(e: true);
		tab.AllowPointerFocus = true;
		tab.RestoreArrive();
		tab.GoTo((float)tab.Info * TabSize.x + TabOffset.x, TabOffset.y);
		if (tab.Pos.x + tab.Scale.x > ScaleMover.Target.x + TabScroller.SoftScroll.Target.x - TabOffset.x * base.Res.x)
		{
			tab.SetEnabled(e: false);
			tab.GoTo((float)tab.Info * TabSize.x + TabOffset.x, TabOffset.y + 64f);
			tab.AllowPointerFocus = false;
			tab.DisableOnArrive();
			if (!RightArrow.IsEnabled())
			{
				RightArrow.ColorNow(1f, 1f, 1f, 1f);
			}
		}
		if (tab.PosMover.Target.x - TabScroller.SoftScroll.Target.x < 0f)
		{
			tab.SetEnabled(e: false);
			tab.GoTo((float)tab.Info * TabSize.x + TabOffset.x + 100f, TabOffset.y + 64f);
			tab.AllowPointerFocus = false;
			tab.DisableOnArrive();
			if (!LeftArrow.IsEnabled())
			{
				LeftArrow.ColorNow(1f, 1f, 1f, 1f);
			}
		}
	}

	private void ClkPrevCategory(object arg)
	{
		if (CurrentTab < 1)
		{
			CurrentTab = TabScroller.Children.Count;
			float x = (float)CurrentTab * TabSize.x - ScaleMover.Target.x / base.Res.x - TabOffset.x;
			TabScroller.ScrollTo(x, 0f);
			CurrentTab--;
			CurrentTabSelected();
			UpdateTabs();
		}
		else
		{
			CurrentTab--;
			CurrentTabSelected();
			UpdateTabs();
		}
	}

	private void ClkNextCategory(object arg)
	{
		if (CurrentTab > TabScroller.Children.Count - 2)
		{
			CurrentTab = 0;
			TabScroller.ScrollTo(0f, 0f);
			UpdateTabs();
			CurrentTabSelected();
		}
		else
		{
			CurrentTab++;
			CurrentTabSelected();
			UpdateTabs();
		}
	}

	public void DoCoverFlow()
	{
		if (CurrentTab != TabScroller.Children.Count - 1)
		{
			TabScroller.ScrollTo((float)CurrentTab * TabSize.x - (float)CoverFlowMargin * TabSize.x, 0f);
		}
	}
}
