using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.LoadingScreen;

public sealed class MilMo_PremiumIcon : MilMo_Widget
{
	private Vector2 _position = Vector2.zero;

	private readonly MilMo_Widget _activeIcon;

	private readonly MilMo_Widget _inactiveIcon;

	private readonly MilMo_Widget _valueWidget;

	private readonly MilMo_ProgressBar _progressBar;

	private bool _isActive;

	private bool _isActiveIcon;

	public MilMo_PremiumIcon(MilMo_UserInterface ui)
		: base(ui)
	{
		_progressBar = new MilMo_ProgressBar(ui, new Vector2(0f, 0f), 35f, 6f, 1f, new Color(0.17f, 0.17f, 0.17f, 1f), new Color(1f, 1f, 1f, 1f), 0f);
		_progressBar.SetAlignment(MilMo_GUI.Align.TopCenter);
		_progressBar.SetFadeSpeed(0.05f);
		_progressBar.UseParentAlpha = false;
		_progressBar.FixedRes = true;
		_progressBar.IgnoreGlobalFade = true;
		ui.AddChild(_progressBar);
		_activeIcon = new MilMo_Widget(ui);
		_activeIcon.SetScale(64f, 64f);
		_activeIcon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_activeIcon.SetFadeSpeed(0.05f);
		_activeIcon.IgnoreGlobalFade = true;
		_activeIcon.FadeToDefaultColor = false;
		_activeIcon.UseParentAlpha = false;
		_activeIcon.FixedRes = true;
		ui.AddChild(_activeIcon);
		_inactiveIcon = new MilMo_Widget(ui);
		_inactiveIcon.SetScale(64f, 64f);
		_inactiveIcon.IgnoreGlobalFade = true;
		_inactiveIcon.UseParentAlpha = false;
		_inactiveIcon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_inactiveIcon.SetColor(0.17f, 0.17f, 0.17f, 1f);
		_inactiveIcon.SetFadeSpeed(0.05f);
		_inactiveIcon.FadeToDefaultColor = false;
		_inactiveIcon.FixedRes = true;
		ui.AddChild(_inactiveIcon);
		_valueWidget = new MilMo_Widget(ui);
		_valueWidget.SetScale(32f, 32f);
		_valueWidget.SetFadeSpeed(0.05f);
		_valueWidget.SetFontScale(0.8f);
		_valueWidget.SetAlignment(MilMo_GUI.Align.TopCenter);
		_valueWidget.SetTextAlignment(MilMo_GUI.Align.BottomRight);
		_valueWidget.SetTextOffset(-5f, 6f);
		_valueWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		_valueWidget.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_valueWidget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_valueWidget.FadeToDefaultColor = false;
		_valueWidget.IgnoreGlobalFade = true;
		_valueWidget.UseParentAlpha = false;
		_valueWidget.FixedRes = true;
		ui.AddChild(_valueWidget);
		SetInactive();
	}

	public void SetActive(bool isActive, int val, float progress)
	{
		SetInactive();
		_isActiveIcon = isActive;
		RefreshUI();
		if (isActive)
		{
			LoadAndSetActiveIconAsync((Mathf.Clamp(val - 15, 0, int.MaxValue) / 5) switch
			{
				0 => "Content/GUI/Batch01/Textures/HUD/IconPremiumBlue64", 
				1 => "Content/GUI/Batch01/Textures/HUD/IconPremiumGreen64", 
				2 => "Content/GUI/Batch01/Textures/HUD/IconPremiumYellow64", 
				_ => "Content/GUI/Batch01/Textures/HUD/IconPremiumRed64", 
			});
		}
		else
		{
			LoadAndSetInactiveIconAsync(progress);
		}
	}

	private async void LoadAndSetInactiveIconAsync(float progress)
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/HUD/IconPremiumInactive");
		_inactiveIcon.SetTexture(texture);
		_progressBar.CurrentProgress = progress;
	}

	private async void LoadAndSetActiveIconAsync(string path)
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(path);
		_activeIcon.SetTexture(texture);
	}

	public void SetInactive()
	{
		_activeIcon.SetPosition(-500f, -500f);
		_inactiveIcon.SetPosition(-500f, -500f);
		_valueWidget.SetPosition(-500f, -500f);
		_progressBar.SetPosition(-500f, -500f);
	}

	private void RefreshUI()
	{
		_position = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f + 138f);
		if (_isActiveIcon)
		{
			_activeIcon.SetPosition(_position);
			_valueWidget.SetPosition(_position.x + 30f, _position.y + 37f - 10f - 28f);
		}
		else
		{
			_inactiveIcon.SetPosition(_position);
			_progressBar.SetPosition(_position.x, _position.y + 37f);
		}
	}

	public override void Draw()
	{
		if (UI.ScreenSizeDirty)
		{
			RefreshUI();
		}
		base.Draw();
	}
}
