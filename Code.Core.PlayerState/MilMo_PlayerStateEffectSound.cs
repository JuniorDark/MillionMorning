using System;
using Code.Core.Avatar;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using UnityEngine;

namespace Code.Core.PlayerState;

public class MilMo_PlayerStateEffectSound : MilMo_PlayerStateEffect
{
	private AudioClip _mSound;

	private readonly string _mPlayMode = "Once";

	private AudioClip _mOldDefaultClip;

	private bool _mWasLooping;

	private bool _mWasPlaying;

	public MilMo_PlayerStateEffectSound(PlayerStateEffectCosmetic effectData, MilMo_Avatar avatar)
		: base(avatar)
	{
		if (effectData.GetParameters().Count > 0)
		{
			string audioClipName = effectData.GetParameters()[0];
			if (effectData.GetParameters().Count > 1)
			{
				_mPlayMode = effectData.GetParameters()[1];
			}
			LoadSoundAsync(audioClipName);
		}
		else
		{
			Debug.LogWarning("Missing parameter in \"Sound\" player state effect");
		}
	}

	private async void LoadSoundAsync(string audioClipName)
	{
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(audioClipName);
		if (audioClip == null)
		{
			Debug.LogWarning("Failed to load audio clip " + audioClipName + " for player state effect.");
		}
		else
		{
			_mSound = audioClip;
		}
	}

	public override void Activate()
	{
		if (Avatar != null && !(_mSound == null) && !(Avatar.GameObject == null) && !(Avatar.GameObject.GetComponent<AudioSourceWrapper>() == null))
		{
			Avatar.GameObject.GetComponent<AudioSourceWrapper>().enabled = true;
			if (_mPlayMode.Equals("Loop", StringComparison.InvariantCultureIgnoreCase))
			{
				_mOldDefaultClip = Avatar.GameObject.GetComponent<AudioSourceWrapper>().Clip;
				_mWasLooping = Avatar.GameObject.GetComponent<AudioSourceWrapper>().Loop;
				_mWasPlaying = Avatar.GameObject.GetComponent<AudioSourceWrapper>().IsPlaying();
				Avatar.GameObject.GetComponent<AudioSourceWrapper>().Loop = true;
			}
			else
			{
				Avatar.GameObject.GetComponent<AudioSourceWrapper>().Loop = false;
			}
			Avatar.GameObject.GetComponent<AudioSourceWrapper>().Clip = _mSound;
			Avatar.GameObject.GetComponent<AudioSourceWrapper>().Play();
		}
	}

	public override void Deactivate()
	{
		if (_mPlayMode.Equals("Loop", StringComparison.InvariantCultureIgnoreCase))
		{
			Avatar.GameObject.GetComponent<AudioSourceWrapper>().Stop();
			Avatar.GameObject.GetComponent<AudioSourceWrapper>().Clip = _mOldDefaultClip;
			Avatar.GameObject.GetComponent<AudioSourceWrapper>().Loop = _mWasLooping;
			if (_mOldDefaultClip != null && _mWasPlaying)
			{
				Avatar.GameObject.GetComponent<AudioSourceWrapper>().Play();
			}
		}
	}
}
