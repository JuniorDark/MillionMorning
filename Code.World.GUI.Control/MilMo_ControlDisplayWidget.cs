using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Control;

public sealed class MilMo_ControlDisplayWidget : MilMo_Window
{
	private readonly MilMo_MoveControlWidget _mMoveControlWidget;

	private readonly MilMo_FightControlWidget _mFightControlWidget;

	private readonly MilMo_QuestControlWidget _mQuestControlWidget;

	private readonly MilMo_SocialControlWidget _mSocialControlWidget;

	private readonly MilMo_CameraControlWidget _mCameraControlWidget;

	private readonly MilMo_MouseControlWidget _mMouseControlWidget;

	private MilMo_Button _mExitButton;

	public MilMo_ControlDisplayWidget(MilMo_UserInterface ui)
		: base(ui)
	{
		SetupMainWindow();
		_mMoveControlWidget = new MilMo_MoveControlWidget(UI);
		AddChild(_mMoveControlWidget);
		_mFightControlWidget = new MilMo_FightControlWidget(UI);
		AddChild(_mFightControlWidget);
		_mQuestControlWidget = new MilMo_QuestControlWidget(UI);
		AddChild(_mQuestControlWidget);
		_mSocialControlWidget = new MilMo_SocialControlWidget(UI);
		AddChild(_mSocialControlWidget);
		_mCameraControlWidget = new MilMo_CameraControlWidget(UI);
		AddChild(_mCameraControlWidget);
		_mMouseControlWidget = new MilMo_MouseControlWidget(UI);
		AddChild(_mMouseControlWidget);
		CreateLabel();
		CreateExitButton();
	}

	public override void Open()
	{
		if (!IsEnabled())
		{
			SetEnabled(e: true);
			SpawnPos = new Vector2((float)Screen.width / 6f, (float)Screen.height / 6f);
			TargetPos = SpawnPos;
			SetChildPositions();
			SetAlpha(0f);
			AlphaTo(1f);
			BringToFront(_mExitButton);
			base.Open();
		}
	}

	public override void Close(object o)
	{
		UI.ResetLayout(10f, 10f);
		GoTo((float)Screen.width / 2f - UI.GlobalInputOffset.x + 50f, -500f);
		SetEnabled(e: false);
	}

	private void SetupMainWindow()
	{
		Identifier = "ControlDisplay";
		SetScale(750f, 500f);
		SpawnScale = Scale;
		TargetScale = Scale;
		ExitScale = Scale;
		SetText(null);
		SetEnabled(e: false);
		SetFadeSpeed(0.2f);
		SetAlpha(1f);
		SetColor(Color.black);
		SetDefaultColor(Color.black);
		SetSkin(2);
	}

	private void CreateLabel()
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetFont(MilMo_GUI.Font.GothamLarge);
		milMo_Widget.SetFontScale(1.3f);
		milMo_Widget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget.SetScale(200f, 30f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetPosition(30f, 10f);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetWordWrap(w: true);
		milMo_Widget.SetText(MilMo_Localization.GetLocString("Options_9189"));
		AddChild(milMo_Widget);
	}

	private void CreateExitButton()
	{
		_mExitButton = new MilMo_Button(UI);
		_mExitButton.SetAllTextures("Batch01/Textures/World/CloseButton");
		_mExitButton.SetHoverTexture("Batch01/Textures/World/CloseButtonMO");
		_mExitButton.SetPosition(Scale.x - 10f, 5f);
		_mExitButton.SetScale(32f, 32f);
		_mExitButton.SetAlignment(MilMo_GUI.Align.TopRight);
		_mExitButton.Function = delegate
		{
			Close(null);
		};
		AddChild(_mExitButton);
	}

	private void SetChildPositions()
	{
		_mMoveControlWidget.SetPosition(15f, 40f);
		_mFightControlWidget.SetPosition(_mMoveControlWidget.Pos.x + _mMoveControlWidget.Scale.x + 10f, 40f);
		_mCameraControlWidget.SetPosition(15f, 300f);
		_mQuestControlWidget.SetPosition(_mCameraControlWidget.Pos.x + _mCameraControlWidget.Scale.x + 10f, 300f);
		_mSocialControlWidget.SetPosition(_mQuestControlWidget.Pos.x + _mQuestControlWidget.Scale.x + 60f, 300f);
		_mMouseControlWidget.SetPosition(_mFightControlWidget.Pos.x + _mFightControlWidget.Scale.x - 40f, 10f);
	}
}
