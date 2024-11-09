using System.Collections.Generic;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.GUI.Widget.Button;

public sealed class MilMo_Tab : MilMo_Button
{
	public delegate void OnSelect(MilMo_Tab tab);

	public readonly List<MilMo_TabSection> Sections = new List<MilMo_TabSection>();

	public bool MForceScrollViewScale;

	public Vector2 ScrollViewScaleTweak = Vector2.zero;

	public Vector2 NextPos = Vector2.zero;

	public Vector2 SamePos = Vector2.zero;

	public Vector2 WindowSize = new Vector2(350f, 400f);

	private readonly List<OnSelect> _mOnSelectCallbacks = new List<OnSelect>();

	public MilMo_ScrollView ScrollView { get; private set; }

	public bool HasBeenSelected { get; private set; }

	public MilMo_Tab(MilMo_UserInterface ui, MilMo_LocString name)
		: base(ui)
	{
		Identifier = "Tab " + MilMo_UserInterface.GetRandomID();
		UI = ui;
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetDefaultColor(0.75f, 0.75f, 0.75f, 1f);
		SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		SetFadeInSpeed(0.02f);
		SetFadeOutSpeed(0.02f);
		SetAlignment(MilMo_GUI.Align.BottomLeft);
		SetTextDropShadowPos(2f, 2f);
		SetFontScale(1f);
		SetTextOffset(0f, 0f);
		SetTextAlignment(MilMo_GUI.Align.BottomCenter);
		SetPosPull(0.06f, 0.06f);
		SetPosDrag(0.6f, 0.6f);
		PosMover.MinVel.x = 0.1f;
		PosMover.MinVel.y = 0.1f;
		SetScalePull(0.08f, 0.08f);
		SetScaleDrag(0.6f, 0.6f);
		Function = ClkSelect;
		GoToNow(0f, 0f);
		SetScale(96f, 32f);
		SetFont(MilMo_GUI.Font.EborgSmall);
		SetText(name);
		ScrollView = new MilMo_ScrollView(UI);
		ScrollView.SetText(MilMo_LocString.Empty);
		ScrollView.SetPosition(0f, 0f);
		ScrollView.SetScale(100f, 100f);
		ScrollView.SetAlignment(MilMo_GUI.Align.TopLeft);
		ScrollView.ColorNow(1f, 1f, 1f, 1f);
		ScrollView.HasBackground(b: false);
		ScrollView.IsUserControlled(b: true);
		ScrollView.SetScrollPull(0.125f, 0.125f);
		ScrollView.SetScrollDrag(0.55f, 0.55f);
		ScrollView.AllowPointerFocus = false;
		ScrollView.SetEnabled(e: false);
		UI.AddChild(ScrollView);
	}

	public void RegisterSelectCallback(OnSelect callback)
	{
		_mOnSelectCallbacks.Add(callback);
	}

	public void AddWidget(MilMo_Widget obj)
	{
		if (obj != null)
		{
			ScrollView.AddChild(obj);
		}
	}

	public MilMo_TabSection AddSection(MilMo_LocString name, string identifier)
	{
		MilMo_TabSection milMo_TabSection = new MilMo_TabSection(UI, name, identifier);
		if (Parent != null)
		{
			milMo_TabSection.SetScale(Parent.ScaleMover.Target.x - 25f, 35f);
		}
		AddWidget(milMo_TabSection);
		milMo_TabSection.RefreshContents();
		Sections.Add(milMo_TabSection);
		RemoveBottomFadeFromLastSection();
		return milMo_TabSection;
	}

	public void RemoveBottomFadeFromLastSection()
	{
		MilMo_TabSection milMo_TabSection = null;
		foreach (MilMo_TabSection section in Sections)
		{
			section.MBottomFade.SetEnabled(e: true);
			milMo_TabSection = section;
		}
		milMo_TabSection?.MBottomFade.SetEnabled(e: false);
	}

	public void ClkSelect(object o)
	{
		if (Parent != null && Parent.GetType() == typeof(MilMo_ScrollView) && Parent.Parent is MilMo_PropertyPage milMo_PropertyPage)
		{
			milMo_PropertyPage.CurrentTab = Info;
			if (milMo_PropertyPage.CoverFlowMargin != 0)
			{
				milMo_PropertyPage.DoCoverFlow();
			}
			milMo_PropertyPage.UpdateTabs();
		}
		Selected();
	}

	public void Selected()
	{
		HasBeenSelected = true;
		foreach (OnSelect mOnSelectCallback in _mOnSelectCallbacks)
		{
			mOnSelectCallback(this);
		}
	}

	public void DisableOnArrive()
	{
		PosMover.Arrive = DoDisableOnArrive;
	}

	public void RestoreArrive()
	{
		PosMover.Arrive = MilMo_Widget.Nothing;
	}

	private void DoDisableOnArrive()
	{
		SetEnabled(e: false);
		PosMover.Arrive = MilMo_Widget.Nothing;
	}

	public override void Draw()
	{
		if (!MForceScrollViewScale && Parent.Parent is MilMo_PropertyPage milMo_PropertyPage)
		{
			ScrollView.SetXPos(2f);
			ScrollView.SetScale(milMo_PropertyPage.Scale.x - 8f + ScrollViewScaleTweak.x, milMo_PropertyPage.Scale.y - milMo_PropertyPage.TabOffset.y - (UI.Padding.y * 2f + 5f) - 52f + ScrollViewScaleTweak.y);
		}
		base.Draw();
	}
}
