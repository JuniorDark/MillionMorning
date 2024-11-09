using System;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class jb_PVPZoneCountdown : MilMo_Widget
{
	private long captureTimeStamp;

	private DateTime startOfCountdown;

	private int zoneId;

	public jb_PVPZoneCountdown(MilMo_UserInterface ui)
		: base(ui)
	{
		UI = ui;
		Identifier = "ZoneCountdown";
		SetAlignment(MilMo_GUI.Align.CenterCenter);
		SetScale(40f, 40f);
		AllowPointerFocus = false;
		SetFont(MilMo_GUI.Font.EborgLarge);
		SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		SetEnabled(e: false);
	}

	public void setCountdown(long captureTimeStamp, int zoneId, Color teamColor)
	{
		this.captureTimeStamp = captureTimeStamp;
		this.zoneId = zoneId;
		SetDefaultTextColor(teamColor);
		SetEnabled(e: true);
	}

	public int getZoneId()
	{
		return zoneId;
	}

	public void RefreshUI()
	{
		SetPosition(Screen.width / 2, Screen.height / 2 - 120);
	}

	public override void Draw()
	{
		if (Enabled)
		{
			int num = Convert.ToInt32(Math.Floor(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(captureTimeStamp).ToLocalTime().Subtract(DateTime.Now)
				.TotalSeconds));
				if (num < 0)
				{
					SetEnabled(e: false);
				}
				else
				{
					SetText(MilMo_Localization.GetNotLocalizedLocString(num.ToString()));
				}
				base.Draw();
			}
		}
	}
