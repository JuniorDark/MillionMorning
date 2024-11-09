using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.World.Level.PVP;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class jb_PVPTeamScore : MilMo_Widget
{
	private new readonly MilMo_UserInterface UI;

	private readonly List<jb_PVPTeamScoreFieldLite> m_ScoreFields;

	public jb_PVPTeamScore(MilMo_UserInterface ui)
		: base(ui)
	{
		UI = ui;
		Identifier = "TeamScore";
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetScale(340f, 0f);
		AllowPointerFocus = false;
		m_ScoreFields = new List<jb_PVPTeamScoreFieldLite>();
		SetEnabled(e: false);
	}

	public void setScore(IList<MilMo_PVPTeam> teams)
	{
		UI.ResetLayout();
		UI.SetNext(15f, 1f);
		RemoveAllChildren();
		m_ScoreFields.Clear();
		jb_PVPTeamScoreFieldLite jb_PVPTeamScoreFieldLite2 = new jb_PVPTeamScoreFieldLite(UI, null, isHeader: true);
		AddChild(jb_PVPTeamScoreFieldLite2);
		m_ScoreFields.Add(jb_PVPTeamScoreFieldLite2);
		foreach (MilMo_PVPTeam team in teams)
		{
			jb_PVPTeamScoreFieldLite jb_PVPTeamScoreFieldLite3 = new jb_PVPTeamScoreFieldLite(UI, team, isHeader: false);
			AddChild(jb_PVPTeamScoreFieldLite3);
			m_ScoreFields.Add(jb_PVPTeamScoreFieldLite3);
		}
	}

	public void RefreshUI()
	{
		SetPosition(15f, 80f);
	}

	public override void Draw()
	{
		if (Enabled)
		{
			GUISkin skin = UnityEngine.GUI.skin;
			UnityEngine.GUI.skin = Skin;
			UnityEngine.GUI.color = new Color(1f, 1f, 1f, 0.4f) * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			Rect screenPosition = GetScreenPosition();
			screenPosition.width = Scale.x + 20f;
			screenPosition.height = GetYScale() + 10f;
			UnityEngine.GUI.Box(screenPosition, "");
			UnityEngine.GUI.skin = skin;
			base.Draw();
		}
	}

	private float GetYScale()
	{
		float num = 0f;
		foreach (jb_PVPTeamScoreFieldLite scoreField in m_ScoreFields)
		{
			num += scoreField.Scale.y;
		}
		return num;
	}
}
