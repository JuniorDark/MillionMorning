using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI.Ladder;

internal class jb_RatingVisualization : MilMo_Widget
{
	private readonly MilMo_Widget m_NumVotesText;

	private readonly float m_Score;

	private readonly List<jb_RatingPoint> m_Points = new List<jb_RatingPoint>();

	public jb_RatingVisualization(MilMo_UserInterface ui, float score, int max, int numVotes, string filledPath, string emptyPath, Vector2 position)
		: base(ui)
	{
		jb_RatingVisualization jb_RatingVisualization2 = this;
		SetPosition(position);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Score = score;
		SetEnabled(e: true);
		m_NumVotesText = new MilMo_Widget(UI);
		m_NumVotesText.SetFont(MilMo_GUI.Font.GothamSmall);
		m_NumVotesText.SetExtraDrawTextSize(250f, 0f);
		m_NumVotesText.SetAlignment(MilMo_GUI.Align.CenterLeft);
		m_NumVotesText.SetPosition(Vector2.zero);
		m_NumVotesText.TextOffset = new Vector2(35f, 10f);
		m_NumVotesText.SetEnabled(e: false);
		m_NumVotesText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(m_NumVotesText);
		if (score <= 0f)
		{
			MilMo_LocString locString = MilMo_Localization.GetLocString("Homes_13298");
			locString.SetFormatArgs(numVotes);
			m_NumVotesText.SetText(locString);
			ActivateText(0f);
			return;
		}
		MilMo_LocString locString2 = MilMo_Localization.GetLocString("Homes_13297");
		locString2.SetFormatArgs(Math.Round(m_Score, 2), numVotes);
		m_NumVotesText.SetText(locString2);
		Vector2 zero = Vector2.zero;
		float num = 2f;
		for (int i = 0; i < max; i++)
		{
			Vector2 posTemp = zero;
			MilMo_EventSystem.At(num, delegate
			{
				jb_RatingPoint jb_RatingPoint3 = new jb_RatingPoint(jb_RatingVisualization2.UI, emptyPath, posTemp);
				jb_RatingVisualization2.m_Points.Add(jb_RatingPoint3);
				jb_RatingVisualization2.AddChild(jb_RatingPoint3);
			});
			zero.x += 46f;
			num += 0.3f;
		}
		zero = Vector2.zero;
		num = 2f;
		while (score > 0f)
		{
			float scoreTemp = score;
			Vector2 posTemp = zero;
			MilMo_EventSystem.At(num, delegate
			{
				jb_RatingPoint jb_RatingPoint2 = new jb_RatingPoint(jb_RatingVisualization2.UI, filledPath, posTemp, scoreTemp);
				jb_RatingVisualization2.m_Points.Add(jb_RatingPoint2);
				jb_RatingVisualization2.AddChild(jb_RatingPoint2);
			});
			zero.x += 46f;
			score -= 1f;
			num += 0.3f;
		}
		ActivateText(num);
	}

	private void ActivateText(float delay)
	{
		MilMo_EventSystem.At(delay + 0.05f, delegate
		{
			m_NumVotesText.SetEnabled(e: true);
			m_NumVotesText.SetScale(0f, 0f);
			m_NumVotesText.ScaleTo(50f, 50f);
			m_NumVotesText.SetScalePull(0.05f, 0.05f);
			m_NumVotesText.SetScaleDrag(0.3f, 0.3f);
			m_NumVotesText.SetPosPull(0.05f, 0.05f);
			m_NumVotesText.SetPosDrag(0.8f, 0.8f);
			m_NumVotesText.SetAngle(0f - 25f * MilMo_Utility.Random() - 12.5f);
			m_NumVotesText.Angle(0f);
			m_NumVotesText.FadeSpeed = 0.05f;
			m_NumVotesText.Impulse(10f, 0f);
		});
	}
}
