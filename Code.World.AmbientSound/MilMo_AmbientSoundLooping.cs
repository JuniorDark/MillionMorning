using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.AmbientSound;

public class MilMo_AmbientSoundLooping : MilMo_AmbientSound
{
	private AudioClip _clip;

	private bool _playing;

	public MilMo_AmbientSoundLooping(int channel)
		: base(channel)
	{
	}

	protected override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("Sound"))
		{
			string @string = file.GetString();
			LoadAndPlay(@string);
			return true;
		}
		return base.ReadLine(file);
	}

	private async void LoadAndPlay(string path)
	{
		_clip = await MilMo_ResourceManager.Instance.LoadAudioAsync(path, "Level", MilMo_ResourceManager.Priority.Low);
		PlayInternal();
	}

	public override void Play()
	{
		_playing = true;
		PlayInternal();
	}

	private void PlayInternal()
	{
		if (_playing && !(_clip == null) && !(base.Wrapper == null))
		{
			base.Wrapper.Loop = true;
			base.Wrapper.Clip = _clip;
			base.Wrapper.Play();
		}
	}
}
