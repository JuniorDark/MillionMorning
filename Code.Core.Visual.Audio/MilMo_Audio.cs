using System;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Audio;

public class MilMo_Audio
{
	private string _mAudioClip;

	private bool _mLoop = true;

	private float _mPitch = 1f;

	private bool _mPlayOnAwake = true;

	private float _mRolloffFactor = 0.1f;

	private float _mVolume = 1f;

	public void Load(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</AUDIOSOURCE>"))
		{
			if (file.IsNext("SoundFile"))
			{
				_mAudioClip = file.GetString();
			}
			else if (file.IsNext("Loop"))
			{
				_mLoop = file.GetBool();
			}
			else if (file.IsNext("Pitch"))
			{
				_mPitch = file.GetFloat();
			}
			else if (file.IsNext("PlayOnAwake"))
			{
				_mPlayOnAwake = file.GetBool();
			}
			else if (file.IsNext("RolloffFactor"))
			{
				_mRolloffFactor = file.GetFloat();
			}
			else if (file.IsNext("Volume"))
			{
				_mVolume = file.GetFloat();
			}
		}
	}

	public void Create(GameObject gameObject)
	{
		Create(gameObject, MilMo_ResourceManager.Priority.High);
	}

	public AudioSourceWrapper Create(GameObject gameObject, MilMo_ResourceManager.Priority priority)
	{
		if (gameObject == null)
		{
			return null;
		}
		AudioSourceWrapper audioSourceWrapper = gameObject.GetComponent<AudioSourceWrapper>();
		if (audioSourceWrapper == null)
		{
			audioSourceWrapper = gameObject.AddComponent<AudioSourceWrapper>();
		}
		audioSourceWrapper.Loop = _mLoop;
		MilMo_AudioUtils.SetRollOffFactor(audioSourceWrapper, _mRolloffFactor);
		audioSourceWrapper.Pitch = _mPitch;
		audioSourceWrapper.PlayOnAwake = _mPlayOnAwake;
		audioSourceWrapper.Volume = _mVolume;
		if (!string.IsNullOrEmpty(_mAudioClip))
		{
			LoadAndSetClip(audioSourceWrapper, priority);
		}
		return audioSourceWrapper;
	}

	private async void LoadAndSetClip(AudioSourceWrapper audioSourceWrapper, MilMo_ResourceManager.Priority priority)
	{
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(_mAudioClip, "Generic", priority);
		if (audioClip == null)
		{
			Debug.LogWarning("Failed to load audio clip " + _mAudioClip);
		}
		else if (!(audioSourceWrapper == null))
		{
			audioSourceWrapper.Clip = audioClip;
		}
	}

	public static void Write(AudioSourceWrapper audioSource, MilMo_SFFile file, MilMo_Audio template)
	{
		if (audioSource == null || audioSource.Clip == null)
		{
			return;
		}
		string text = MilMo_ResourceManager.Instance.ResolveAudioClipPath(audioSource.Clip);
		if (string.IsNullOrEmpty(text) || (template != null && !template.Equals(audioSource, text)))
		{
			return;
		}
		file.AddRow();
		file.Write("<AUDIOSOURCE>");
		try
		{
			if (template == null || text != template._mAudioClip)
			{
				file.AddRow();
				file.Write("SoundFile");
				file.Write(text);
			}
			if (template == null || audioSource.Loop != template._mLoop)
			{
				file.AddRow();
				file.Write("Loop");
				file.Write(audioSource.Loop);
			}
			if (template == null || (double)Math.Abs(audioSource.Pitch - template._mPitch) > 0.0)
			{
				file.AddRow();
				file.Write("Pitch");
				file.Write(audioSource.Pitch);
			}
			if (template == null || audioSource.PlayOnAwake != template._mPlayOnAwake)
			{
				file.AddRow();
				file.Write("PlayOnAwake");
				file.Write(audioSource.PlayOnAwake);
			}
			if (template == null || (double)Math.Abs(audioSource.MaxDistance - MilMo_AudioUtils.RollOffToDistance(template._mRolloffFactor)) > 0.0)
			{
				file.AddRow();
				file.Write("RolloffFactor");
				file.Write(MilMo_AudioUtils.GetRollOffFactorFromDistance(audioSource.MaxDistance));
			}
			if (template == null || (double)Math.Abs(audioSource.Volume - template._mVolume) > 0.0)
			{
				file.AddRow();
				file.Write("Volume");
				file.Write(audioSource.Volume);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to write audio source");
			Debug.LogWarning(ex.ToString());
		}
		file.AddRow();
		file.Write("</AUDIOSOURCE>");
	}

	private bool Equals(AudioSourceWrapper audioSource, string path)
	{
		if (audioSource == null)
		{
			return false;
		}
		if (_mAudioClip == path && _mLoop == audioSource.Loop && Math.Abs(_mPitch - audioSource.Pitch) < 0.01f && _mPlayOnAwake == audioSource.PlayOnAwake && Math.Abs(_mRolloffFactor - MilMo_AudioUtils.GetRollOffFactorFromDistance(audioSource.MaxDistance)) < 0.01f)
		{
			return Math.Abs(_mVolume - audioSource.Volume) < 0.01f;
		}
		return false;
	}
}
