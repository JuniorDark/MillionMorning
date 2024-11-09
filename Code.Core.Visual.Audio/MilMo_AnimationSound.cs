using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using UnityEngine;

namespace Code.Core.Visual.Audio;

public class MilMo_AnimationSound
{
	private const float SYNC_TOLERANCE = 0.1f;

	private const float SYNC_INTERVAL = 1f;

	private AudioClip _clip;

	private bool _isPlaying;

	public float Tiling { get; private set; }

	public string Path { get; private set; }

	public string Animation { get; private set; }

	public MilMo_AnimationSound()
	{
		Animation = "";
		Tiling = 1f;
	}

	public bool Read(MilMo_SFFile file)
	{
		Animation = file.GetString();
		Path = file.GetString();
		if (file.HasMoreTokens())
		{
			Tiling = file.GetFloat();
		}
		return true;
	}

	public void Write(MilMo_SFFile file)
	{
		file.AddRow();
		file.Write("AnimationSound");
		file.Write(Animation);
		file.Write(Path);
		file.Write(Tiling);
	}

	public void Play(AudioSourceWrapper audioSourceWrapper, Animation animation)
	{
		if (!_isPlaying)
		{
			_isPlaying = true;
			if (!_clip && !string.IsNullOrEmpty(Path))
			{
				LoadAndPlayAsync(audioSourceWrapper, animation);
			}
			else if ((bool)_clip)
			{
				PlayClip(audioSourceWrapper, animation);
			}
		}
	}

	private async void LoadAndPlayAsync(AudioSourceWrapper audioSourceWrapper, Animation animation)
	{
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(Path);
		if (!(audioClip == null))
		{
			_clip = audioClip;
			if (_isPlaying)
			{
				PlayClip(audioSourceWrapper, animation);
			}
		}
	}

	public void Stop(AudioSourceWrapper audioSourceWrapper)
	{
		_isPlaying = false;
		if ((bool)audioSourceWrapper && (bool)audioSourceWrapper.Clip && (bool)_clip && audioSourceWrapper.Clip.name.Equals(_clip.name))
		{
			audioSourceWrapper.Pause();
		}
	}

	public bool Equals(MilMo_AnimationSound animSound)
	{
		if (animSound == null)
		{
			return false;
		}
		if (Animation == animSound.Animation && Path == animSound.Path)
		{
			return Tiling == animSound.Tiling;
		}
		return false;
	}

	private void PlayClip(AudioSourceWrapper audioSourceWrapper, Animation animation)
	{
		if ((bool)audioSourceWrapper && (bool)_clip)
		{
			audioSourceWrapper.Clip = _clip;
			float time = 0f;
			if (animation[Animation] != null && animation[Animation].wrapMode == WrapMode.Loop)
			{
				audioSourceWrapper.Loop = true;
				time = Mathf.Repeat(animation[Animation].time, animation[Animation].length) / Tiling;
				time = Mathf.Clamp(time, 0f, _clip.length);
				ScheduleSync(audioSourceWrapper, animation);
			}
			else
			{
				audioSourceWrapper.Loop = false;
			}
			audioSourceWrapper.Time = time;
			audioSourceWrapper.Play();
		}
	}

	private void Synchronize(AudioSourceWrapper audioSourceWrapper, Animation animation)
	{
		if (!_isPlaying || audioSourceWrapper == null || animation == null || _clip == null || animation[Animation] == null || animation[Animation].wrapMode != WrapMode.Loop)
		{
			return;
		}
		if (IsVisible(animation))
		{
			float value = Mathf.Repeat(animation[Animation].time, animation[Animation].length) / Tiling;
			value = Mathf.Clamp(value, 0f, _clip.length);
			if (Mathf.Abs(value - audioSourceWrapper.Time) >= 0.1f)
			{
				audioSourceWrapper.Time = value;
			}
		}
		ScheduleSync(audioSourceWrapper, animation);
	}

	private void ScheduleSync(AudioSourceWrapper audioSourceWrapper, Animation animation)
	{
		MilMo_EventSystem.At(1f, delegate
		{
			Synchronize(audioSourceWrapper, animation);
		});
	}

	private static bool IsVisible(Component animation)
	{
		if (animation == null)
		{
			return false;
		}
		Renderer componentInChildren = animation.gameObject.GetComponentInChildren<Renderer>();
		if (componentInChildren != null)
		{
			return componentInChildren.isVisible;
		}
		return false;
	}
}
