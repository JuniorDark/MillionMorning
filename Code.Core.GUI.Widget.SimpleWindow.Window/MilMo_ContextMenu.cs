using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.GUI.Widget.SimpleWindow.Window;

public class MilMo_ContextMenu : MilMo_Window
{
	private const float DEFAULT_WIDTH = 195f;

	private readonly float _spawnTime;

	private float _closeTimer;

	private const int ADDED_WIDGET = -99999;

	private bool _allowClose;

	private bool _shouldClose;

	protected MilMo_ContextMenu(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "ContextMenu";
		UI = ui;
		base.FixedRes = true;
		base.Draggable = false;
		HasCloseButton = false;
		base.Text = MilMo_LocString.Empty;
		SetScalePull(0f, 0.1f);
		SetScaleDrag(0f, 0.7f);
		SpawnScale = new Vector2(195f, 0f);
		TargetScale = new Vector2(195f, 200f);
		ExitScale = new Vector2(195f, 0f);
		SpawnPos = new Vector2(MilMo_Pointer.Position.x - UI.GlobalPosOffset.x, MilMo_Pointer.Position.y - UI.GlobalPosOffset.y);
		TargetPos = SpawnPos;
		_spawnTime = Time.time;
		UI.ResetLayout(0f, 0f, this);
		UI.SetNext(12f, 5f);
		MilMo_EventSystem.At(0.1f, delegate
		{
			_allowClose = true;
		});
	}

	protected void AddLabel(MilMo_LocString text)
	{
		UI.BypassResolution();
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Button.Info = -99999;
		milMo_Button.FixedRes = true;
		milMo_Button.SetScale(195f, 30f);
		milMo_Button.SetPosition(milMo_Button.ScaleMover.Target.x / 2f, 5f);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Button.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Button.SetTextDropShadowPos(2f, 2f);
		milMo_Button.SetTextOutline(1f, 1f);
		milMo_Button.TextOutlineColor = new Color(0f, 0f, 0f, 0.25f);
		milMo_Button.SetText(text);
		AddChild(milMo_Button);
		RefreshSize();
		UI.RestoreResolution();
	}

	protected MilMo_Button AddButton(string texture, MilMo_LocString text)
	{
		UI.BypassResolution();
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.FixedRes = true;
		milMo_Widget.SetTexture(texture);
		milMo_Widget.SetPosition(24f, UI.Next.y + 12f);
		milMo_Widget.SetScale(24f, 24f);
		AddChild(milMo_Widget);
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Button.Info = -99999;
		milMo_Button.FixedRes = true;
		milMo_Button.SetPosition(UI.Next.x - 8f, UI.Same.y - 12f);
		milMo_Button.SetScale(150f, 28f);
		milMo_Button.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		milMo_Button.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		milMo_Button.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		milMo_Button.SetDefaultColor(1f, 1f, 1f, 0.8f);
		milMo_Button.SetTextOffset(-3f, -3f);
		milMo_Button.SetFontScale(0.8f);
		milMo_Button.SetFadeInSpeed(0.1f);
		milMo_Button.SetFadeOutSpeed(0.1f);
		milMo_Button.SetFadeSpeed(0.1f);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.SetText(text);
		AddChild(milMo_Button);
		RefreshSize();
		UI.RestoreResolution();
		return milMo_Button;
	}

	private void RefreshSize()
	{
		TargetScale = new Vector2(195f, 0f);
		for (int num = base.Children.Count - 1; num >= 0; num--)
		{
			MilMo_Widget milMo_Widget = base.Children[num];
			if (milMo_Widget.Info == -99999)
			{
				Vector2 zero = Vector2.zero;
				zero.x = milMo_Widget.PosMover.Target.x + milMo_Widget.ScaleMover.Target.x * (1f - milMo_Widget.Align.x);
				if (zero.x > TargetScale.x)
				{
					TargetScale = new Vector2(zero.x, TargetScale.y);
				}
				zero.y = milMo_Widget.PosMover.Target.y + milMo_Widget.ScaleMover.Target.y * (1f - milMo_Widget.Align.y);
				if (zero.y > TargetScale.y)
				{
					TargetScale = new Vector2(TargetScale.x, zero.y);
				}
			}
		}
		TargetScale = new Vector2(TargetScale.x + 12f, TargetScale.y + 20f);
		SpawnScale = new Vector2(TargetScale.x, 0f);
		ExitScale = SpawnScale;
	}

	public override void Step()
	{
		base.Step();
		if (_shouldClose && Time.time - _closeTimer > 0.05f)
		{
			Close(null);
			_shouldClose = false;
		}
		else if (Time.time - _spawnTime > 0.5f && ShouldClose())
		{
			_closeTimer = Time.time;
			_shouldClose = true;
		}
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		if (ScaleMover.Val.x < 38f || ScaleMover.Val.y < 38f)
		{
			SetAlpha(0f);
			for (int num = base.Children.Count - 1; num >= 0; num--)
			{
				base.Children[num].SetAlpha(0f);
			}
		}
		else
		{
			SetAlpha(1f);
			for (int num2 = base.Children.Count - 1; num2 >= 0; num2--)
			{
				base.Children[num2].SetAlpha(1f);
			}
		}
		base.Draw();
	}

	public override void Open()
	{
		RefreshSize();
		SpawnPos = new Vector2(MilMo_Pointer.Position.x - UI.GlobalPosOffset.x - Scale.x / 2f, MilMo_Pointer.Position.y - UI.GlobalPosOffset.y);
		TargetPos = SpawnPos;
		base.Open();
	}

	public override void Close(object o)
	{
		MilMo_EventSystem.At(0.1f, delegate
		{
			SetEnabled(e: false);
			UI.RemoveChild(this);
		});
	}

	private bool ShouldClose()
	{
		if (_allowClose)
		{
			if (!MilMo_Pointer.LeftClick)
			{
				return MilMo_Pointer.LeftButton;
			}
			return true;
		}
		return false;
	}
}
