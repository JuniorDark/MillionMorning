using System.Collections.Generic;
using System.Linq;
using Code.Apps.Fade;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Input;
using Code.Core.Music;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.Slides;
using Core.Analytics;
using Core.Input;
using UnityEngine;

namespace Code.World.GUI.Slides;

public sealed class MilMo_SlidesView : MilMo_Widget
{
	private readonly MilMo_EventSystem.MilMo_Callback _mAllDoneCallback;

	private MilMo_EventSystem.MilMo_Callback _mTextDoneCallback;

	private MilMo_EventSystem.MilMo_Callback _mFadeDoneCallback;

	private readonly List<MilMo_TimerEvent> _mSoundEvents;

	private MilMo_TimerEvent _mFadeInEvent;

	private MilMo_Widget _mFade;

	private MilMo_Widget _mBlackBackground;

	private MilMo_Button _mSkipButton;

	private MilMo_Widget _mTxt;

	private MilMo_Widget _mSlide;

	private readonly List<MilMo_Slide> _mSlides;

	private int _mCurrentSlide;

	private MilMo_TimerEvent _mTextEvent;

	private MilMo_TimerEvent _mFadeTextEvent;

	private MilMo_TimerEvent _mAutoCloseEvent;

	private int _mTextLen;

	private bool _mTextAudioPlaying;

	private bool _mTextIsDone;

	private bool _mLoadIsDone;

	private bool _mAllowNext;

	private AudioClip _mTextClip;

	private AudioSourceWrapper _mTextAudioSource;

	public MilMo_SlidesView(MilMo_UserInterface ui, MilMo_SlidesTemplate template, MilMo_EventSystem.MilMo_Callback callback)
		: base(ui)
	{
		MilMo_SlidesTemplate mTemplate = template;
		LoadTextClipSoundAsync();
		_mTextAudioSource = MilMo_Global.AudioListener.AddComponent<AudioSourceWrapper>();
		_mSoundEvents = new List<MilMo_TimerEvent>();
		MilMo_Fade.Instance.FadeInAll();
		_mAllDoneCallback = callback;
		if (!string.IsNullOrEmpty(template.Music))
		{
			MilMo_Music.Instance.FadeIn(template.Music);
		}
		CreateViewWidgets();
		_mSlides = new List<MilMo_Slide>();
		foreach (MilMo_SlideData slide in template.Slides)
		{
			_mSlides.Add(new MilMo_Slide(slide));
		}
		_mCurrentSlide = -1;
		_mSlides[0].Load(delegate
		{
			MilMo_Fade.Instance.FadeOutAll();
			Fade(mTemplate.FirstFadeColor, mTemplate.FirstFadeTime, delegate
			{
				ShowSlide("NEXT");
			});
		});
		UI.AddChild(this);
	}

	private async void LoadTextClipSoundAsync()
	{
		_mTextClip = await MilMo_ResourceManager.Instance.LoadAudioAsync("Content/Sounds/Batch01/GUI/TextTick");
	}

	private bool SetTextIsDone()
	{
		_mTextIsDone = true;
		if (_mLoadIsDone)
		{
			return _mTextIsDone;
		}
		return false;
	}

	private bool SetLoadIsDone()
	{
		_mLoadIsDone = true;
		if (_mLoadIsDone)
		{
			return _mTextIsDone;
		}
		return false;
	}

	private void HideText()
	{
		_mTxt.SetText(MilMo_LocString.Empty);
		_mTextLen = -1;
	}

	private void StartText(string text, MilMo_EventSystem.MilMo_Callback callback)
	{
		if (string.IsNullOrEmpty(text))
		{
			callback();
			return;
		}
		_mTextDoneCallback = callback;
		_mTextLen = 0;
		if (_mTextAudioSource != null && !_mTextAudioSource.IsPlaying() && _mTextClip != null)
		{
			_mTextAudioSource.Loop = true;
			_mTextAudioSource.Clip = _mTextClip;
			_mTextAudioSource.Play();
			_mTextAudioPlaying = true;
		}
		SetText(text);
	}

	private void SetText(string text)
	{
		if (_mTextLen > text.Length)
		{
			if (_mTextAudioSource != null)
			{
				_mTextAudioSource.Loop = false;
				_mTextAudioSource.Stop();
			}
			_mTextAudioPlaying = false;
			_mTextDoneCallback();
		}
		else
		{
			string textNoLocalization = text.Substring(0, _mTextLen);
			_mTxt.SetTextNoLocalization(textNoLocalization);
			_mTextLen++;
			_mTextEvent = MilMo_EventSystem.At(0.025f, delegate
			{
				SetText(text);
			});
		}
	}

	private void ShowSlide(object o)
	{
		AllowNext(allowNext: false);
		foreach (MilMo_TimerEvent mSoundEvent in _mSoundEvents)
		{
			MilMo_EventSystem.RemoveTimerEvent(mSoundEvent);
		}
		if (_mAutoCloseEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mAutoCloseEvent);
			_mAutoCloseEvent = null;
		}
		if (_mFadeTextEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mFadeTextEvent);
			_mFadeTextEvent = null;
		}
		if (_mTextEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mTextEvent);
		}
		if (_mTextAudioSource != null)
		{
			_mTextAudioSource.Loop = false;
			_mTextAudioSource.Stop();
		}
		_mTextIsDone = false;
		_mLoadIsDone = false;
		if (o as string == "NEXT")
		{
			_mCurrentSlide++;
		}
		else
		{
			_mCurrentSlide--;
		}
		SetBorderColor(_mSlides[_mCurrentSlide].Template.BorderColor);
		_mTxt.SetDefaultTextColor(_mSlides[_mCurrentSlide].Template.TextColor);
		if (_mCurrentSlide < _mSlides.Count - 1)
		{
			_mSlides[_mCurrentSlide + 1].Load(delegate
			{
				AllowNext(allowNext: true);
				if (SetLoadIsDone() && _mAutoCloseEvent == null)
				{
					_mAutoCloseEvent = MilMo_EventSystem.At(_mSlides[_mCurrentSlide].Template.CloseDelay, delegate
					{
						_mAutoCloseEvent = null;
						HideText();
						Fade(_mSlides[_mCurrentSlide].Template.FadeColor, _mSlides[_mCurrentSlide].Template.FadeTime, delegate
						{
							ShowSlide("NEXT");
						});
					});
				}
			});
		}
		else
		{
			AllowNext(allowNext: true);
			SetLoadIsDone();
		}
		foreach (MilMo_Slide.Sound sound2 in _mSlides[_mCurrentSlide].Sounds)
		{
			MilMo_Slide.Sound sound = sound2;
			MilMo_TimerEvent soundEvent = null;
			soundEvent = MilMo_EventSystem.At(sound.MDelay, delegate
			{
				_mSoundEvents.Remove(soundEvent);
				MilMo_GuiSoundManager.Instance.PlaySoundFx(sound.MClip);
			});
			_mSoundEvents.Add(soundEvent);
		}
		if (!string.IsNullOrEmpty(_mSlides[_mCurrentSlide].Template.MusicPath))
		{
			MilMo_Music.Instance.FadeIn(_mSlides[_mCurrentSlide].Template.MusicPath);
		}
		StartText(_mSlides[_mCurrentSlide].Template.Text.String, delegate
		{
			if (SetTextIsDone() && _mAutoCloseEvent == null)
			{
				if (_mCurrentSlide >= _mSlides.Count - 1)
				{
					_mAutoCloseEvent = MilMo_EventSystem.At(_mSlides[_mCurrentSlide].Template.CloseDelay, delegate
					{
						_mAutoCloseEvent = null;
						MilMo_Music.Instance.StopCurrent();
						if (_mTextAudioSource != null)
						{
							_mTextAudioSource.Loop = false;
							_mTextAudioSource.Stop();
						}
						Fade(_mSlides[_mCurrentSlide].Template.FadeColor, _mSlides[_mCurrentSlide].Template.FadeTime, Done);
					});
				}
				else
				{
					_mFadeTextEvent = MilMo_EventSystem.At(_mSlides[_mCurrentSlide].Template.CloseDelay - 1f, delegate
					{
						_mFadeTextEvent = null;
						Color textColor = _mSlides[_mCurrentSlide].Template.TextColor;
						_mTxt.SetDefaultTextColor(textColor.r, textColor.g, textColor.b, 0f);
						_mTxt.SetTextColor(textColor.r, textColor.g, textColor.b, 1f);
					});
					_mAutoCloseEvent = MilMo_EventSystem.At(_mSlides[_mCurrentSlide].Template.CloseDelay, delegate
					{
						_mAutoCloseEvent = null;
						HideText();
						Fade(_mSlides[_mCurrentSlide].Template.FadeColor, _mSlides[_mCurrentSlide].Template.FadeTime, delegate
						{
							ShowSlide("NEXT");
						});
					});
				}
			}
		});
		if (_mSlides[_mCurrentSlide].Texture != null)
		{
			_mSlide.SetTexture(_mSlides[_mCurrentSlide].Texture);
			RefreshUI();
		}
		else
		{
			_mSlide.SetTextureInvisible();
		}
	}

	private void AllowNext(bool allowNext)
	{
		_mAllowNext = allowNext;
	}

	private void Done()
	{
		if (_mTextEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mTextEvent);
		}
		if (_mFadeTextEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mFadeTextEvent);
		}
		if (_mAutoCloseEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mAutoCloseEvent);
		}
		if (_mFadeInEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mFadeInEvent);
		}
		foreach (MilMo_TimerEvent item in _mSoundEvents.Where((MilMo_TimerEvent e) => e != null))
		{
			MilMo_EventSystem.RemoveTimerEvent(item);
		}
		foreach (MilMo_Slide mSlide in _mSlides)
		{
			mSlide.Destroy();
		}
		_mTextEvent = null;
		_mFadeTextEvent = null;
		_mAutoCloseEvent = null;
		_mFadeInEvent = null;
		MilMo_Global.Destroy(_mTextAudioSource);
		_mTextAudioSource = null;
		if (_mAllDoneCallback != null)
		{
			_mAllDoneCallback();
		}
	}

	private void Fade(Color color, float time, MilMo_EventSystem.MilMo_Callback callback)
	{
		_mFadeDoneCallback = callback;
		_mFade.SetColor(color);
		_mFade.SetAlpha(0f);
		UI.BringToFront(_mFade);
		_mFade.AlphaTo(1f);
		_mFadeInEvent = MilMo_EventSystem.At(time, delegate
		{
			_mFadeInEvent = null;
			_mFade.AlphaTo(0f);
			if (_mFadeDoneCallback != null)
			{
				_mFadeDoneCallback();
			}
		});
	}

	private void SetBorderColor(Color color)
	{
		_mBlackBackground.SetDefaultColor(color);
	}

	private void CreateViewWidgets()
	{
		_mBlackBackground = new MilMo_Widget(UI);
		_mBlackBackground.SetTextureWhite();
		_mBlackBackground.SetDefaultColor(Color.black);
		_mBlackBackground.SetScale(Screen.width, Screen.height);
		_mBlackBackground.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_mBlackBackground.SetPosition(0f, (float)Screen.height * 0.5f);
		UI.AddChild(_mBlackBackground);
		_mFade = new MilMo_Widget(UI);
		_mFade.SetTextureWhite();
		_mFade.SetAlpha(0f);
		_mFade.FadeToDefaultColor = false;
		_mFade.AllowPointerFocus = false;
		_mFade.SetAlignment(MilMo_GUI.Align.TopLeft);
		UI.AddChild(_mFade);
		_mSlide = new MilMo_Widget(UI);
		_mSlide.SetAlignment(MilMo_GUI.Align.TopCenter);
		UI.AddChild(_mSlide);
		_mSkipButton = new MilMo_Button(UI);
		_mSkipButton.SetAlignment(MilMo_GUI.Align.BottomRight);
		_mSkipButton.SetTexture("Batch01/Textures/Dialog/ButtonMO");
		_mSkipButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonBright");
		_mSkipButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonBright");
		_mSkipButton.SetText(MilMo_Localization.GetLocString("Generic_92"));
		_mSkipButton.AllowPointerFocus = true;
		_mSkipButton.Enabled = true;
		_mSkipButton.Function = delegate
		{
			if (_mTextAudioSource != null && _mTextAudioPlaying)
			{
				_mTextAudioSource.Stop();
			}
			MilMo_Music.Instance.StopCurrent();
			Analytics.CutsceneSkip("Intro");
			Done();
		};
		UI.AddChild(_mSkipButton);
		_mTxt = new MilMo_Widget(UI);
		_mTxt.SetWordWrap(w: true);
		_mTxt.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		_mTxt.SetAlignment(MilMo_GUI.Align.BottomCenter);
		_mTxt.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_mTxt.SetFont(MilMo_GUI.Font.EborgLarge);
		_mTxt.SetFadeSpeed(0.015f);
		UI.AddChild(_mTxt);
		RefreshUI();
	}

	public override void Draw()
	{
		if (UI.ScreenSizeDirty)
		{
			RefreshUI();
		}
		base.Draw();
	}

	public override void Step()
	{
		if (MilMo_Pointer.LeftClick && _mAllowNext && InputSwitch.MousePosition.x >= 0f && InputSwitch.MousePosition.x < (float)Screen.width && InputSwitch.MousePosition.y >= 0f && InputSwitch.MousePosition.y < (float)Screen.height)
		{
			if (_mFadeInEvent != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(_mFadeInEvent);
				_mFadeInEvent = null;
				_mFade.SetAlpha(0f);
				if (_mFadeDoneCallback != null)
				{
					_mFadeDoneCallback();
					_mFadeDoneCallback = null;
				}
			}
			else if (!_mTextIsDone)
			{
				if (_mTextEvent != null)
				{
					MilMo_EventSystem.RemoveTimerEvent(_mTextEvent);
				}
				_mTxt.SetTextNoLocalization(_mSlides[_mCurrentSlide].Template.Text.String);
				_mTextLen = _mSlides[_mCurrentSlide].Template.Text.Length + 1;
				SetText(_mSlides[_mCurrentSlide].Template.Text.String);
			}
			else if (_mCurrentSlide >= _mSlides.Count - 1)
			{
				MilMo_Music.Instance.StopCurrent();
				if (_mTextAudioSource != null)
				{
					_mTextAudioSource.Loop = false;
					_mTextAudioSource.Stop();
				}
				AllowNext(allowNext: false);
				Fade(_mSlides[_mCurrentSlide].Template.FadeColor, _mSlides[_mCurrentSlide].Template.FadeTime, Done);
			}
			else
			{
				HideText();
				ShowSlide("NEXT");
			}
		}
		base.Step();
	}

	private void RefreshUI()
	{
		float num = Mathf.Min((float)Screen.width / 1024f, 1f);
		float num2 = Mathf.Min((float)Screen.height / 720f, 1f);
		_mFade.SetPosition(0f, 0f);
		_mFade.SetScale(Screen.width, Screen.height);
		if (_mSlide.Texture != null && _mSlide.Texture.Texture != null)
		{
			float num3 = (float)Screen.height - 138f * num2 + 20f - 80f * num2;
			float x = (float)_mSlide.Texture.Texture.width * (num3 / (float)_mSlide.Texture.Texture.height);
			_mSlide.SetScaleAbsolute(x, num3);
			_mSlide.SetPosition((float)Screen.width * 0.5f, 80f * num2);
			_mBlackBackground.SetScale(Screen.width, Screen.height);
			_mBlackBackground.SetPosition(0f, (float)Screen.height * 0.5f);
		}
		UI.BringToFront(_mFade);
		_mTxt.SetScale(700f * num, 80f * num2);
		_mTxt.SetPosition((float)Screen.width / 2f, (float)Screen.height - 20f * num2);
		_mTxt.SetFontScale(num2);
		_mSkipButton.SetScale(80f, 30f);
		_mSkipButton.SetPosition(Screen.width - 20, Screen.height - 30);
		UI.BringToFront(_mSkipButton);
	}
}
