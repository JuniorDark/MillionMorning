using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.Button;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.Core.Visual.PostEffects;
using Code.World.GUI;
using Code.World.GUI.HudCounter.Counters;
using Core.Settings;
using Core.State;
using UnityEngine;

namespace Code.World.CharBuilder.MakeOverStudio;

public sealed class MilMo_MakeOverStudioWindow
{
	private readonly MilMo_UserInterface _ui;

	private readonly MilMo_MakeOverStudio _makeOverStudio;

	private int _numPageIcons;

	private const float SLOW_FADE = 0.01f;

	private const float FAST_FADE = 0.06f;

	private const float SMALL = 70f;

	private const float BIG = 100f;

	private readonly Color _winButDefaultColor = new Color(1f, 1f, 1f, 1f);

	private readonly Color _winButMouseOverColor = new Color(1f, 1f, 1f, 1f);

	private const int PHYSIQUE = 0;

	private const int EYES = 1;

	private const int MOUTH = 2;

	private const int HAIR = 3;

	private const int SHIRT = 4;

	private const int PANTS = 5;

	private const int SHOES = 6;

	private const int NAME = 7;

	private MilMo_Button _buyButton;

	private MilMo_Button _resetButton;

	private MilMo_Button _randomizeButton;

	private MilMo_GemCounter _gemCounterBack;

	private MilMo_Button _skipButton;

	public int ActivePage;

	private int _numPages;

	private MilMo_Widget _menuBar;

	private MilMo_PulseCursor _selectArrow;

	private Color[] _pageColor;

	private readonly List<MilMo_Button> _pageButtons = new List<MilMo_Button>();

	private MilMo_LoadingPane _loadingPane;

	private readonly Vector2 _physiquePageScale = new Vector2(300f, 407f);

	private MilMo_Widget _physiqueCaption;

	public MilMo_Button MaleButton;

	public MilMo_Button FemaleButton;

	private MilMo_SimpleBox _genderLabel;

	private MilMo_SimpleBox _heightLabel;

	public MilMo_ColorBar SkinColorBar;

	public MilMo_SimpleSlider HeightSlider;

	private readonly List<MilMo_Widget> _physiquePage = new List<MilMo_Widget>();

	private readonly Vector2 _eyesPageScale = new Vector2(350f, 590f);

	private const float EYES_BUTTON_HEIGHT = 64f;

	private MilMo_Widget _eyesCaption;

	public readonly MilMo_AvatarButton[] EyesButton = new MilMo_AvatarButton[6];

	public MilMo_ColorBar EyeColorBar;

	public MilMo_ToggleBar EyeBrowBar;

	private readonly List<MilMo_Widget> _eyesPage = new List<MilMo_Widget>();

	private readonly Vector2 _mouthPageScale = new Vector2(340f, 320f);

	private const float MOUTH_BUTTON_HEIGHT = 64f;

	private MilMo_Widget _mouthCaption;

	public readonly MilMo_AvatarButton[] MouthButton = new MilMo_AvatarButton[6];

	private readonly List<MilMo_Widget> _mouthPage = new List<MilMo_Widget>();

	private readonly Vector2 _hairPageScale = new Vector2(300f, 590f);

	private const float HAIR_BUTTON_HEIGHT = 100f;

	private MilMo_Widget _hairCaption;

	private readonly MilMo_WearableButton[] _hairButton = new MilMo_WearableButton[6];

	public MilMo_ColorBar HairColorBar;

	public readonly List<MilMo_Widget> HairPage = new List<MilMo_Widget>();

	private readonly Vector2 _shirtPageScale = new Vector2(300f, 562f);

	private const float SHIRT_BUTTON_HEIGHT = 109f;

	private MilMo_Widget _shirtCaption;

	private readonly MilMo_WearableButton[] _shirtButton = new MilMo_WearableButton[6];

	public readonly List<MilMo_Widget> ShirtPage = new List<MilMo_Widget>();

	public MilMo_ColorBar ShirtColorBar;

	private readonly Vector2 _pantsPageScale = new Vector2(300f, 562f);

	private const float PANTS_BUTTON_HEIGHT = 109f;

	private MilMo_Widget _pantsCaption;

	private readonly MilMo_WearableButton[] _pantsButton = new MilMo_WearableButton[6];

	public readonly List<MilMo_Widget> PantsPage = new List<MilMo_Widget>();

	public MilMo_ColorBar PantsColorBar;

	private readonly Vector2 _shoesPageScale = new Vector2(300f, 540f);

	private const float SHOES_BUTTON_HEIGHT = 109f;

	private MilMo_Widget _shoesCaption;

	private readonly MilMo_WearableButton[] _shoesButton = new MilMo_WearableButton[6];

	public readonly List<MilMo_Widget> ShoesPage = new List<MilMo_Widget>();

	public MilMo_ColorBar ShoesColorBar;

	public MilMo_ColorBar ShoesColorBar2;

	public MilMo_TimerEvent StartTimerEvent;

	private MilMo_TimerEvent _fadeInTimerEvent;

	private MilMo_TimerEvent _rescaleTimerEvent;

	private float _lastSelectPageSound;

	private readonly Vector2 _eyesButtonScale = new Vector2(134f, 67f);

	private readonly Vector2 _mouthButtonScale = new Vector2(129f, 64f);

	private readonly Vector2 _hairButtonScale = new Vector2(109f, 109f);

	private readonly Vector2 _shirtButtonScale = new Vector2(109f, 109f);

	private readonly Vector2 _pantsButtonScale = new Vector2(109f, 109f);

	private readonly Vector2 _shoesButtonScale = new Vector2(109f, 109f);

	public GameObject MBgCamGameObject;

	public Camera BgCam;

	public MilMo_SimpleBox Window { get; private set; }

	public MilMo_GemCounter AvatarGemCounter { get; private set; }

	public MilMo_GemCounter GemCounter { get; private set; }

	public MilMo_MakeOverStudioWindow(MilMo_UserInterface ui, MilMo_MakeOverStudio cb)
	{
		_ui = ui;
		_makeOverStudio = cb;
	}

	public void RefreshUI()
	{
		float num = -((_makeOverStudio.screenHeight - Screen.height) / 30);
		_skipButton.SetPosition(_makeOverStudio.screenWidth - (144 + ((_makeOverStudio.screenWidth >= 1280) ? 22 : 0)), _makeOverStudio.screenHeight - 113);
		_skipButton.SetScale(136f, 60f);
		SelectPage(ActivePage + 1, playSound: false);
		int num2 = ((_makeOverStudio.screenWidth > 1024) ? ((_makeOverStudio.screenWidth - 1024) / 5) : 0);
		int num3 = ((_makeOverStudio.screenHeight > 720) ? ((_makeOverStudio.screenHeight - 720) / 5) : 0);
		Window.SetPosition(262 + num2, 134 + num3);
		Window.SetScale(_physiquePageScale.x, _physiquePageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_physiqueCaption.SetPosition(_physiquePageScale.x / 2f, _ui.Align.Top - _ui.Padding.x / 3f);
		_physiqueCaption.SetScale(300f, 35f);
		_genderLabel.SetPosition(150f, _ui.Same.y + 32f + _ui.Padding.x / 3f);
		_genderLabel.SetScale(_ui.Width, 72f + _ui.Padding.y / 3f);
		MaleButton.SetPosition(_ui.Align.Left + _ui.Padding.x / 2f + 62f, _ui.Same.y + 32f);
		MaleButton.SetScale(86f, 40f);
		FemaleButton.SetPosition(_ui.Align.Right - _ui.Padding.x / 2f - 62f, _ui.Same.y);
		FemaleButton.SetScale(86f, 40f);
		_heightLabel.SetPosition(150f, _ui.Next.y + 5f);
		_heightLabel.SetScale(_ui.Width, 52f + _ui.Padding.y / 3f);
		HeightSlider.SetPosition(150f, _ui.Same.y + 32f);
		HeightSlider.SetScale(_ui.Width - _ui.Padding.x, 30f);
		SkinColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 5f);
		SkinColorBar.SetScale(_ui.Width, 0f);
		SkinColorBar.SetTextOffset(0f, num);
		Window.SetScale(_eyesPageScale.x, _eyesPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_eyesCaption.SetPosition(_eyesPageScale.x / 2f, _ui.Align.Top - _ui.Padding.x / 3f);
		_eyesCaption.SetScale(400f, 35f);
		int num4 = 0;
		float num5 = _ui.Next.y;
		for (int i = 0; i < 3; i++)
		{
			EyesButton[num4].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num5 + 64f);
			EyesButton[num4].SetScale(_eyesButtonScale);
			num4++;
			EyesButton[num4].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num5 + 64f);
			EyesButton[num4].SetScale(_eyesButtonScale);
			num4++;
			num5 += 64f + _ui.Padding.y;
		}
		EyeColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 5f - 64f);
		EyeColorBar.SetScale(_ui.Width, 0f);
		EyeColorBar.SetTextOffset(0f, num);
		EyeBrowBar.SetPosition(_ui.Align.Left, _ui.Next.y);
		EyeBrowBar.SetScale(_ui.Width, 75f);
		EyeBrowBar.SetTextOffset(0f, num / 2f);
		Window.SetScale(_mouthPageScale.x, _mouthPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_mouthCaption.SetPosition(_mouthPageScale.x / 2f, _ui.Align.Top - _ui.Padding.x / 3f);
		_mouthCaption.SetScale(400f, 35f);
		num4 = 0;
		num5 = _ui.Next.y;
		for (int j = 0; j < 3; j++)
		{
			MouthButton[num4].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num5);
			MouthButton[num4].SetScale(_mouthButtonScale);
			num4++;
			MouthButton[num4].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num5);
			MouthButton[num4].SetScale(_mouthButtonScale);
			num4++;
			num5 += 64f + _ui.Padding.y;
		}
		Window.SetScale(_hairPageScale.x, _hairPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_hairCaption.SetPosition(_hairPageScale.x / 2f, _ui.Align.Top - _ui.Padding.x / 3f);
		_hairCaption.SetScale(400f, 35f);
		num4 = 0;
		num5 = _ui.Next.y;
		for (int k = 0; k < 3; k++)
		{
			_hairButton[num4].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num5 + _ui.Width / 4f - 20f);
			_hairButton[num4].SetScale(_hairButtonScale);
			num4++;
			if (num4 < 4)
			{
				_hairButton[num4].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num5 + _ui.Width / 4f - 20f);
				_hairButton[num4].SetScale(_hairButtonScale);
			}
			num4++;
			num5 += 100f + _ui.Padding.y;
		}
		HairColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 54f);
		HairColorBar.SetScale(_ui.Width, 0f);
		HairColorBar.SetTextOffset(0f, num);
		Window.SetScale(_shirtPageScale.x, _shirtPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_shirtCaption.SetPosition(_shirtPageScale.x / 2f, _ui.Align.Top - _ui.Padding.x / 3f);
		_shirtCaption.SetScale(400f, 35f);
		num4 = 0;
		num5 = _ui.Next.y;
		for (int l = 0; l < 2; l++)
		{
			_shirtButton[num4].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num5 + _ui.Width / 4f);
			_shirtButton[num4].SetScale(_shirtButtonScale);
			num4++;
			if (num4 < 4)
			{
				_shirtButton[num4].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num5 + _ui.Width / 4f);
				_shirtButton[num4].SetScale(_shirtButtonScale);
			}
			num4++;
			num5 += 109f + _ui.Padding.y;
		}
		ShirtColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 44f);
		ShirtColorBar.SetScale(_ui.Width, 0f);
		ShirtColorBar.SetTextOffset(0f, num);
		Window.SetScale(_pantsPageScale.x, _pantsPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_pantsCaption.SetPosition(_pantsPageScale.x / 2f, _ui.Align.Top - _ui.Padding.x / 3f);
		_pantsCaption.SetScale(400f, 35f);
		num4 = 0;
		num5 = _ui.Next.y;
		for (int m = 0; m < 2; m++)
		{
			_pantsButton[num4].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num5 + _ui.Width / 4f);
			_pantsButton[num4].SetScale(_pantsButtonScale);
			num4++;
			if (num4 < 4)
			{
				_pantsButton[num4].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num5 + _ui.Width / 4f);
				_pantsButton[num4].SetScale(_pantsButtonScale);
			}
			num4++;
			num5 += 109f + _ui.Padding.y;
		}
		PantsColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 44f);
		PantsColorBar.SetScale(_ui.Width, 0f);
		PantsColorBar.SetTextOffset(0f, num);
		Window.SetScale(_shoesPageScale.x, _shoesPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_shoesCaption.SetPosition(_shoesPageScale.x / 2f, _ui.Align.Top - _ui.Padding.x / 3f);
		_shoesCaption.SetScale(400f, 35f);
		num4 = 0;
		num5 = _ui.Next.y;
		for (int n = 0; n < 2; n++)
		{
			_shoesButton[num4].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num5 + _ui.Width / 2f);
			_shoesButton[num4].SetScale(_shoesButtonScale);
			num4++;
			if (num4 < 4)
			{
				_shoesButton[num4].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num5 + _ui.Width / 2f);
				_shoesButton[num4].SetScale(_shoesButtonScale);
			}
			num4++;
			num5 += 109f + _ui.Padding.y;
		}
		ShoesColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 109f);
		ShoesColorBar.SetScale(_ui.Width, 0f);
		ShoesColorBar.SetTextOffset(0f, num);
		ShoesColorBar2.SetPosition(_ui.Align.Left, _ui.Next.y - 5f);
		ShoesColorBar2.SetScale(_ui.Width, 0f);
		ShoesColorBar2.SetTextOffset(0f, num);
		Window.SetScale(0f, 0f);
		if (_buyButton != null)
		{
			_buyButton.SetPosition(_makeOverStudio.screenWidth - (144 + ((_makeOverStudio.screenWidth >= 1280) ? 22 : 0)), _makeOverStudio.screenHeight - 340);
			_buyButton.SetScale(132f, 64f);
		}
		if (GemCounter != null)
		{
			Vector2 vector = new Vector2((float)_makeOverStudio.screenWidth - (float)(126 + ((_makeOverStudio.screenWidth >= 1280) ? 22 : 50)) * _ui.Res.x, (float)(_makeOverStudio.screenHeight - 386) * _ui.Res.y);
			GemCounter.SpawnPosition = vector;
			GemCounter.TargetPosition = vector;
			GemCounter.GoToNow(GemCounter.TargetPosition);
		}
		if (AvatarGemCounter != null)
		{
			Vector2 vector2 = new Vector2((float)_makeOverStudio.screenWidth - (float)(144 + ((_makeOverStudio.screenWidth >= 1280) ? 22 : 50)) * _ui.Res.x, 64f / _ui.Res.y);
			AvatarGemCounter.SpawnPosition = vector2;
			AvatarGemCounter.TargetPosition = vector2;
			AvatarGemCounter.GoToNow(AvatarGemCounter.TargetPosition);
		}
		if (_resetButton != null)
		{
			_resetButton.SetPosition(_makeOverStudio.screenWidth - (144 + ((_makeOverStudio.screenWidth >= 1280) ? 22 : 0)), _makeOverStudio.screenHeight - 282);
			_resetButton.SetScale(132f, 64f);
		}
		if (_randomizeButton != null)
		{
			_randomizeButton.SetPosition(_makeOverStudio.screenWidth - (144 + ((_makeOverStudio.screenWidth >= 1280) ? 22 : 0)), _makeOverStudio.screenHeight - 224);
			_randomizeButton.SetScale(132f, 64f);
		}
		if (_loadingPane != null)
		{
			_loadingPane.SetPosition((float)_makeOverStudio.screenWidth * _ui.Res.x - 67f, 173f);
		}
	}

	private static void SetCommonParameters(MilMo_Widget w)
	{
		w.SetAlpha(0f);
		w.FadeToDefaultColor = false;
	}

	private void SetCommonParametersAndAdd(MilMo_Widget w)
	{
		SetCommonParameters(w);
		Window.AddChild(w);
	}

	public void StartCallback()
	{
		Window.SetScale(0f, 0f);
		MilMo_EventSystem.At(0.1f, delegate
		{
			Window.ColorTo(1f, 1f, 1f, 1f);
		});
		SelectPage(1);
		if (_rescaleTimerEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_rescaleTimerEvent);
		}
		DisableAvatarIcons();
		_rescaleTimerEvent = MilMo_EventSystem.At(0.2f, RescaleCallback);
		for (int i = 0; i < _pageButtons.Count; i++)
		{
			MilMo_Button milMo_Button = _pageButtons[i];
			if (milMo_Button != null)
			{
				float num = ((milMo_Button.Info == 1) ? 100f : 70f);
				Vector2 vector = new Vector2(num / _ui.Res.x * ((_ui.Res.x <= _ui.Res.y) ? _ui.Res.x : _ui.Res.y), num / _ui.Res.y * ((_ui.Res.x <= _ui.Res.y) ? _ui.Res.x : _ui.Res.y));
				milMo_Button.SetAlpha(1f);
				milMo_Button.ScaleTo(vector);
				milMo_Button.SetMinScale(vector);
				milMo_Button.SetDefaultScale(vector);
			}
		}
	}

	public void NextPage(object o)
	{
		if (ActivePage + 1 < _numPages)
		{
			SelectPage(ActivePage + 2);
		}
	}

	public void PrevPage(object o)
	{
		if (ActivePage > 0)
		{
			SelectPage(ActivePage);
		}
	}

	public void EnableBuyButton(bool enable)
	{
		if (enable)
		{
			_buyButton.SetAlpha(1f);
			GemCounter.Show();
		}
		else
		{
			_buyButton.SetAlpha(0f);
			GemCounter.Hide();
		}
	}

	public void EnableResetButton(bool enable)
	{
		_resetButton.SetAlpha(enable ? 1 : 0);
	}

	public void UpdatePrice(int price)
	{
		if (price > 0)
		{
			_buyButton.SetAlpha(1f);
			GemCounter.SetAlpha(1f);
		}
		else
		{
			_buyButton.SetAlpha(0f);
			GemCounter.SetAlpha(0f);
		}
		GemCounter.SetPrice(price);
	}

	private void StartFade()
	{
		foreach (MilMo_Widget child in Window.Children)
		{
			child.SetEnabled(e: false);
			child.SetAlpha(0f);
		}
		DisableAvatarIcons();
		if (_rescaleTimerEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_rescaleTimerEvent);
		}
		_rescaleTimerEvent = MilMo_EventSystem.At(0.1f, RescaleCallback);
		_menuBar.SetAlpha(0f);
		_menuBar.GoTo(0f, _makeOverStudio.screenHeight);
		_skipButton.SetAlpha(1f);
		_selectArrow.GoTo((float)ActivePage * 71.75f + (float)(_makeOverStudio.screenWidth / 2 - 320), 50f);
	}

	private void SelectArrowPulseUp()
	{
		_selectArrow.ScaleTo(50f, 50f);
		_selectArrow.ScaleMover.Arrive = SelectArrowPulseDown;
		_selectArrow.ColorTo(1f, 0.95f, 0f, 1f);
	}

	private void SelectArrowPulseDown()
	{
		_selectArrow.ScaleTo(40f, 40f);
		_selectArrow.ScaleMover.Arrive = SelectArrowPulseUp;
		_selectArrow.ColorTo(1f, 0.95f, 0f, 1f);
	}

	private void RescaleCallback()
	{
		switch (ActivePage)
		{
		case 0:
			Window.ScaleTo(_physiquePageScale.x, _physiquePageScale.y);
			break;
		case 1:
			Window.ScaleTo(_eyesPageScale.x, _eyesPageScale.y);
			break;
		case 2:
			Window.ScaleTo(_mouthPageScale.x, _mouthPageScale.y);
			break;
		case 3:
			Window.ScaleTo(_hairPageScale.x, _hairPageScale.y);
			break;
		case 4:
			Window.ScaleTo(_shirtPageScale.x, _shirtPageScale.y);
			break;
		case 5:
			Window.ScaleTo(_pantsPageScale.x, _pantsPageScale.y);
			break;
		case 6:
			Window.ScaleTo(_shoesPageScale.x, _shoesPageScale.y);
			break;
		}
		DisableAvatarIcons();
		foreach (MilMo_Widget child in Window.Children)
		{
			child.SetEnabled(e: false);
		}
		if (_fadeInTimerEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_fadeInTimerEvent);
		}
		_fadeInTimerEvent = MilMo_EventSystem.At(0f, FadeInCallback);
	}

	private void PlayDrawerCloseSound()
	{
		if (_ui.SoundFx.IsPlaying())
		{
			_ui.SoundFx.Stop();
		}
		foreach (MilMo_Widget child in Window.Children)
		{
			SetFadeSpeed(child, 0.06f);
		}
		for (int i = 0; i < 6; i++)
		{
			EyesButton[i].FadeToDefaultColor = true;
			SetFadeSpeed(EyesButton[i], 0.06f);
			MouthButton[i].FadeToDefaultColor = true;
			SetFadeSpeed(MouthButton[i], 0.06f);
			_hairButton[i].FadeToDefaultColor = true;
			SetFadeSpeed(_hairButton[i], 0.06f);
			if (i < 4)
			{
				_shirtButton[i].FadeToDefaultColor = true;
				SetFadeSpeed(_shirtButton[i], 0.06f);
				_pantsButton[i].FadeToDefaultColor = true;
				SetFadeSpeed(_pantsButton[i], 0.06f);
				_shoesButton[i].FadeToDefaultColor = true;
				SetFadeSpeed(_shoesButton[i], 0.06f);
			}
		}
	}

	private void FadeInCallback()
	{
		Window.SetAlpha(1f);
		foreach (MilMo_Widget child in Window.Children)
		{
			child.SetEnabled(e: false);
		}
		switch (ActivePage)
		{
		case 0:
			_physiquePage.ForEach(delegate(MilMo_Widget widget)
			{
				widget.SetEnabled(e: true);
				widget.AlphaTo(1f);
				SetFadeSpeed(widget, 0.01f);
			});
			break;
		case 1:
			_eyesPage.ForEach(delegate(MilMo_Widget widget)
			{
				widget.SetEnabled(e: true);
				widget.AlphaTo(1f);
				SetFadeSpeed(widget, 0.01f);
			});
			EnableAvatarIcons();
			break;
		case 2:
			_mouthPage.ForEach(delegate(MilMo_Widget widget)
			{
				widget.SetEnabled(e: true);
				widget.AlphaTo(1f);
				SetFadeSpeed(widget, 0.01f);
			});
			EnableAvatarIcons();
			break;
		case 3:
			HairPage.ForEach(delegate(MilMo_Widget widget)
			{
				widget.SetEnabled(e: true);
				widget.AlphaTo(1f);
				SetFadeSpeed(widget, 0.01f);
			});
			break;
		case 4:
			ShirtPage.ForEach(delegate(MilMo_Widget widget)
			{
				widget.SetEnabled(e: true);
				widget.AlphaTo(1f);
				SetFadeSpeed(widget, 0.01f);
			});
			break;
		case 5:
			PantsPage.ForEach(delegate(MilMo_Widget widget)
			{
				widget.SetEnabled(e: true);
				widget.AlphaTo(1f);
				SetFadeSpeed(widget, 0.01f);
			});
			break;
		case 6:
			ShoesPage.ForEach(delegate(MilMo_Widget widget)
			{
				widget.SetEnabled(e: true);
				widget.AlphaTo(1f);
				SetFadeSpeed(widget, 0.01f);
			});
			break;
		}
	}

	private void EnableAvatarIcons()
	{
		for (int i = 0; i < _makeOverStudio.AvatarIcons.Length; i++)
		{
			EyesButton[i].AvatarIcon = _makeOverStudio.AvatarIcons[i];
			MouthButton[i].AvatarIcon = _makeOverStudio.AvatarIcons[i];
		}
		MilMo_AvatarIcon[] avatarIcons = _makeOverStudio.AvatarIcons;
		foreach (MilMo_AvatarIcon milMo_AvatarIcon in avatarIcons)
		{
			MilMo_AvatarIcon a = milMo_AvatarIcon;
			if (a != null)
			{
				MilMo_EventSystem.At(0.1f, delegate
				{
					a.Enabled = true;
				});
			}
		}
		MilMo_AvatarButton[] eyesButton = EyesButton;
		foreach (MilMo_AvatarButton obj in eyesButton)
		{
			obj.SetScale(0f, 0f);
			obj.ScaleTo(_eyesButtonScale);
		}
		eyesButton = MouthButton;
		foreach (MilMo_AvatarButton obj2 in eyesButton)
		{
			obj2.SetScale(0f, 0f);
			obj2.ScaleTo(_mouthButtonScale);
		}
		MilMo_WearableButton[] hairButton = _hairButton;
		foreach (MilMo_WearableButton obj3 in hairButton)
		{
			obj3.SetScale(0f, 0f);
			obj3.ScaleTo(_hairButtonScale);
		}
		_hairButton[5].SetScale(0f, 0f);
	}

	private void DisableAvatarIcons()
	{
		MilMo_AvatarIcon[] avatarIcons = _makeOverStudio.AvatarIcons;
		foreach (MilMo_AvatarIcon milMo_AvatarIcon in avatarIcons)
		{
			if (milMo_AvatarIcon != null)
			{
				milMo_AvatarIcon.Enabled = false;
			}
		}
	}

	private static void SetFadeSpeed(MilMo_Widget w, float speed)
	{
		w.SetFadeSpeed(speed);
		if (!(w.GetType() != typeof(MilMo_Button)))
		{
			MilMo_Button obj = (MilMo_Button)w;
			obj.SetFadeOutSpeed(speed);
			obj.SetFadeInSpeed(speed);
		}
	}

	public void InitUI()
	{
		_pageColor = new Color[7];
		_pageColor[0] = new Color(0.7f, 0.7f, 1f, 0.9f);
		_pageColor[1] = new Color(0.5f, 0.9f, 0.5f, 0.9f);
		_pageColor[2] = new Color(1f, 0.8f, 0.3f, 0.9f);
		_pageColor[3] = new Color(1f, 0.6f, 0.6f, 0.9f);
		_pageColor[4] = new Color(1f, 1f, 1f, 0.9f);
		_pageColor[5] = new Color(1f, 0.5f, 0.5f, 0.9f);
		_pageColor[6] = new Color(0.9f, 0.7f, 0.5f, 0.9f);
		Window = new MilMo_SimpleBox(_ui);
		int num = ((_makeOverStudio.screenWidth > 1024) ? ((_makeOverStudio.screenWidth - 1024) / 5) : 0);
		int num2 = ((_makeOverStudio.screenHeight > 720) ? ((_makeOverStudio.screenHeight - 720) / 5) : 0);
		Window.SetPosition(262 + num, 134 + num2);
		Window.SetScalePull(0.09f, 0.09f);
		Window.SetScaleDrag(0.6f, 0.6f);
		Window.SetAlpha(0f);
		Window.FadeToDefaultColor = false;
		Window.ScaleMover.MinVel.x = 0.001f;
		Window.ScaleMover.MinVel.y = 0.001f;
		Window.ScaleMover.Arrive = PlayDrawerCloseSound;
		Window.SetAlignment(MilMo_GUI.Align.TopCenter);
		Window.Skin = MilMo_GUISkins.GetSkin("WindowBox");
		_ui.AddChild(Window);
		MBgCamGameObject = new GameObject("BGCam", typeof(Camera), typeof(MilMo_ColorOverlay));
		BgCam = MBgCamGameObject.GetComponent<Camera>();
		BgCam.clearFlags = CameraClearFlags.Nothing;
		BgCam.fieldOfView = 12f;
		BgCam.depth = 10f;
		BgCam.rect = new Rect(0.2f, 0.2f, 0.5f, 0.5f);
		BgCam.nearClipPlane = 0.01f;
		BgCam.farClipPlane = 8f;
		BgCam.transform.position = new Vector3(1000f, 1000f, 1000f);
		BgCam.backgroundColor = new Color(0.14f, 0.24f, 0.5f, 1f);
		BgCam.enabled = true;
		if (Settings.ResolutionWidth <= 1024 && Settings.ResolutionHeight <= 720)
		{
			_ui.ResetLayout(40f, 40f);
		}
		_menuBar = new MilMo_Widget(_ui);
		_menuBar.Identifier = "menubar";
		_menuBar.SetTexture("Batch01/Textures/Shop/MainCaptionBackground");
		_menuBar.SetPosition(0f, (float)_makeOverStudio.screenHeight + 64f);
		_menuBar.SetPosPull(0.24f, 0.24f);
		_menuBar.SetPosDrag(0.6f, 0.6f);
		_menuBar.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_menuBar.SetScale(_makeOverStudio.screenWidth, 64f);
		_menuBar.SetColor(_pageColor[0]);
		_menuBar.PosMover.MinVel.x = 0.1f;
		_menuBar.PosMover.MinVel.y = 0.1f;
		_menuBar.AllowPointerFocus = false;
		SetCommonParameters(_menuBar);
		_ui.AddChild(_menuBar);
		_pageButtons.Add(CreatePageButton("Batch01/Textures/CharBuilder/IconPhysique"));
		_pageButtons.Add(CreatePageButton("Batch01/Textures/CharBuilder/IconEyes"));
		_pageButtons.Add(CreatePageButton("Batch01/Textures/CharBuilder/IconMouth"));
		_pageButtons.Add(CreatePageButton("Batch01/Textures/Shop/IconHair"));
		_pageButtons.Add(CreatePageButton("Batch01/Textures/Shop/IconClothes"));
		_pageButtons.Add(CreatePageButton("Batch01/Textures/Shop/IconPants"));
		_pageButtons.Add(CreatePageButton("Batch01/Textures/Shop/IconShoes"));
		_selectArrow = new MilMo_PulseCursor(_ui);
		_ui.AddChild(_selectArrow);
		_selectArrow.SetPosition(-100f, 50f);
		_selectArrow.SetScalePull(0.05f, 0.05f);
		_selectArrow.SetScaleDrag(0.6f, 0.6f);
		_selectArrow.SetScale(100f, 100f);
		_selectArrow.PulseDownDelay = 0f;
		_selectArrow.PulseUpDelay = 0f;
		_selectArrow.ScaleMover.MinVel.x = 0.0001f;
		_selectArrow.ScaleMover.MinVel.y = 0.0001f;
		_selectArrow.MinSize = new Vector2(44f, 44f);
		_selectArrow.MaxSize = new Vector2(70f, 70f);
		_selectArrow.MinColor = new Color(1f, 1f, 1f, 0.3f);
		_selectArrow.MaxColor = new Color(1f, 1f, 1f, 1f);
		_selectArrow.SetDefaultColor(1f, 1f, 1f, 1f);
		_selectArrow.SetAlpha(0.4f);
		_selectArrow.SetFadeSpeed(0.04f);
		_selectArrow.AngleMover.Vel.x = 1f;
		_selectArrow.SetPosition(0f, -100f);
		GemCounter = new MilMo_GemCounter(_ui, _makeOverStudio.screenWidth - 126, _makeOverStudio.screenHeight - 396);
		GemCounter.FixedRes = false;
		GemCounter.SetAlignment(MilMo_GUI.Align.BottomLeft);
		GemCounter.RemoveChild(GemCounter.Pane);
		GemCounter.SetAlpha(0f);
		GemCounter.NumberAngle = 8.8f;
		GemCounter.Number.SetTextOffset(-3f, 4f);
		_ui.AddChild(GemCounter);
		GemCounter.Open();
		MilMo_Widget milMo_Widget = new MilMo_Widget(_ui);
		milMo_Widget.SetTexture("Batch01/Textures/CharBuilder/ModifyPriceTag");
		milMo_Widget.SetDefaultColor(0f, 0f, 0.3f, 1f);
		milMo_Widget.SetPosition(33f, 0f);
		milMo_Widget.SetScale(144f, 68f);
		GemCounter.AddChild(milMo_Widget);
		GemCounter.BringToFront(GemCounter.Number);
		GemCounter.BringToFront(GemCounter.Icon);
		_buyButton = new MilMo_Button(_ui);
		_buyButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_buyButton.SetTexture("Batch01/Textures/CharBuilder/PrevNextButton");
		_buyButton.SetHoverTexture("Batch01/Textures/CharBuilder/PrevNextButtonMO");
		_buyButton.SetPressedTexture("Batch01/Textures/CharBuilder/PrevNextButton");
		_buyButton.SetScalePull(0.06f, 0.06f);
		_buyButton.SetScaleDrag(0.6f, 0.6f);
		_buyButton.SetText(MilMo_Localization.GetLocString("CharBuilder_6053"));
		_buyButton.SetFont(MilMo_GUI.Font.EborgLarge);
		_buyButton.SetTextOutline(1f, 1f);
		_buyButton.TextOutlineColor = new Color(0f, 0f, 0f, 0.25f);
		_buyButton.SetTextDropShadowPos(2f, 2f);
		_buyButton.Function = _makeOverStudio.Buy;
		_buyButton.SetDefaultColor(1f, 1f, 0f, 1f);
		_buyButton.SetFadeSpeed(0.1f);
		_buyButton.SetFadeSpeed(0.075f);
		_buyButton.SetFadeInSpeed(0.075f);
		_buyButton.SetFadeOutSpeed(0.075f);
		SetCommonParameters(_buyButton);
		_buyButton.AlphaTo(0f);
		_buyButton.SetHoverSound(_makeOverStudio.TickSound);
		_ui.AddChild(_buyButton);
		_resetButton = new MilMo_Button(_ui);
		_resetButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_resetButton.SetTexture("Batch01/Textures/CharBuilder/PrevNextButton");
		_resetButton.SetHoverTexture("Batch01/Textures/CharBuilder/PrevNextButtonMO");
		_resetButton.SetPressedTexture("Batch01/Textures/CharBuilder/PrevNextButton");
		_resetButton.SetScalePull(0.06f, 0.06f);
		_resetButton.SetScaleDrag(0.6f, 0.6f);
		_resetButton.SetText(MilMo_Localization.GetLocString("CharBuilder_6054"));
		_resetButton.SetFont(MilMo_GUI.Font.EborgMedium);
		_resetButton.SetTextOutline(1f, 1f);
		_resetButton.TextOutlineColor = new Color(0f, 0f, 0f, 0.25f);
		_resetButton.SetTextDropShadowPos(2f, 2f);
		_resetButton.Function = _makeOverStudio.ResetAvatar;
		_resetButton.SetDefaultColor(1f, 1f, 1f, 1f);
		_resetButton.SetFadeSpeed(0.1f);
		_resetButton.SetFadeSpeed(0.075f);
		_resetButton.SetFadeInSpeed(0.075f);
		_resetButton.SetFadeOutSpeed(0.075f);
		SetCommonParameters(_resetButton);
		_resetButton.AlphaTo(0f);
		_resetButton.SetHoverSound(_makeOverStudio.TickSound);
		_ui.AddChild(_resetButton);
		_randomizeButton = new MilMo_Button(_ui);
		_randomizeButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_randomizeButton.SetTexture("Batch01/Textures/CharBuilder/PrevNextButton");
		_randomizeButton.SetHoverTexture("Batch01/Textures/CharBuilder/PrevNextButtonMO");
		_randomizeButton.SetPressedTexture("Batch01/Textures/CharBuilder/PrevNextButton");
		_randomizeButton.SetScalePull(0.06f, 0.06f);
		_randomizeButton.SetScaleDrag(0.6f, 0.6f);
		_randomizeButton.SetText(MilMo_Localization.GetLocString("CharBuilder_6055"));
		_randomizeButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_randomizeButton.SetTextOutline(1f, 1f);
		_randomizeButton.TextOutlineColor = new Color(0f, 0f, 0f, 0.25f);
		_randomizeButton.SetTextDropShadowPos(2f, 2f);
		_randomizeButton.Function = _makeOverStudio.RandomizeAvatar;
		_randomizeButton.SetDefaultColor(1f, 1f, 1f, 1f);
		_randomizeButton.SetFadeSpeed(0.1f);
		_randomizeButton.SetFadeSpeed(0.075f);
		_randomizeButton.SetFadeInSpeed(0.075f);
		_randomizeButton.SetFadeOutSpeed(0.075f);
		SetCommonParameters(_randomizeButton);
		_randomizeButton.AlphaTo(1f);
		_randomizeButton.SetHoverSound(_makeOverStudio.TickSound);
		_ui.AddChild(_randomizeButton);
		AvatarGemCounter = new MilMo_GemCounter(_ui);
		AvatarGemCounter.FixedRes = false;
		_ui.AddChild(AvatarGemCounter);
		AvatarGemCounter.SpawnPosition.x = _makeOverStudio.screenWidth - 144;
		AvatarGemCounter.TargetPosition.x = _makeOverStudio.screenWidth - 144;
		AvatarGemCounter.SpawnPosition.y = 64f;
		AvatarGemCounter.TargetPosition.y = 64f;
		AvatarGemCounter.Open();
		GlobalStates.Instance.playerState.gems.OnChange += UpdateGems;
		UpdateGems();
		_skipButton = new MilMo_Button(_ui);
		_skipButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_skipButton.SetTexture("Batch01/Textures/CharBuilder/PrevNextButton");
		_skipButton.SetHoverTexture("Batch01/Textures/CharBuilder/PrevNextButtonMO");
		_skipButton.SetPressedTexture("Batch01/Textures/CharBuilder/PrevNextButton");
		_skipButton.SetFontScale(0.9f);
		_skipButton.SetScalePull(0.06f, 0.06f);
		_skipButton.SetScaleDrag(0.6f, 0.6f);
		_skipButton.SetText(MilMo_Localization.GetLocString("CharBuilder_6056"));
		_skipButton.SetFont(MilMo_GUI.Font.EborgLarge);
		_skipButton.SetTextOutline(1f, 1f);
		_skipButton.TextOutlineColor = new Color(0f, 0f, 0f, 0.25f);
		_skipButton.SetTextDropShadowPos(2f, 2f);
		_skipButton.Function = MilMo_MakeOverStudio.Exit;
		_skipButton.SetDefaultColor(1f, 1f, 0f, 1f);
		_skipButton.SetFadeSpeed(0.1f);
		_skipButton.SetFadeSpeed(0.075f);
		_skipButton.SetFadeInSpeed(0.075f);
		_skipButton.SetFadeOutSpeed(0.075f);
		SetCommonParameters(_skipButton);
		_skipButton.AlphaTo(1f);
		_skipButton.SetHoverSound(_makeOverStudio.TickSound);
		_ui.AddChild(_skipButton);
		_loadingPane = new MilMo_LoadingPane(_ui);
		_ui.AddChild(_loadingPane);
		_numPages++;
		Window.SetScale(_physiquePageScale.x, _physiquePageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_physiqueCaption = new MilMo_Widget(_ui);
		_physiqueCaption.SetPosition(_ui.Center.x, _ui.Align.Top - _ui.Padding.x / 3f);
		_physiqueCaption.SetText(MilMo_Localization.GetLocString("CharBuilder_63"));
		_physiqueCaption.SetTexture("Batch01/Textures/Core/Invisible");
		_physiqueCaption.SetAlignment(MilMo_GUI.Align.TopCenter);
		_physiqueCaption.SetScale(300f, 35f);
		_physiqueCaption.SetFont(MilMo_GUI.Font.EborgLarge);
		_physiqueCaption.SetExtraDrawTextSize(20f, 20f);
		_physiquePage.Add(_physiqueCaption);
		SetCommonParametersAndAdd(_physiqueCaption);
		_genderLabel = new MilMo_SimpleBox(_ui);
		_genderLabel.SetPosition(_ui.Center.x, _ui.Same.y + 32f + _ui.Padding.x / 3f);
		_genderLabel.SetAlignment(MilMo_GUI.Align.TopCenter);
		_genderLabel.SetScale(_ui.Width, 72f + _ui.Padding.y / 3f);
		_genderLabel.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		_genderLabel.SetText(MilMo_Localization.GetLocString("CharBuilder_64"));
		_genderLabel.SetFont(MilMo_GUI.Font.EborgSmall);
		_physiquePage.Add(_genderLabel);
		SetCommonParametersAndAdd(_genderLabel);
		MaleButton = new MilMo_Button(_ui);
		MaleButton.SetPosition(_ui.Align.Left + _ui.Padding.x / 2f + 62f, _ui.Same.y + 32f);
		MaleButton.SetAlignment(MilMo_GUI.Align.TopCenter);
		MaleButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		MaleButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		MaleButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		MaleButton.SetFontScale(0.9f);
		MaleButton.SetScale(86f, 40f);
		MaleButton.SetText(MilMo_Localization.GetLocString("CharBuilder_65"));
		MaleButton.SetFont(MilMo_GUI.Font.EborgSmall);
		MaleButton.Function = _makeOverStudio.SwapToMale;
		_physiquePage.Add(MaleButton);
		SetCommonParametersAndAdd(MaleButton);
		FemaleButton = new MilMo_Button(_ui);
		FemaleButton.SetPosition(_ui.Align.Right - _ui.Padding.x / 2f - 62f, _ui.Same.y);
		FemaleButton.SetAlignment(MilMo_GUI.Align.TopCenter);
		FemaleButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		FemaleButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		FemaleButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		FemaleButton.SetFontScale(0.9f);
		FemaleButton.SetScale(86f, 40f);
		FemaleButton.SetText(MilMo_Localization.GetLocString("CharBuilder_66"));
		FemaleButton.SetFont(MilMo_GUI.Font.EborgSmall);
		FemaleButton.Function = _makeOverStudio.SwapToFemale;
		_physiquePage.Add(FemaleButton);
		SetCommonParametersAndAdd(FemaleButton);
		_heightLabel = new MilMo_SimpleBox(_ui);
		_heightLabel.SetPosition(_ui.Center.x, _ui.Next.y + 5f);
		_heightLabel.SetAlignment(MilMo_GUI.Align.TopCenter);
		_heightLabel.SetScale(_ui.Width, 52f + _ui.Padding.y / 3f);
		_heightLabel.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		_heightLabel.SetText(MilMo_Localization.GetLocString("CharBuilder_67"));
		_heightLabel.SetFont(MilMo_GUI.Font.EborgSmall);
		_physiquePage.Add(_heightLabel);
		SetCommonParametersAndAdd(_heightLabel);
		HeightSlider = new MilMo_SimpleSlider(_ui);
		HeightSlider.SetPosition(_ui.Center.x, _ui.Same.y + 32f);
		HeightSlider.SetAlignment(MilMo_GUI.Align.TopCenter);
		HeightSlider.SetScale(_ui.Width - _ui.Padding.x, 30f);
		_physiquePage.Add(HeightSlider);
		SetCommonParametersAndAdd(HeightSlider);
		HeightSlider.Min = 0.9f;
		HeightSlider.Max = 1.1f;
		HeightSlider.Val = 1f;
		SkinColorBar = new MilMo_ColorBar(_ui, 8, 4);
		SkinColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 5f);
		SkinColorBar.SetScale(_ui.Width, 0f);
		SkinColorBar.SetText(MilMo_Localization.GetLocString("CharBuilder_68"));
		SkinColorBar.SetFont(MilMo_GUI.Font.EborgSmall);
		_physiquePage.Add(SkinColorBar);
		SetCommonParametersAndAdd(SkinColorBar);
		Color defaultColor = new Color(0.1f, 0.1f, 0.1f, 1f);
		Color hoverColor = new Color(0.2f, 0.2f, 0.6f, 1f);
		_numPages++;
		Window.SetScale(_eyesPageScale.x, _eyesPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_eyesCaption = new MilMo_Widget(_ui);
		_eyesCaption.SetPosition(_ui.Center.x, _ui.Align.Top - _ui.Padding.x / 3f);
		_eyesCaption.SetText(MilMo_Localization.GetLocString("CharBuilder_69"));
		_eyesCaption.SetTexture("Batch01/Textures/Core/Invisible");
		_eyesCaption.SetAlignment(MilMo_GUI.Align.TopCenter);
		_eyesCaption.SetScale(400f, 35f);
		_eyesCaption.SetFont(MilMo_GUI.Font.EborgLarge);
		_eyesCaption.SetExtraDrawTextSize(20f, 20f);
		_eyesPage.Add(_eyesCaption);
		SetCommonParametersAndAdd(_eyesCaption);
		int num3 = 0;
		float num4 = _ui.Next.y;
		for (int i = 0; i < 3; i++)
		{
			EyesButton[num3] = new MilMo_AvatarButton(_ui);
			_ui.AddChild(EyesButton[num3]);
			EyesButton[num3].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num4 + 64f);
			EyesButton[num3].SetAlignment(MilMo_GUI.Align.BottomLeft);
			EyesButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, (_ui.Width / 2f - _ui.Padding.x) / 2f);
			EyesButton[num3].SetScalePull(0.09f, 0.09f);
			EyesButton[num3].SetScaleDrag(0.6f, 0.6f);
			EyesButton[num3].SetAllTextures("Batch01/Textures/Core/Invisible");
			EyesButton[num3].SetDefaultColor(defaultColor);
			EyesButton[num3].SetHoverColor(hoverColor);
			EyesButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
			EyesButton[num3].PointerHoverFunction = RefreshAvatarIconMouseIdle;
			EyesButton[num3].PointerLeaveFunction = RefreshAvatarIconMouseIdle;
			EyesButton[num3].SetFadeSpeed(0.01f);
			EyesButton[num3].SetFadeInSpeed(0.01f);
			EyesButton[num3].Function = _makeOverStudio.SetEyeTexture;
			EyesButton[num3].Args = num3;
			EyesButton[num3].SetHoverSound(_makeOverStudio.TickSound);
			_eyesPage.Add(EyesButton[num3]);
			SetCommonParametersAndAdd(EyesButton[num3]);
			EyesButton[num3].SetEnabled(e: false);
			num3++;
			EyesButton[num3] = new MilMo_AvatarButton(_ui);
			_ui.AddChild(EyesButton[num3]);
			EyesButton[num3].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num4 + 64f);
			EyesButton[num3].SetAlignment(MilMo_GUI.Align.BottomRight);
			EyesButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, (_ui.Width / 2f - _ui.Padding.x) / 2f);
			EyesButton[num3].SetScalePull(0.09f, 0.09f);
			EyesButton[num3].SetScaleDrag(0.6f, 0.6f);
			EyesButton[num3].SetAllTextures("Batch01/Textures/Core/Invisible");
			EyesButton[num3].SetDefaultColor(defaultColor);
			EyesButton[num3].SetHoverColor(hoverColor);
			EyesButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
			EyesButton[num3].PointerHoverFunction = RefreshAvatarIconMouseIdle;
			EyesButton[num3].PointerLeaveFunction = RefreshAvatarIconMouseIdle;
			EyesButton[num3].SetFadeSpeed(0.01f);
			EyesButton[num3].SetFadeInSpeed(0.01f);
			EyesButton[num3].Function = _makeOverStudio.SetEyeTexture;
			EyesButton[num3].Args = num3;
			EyesButton[num3].SetHoverSound(_makeOverStudio.TickSound);
			_eyesPage.Add(EyesButton[num3]);
			SetCommonParametersAndAdd(EyesButton[num3]);
			EyesButton[num3].SetEnabled(e: false);
			num3++;
			num4 += 64f + _ui.Padding.y;
		}
		EyeColorBar = new MilMo_ColorBar(_ui, 10, 5);
		EyeColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 5f - 64f);
		EyeColorBar.SetScale(_ui.Width, 0f);
		EyeColorBar.SetText(MilMo_Localization.GetLocString("CharBuilder_70"));
		EyeColorBar.SetFont(MilMo_GUI.Font.EborgSmall);
		_eyesPage.Add(EyeColorBar);
		SetCommonParametersAndAdd(EyeColorBar);
		EyeBrowBar = new MilMo_ToggleBar(_ui);
		EyeBrowBar.SetPosition(_ui.Align.Left, _ui.Next.y);
		EyeBrowBar.SetScale(_ui.Width, 75f);
		EyeBrowBar.SetText(MilMo_Localization.GetLocString("CharBuilder_71"));
		EyeBrowBar.SetFont(MilMo_GUI.Font.EborgSmall);
		EyeBrowBar.TextLabel.SetFontScale(0.9f);
		EyeBrowBar.LeftButton.Function = _makeOverStudio.PrevEyeBrows;
		EyeBrowBar.RightButton.Function = _makeOverStudio.NextEyeBrows;
		EyeBrowBar.LeftButton.DoubleClickFunction = _makeOverStudio.PrevEyeBrows;
		EyeBrowBar.RightButton.DoubleClickFunction = _makeOverStudio.NextEyeBrows;
		_eyesPage.Add(EyeBrowBar);
		SetCommonParametersAndAdd(EyeBrowBar);
		_numPages++;
		Window.SetScale(_mouthPageScale.x, _mouthPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_mouthCaption = new MilMo_Widget(_ui);
		_mouthCaption.SetPosition(_ui.Center.x, _ui.Align.Top - _ui.Padding.x / 3f);
		_mouthCaption.SetText(MilMo_Localization.GetLocString("CharBuilder_72"));
		_mouthCaption.SetTexture("Batch01/Textures/Core/Invisible");
		_mouthCaption.SetAlignment(MilMo_GUI.Align.TopCenter);
		_mouthCaption.SetScale(400f, 35f);
		_mouthCaption.SetFont(MilMo_GUI.Font.EborgLarge);
		_mouthCaption.SetExtraDrawTextSize(20f, 20f);
		_mouthPage.Add(_mouthCaption);
		SetCommonParametersAndAdd(_mouthCaption);
		num3 = 0;
		num4 = _ui.Next.y;
		for (int j = 0; j < 3; j++)
		{
			MouthButton[num3] = new MilMo_AvatarButton(_ui);
			_ui.AddChild(MouthButton[num3]);
			MouthButton[num3].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num4);
			MouthButton[num3].SetAlignment(MilMo_GUI.Align.TopLeft);
			MouthButton[num3].SetAllTextures("Batch01/Textures/Core/Invisible");
			MouthButton[num3].SetDefaultColor(defaultColor);
			MouthButton[num3].SetHoverColor(hoverColor);
			MouthButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
			MouthButton[num3].PointerHoverFunction = RefreshAvatarIconMouseIdle;
			MouthButton[num3].PointerLeaveFunction = RefreshAvatarIconMouseIdle;
			MouthButton[num3].SetFadeSpeed(0.01f);
			MouthButton[num3].SetFadeInSpeed(0.01f);
			MouthButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, (_ui.Width / 2f - _ui.Padding.x) / 2f);
			MouthButton[num3].SetScalePull(0.09f, 0.09f);
			MouthButton[num3].SetScaleDrag(0.6f, 0.6f);
			MouthButton[num3].Function = _makeOverStudio.SetMouthTexture;
			MouthButton[num3].Args = num3;
			MouthButton[num3].SetHoverSound(_makeOverStudio.TickSound);
			_mouthPage.Add(MouthButton[num3]);
			SetCommonParametersAndAdd(MouthButton[num3]);
			MouthButton[num3].SetEnabled(e: false);
			num3++;
			MouthButton[num3] = new MilMo_AvatarButton(_ui);
			_ui.AddChild(MouthButton[num3]);
			MouthButton[num3].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num4);
			MouthButton[num3].SetAlignment(MilMo_GUI.Align.TopRight);
			MouthButton[num3].SetAllTextures("Batch01/Textures/Core/Invisible");
			MouthButton[num3].SetDefaultColor(defaultColor);
			MouthButton[num3].SetHoverColor(hoverColor);
			MouthButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
			MouthButton[num3].PointerHoverFunction = RefreshAvatarIconMouseIdle;
			MouthButton[num3].PointerLeaveFunction = RefreshAvatarIconMouseIdle;
			MouthButton[num3].SetFadeSpeed(0.01f);
			MouthButton[num3].SetFadeInSpeed(0.01f);
			MouthButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, (_ui.Width / 2f - _ui.Padding.x) / 2f);
			MouthButton[num3].SetScalePull(0.09f, 0.09f);
			MouthButton[num3].SetScaleDrag(0.6f, 0.6f);
			MouthButton[num3].Function = _makeOverStudio.SetMouthTexture;
			MouthButton[num3].Args = num3;
			MouthButton[num3].SetHoverSound(_makeOverStudio.TickSound);
			_mouthPage.Add(MouthButton[num3]);
			SetCommonParametersAndAdd(MouthButton[num3]);
			MouthButton[num3].SetEnabled(e: false);
			num3++;
			num4 += 64f + _ui.Padding.y;
		}
		_numPages++;
		Window.SetScale(_hairPageScale.x, _hairPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_hairCaption = new MilMo_Widget(_ui);
		_hairCaption.SetPosition(_ui.Center.x, _ui.Align.Top - _ui.Padding.x / 3f);
		_hairCaption.SetText(MilMo_Localization.GetLocString("CharBuilder_73"));
		_hairCaption.SetTexture("Batch01/Textures/Core/Invisible");
		_hairCaption.SetAlignment(MilMo_GUI.Align.TopCenter);
		_hairCaption.SetScale(400f, 35f);
		_hairCaption.SetFont(MilMo_GUI.Font.EborgLarge);
		_hairCaption.SetExtraDrawTextSize(20f, 20f);
		HairPage.Add(_hairCaption);
		SetCommonParametersAndAdd(_hairCaption);
		num3 = 0;
		num4 = _ui.Next.y;
		for (int k = 0; k < 3; k++)
		{
			_hairButton[num3] = new MilMo_WearableButton(_ui);
			_ui.AddChild(_hairButton[num3]);
			_hairButton[num3].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num4 + _ui.Width / 4f);
			_hairButton[num3].SetAlignment(MilMo_GUI.Align.CenterLeft);
			_hairButton[num3].SetAllTextures("Batch01/Textures/Core/Invisible");
			_hairButton[num3].SetDefaultColor(defaultColor);
			_hairButton[num3].SetHoverColor(hoverColor);
			_hairButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
			_hairButton[num3].SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
			_hairButton[num3].SetExtraScaleOnHover(5f, 5f);
			_hairButton[num3].SetDefaultColor(_winButDefaultColor);
			_hairButton[num3].SetHoverColor(_winButMouseOverColor);
			_hairButton[num3].PointerHoverFunction = RefreshAvatarIconMouseIdle;
			_hairButton[num3].PointerLeaveFunction = RefreshAvatarIconMouseIdle;
			_hairButton[num3].SetFadeSpeed(0.01f);
			_hairButton[num3].SetFadeInSpeed(0.01f);
			_hairButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, _ui.Width / 2f - _ui.Padding.x);
			_hairButton[num3].SetScalePull(0.09f, 0.09f);
			_hairButton[num3].SetScaleDrag(0.7f, 0.7f);
			_hairButton[num3].SetHoverSound(_makeOverStudio.TickSound);
			if (num3 < _makeOverStudio.RemoteCharBuilder.BoyHairStyleItems.Count)
			{
				_hairButton[num3].Function = _makeOverStudio.SetHairStyle;
				MilMo_Wearable[] args = new MilMo_Wearable[2]
				{
					_makeOverStudio.RemoteCharBuilder.BoyHairStyleItems[num3],
					_makeOverStudio.RemoteCharBuilder.GirlHairStyleItems[num3]
				};
				_hairButton[num3].Args = args;
			}
			HairPage.Add(_hairButton[num3]);
			SetCommonParametersAndAdd(_hairButton[num3]);
			_hairButton[num3].SetEnabled(e: false);
			num3++;
			_hairButton[num3] = new MilMo_WearableButton(_ui);
			_ui.AddChild(_hairButton[num3]);
			_hairButton[num3].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num4 + _ui.Width / 4f);
			_hairButton[num3].SetAlignment(MilMo_GUI.Align.CenterRight);
			_hairButton[num3].SetAllTextures("Batch01/Textures/Core/Invisible");
			_hairButton[num3].SetDefaultColor(defaultColor);
			_hairButton[num3].SetHoverColor(hoverColor);
			_hairButton[num3].SetDefaultColor(_winButDefaultColor);
			_hairButton[num3].SetHoverColor(_winButMouseOverColor);
			_hairButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
			_hairButton[num3].SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
			_hairButton[num3].SetExtraScaleOnHover(5f, 5f);
			_hairButton[num3].PointerHoverFunction = RefreshAvatarIconMouseIdle;
			_hairButton[num3].PointerLeaveFunction = RefreshAvatarIconMouseIdle;
			_hairButton[num3].SetFadeSpeed(0.01f);
			_hairButton[num3].SetFadeInSpeed(0.01f);
			_hairButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, _ui.Width / 2f - _ui.Padding.x);
			_hairButton[num3].SetScalePull(0.09f, 0.09f);
			_hairButton[num3].SetScaleDrag(0.6f, 0.6f);
			if (num3 < _makeOverStudio.RemoteCharBuilder.BoyHairStyleItems.Count)
			{
				_hairButton[num3].Function = _makeOverStudio.SetHairStyle;
				MilMo_Wearable[] args2 = new MilMo_Wearable[2]
				{
					_makeOverStudio.RemoteCharBuilder.BoyHairStyleItems[num3],
					_makeOverStudio.RemoteCharBuilder.GirlHairStyleItems[num3]
				};
				_hairButton[num3].Args = args2;
			}
			_hairButton[num3].SetHoverSound(_makeOverStudio.TickSound);
			HairPage.Add(_hairButton[num3]);
			SetCommonParametersAndAdd(_hairButton[num3]);
			_hairButton[num3].SetEnabled(e: false);
			num3++;
			num4 += 100f + _ui.Padding.y;
		}
		MilMo_Wearable[] args3 = new MilMo_Wearable[2] { null, null };
		_hairButton[5].Args = args3;
		_hairButton[5].Function = delegate(object o)
		{
			_makeOverStudio.SetHairStyle(o);
		};
		HairColorBar = new MilMo_ColorBar(_ui, 10, 5);
		HairColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 5f);
		HairColorBar.SetScale(_ui.Width, 0f);
		HairColorBar.SetText(MilMo_Localization.GetLocString("CharBuilder_74"));
		HairColorBar.SetFont(MilMo_GUI.Font.EborgSmall);
		HairPage.Add(HairColorBar);
		SetCommonParametersAndAdd(HairColorBar);
		_numPages++;
		Window.SetScale(_shirtPageScale.x, _shirtPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_shirtCaption = new MilMo_Widget(_ui);
		_shirtCaption.SetPosition(_ui.Center.x, _ui.Align.Top - _ui.Padding.x / 3f);
		_shirtCaption.SetText(MilMo_Localization.GetLocString("CharBuilder_75"));
		_shirtCaption.SetTexture("Batch01/Textures/Core/Invisible");
		_shirtCaption.SetAlignment(MilMo_GUI.Align.TopCenter);
		_shirtCaption.SetScale(400f, 35f);
		_shirtCaption.SetFont(MilMo_GUI.Font.EborgLarge);
		_shirtCaption.SetExtraDrawTextSize(20f, 20f);
		ShirtPage.Add(_shirtCaption);
		SetCommonParametersAndAdd(_shirtCaption);
		num3 = 0;
		num4 = _ui.Next.y;
		for (int l = 0; l < 2; l++)
		{
			_shirtButton[num3] = new MilMo_WearableButton(_ui);
			_shirtButton[num3].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num4 + _ui.Width / 4f);
			_shirtButton[num3].SetAlignment(MilMo_GUI.Align.CenterLeft);
			_shirtButton[num3].SetTexture("Batch01/Textures/Core/Invisible");
			_shirtButton[num3].SetHoverTexture("Batch01/Textures/Core/Default");
			_shirtButton[num3].SetPressedTexture("Batch01/Textures/Core/Black");
			_shirtButton[num3].SetFontScale(0.9f);
			_shirtButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, _ui.Width / 2f - _ui.Padding.x);
			_shirtButton[num3].SetFont(MilMo_GUI.Font.EborgLarge);
			if (num3 <= _makeOverStudio.RemoteCharBuilder.BoyShirtItems.Count)
			{
				_shirtButton[num3].Function = _makeOverStudio.SetShirt;
				_shirtButton[num3].Args = num3;
			}
			_shirtButton[num3].SetHoverSound(_makeOverStudio.TickSound);
			_shirtButton[num3].SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
			_shirtButton[num3].SetExtraScaleOnHover(0f, 10f);
			_shirtButton[num3].SetScalePull(0.09f, 0.09f);
			_shirtButton[num3].SetScaleDrag(0.7f, 0.7f);
			_shirtButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
			_shirtButton[num3].SetDefaultColor(_winButDefaultColor);
			_shirtButton[num3].SetHoverColor(_winButMouseOverColor);
			ShirtPage.Add(_shirtButton[num3]);
			SetCommonParametersAndAdd(_shirtButton[num3]);
			num3++;
			if (num3 < 4)
			{
				_shirtButton[num3] = new MilMo_WearableButton(_ui);
				_shirtButton[num3].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num4 + _ui.Width / 4f);
				_shirtButton[num3].SetAlignment(MilMo_GUI.Align.CenterRight);
				_shirtButton[num3].SetTexture("Batch01/Textures/Core/Invisible");
				_shirtButton[num3].SetHoverTexture("Batch01/Textures/Core/Default");
				_shirtButton[num3].SetPressedTexture("Batch01/Textures/Core/Black");
				_shirtButton[num3].SetFontScale(0.9f);
				_shirtButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, _ui.Width / 2f - _ui.Padding.x);
				_shirtButton[num3].SetFont(MilMo_GUI.Font.EborgLarge);
				if (num3 < _makeOverStudio.RemoteCharBuilder.BoyShirtItems.Count)
				{
					_shirtButton[num3].Function = _makeOverStudio.SetShirt;
					_shirtButton[num3].Args = num3;
				}
				_shirtButton[num3].SetHoverSound(_makeOverStudio.TickSound);
				_shirtButton[num3].SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
				_shirtButton[num3].SetExtraScaleOnHover(0f, 10f);
				_shirtButton[num3].SetScalePull(0.09f, 0.09f);
				_shirtButton[num3].SetScaleDrag(0.7f, 0.7f);
				_shirtButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
				_shirtButton[num3].SetDefaultColor(_winButDefaultColor);
				_shirtButton[num3].SetHoverColor(_winButMouseOverColor);
				ShirtPage.Add(_shirtButton[num3]);
				SetCommonParametersAndAdd(_shirtButton[num3]);
			}
			num3++;
			num4 += 109f + _ui.Padding.y;
		}
		_shirtButton[0].SetTexture("Batch01/Textures/Core/Black");
		ShirtColorBar = new MilMo_ColorBar(_ui, 15, 5);
		ShirtColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 44f);
		ShirtColorBar.SetScale(_ui.Width, 0f);
		ShirtColorBar.SetFont(MilMo_GUI.Font.EborgSmall);
		ShirtPage.Add(ShirtColorBar);
		SetCommonParametersAndAdd(ShirtColorBar);
		_numPages++;
		Window.SetScale(_pantsPageScale.x, _pantsPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_pantsCaption = new MilMo_Widget(_ui);
		_pantsCaption.SetPosition(_ui.Center.x, _ui.Align.Top - _ui.Padding.x / 3f);
		_pantsCaption.SetText(MilMo_Localization.GetLocString("CharBuilder_76"));
		_pantsCaption.SetTexture("Batch01/Textures/Core/Invisible");
		_pantsCaption.SetAlignment(MilMo_GUI.Align.TopCenter);
		_pantsCaption.SetScale(400f, 35f);
		_pantsCaption.SetFont(MilMo_GUI.Font.EborgLarge);
		_pantsCaption.SetExtraDrawTextSize(20f, 20f);
		PantsPage.Add(_pantsCaption);
		SetCommonParametersAndAdd(_pantsCaption);
		num3 = 0;
		num4 = _ui.Next.y;
		for (int m = 0; m < 2; m++)
		{
			_pantsButton[num3] = new MilMo_WearableButton(_ui);
			_pantsButton[num3].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num4 + _ui.Width / 4f);
			_pantsButton[num3].SetAlignment(MilMo_GUI.Align.CenterLeft);
			_pantsButton[num3].SetTexture("Batch01/Textures/Core/Invisible");
			_pantsButton[num3].SetHoverTexture("Batch01/Textures/Core/Default");
			_pantsButton[num3].SetPressedTexture("Batch01/Textures/Core/Black");
			_pantsButton[num3].SetFontScale(0.9f);
			_pantsButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, _ui.Width / 2f - _ui.Padding.x);
			_pantsButton[num3].SetText(MilMo_LocString.Empty);
			_pantsButton[num3].SetFont(MilMo_GUI.Font.EborgLarge);
			if (num3 <= _makeOverStudio.RemoteCharBuilder.BoyPantsItems.Count)
			{
				_pantsButton[num3].Function = _makeOverStudio.SetPants;
				_pantsButton[num3].Args = num3;
			}
			_pantsButton[num3].SetHoverSound(_makeOverStudio.TickSound);
			_pantsButton[num3].SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
			_pantsButton[num3].SetExtraScaleOnHover(0f, 10f);
			_pantsButton[num3].SetScalePull(0.09f, 0.09f);
			_pantsButton[num3].SetScaleDrag(0.7f, 0.7f);
			_pantsButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
			_pantsButton[num3].SetDefaultColor(_winButDefaultColor);
			_pantsButton[num3].SetHoverColor(_winButMouseOverColor);
			PantsPage.Add(_pantsButton[num3]);
			SetCommonParametersAndAdd(_pantsButton[num3]);
			num3++;
			if (num3 < 4)
			{
				_pantsButton[num3] = new MilMo_WearableButton(_ui);
				_pantsButton[num3].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num4 + _ui.Width / 4f);
				_pantsButton[num3].SetAlignment(MilMo_GUI.Align.CenterRight);
				_pantsButton[num3].SetTexture("Batch01/Textures/Core/Invisible");
				_pantsButton[num3].SetHoverTexture("Batch01/Textures/Core/Default");
				_pantsButton[num3].SetPressedTexture("Batch01/Textures/Core/Black");
				_pantsButton[num3].SetFontScale(0.9f);
				_pantsButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, _ui.Width / 2f - _ui.Padding.x);
				_pantsButton[num3].SetText(MilMo_LocString.Empty);
				_pantsButton[num3].SetFont(MilMo_GUI.Font.EborgLarge);
				if (num3 <= _makeOverStudio.RemoteCharBuilder.BoyPantsItems.Count)
				{
					_pantsButton[num3].Function = _makeOverStudio.SetPants;
					_pantsButton[num3].Args = num3;
				}
				_pantsButton[num3].SetHoverSound(_makeOverStudio.TickSound);
				_pantsButton[num3].SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
				_pantsButton[num3].SetExtraScaleOnHover(0f, 10f);
				_pantsButton[num3].SetScalePull(0.09f, 0.09f);
				_pantsButton[num3].SetScaleDrag(0.7f, 0.7f);
				_pantsButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
				_pantsButton[num3].SetDefaultColor(_winButDefaultColor);
				_pantsButton[num3].SetHoverColor(_winButMouseOverColor);
				PantsPage.Add(_pantsButton[num3]);
				SetCommonParametersAndAdd(_pantsButton[num3]);
			}
			num3++;
			num4 += 109f + _ui.Padding.y;
		}
		_pantsButton[0].SetTexture("Batch01/Textures/Core/Black");
		PantsColorBar = new MilMo_ColorBar(_ui, 15, 5);
		PantsColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 50f);
		PantsColorBar.SetScale(_ui.Width, 0f);
		PantsColorBar.SetFont(MilMo_GUI.Font.EborgSmall);
		PantsPage.Add(PantsColorBar);
		SetCommonParametersAndAdd(PantsColorBar);
		_numPages++;
		Window.SetScale(_shoesPageScale.x, _shoesPageScale.y);
		_ui.ResetLayout(20f, 20f, Window);
		_shoesCaption = new MilMo_Widget(_ui);
		_shoesCaption.SetPosition(_ui.Center.x, _ui.Align.Top - _ui.Padding.x / 3f);
		_shoesCaption.SetText(MilMo_Localization.GetLocString("CharBuilder_77"));
		_shoesCaption.SetTexture("Batch01/Textures/Core/Invisible");
		_shoesCaption.SetAlignment(MilMo_GUI.Align.TopCenter);
		_shoesCaption.SetScale(400f, 35f);
		_shoesCaption.SetFont(MilMo_GUI.Font.EborgLarge);
		_shoesCaption.SetExtraDrawTextSize(20f, 20f);
		ShoesPage.Add(_shoesCaption);
		SetCommonParametersAndAdd(_shoesCaption);
		num3 = 0;
		num4 = _ui.Next.y;
		for (int n = 0; n < 2; n++)
		{
			_shoesButton[num3] = new MilMo_WearableButton(_ui);
			_shoesButton[num3].SetPosition(_ui.Align.Left + _ui.Padding.x / 2f, num4 + _ui.Width / 2f);
			_shoesButton[num3].SetAlignment(MilMo_GUI.Align.BottomLeft);
			_shoesButton[num3].SetTexture("Batch01/Textures/Core/Invisible");
			_shoesButton[num3].SetHoverTexture("Batch01/Textures/Core/Default");
			_shoesButton[num3].SetPressedTexture("Batch01/Textures/Core/Black");
			_shoesButton[num3].SetFontScale(0.9f);
			_shoesButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, _ui.Width / 2f - _ui.Padding.x);
			_shoesButton[num3].SetText(MilMo_LocString.Empty);
			_shoesButton[num3].SetFont(MilMo_GUI.Font.EborgLarge);
			if (num3 <= _makeOverStudio.RemoteCharBuilder.BoyShoesItems.Count)
			{
				_shoesButton[num3].Function = _makeOverStudio.SetShoes;
				_shoesButton[num3].Args = num3;
			}
			_shoesButton[num3].SetHoverSound(_makeOverStudio.TickSound);
			_shoesButton[num3].SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
			_shoesButton[num3].SetExtraScaleOnHover(0f, 10f);
			_shoesButton[num3].SetScalePull(0.09f, 0.09f);
			_shoesButton[num3].SetScaleDrag(0.7f, 0.7f);
			_shoesButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
			_shoesButton[num3].SetDefaultColor(_winButDefaultColor);
			_shoesButton[num3].SetHoverColor(_winButMouseOverColor);
			ShoesPage.Add(_shoesButton[num3]);
			SetCommonParametersAndAdd(_shoesButton[num3]);
			num3++;
			if (num3 < 4)
			{
				_shoesButton[num3] = new MilMo_WearableButton(_ui);
				_shoesButton[num3].SetPosition(_ui.Align.Right - _ui.Padding.x / 2f, num4 + _ui.Width / 2f);
				_shoesButton[num3].SetAlignment(MilMo_GUI.Align.BottomRight);
				_shoesButton[num3].SetTexture("Batch01/Textures/Core/Invisible");
				_shoesButton[num3].SetHoverTexture("Batch01/Textures/Core/Default");
				_shoesButton[num3].SetPressedTexture("Batch01/Textures/Core/Black");
				_shoesButton[num3].SetFontScale(0.9f);
				_shoesButton[num3].SetScale(_ui.Width / 2f - _ui.Padding.x, _ui.Width / 2f - _ui.Padding.x);
				_shoesButton[num3].SetText(MilMo_LocString.Empty);
				_shoesButton[num3].SetFont(MilMo_GUI.Font.EborgLarge);
				if (num3 <= _makeOverStudio.RemoteCharBuilder.BoyShoesItems.Count)
				{
					_shoesButton[num3].Function = _makeOverStudio.SetShoes;
					_shoesButton[num3].Args = num3;
				}
				_shoesButton[num3].SetHoverSound(_makeOverStudio.TickSound);
				_shoesButton[num3].SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
				_shoesButton[num3].SetExtraScaleOnHover(0f, 10f);
				_shoesButton[num3].SetScalePull(0.09f, 0.09f);
				_shoesButton[num3].SetScaleDrag(0.7f, 0.7f);
				_shoesButton[num3].SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
				_shoesButton[num3].SetDefaultColor(_winButDefaultColor);
				_shoesButton[num3].SetHoverColor(_winButMouseOverColor);
				ShoesPage.Add(_shoesButton[num3]);
				SetCommonParametersAndAdd(_shoesButton[num3]);
			}
			num3++;
			num4 += 109f + _ui.Padding.y;
		}
		_shoesButton[0].SetTexture("Batch01/Textures/Core/Black");
		ShoesColorBar = new MilMo_ColorBar(_ui, 6, 6);
		ShoesColorBar.SetPosition(_ui.Align.Left, _ui.Next.y - 79f);
		ShoesColorBar.SetScale(_ui.Width, 0f);
		ShoesColorBar.SetFont(MilMo_GUI.Font.EborgSmall);
		ShoesPage.Add(ShoesColorBar);
		SetCommonParametersAndAdd(ShoesColorBar);
		ShoesColorBar2 = new MilMo_ColorBar(_ui, 6, 6);
		ShoesColorBar2.SetPosition(_ui.Align.Left, _ui.Next.y - 5f);
		ShoesColorBar2.SetScale(_ui.Width, 0f);
		ShoesColorBar2.SetFont(MilMo_GUI.Font.EborgSmall);
		ShoesPage.Add(ShoesColorBar2);
		SetCommonParametersAndAdd(ShoesColorBar2);
	}

	public void Destroy()
	{
		GlobalStates.Instance.playerState.gems.OnChange -= UpdateGems;
	}

	private void UpdateGems(int newValue = 0)
	{
		int num = GlobalStates.Instance.playerState.gems.Get();
		AvatarGemCounter.SetNumber(num.ToString());
	}

	private void RefreshAvatarIconMouseIdle()
	{
		MilMo_AvatarIcon[] avatarIcons = _makeOverStudio.AvatarIcons;
		for (int i = 0; i < avatarIcons.Length; i++)
		{
			avatarIcons[i]?.Idle(enabled: false);
		}
		if (MilMo_UserInterface.PointerFocus != null && MilMo_UserInterface.PointerFocus is MilMo_AvatarButton)
		{
			((MilMo_AvatarButton)MilMo_UserInterface.PointerFocus).AvatarIcon.Idle(enabled: true);
		}
	}

	private MilMo_Button CreatePageButton(string icon)
	{
		MilMo_Button milMo_Button = new MilMo_Button(_ui);
		_numPageIcons++;
		milMo_Button.SetPosition((float)_numPageIcons * 71.75f + (float)(_makeOverStudio.screenWidth / 2 - 437), 0f);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Button.SetPosPull(0.36f, 0.36f);
		milMo_Button.SetPosDrag(0.7f, 0.7f);
		milMo_Button.SetScale(0f, 0f);
		milMo_Button.SetMinScale(0f, 0f);
		milMo_Button.SetDefaultScale(0f, 0f);
		milMo_Button.SetExtraScaleOnHover(5f, 5f);
		milMo_Button.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		milMo_Button.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Fade);
		milMo_Button.SetScalePull(0.09f, 0.09f);
		milMo_Button.SetScaleDrag(0.8f, 0.6f);
		milMo_Button.SetHoverSound(_makeOverStudio.TickSound);
		milMo_Button.SetTexture(icon);
		milMo_Button.SetHoverTexture(icon);
		milMo_Button.SetPressedTexture(icon);
		milMo_Button.SetDefaultColor(1f, 1f, 1f, 1f);
		milMo_Button.SetHoverColor(1f, 1f, 1f, 1f);
		milMo_Button.SetFixedPointerZoneSize(75f, 75f);
		milMo_Button.Function = SelectPage;
		milMo_Button.Args = milMo_Button;
		milMo_Button.Info = _numPageIcons;
		milMo_Button.SetFontScale(0.5f);
		milMo_Button.TextColorNow(0f, 0f, 0f, 0f);
		milMo_Button.SetHoverTextColor(1f, 1f, 1f, 0.3f);
		_ui.AddChild(milMo_Button);
		return milMo_Button;
	}

	private void SelectPage(object o)
	{
		SelectPage(o, Time.time - _lastSelectPageSound > 0.05f);
	}

	private void SelectPage(object o, bool playSound)
	{
		int num = 1;
		Vector2 vector = new Vector2(70f / _ui.Res.x * ((_ui.Res.x <= _ui.Res.y) ? _ui.Res.x : _ui.Res.y), 70f / _ui.Res.y * ((_ui.Res.x <= _ui.Res.y) ? _ui.Res.x : _ui.Res.y));
		Vector2 vector2 = new Vector2(100f / _ui.Res.x * ((_ui.Res.x <= _ui.Res.y) ? _ui.Res.x : _ui.Res.y), 100f / _ui.Res.y * ((_ui.Res.x <= _ui.Res.y) ? _ui.Res.x : _ui.Res.y));
		foreach (MilMo_Button pageButton in _pageButtons)
		{
			pageButton.ScaleTo(vector);
			pageButton.SetMinScale(vector);
			pageButton.SetDefaultScale(vector);
			pageButton.SetExtraScaleOnHover(5f, 5f);
			pageButton.SetDefaultColor(1f, 1f, 1f, 0.5f);
			pageButton.SetPosition((float)num * 71.75f + (float)(_makeOverStudio.screenWidth / 2 - 320), 0f);
			pageButton.SetFixedPointerZoneSize(75f, 75f);
			num++;
		}
		MilMo_Button milMo_Button = (MilMo_Button)o;
		if (milMo_Button == null)
		{
			return;
		}
		milMo_Button.ScaleTo(vector2);
		milMo_Button.SetMinScale(vector2);
		milMo_Button.SetDefaultScale(vector2);
		milMo_Button.SetExtraScaleOnHover(0f, 0f);
		_ui.RemoveChild(milMo_Button);
		_ui.AddChild(milMo_Button);
		if (playSound && ActivePage != milMo_Button.Info)
		{
			if (_ui.SoundFx.IsPlaying())
			{
				_ui.SoundFx.Stop();
			}
			_ui.SoundFx.Play(_makeOverStudio.SelectCatSound);
			_lastSelectPageSound = Time.time;
		}
		ActivePage = milMo_Button.Info;
		switch (ActivePage)
		{
		case 1:
			_makeOverStudio.FocusOnBody();
			_makeOverStudio.AvatarIconDisable();
			break;
		case 5:
			_makeOverStudio.FocusOnClothes();
			_makeOverStudio.AvatarIconDisable();
			break;
		case 6:
			_makeOverStudio.FocusOnClothes();
			_makeOverStudio.AvatarIconDisable();
			break;
		case 7:
			_makeOverStudio.FocusOnShoes();
			_makeOverStudio.AvatarIconDisable();
			break;
		case 2:
			_makeOverStudio.FocusOnFace();
			_makeOverStudio.AvatarIconShowEyes();
			break;
		case 3:
			_makeOverStudio.FocusOnFace();
			_makeOverStudio.AvatarIconShowMouths();
			break;
		case 4:
			_makeOverStudio.FocusOnHair();
			_makeOverStudio.AvatarIconDisable();
			break;
		}
		milMo_Button.SetDefaultColor(1f, 1f, 1f, 1f);
		StartFade();
		ActivePage--;
		_makeOverStudio.ScheduleAutoEmote();
		_makeOverStudio.PlayIdleAnimation();
	}

	private void SelectPage(int page)
	{
		SelectPage(page, (double)(Time.time - _lastSelectPageSound) > 0.05);
	}

	private void SelectPage(int page, bool playSound)
	{
		MilMo_Button milMo_Button = null;
		foreach (MilMo_Button pageButton in _pageButtons)
		{
			if (pageButton.Info == page)
			{
				milMo_Button = pageButton;
			}
		}
		if (milMo_Button != null)
		{
			SelectPage(milMo_Button, playSound);
		}
	}
}
