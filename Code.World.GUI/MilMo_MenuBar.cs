using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Core.GameEvent;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_MenuBar : MilMo_Widget
{
	public readonly MilMo_Button ExitPVPButton;

	public readonly MilMo_Button PvpKillsButton;

	public readonly MilMo_Button PvpDeathsButton;

	public readonly MilMo_Button PvpScoreBoardButton;

	private Vector2 _exitPVPButtonPos;

	private Vector2 _exitPVPButtonStartPos;

	private readonly MilMo_TimerEvent _disableSchedule;

	private bool _isPvp;

	private readonly MilMo_AudioClip _tickSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/CharBuilderTick");

	public MilMo_MenuBar(MilMo_UserInterface ui)
		: base(ui)
	{
		SetTextureInvisible();
		ExitPVPButton = new MilMo_Button(UI);
		ExitPVPButton.Identifier = "Map";
		ExitPVPButton.FixedRes = true;
		ExitPVPButton.SetAlignment(MilMo_GUI.Align.TopCenter);
		ExitPVPButton.SetAllTextures("Batch01/Textures/HUD/IconMenuBack");
		ExitPVPButton.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("PVP_9370"));
		ExitPVPButton.SetDefaultAngle(0f);
		ExitPVPButton.SetHoverAngle(0f);
		ExitPVPButton.SetAngle(0f);
		ExitPVPButton.AngleMover.MinVel.x = 0.01f;
		ExitPVPButton.SetAnglePull(0.03f);
		ExitPVPButton.SetAngleDrag(0.9f);
		ExitPVPButton.SetScalePull(0.07f, 0.07f);
		ExitPVPButton.SetScaleDrag(0.6f, 0.6f);
		ExitPVPButton.SetPosPull(0f, 0.02f);
		ExitPVPButton.SetPosDrag(0f, 0.9f);
		ExitPVPButton.PosMover.MinVel = new Vector2(0.0001845f, 0.0001845f);
		ExitPVPButton.SetFadeSpeed(0.02f);
		ExitPVPButton.SetFadeInSpeed(0.02f);
		ExitPVPButton.SetFadeOutSpeed(0.02f);
		ExitPVPButton.Function = delegate
		{
			GameEvent.OpenTownEvent.RaiseEvent();
		};
		ExitPVPButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		ExitPVPButton.SetExtraScaleOnHover(5f, 5f);
		ExitPVPButton.SetEnabled(e: false);
		ExitPVPButton.SetHoverSound(_tickSound);
		UI.AddChild(ExitPVPButton);
		PvpKillsButton = new MilMo_Button(UI);
		PvpKillsButton.SetFont(MilMo_GUI.Font.EborgLarge);
		PvpKillsButton.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		PvpKillsButton.SetScale(40f, 40f);
		PvpKillsButton.SetPosPull(0.1f, 0.1f);
		PvpKillsButton.SetPosDrag(0.3f, 0.3f);
		PvpKillsButton.SetScalePull(0.1f, 0.1f);
		PvpKillsButton.SetScaleDrag(0.3f, 0.3f);
		PvpKillsButton.SetAllTextures("Batch01/Textures/HUD/IconPVPKills64");
		PvpKillsButton.SetText(MilMo_Localization.GetNotLocalizedLocString("0"));
		PvpKillsButton.SetAlignment(MilMo_GUI.Align.TopRight);
		PvpKillsButton.SetTextAlignment(MilMo_GUI.Align.TopRight);
		PvpKillsButton.SetExtraDrawTextSize(10f, 10f);
		PvpKillsButton.SetTextOffset(20f, 4f);
		PvpKillsButton.SetEnabled(e: false);
		PvpKillsButton.SetDefaultColor(1f, 1f, 1f, 1f);
		PvpKillsButton.SetFadeSpeed(0.075f);
		UI.AddChild(PvpKillsButton);
		PvpDeathsButton = new MilMo_Button(UI);
		PvpDeathsButton.SetFont(MilMo_GUI.Font.EborgLarge);
		PvpDeathsButton.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		PvpDeathsButton.SetScale(45f, 45f);
		PvpDeathsButton.SetPosPull(0.1f, 0.1f);
		PvpDeathsButton.SetPosDrag(0.3f, 0.3f);
		PvpDeathsButton.SetScalePull(0.1f, 0.1f);
		PvpDeathsButton.SetScaleDrag(0.3f, 0.3f);
		PvpDeathsButton.SetAllTextures("Content/Items/Batch01/SpecialAbilities/IconDamage", prefixStandardGuiPath: false);
		PvpDeathsButton.SetText(MilMo_Localization.GetNotLocalizedLocString("0"));
		PvpDeathsButton.SetAlignment(MilMo_GUI.Align.TopRight);
		PvpDeathsButton.SetTextAlignment(MilMo_GUI.Align.TopRight);
		PvpDeathsButton.SetExtraDrawTextSize(10f, 10f);
		PvpDeathsButton.SetTextOffset(15f, 6f);
		PvpDeathsButton.SetEnabled(e: false);
		PvpDeathsButton.SetDefaultColor(1f, 1f, 1f, 1f);
		PvpDeathsButton.SetFadeSpeed(0.075f);
		UI.AddChild(PvpDeathsButton);
		PvpScoreBoardButton = new MilMo_Button(UI);
		PvpScoreBoardButton.SetFont(MilMo_GUI.Font.EborgLarge);
		PvpScoreBoardButton.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		PvpScoreBoardButton.SetScale(180f, 45f);
		PvpScoreBoardButton.SetPosPull(0.1f, 0.1f);
		PvpScoreBoardButton.SetPosDrag(0.3f, 0.3f);
		PvpScoreBoardButton.SetScalePull(0.1f, 0.1f);
		PvpScoreBoardButton.SetScaleDrag(0.3f, 0.3f);
		ToggledScoreBoard(open: false);
		PvpScoreBoardButton.SetAlignment(MilMo_GUI.Align.TopCenter);
		PvpScoreBoardButton.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		PvpScoreBoardButton.SetExtraDrawTextSize(10f, 10f);
		PvpScoreBoardButton.SetTextOffset(0f, 0f);
		PvpScoreBoardButton.SetEnabled(e: false);
		PvpScoreBoardButton.SetDefaultColor(1f, 1f, 1f, 1f);
		PvpScoreBoardButton.SetFadeSpeed(0.075f);
		PvpScoreBoardButton.PointerHoverFunction = delegate
		{
			PvpScoreBoardButton.SetFontScale(1.1f);
		};
		PvpScoreBoardButton.PointerLeaveFunction = delegate
		{
			PvpScoreBoardButton.SetFontScale(1f);
		};
		PvpScoreBoardButton.Function = delegate
		{
			MilMo_World.PvpScoreBoard.Toggle();
		};
		UI.AddChild(PvpScoreBoardButton);
	}

	public void ToggledScoreBoard(bool open)
	{
		MilMo_LocString copy = MilMo_Localization.GetLocString("PVP_9400").GetCopy();
		copy.SetFormatArgs(open ? "▲" : "▼");
		PvpScoreBoardButton.SetText(copy);
	}

	public void RefreshUI()
	{
		_exitPVPButtonPos = new Vector2(Screen.width - 72, 8f);
		_exitPVPButtonStartPos = new Vector2(Screen.width - 72, -150f);
		ExitPVPButton.SetPosition(_exitPVPButtonPos);
		PvpKillsButton.SetPosition(Screen.width - 330, 16f);
		PvpDeathsButton.SetPosition(Screen.width - 210, 14f);
		PvpScoreBoardButton.SetPosition((float)Screen.width / 2f, 16f);
	}

	public override void Draw()
	{
		if (Enabled)
		{
			if (UI.ScreenSizeDirty)
			{
				RefreshUI();
			}
			base.Draw();
		}
	}

	private void ShowButtons()
	{
		if (!_isPvp)
		{
			return;
		}
		Vector2 margin = new Vector2(50f, 10f);
		float spacing = -10f;
		float num = 0.22f;
		int num2 = 0;
		foreach (MilMo_Widget child in base.Children)
		{
			MilMo_Widget but = child;
			int x = num2;
			but.SetPosition(500f, margin.y);
			MilMo_EventSystem.At((float)x * num, delegate
			{
				but.GoTo((float)x * (but.ScaleMover.Target.x / UI.Res.x) + margin.x + spacing, margin.y);
			});
			num2++;
		}
		MilMo_EventSystem.At((float)num2 * num, ShowExitPVPButton);
	}

	private void ShowExitPVPButton()
	{
		ExitPVPButton.SetPosition(_exitPVPButtonStartPos);
		ExitPVPButton.GoTo(_exitPVPButtonPos);
		ExitPVPButton.SetDefaultAngle(0f);
		ExitPVPButton.SetAngle(-180f);
		ExitPVPButton.Angle(0f);
		ExitPVPButton.Enabled = true;
		UI.BringToFront(ExitPVPButton);
	}

	public void SetNormalMode()
	{
		_isPvp = false;
		ExitPVPButton.SetEnabled(e: false);
		PvpKillsButton.SetEnabled(e: false);
		PvpDeathsButton.SetEnabled(e: false);
		PvpScoreBoardButton.SetEnabled(e: false);
		RefreshUI();
	}

	public void SetPvpMode()
	{
		_isPvp = true;
		ExitPVPButton.SetEnabled(e: true);
		PvpKillsButton.SetEnabled(e: true);
		PvpDeathsButton.SetEnabled(e: true);
		PvpScoreBoardButton.SetEnabled(e: true);
		RefreshUI();
	}

	public void UpdatePvpKillCounter(int kills)
	{
		PvpKillsButton.SetText(MilMo_Localization.GetNotLocalizedLocString(kills.ToString()));
		PvpKillsButton.ScaleImpulse(10f, 10f);
		PvpKillsButton.Impulse(0f, -10f);
	}

	public void UpdatePvpDeathCounter(int deaths)
	{
		PvpDeathsButton.SetText(MilMo_Localization.GetNotLocalizedLocString(deaths.ToString()));
		PvpDeathsButton.ScaleImpulse(10f, 10f);
		PvpDeathsButton.Impulse(0f, -10f);
	}

	public void UpdatePvpAliveCounter(int alive)
	{
		PvpDeathsButton.SetAllTextures("Content/GUI/Batch01/Textures/WorldMap/IconMakeOver", prefixStandardGuiPath: false);
		PvpDeathsButton.SetText(MilMo_Localization.GetNotLocalizedLocString(alive.ToString()));
		PvpDeathsButton.ScaleImpulse(10f, 10f);
		PvpDeathsButton.Impulse(0f, -10f);
	}

	public void Open()
	{
		if (_disableSchedule != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_disableSchedule);
		}
		SetEnabled(e: true);
		ShowButtons();
		GoToX(Screen.width);
	}

	public override void Step()
	{
		if (Enabled)
		{
			base.Step();
		}
	}
}
