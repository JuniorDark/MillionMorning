using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Utility;

namespace Code.World.GUI;

public sealed class MilMo_LoadingPane : MilMo_Widget
{
	private readonly MilMo_Widget _mLoadingArrow;

	private readonly MilMo_Widget _mLoadingPaneNumber;

	public MilMo_LoadingPane(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "StreamingArrow";
		SetTexture("Batch01/Textures/HUD/LoadingArrowPane");
		SetPosition(150f, 150f);
		SetScale(83f, 83f);
		SetAlignment(MilMo_GUI.Align.CenterCenter);
		SetDefaultColor(0f, 0f, 0f, 1f);
		base.FixedRes = true;
		AllowPointerFocus = false;
		FadeToDefaultColor = false;
		_mLoadingArrow = new MilMo_Widget(UI);
		_mLoadingArrow.SetTexture("Batch01/Textures/World/StreamingArrow");
		_mLoadingArrow.SetScale(64f, 64f);
		_mLoadingArrow.SetPosition(41.5f, 47.5f);
		_mLoadingArrow.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_mLoadingArrow.SetDefaultColor(1f, 1f, 1f, 1f);
		_mLoadingArrow.FixedRes = true;
		_mLoadingArrow.AllowPointerFocus = false;
		_mLoadingArrow.SetFadeSpeed(0.05f);
		_mLoadingArrow.FadeToDefaultColor = false;
		_mLoadingArrow.AngleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Linear);
		_mLoadingArrow.AngleMover.Vel.x = 6f;
		BringToFront(_mLoadingArrow);
		AddChild(_mLoadingArrow);
		_mLoadingPaneNumber = new MilMo_Widget(UI);
		_mLoadingPaneNumber.SetScale(64f, 64f);
		_mLoadingPaneNumber.SetPosition(41.5f, 47.5f);
		_mLoadingPaneNumber.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_mLoadingPaneNumber.SetDefaultColor(1f, 1f, 1f, 1f);
		_mLoadingPaneNumber.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_mLoadingPaneNumber.SetFont(MilMo_GUI.Font.EborgSmall);
		_mLoadingPaneNumber.FixedRes = true;
		_mLoadingPaneNumber.AllowPointerFocus = false;
		_mLoadingPaneNumber.SetFadeSpeed(0.05f);
		_mLoadingPaneNumber.FadeToDefaultColor = false;
		AddChild(_mLoadingPaneNumber);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetScale(64f, 64f);
		milMo_Widget.SetPosition(41.5f, 47.5f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Widget.SetColor(1f, 1f, 1f, 1f);
		milMo_Widget.SetDefaultTextColor(1f, 1f, 1f, 1f);
		milMo_Widget.SetFont(MilMo_GUI.Font.GothamSmall);
		milMo_Widget.SetFontScale(0.8f);
		milMo_Widget.FixedRes = true;
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.SetFadeSpeed(0.05f);
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetText(MilMo_Localization.GetLocString("World_388"));
		milMo_Widget.SetTextOffset(0f, -12f);
		AddChild(milMo_Widget);
	}

	public override void Step()
	{
		SetNumber(MilMo_ResourceManager.Instance.NumberOfLoadingAssets);
		base.Step();
	}

	private void SetNumber(int number)
	{
		_mLoadingPaneNumber.SetTextNoLocalization(number.ToString());
		if (number > 999)
		{
			_mLoadingPaneNumber.SetFontScale(0.7f);
		}
		else if (number > 99)
		{
			_mLoadingPaneNumber.SetFontScale(0.8f);
		}
		if (number < 1)
		{
			SetFadeSpeed(0.015f);
			AlphaTo(0f);
		}
		else
		{
			MilMo_EventSystem.At(1f, delegate
			{
				SetFadeSpeed(0.015f);
				AlphaTo(1f);
			});
		}
		_mLoadingArrow.AngleMover.Vel.x = 6f * CurrentColor.a;
	}
}
