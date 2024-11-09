using System.Linq;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Network;
using Code.Core.ResourceSystem;
using Core;
using UnityEngine;

namespace Code.Apps.Fade;

public class MilMo_Fade
{
	private static MilMo_Fade _fade;

	private static MilMo_GenericReaction _fixedUpdate;

	private static MilMo_GenericReaction _renderUpdate;

	private const float RESOURCE_MANAGER_STOP_TIME = 0.4f;

	private const float DEFAULT_FADE_SPEED = 0.05f;

	private MilMo_Widget _background;

	private const string BACKGROUND_IDENTIFIER = "<!Background!>";

	private bool _delayedWidgetFade;

	private bool _hasDoneFirstFadeout;

	public MilMo_UserInterface UserInterface { get; private set; }

	public static MilMo_Fade Instance
	{
		get
		{
			if (_fade == null)
			{
				Create();
			}
			return _fade;
		}
	}

	private static void Create()
	{
		if (_fade == null)
		{
			_fade = new MilMo_Fade
			{
				UserInterface = MilMo_UserInterfaceManager.CreateUserInterface("Fade")
			};
			_fade.UserInterface.ResetLayout();
			MilMo_UserInterfaceManager.SetUserInterfaceDepth(_fade.UserInterface, -1000);
			MilMo_EventSystem.UnregisterFixedUpdate(_fixedUpdate);
			_fixedUpdate = MilMo_EventSystem.RegisterFixedUpdate(_fade.FixedUpdate);
			MilMo_EventSystem.UnregisterPreRender(_renderUpdate);
			_renderUpdate = MilMo_EventSystem.RegisterPreRender(OnPreRender);
			_fade._background = new MilMo_Widget(_fade.UserInterface)
			{
				Identifier = "<!Background!>"
			};
			_fade._background.SetAlignment(MilMo_GUI.Align.TopLeft);
			_fade._background.SetTexture("Batch01/Textures/Core/White");
			_fade._background.SetFadeSpeed(0.05f);
			_fade._background.Enabled = false;
			_fade._background.IgnoreGlobalFade = true;
			_fade._background.AllowPointerFocus = false;
			_fade.UserInterface.AddChild(_fade._background);
			RefreshUI();
		}
	}

	private void FixedUpdate(object obj)
	{
		MilMo_GUI.GlobalFade = 1f - _background.CurrentColor.a;
		if (_delayedWidgetFade && _background.CurrentColor.a != 1f)
		{
			_delayedWidgetFade = false;
			FadeInWidgets();
		}
	}

	private static void RefreshUI()
	{
		_fade._background.SetPosition(0f, 0f);
		_fade._background.SetScale(Screen.width, Screen.height);
	}

	private static void OnPreRender(object obj)
	{
		if (_fade.UserInterface.ScreenSizeDirty)
		{
			RefreshUI();
		}
	}

	public void FadeInBackground(Color fadeToColor, bool delayFadeWidgets, bool resetAlpha = true)
	{
		_background.SetDefaultColor(fadeToColor);
		_background.SetColor(fadeToColor.r, fadeToColor.g, fadeToColor.b, resetAlpha ? 0f : _background.CurrentColor.a);
		_background.Enabled = true;
		_delayedWidgetFade = delayFadeWidgets;
		MilMo_ResourceManager.Instance.Stop();
		MilMo_EventSystem.At(0.4f, MilMo_ResourceManager.Instance.Resume);
	}

	public void FadeInBackground(bool delayFadeWidgets, bool resetAlpha)
	{
		FadeInBackground(Color.black, delayFadeWidgets, resetAlpha);
	}

	public void FadeInBackground()
	{
		FadeInBackground(Color.black, delayFadeWidgets: false);
	}

	public void FadeOutBackground()
	{
		Color defaultColor = _background.DefaultColor;
		_background.SetDefaultColor(defaultColor.r, defaultColor.g, defaultColor.b, 0f);
		_background.SetColor(defaultColor.r, defaultColor.g, defaultColor.b, 1f);
		MilMo_ResourceManager.Instance.Stop();
		MilMo_EventSystem.At(0.4f, MilMo_ResourceManager.Instance.Resume);
	}

	private void FadeInWidgets()
	{
		foreach (MilMo_Widget item in UserInterface.Children.Where((MilMo_Widget widget) => widget.Identifier != "<!Background!>"))
		{
			item.AlphaTo(1f);
			item.Enabled = true;
		}
	}

	private void FadeOutWidgets()
	{
		foreach (MilMo_Widget item in UserInterface.Children.Where((MilMo_Widget widget) => widget.Identifier != "<!Background!>"))
		{
			item.AlphaTo(0f);
		}
	}

	public void FadeInAll()
	{
		FadeInBackground();
		FadeInWidgets();
	}

	public void FadeOutAll()
	{
		FadeOutBackground();
		FadeOutWidgets();
		if (!_hasDoneFirstFadeout)
		{
			Singleton<GameNetwork>.Instance.SendLoadAndLoginTimeReport();
			_hasDoneFirstFadeout = true;
		}
	}
}
