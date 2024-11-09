using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Utility;

namespace Code.World.GUI;

public class MilMo_LoadingStartup : MilMo_Widget
{
	private MilMo_Widget _rotatingArrow;

	public MilMo_LoadingStartup(MilMo_UserInterface ui)
		: base(ui)
	{
		Initialize();
	}

	private void Initialize()
	{
		SetFont(MilMo_GUI.Font.EborgMedium);
		SetTextAlignment(MilMo_GUI.Align.TopCenter);
		SetAlignment(MilMo_GUI.Align.CenterCenter);
		SetText(MilMo_Localization.GetLocString("LoginScreen_11"));
		_rotatingArrow = new MilMo_Widget(UI);
		_rotatingArrow.SetTexture("Batch01/Textures/World/StreamingArrow");
		_rotatingArrow.SetScale(64f, 64f);
		_rotatingArrow.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_rotatingArrow.SetDefaultColor(1f, 1f, 1f, 1f);
		_rotatingArrow.FixedRes = true;
		_rotatingArrow.AllowPointerFocus = false;
		_rotatingArrow.SetFadeSpeed(0.05f);
		_rotatingArrow.FadeToDefaultColor = false;
		_rotatingArrow.AngleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Linear);
		_rotatingArrow.AngleMover.Vel.x = 6f;
		BringToFront(_rotatingArrow);
		AddChild(_rotatingArrow);
		RefreshUI();
	}

	public void RefreshUI()
	{
		SetScale(300f, 100f);
		_rotatingArrow.SetPosition(Scale.x * 0.5f, 60f);
	}
}
