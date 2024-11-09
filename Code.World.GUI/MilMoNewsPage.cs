using Code.Core.Command;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.Button;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World.Player;
using UnityEngine;

namespace Code.World.GUI;

public class MilMoNewsPage : MilMo_Window
{
	private readonly MilMo_GenericReaction m_ToggleReaction;

	public MilMoNewsPage(MilMo_UserInterface ui)
		: base(ui)
	{
		m_ToggleReaction = MilMo_EventSystem.Listen("button_ToggleNewsPage", delegate
		{
			if (!MilMo_Player.Instance.AnyShopState)
			{
				Toggle();
			}
		});
		m_ToggleReaction.Repeating = true;
		Identifier = "NewsPage";
		UI = ui;
		HasCloseButton = true;
		UI.ResetLayout(5f, 5f);
		MilMo_Command.Instance.RegisterCommand("NewsPage.GoTo", DebugGoTo);
		SetText(MilMo_LocString.Empty);
		AddPropertyPage();
		if (base.PropertyPage != null)
		{
			SetBackgroundTexture("Batch01/Textures/Core/Invisible");
			SetDividerTexture("Batch01/Textures/Core/Invisible");
			SetActiveTabTexture("Batch01/Textures/Core/Invisible");
			SetInactiveTabTexture("Batch01/Textures/Core/Invisible");
			SetTabSize(80f, 32f);
			SetTabOffset(16f, 55f);
			SetTabFont(MilMo_GUI.Font.EborgLarge, 1f);
			base.PropertyPage.CurrentTab = 0;
			base.PropertyPage.CoverFlowMargin = 0;
			base.PropertyPage.ScaleParent = true;
			CreateTab(MilMo_Localization.GetLocString("World_391")).WindowSize = new Vector2(400f, 450f);
			UpdateTabs();
		}
		else
		{
			MilMo_Window.Warning(0);
		}
		SpawnScale = new Vector2(400f, 300f);
		TargetScale = new Vector2(400f, 300f);
		ExitScale = new Vector2(400f, 300f);
		BringToFront(base.CloseButton);
		Step();
		Open();
	}

	public override void Draw()
	{
		base.Draw();
	}

	public override void Step()
	{
		base.Step();
	}

	public override void Refresh()
	{
		if (!HasPropertyPage || base.PropertyPage == null)
		{
			MilMo_Window.Warning(0);
			return;
		}
		if (base.PropertyPage != null)
		{
			foreach (MilMo_Tab child in base.PropertyPage.TabScroller.Children)
			{
				child.ScrollView.RemoveAllChildren();
				UI.ResetLayout(5f, 5f, child.ScrollView);
				UI.Next.x += 5f;
				child.NextPos = UI.Next;
				child.SamePos = UI.Same;
			}
		}
		AddNewsPost("News", MilMo_NewsManager.Instance.Date, MilMo_NewsManager.Instance.Headline, MilMo_NewsManager.Instance.TextBody);
		BringToFront(base.FadeBottom);
		BringToFront(base.FadeTop);
	}

	private void AddNewsPost(string tabName, string date, string headline, string post)
	{
		MilMo_Tab tab = GetTab(tabName);
		if (tab != null)
		{
			Vector2 windowSize = tab.WindowSize;
			MilMo_Widget widget = new MilMo_NewsPost(UI, date, headline, post, windowSize);
			AddWidget(widget, tabName);
		}
		else
		{
			Debug.LogWarning("'MilMoNewsPage:AddNewsPost' failed : tab is null.");
		}
	}

	public string DebugGoTo(string[] args)
	{
		float x = MilMo_Utility.StringToFloat(args[1]);
		float y = MilMo_Utility.StringToFloat(args[2]);
		SetPosition(x, y);
		return "";
	}
}
