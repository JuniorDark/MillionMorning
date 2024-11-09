using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Sound;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public class MilMo_SoundSubEffect : MilMo_SubEffect
{
	private bool _didPlaySound;

	private readonly MilMo_SoundAction _soundAction;

	private readonly AudioSourceWrapper _audioSourceWrapper;

	public MilMo_SoundSubEffect(MilMo_SoundAction action, Vector3 position)
	{
		_soundAction = action;
		base.EmittingObject = new GameObject(action.Name);
		_audioSourceWrapper = base.EmittingObject.AddComponent<AudioSourceWrapper>();
		base.EmittingObject.transform.position = position;
		IsStopped = false;
	}

	public MilMo_SoundSubEffect(MilMo_SoundAction action, GameObject parent, Vector3 dynamicOffset)
	{
		_soundAction = action;
		base.EmittingObject = new GameObject(action.Name);
		_audioSourceWrapper = base.EmittingObject.AddComponent<AudioSourceWrapper>();
		base.EmittingObject.transform.parent = parent.transform;
		base.EmittingObject.transform.localPosition = dynamicOffset;
		IsStopped = false;
	}

	public MilMo_SoundSubEffect(MilMo_SoundAction action, GameObject parent, float staticYPos)
	{
		_soundAction = action;
		base.EmittingObject = new GameObject(action.Name);
		_audioSourceWrapper = base.EmittingObject.AddComponent<AudioSourceWrapper>();
		YLocked = true;
		YPos = staticYPos;
		if (parent != null)
		{
			base.EmittingObject.transform.parent = parent.transform;
			Vector3 localPosition = base.EmittingObject.transform.localPosition;
			localPosition.y = parent.transform.position.y - staticYPos;
			base.EmittingObject.transform.localPosition = localPosition;
		}
		else
		{
			Vector3 position = base.EmittingObject.transform.position;
			position.y = staticYPos;
			base.EmittingObject.transform.position = position;
		}
		IsStopped = false;
	}

	public override bool Update()
	{
		if (base.EmittingObject == null)
		{
			return false;
		}
		TryPlaySound();
		if (YLocked)
		{
			Vector3 position = base.EmittingObject.transform.position;
			position.y = YPos;
			base.EmittingObject.transform.position = position;
		}
		CurrentTime += Time.deltaTime;
		if (!IsStopped && CurrentTime < Duration)
		{
			return true;
		}
		Destroy();
		return false;
	}

	public override void Stop()
	{
		if ((bool)_audioSourceWrapper)
		{
			_audioSourceWrapper.Stop();
		}
		IsStopped = true;
	}

	public override void DestroyWhenDone()
	{
		MilMo_EventSystem.At(Duration, delegate
		{
			Stop();
			MilMo_Global.Destroy(base.EmittingObject);
		});
	}

	private void TryPlaySound()
	{
		if (_didPlaySound || IsStopped || !_audioSourceWrapper)
		{
			return;
		}
		MilMo_AudioUtils.SetRollOffFactor(_audioSourceWrapper, _soundAction.RolloffFactor);
		AudioClip sound = _soundAction.Sound;
		if (sound != null)
		{
			if (_soundAction.Looping)
			{
				Duration = _soundAction.Duration;
				_audioSourceWrapper.Loop = true;
			}
			else
			{
				Duration = sound.length;
				_audioSourceWrapper.Loop = false;
			}
			_audioSourceWrapper.Clip = sound;
			_audioSourceWrapper.Play();
		}
		else
		{
			if (_soundAction.Looping)
			{
				Duration = _soundAction.Duration;
				return;
			}
			Duration = 0f;
		}
		_didPlaySound = true;
	}
}
