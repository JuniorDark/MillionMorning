using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Sound;

public class MilMo_AudioClip
{
	private static readonly List<MilMo_AudioClip> ClipsToLoad = new List<MilMo_AudioClip>();

	private static MilMo_GenericReaction _theUpdateReaction;

	private readonly string _filename;

	public float Volume;

	public float Pitch;

	public readonly bool Looping;

	public readonly float Rolloff;

	public AudioClip AudioClip { get; private set; }

	public MilMo_SoundType MilMoSoundType { get; private set; }

	public MilMo_AudioClip(string filename, MilMo_SoundType type = MilMo_SoundType.None)
	{
		_filename = filename;
		MilMoSoundType = type;
		Volume = 1f;
		Pitch = 1f;
		Looping = false;
		Rolloff = 0.1f;
		ClipsToLoad.Add(this);
		if (_theUpdateReaction == null)
		{
			_theUpdateReaction = MilMo_EventSystem.RegisterUpdate(Update);
		}
	}

	private static void Update(object obj)
	{
		foreach (MilMo_AudioClip item in ClipsToLoad)
		{
			item.AsyncLoad();
		}
		ClipsToLoad.Clear();
	}

	private async void AsyncLoad()
	{
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(_filename);
		if (audioClip == null)
		{
			Debug.LogWarning("MilMo_AudioClip: No such sound " + _filename);
		}
		else
		{
			AudioClip = audioClip;
		}
	}

	public void Destroy()
	{
		if ((bool)AudioClip)
		{
			MilMo_ResourceManager.Instance.UnloadAsset(_filename);
			AudioClip = null;
		}
	}

	public void SetVolume(float vol)
	{
		Volume = vol;
	}

	public void SetPitch(float pitch)
	{
		Pitch = pitch;
	}
}
