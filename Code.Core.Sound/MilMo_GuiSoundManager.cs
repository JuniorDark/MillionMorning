using System.Collections.Generic;
using System.Linq;
using Code.Core.Global;
using UnityEngine;

namespace Code.Core.Sound;

public class MilMo_GuiSoundManager
{
	private static MilMo_GuiSoundManager _instance;

	private static List<MilMo_AudioClip> _audioClips;

	private static AudioSourceWrapper _soundFx;

	public static MilMo_GuiSoundManager Instance => _instance ?? (_instance = new MilMo_GuiSoundManager());

	private MilMo_GuiSoundManager()
	{
		if (MilMo_Global.AudioListener != null)
		{
			_soundFx = MilMo_Global.AudioListener.AddComponent<AudioSourceWrapper>();
		}
		_audioClips = new List<MilMo_AudioClip>();
		PreLoadDefaultSounds();
	}

	public void PlaySoundFx(MilMo_SoundType milMoSoundType)
	{
		_soundFx.Loop = false;
		foreach (MilMo_AudioClip item in _audioClips.Where((MilMo_AudioClip clip) => clip.MilMoSoundType == milMoSoundType))
		{
			_soundFx.Clip = item.AudioClip;
			_soundFx.Play();
		}
	}

	public void PlaySoundFx(AudioClip clip)
	{
		_soundFx.Loop = false;
		_soundFx.Clip = clip;
		_soundFx.Play();
	}

	public static void AddClip(string path, MilMo_SoundType milMoSoundType)
	{
		MilMo_AudioClip item = new MilMo_AudioClip(path, milMoSoundType);
		_audioClips.Add(item);
	}

	private static void PreLoadDefaultSounds()
	{
		foreach (KeyValuePair<string, MilMo_SoundType> path in MilMo_AudioClipLibrary.Paths)
		{
			AddClip(path.Key, path.Value);
		}
	}
}
