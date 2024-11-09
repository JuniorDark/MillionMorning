using System;
using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class jb_PVPModeInfoWidget : MilMo_Widget
{
	private readonly MilMo_Widget m_TitleText;

	private readonly MilMo_Widget m_InfoText;

	private readonly Vector2 m_parentScale;

	private readonly ICollection<MilMo_Widget> icons;

	public jb_PVPModeInfoWidget(MilMo_UserInterface ui, Vector2 parentScale)
		: base(ui)
	{
		icons = new List<MilMo_Widget>();
		m_parentScale = parentScale;
		PosMover.Val.x = 0f;
		PosMover.Val.y = 0f;
		ScaleMover.Val.x = 100f * base.Res.x;
		ScaleMover.Val.y = 20f * base.Res.y;
		PosMover.Target.x = PosMover.Val.x;
		PosMover.Target.y = PosMover.Val.y;
		ScaleMover.Target.x = ScaleMover.Val.x;
		ScaleMover.Target.y = ScaleMover.Val.y;
		SetAlignment(MilMo_GUI.Align.TopLeft);
		m_TitleText = new MilMo_Widget(UI);
		m_TitleText.SetFont(MilMo_GUI.Font.EborgLarge);
		m_TitleText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_TitleText.SetScale(parentScale.x, 50f);
		m_TitleText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_TitleText.SetPosition(90f, 20f);
		m_TitleText.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(m_TitleText);
		m_InfoText = new MilMo_Widget(UI);
		m_InfoText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_InfoText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_InfoText.SetScale(parentScale.x - 40f, 200f);
		m_InfoText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_InfoText.SetPosition(30f, 80f);
		m_InfoText.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		m_InfoText.SetWordWrap(w: true);
		AddChild(m_InfoText);
	}

	public void SetDescription(MilMo_MatchMode matchMode, int scoreGoal)
	{
		SetTitleText(matchMode.Title());
		SetInfoText(matchMode.Description(), scoreGoal);
		SetDescriptionIcons(matchMode);
	}

	private void SetDescriptionIcons(MilMo_MatchMode matchMode)
	{
		ICollection<jb_IconInfo> iconContainers = jb_IconInfo.GetIconContainers(matchMode);
		RemoveDescriptionIcons();
		AddDescriptionIcons(iconContainers);
	}

	private void SetInfoText(string info, int scoreGoal)
	{
		MilMo_LocString locString = MilMo_Localization.GetLocString(info);
		locString.SetFormatArgs(scoreGoal);
		m_InfoText.SetText(locString);
	}

	private void SetTitleText(string title)
	{
		m_TitleText.SetText(MilMo_Localization.GetLocString(title));
	}

	private void RemoveDescriptionIcons()
	{
		foreach (MilMo_Widget icon in icons)
		{
			RemoveChild(icon);
		}
		icons.Clear();
	}

	private void AddDescriptionIcons(ICollection<jb_IconInfo> iconInfos)
	{
		float num = m_parentScale.x / 2f;
		int num2 = (int)Math.Floor((float)iconInfos.Count / 2f) * -1;
		float num3 = ((iconInfos.Count % 2 == 0) ? 0.5f : 0f);
		foreach (jb_IconInfo iconInfo in iconInfos)
		{
			MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
			milMo_Widget.SetTexture(iconInfo.Path, prefixStandardGuiPath: false);
			milMo_Widget.SetScale(iconInfo.Width, iconInfo.Height);
			milMo_Widget.SetAlignment(MilMo_GUI.Align.BottomCenter);
			milMo_Widget.SetPosition(90f * ((float)num2 + num3) + num + (float)iconInfo.OffsetX, m_parentScale.y - 70f + (float)iconInfo.OffsetY);
			milMo_Widget.SetFont(MilMo_GUI.Font.EborgSmall);
			milMo_Widget.AllowPointerFocus = false;
			AddChild(milMo_Widget);
			icons.Add(milMo_Widget);
			num2++;
		}
	}
}
