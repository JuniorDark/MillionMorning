using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI.Ladder;

internal class MilMoVotingPoint : MilMo_Button
{
	private readonly MilMo_Widget m_Outline;

	private readonly MilMo_Texture m_FilledTexture;

	private readonly MilMo_Texture m_EmptyTexture;

	public MilMoVotingPoint(MilMo_UserInterface ui, Vector2 position, string outlinePath, string filledPath, string emptyPath)
		: base(ui)
	{
		SetScale(25f, 25f);
		m_FilledTexture = new MilMo_Texture("Content/GUI/" + filledPath);
		m_FilledTexture.AsyncLoad();
		m_EmptyTexture = new MilMo_Texture("Content/GUI/" + emptyPath);
		m_EmptyTexture.AsyncLoad();
		SetEnabled(e: true);
		m_Outline = new MilMo_Widget(UI);
		m_Outline.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Outline.SetPosition(0f, 0f);
		m_Outline.AllowPointerFocus = false;
		m_Outline.SetScale(Scale);
		m_Outline.SetTexture(outlinePath);
		AddChild(m_Outline);
		SetPosition(position);
		SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetScale(0f, 0f);
		ScaleTo(25f, 25f);
		SetScalePull(0.05f, 0.05f);
		SetScaleDrag(0.7f, 0.7f);
		SetPosPull(0.05f, 0.05f);
		SetPosDrag(0.8f, 0.8f);
		SetAngle(0f - 25f * MilMo_Utility.Random() - 12.5f);
		Angle(0f);
		Impulse(10f, 0f);
		SetAlpha(0f);
		FadeSpeed = 0.25f;
	}

	public void ActivateOutline()
	{
		m_Outline.SetEnabled(e: true);
	}

	public void DectivateOutline()
	{
		m_Outline.SetEnabled(e: false);
	}

	public void SetFilled()
	{
		SetAllTextures(m_FilledTexture);
		SetDefaultColor(1f, 1f, 1f, 1f);
	}

	public void SetEmpty()
	{
		SetAllTextures(m_EmptyTexture);
		SetDefaultColor(1f, 1f, 1f, 0.65f);
	}
}
