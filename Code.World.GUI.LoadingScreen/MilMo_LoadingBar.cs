using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.LoadingScreen;

public sealed class MilMo_LoadingBar : MilMo_Widget
{
	private readonly MilMo_Widget _progressRect;

	private readonly MilMo_Widget _progressPoint;

	private float _currentProgress;

	public float CurrentProgress
	{
		get
		{
			return _currentProgress;
		}
		set
		{
			_currentProgress = Mathf.Clamp01(value);
			_progressRect.ScaleTo(304f * _currentProgress, 14f);
		}
	}

	public MilMo_LoadingBar(MilMo_UserInterface ui)
		: base(ui)
	{
		SetAlignment(MilMo_GUI.Align.CenterLeft);
		SetScale(0f, 0f);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetTexture(new MilMo_Texture("LoadingScreen/Bar", loadLocal: true));
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget.SetPosition(0f, 0f);
		milMo_Widget.SetScale(304f, 14f);
		milMo_Widget.SetFadeSpeed(0.05f);
		AddChild(milMo_Widget);
		_progressRect = new MilMo_Widget(UI);
		_progressRect.SetTexture("Batch01/Textures/Core/White");
		_progressRect.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_progressRect.SetPosition(2f, 0f);
		_progressRect.SetScale(304f * _currentProgress, 14f);
		_progressRect.SetFadeSpeed(0.05f);
		_progressRect.SetScaleDrag(0.5f, 0.5f);
		AddChild(_progressRect);
		_progressPoint = new MilMo_Widget(UI);
		_progressPoint.SetTexture(new MilMo_Texture("LoadingScreen/BarPoint", loadLocal: true));
		_progressPoint.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_progressPoint.SetPosition(0f, 0f);
		_progressPoint.SetScale(14f, 14f);
		_progressPoint.SetFadeSpeed(0.05f);
		_progressPoint.SetScaleDrag(0.5f, 0.5f);
		AddChild(_progressPoint);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.SetTexture(new MilMo_Texture("LoadingScreen/BarFrame", loadLocal: true));
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget2.SetPosition(0f, 0f);
		milMo_Widget2.SetScale(14f, 15f);
		milMo_Widget2.SetFadeSpeed(0.05f);
		AddChild(milMo_Widget2);
		MilMo_Widget milMo_Widget3 = new MilMo_Widget(UI);
		milMo_Widget3.SetTexture(new MilMo_Texture("LoadingScreen/BarFrame", loadLocal: true));
		milMo_Widget3.SetAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget3.SetPosition(304f, 0f);
		milMo_Widget3.SetScale(-14f, 15f);
		milMo_Widget3.SetFadeSpeed(0.05f);
		AddChild(milMo_Widget3);
		MilMo_Widget milMo_Widget4 = new MilMo_Widget(UI);
		milMo_Widget4.SetTexture("Batch01/Textures/Core/Black");
		milMo_Widget4.SetAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget4.SetPosition(304f, 0f);
		milMo_Widget4.SetScale(14f, 15f);
		milMo_Widget4.SetFadeSpeed(0.05f);
		AddChild(milMo_Widget4);
		IgnoreGlobalFade = true;
		RefreshUI();
	}

	private void RefreshUI()
	{
		Vector2 progressBarCenterOffset = MilMo_LoadingScreenConf.ProgressBarCenterOffset;
		Vector2 position = new Vector2((float)(Screen.width / 2) + progressBarCenterOffset.x - 152f, (float)(Screen.height / 2) + progressBarCenterOffset.y);
		SetPosition(position);
	}

	public override void Draw()
	{
		if (UI.ScreenSizeDirty)
		{
			RefreshUI();
		}
		if (MilMo_GUI.GlobalFade == 0f)
		{
			float a = _progressRect.ScaleMover.Val.x / base.Res.x;
			a = Mathf.Min(a, 304f);
			_progressPoint.SetPosition(a, 0f);
			base.Draw();
		}
	}

	public override void Step()
	{
		if (MilMo_GUI.GlobalFade == 0f)
		{
			base.Step();
		}
	}
}
