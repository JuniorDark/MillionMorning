using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using Code.Core.Utility;

namespace Code.Core.GUI;

public sealed class MilMo_WidgetAnimation
{
	private readonly List<MilMo_WidgetAnimationFrame> _frames = new List<MilMo_WidgetAnimationFrame>();

	private readonly MilMo_Widget _widget;

	private int _currentFrame;

	public MilMo_WidgetAnimation(MilMo_Widget widget)
	{
		_widget = widget;
	}

	public async void Play()
	{
		if (_widget == null || _frames.Count < 1)
		{
			return;
		}
		foreach (MilMo_WidgetAnimationFrame frame in _frames)
		{
			await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/" + frame.Texture);
		}
		AttemptPlay();
	}

	private void AttemptPlay()
	{
		DoPlay();
	}

	private void DoPlay()
	{
		_currentFrame = 0;
		StepFrame();
	}

	private void StepFrame()
	{
		_widget.SetTexture(_frames[_currentFrame].Texture);
		MilMo_EventSystem.At(((double)_frames[_currentFrame].MaxDelay == 0.0) ? _frames[_currentFrame].MinDelay : MilMo_Utility.RandomFloat(_frames[_currentFrame].MinDelay, _frames[_currentFrame].MaxDelay), delegate
		{
			if (_currentFrame < _frames.Count - 1)
			{
				_currentFrame++;
				StepFrame();
			}
			else
			{
				AttemptPlay();
			}
		});
	}

	public void AddFrame(string texture, float minDelay, float maxDelay)
	{
		MilMo_WidgetAnimationFrame item = new MilMo_WidgetAnimationFrame(texture, minDelay, maxDelay);
		_frames.Add(item);
	}
}
