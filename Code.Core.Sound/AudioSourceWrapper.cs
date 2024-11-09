using Code.Core.Utility;
using Core.Settings;
using UnityEngine;

namespace Code.Core.Sound;

public class AudioSourceWrapper : MonoBehaviour
{
	private float _volume = 1f;

	public AudioSource Source { get; private set; }

	public float Volume
	{
		get
		{
			return _volume;
		}
		set
		{
			_volume = value;
			if ((bool)Source)
			{
				Source.volume = _volume * Settings.SFXVolume;
			}
		}
	}

	public bool PlayOnAwake
	{
		get
		{
			if ((bool)Source)
			{
				return Source.playOnAwake;
			}
			return false;
		}
		set
		{
			if ((bool)Source)
			{
				Source.playOnAwake = value;
			}
		}
	}

	public float Pitch
	{
		get
		{
			if (!Source)
			{
				return 0f;
			}
			return Source.pitch;
		}
		set
		{
			if ((bool)Source)
			{
				Source.pitch = value;
			}
		}
	}

	public bool Loop
	{
		get
		{
			if ((bool)Source)
			{
				return Source.loop;
			}
			return false;
		}
		set
		{
			if ((bool)Source)
			{
				Source.loop = value;
			}
		}
	}

	public AudioClip Clip
	{
		get
		{
			if (!Source)
			{
				return null;
			}
			return Source.clip;
		}
		set
		{
			if ((bool)Source)
			{
				Source.clip = value;
			}
		}
	}

	public AudioRolloffMode RolloffMode
	{
		get
		{
			if (!Source)
			{
				return AudioRolloffMode.Linear;
			}
			return Source.rolloffMode;
		}
		set
		{
			if ((bool)Source)
			{
				Source.rolloffMode = value;
			}
		}
	}

	public float MinDistance
	{
		set
		{
			if ((bool)Source)
			{
				Source.minDistance = value;
			}
		}
	}

	public float MaxDistance
	{
		get
		{
			if (!Source)
			{
				return 1f;
			}
			return Source.maxDistance;
		}
		set
		{
			if ((bool)Source)
			{
				Source.maxDistance = value;
			}
		}
	}

	public float SpatialBlend
	{
		set
		{
			if ((bool)Source)
			{
				Source.spatialBlend = value;
			}
		}
	}

	public float Time
	{
		get
		{
			if (!Source)
			{
				return 0f;
			}
			return Source.time;
		}
		set
		{
			if ((bool)Source)
			{
				Source.time = value;
			}
		}
	}

	private void Awake()
	{
		CreateAudioSource();
	}

	private void CreateAudioSource()
	{
		if (!Source)
		{
			Source = base.gameObject.AddComponent<AudioSource>();
			Source.volume = _volume * Settings.SFXVolume;
		}
	}

	public bool IsPlaying()
	{
		if ((bool)Source)
		{
			return Source.isPlaying;
		}
		return false;
	}

	public void Play()
	{
		if ((bool)Source)
		{
			Source.volume = _volume * Settings.SFXVolume;
			Source.Play();
		}
	}

	public void Play(MilMo_AudioClip audioClip)
	{
		if ((bool)Source && audioClip != null)
		{
			Source.clip = audioClip.AudioClip;
			if (!Source.clip)
			{
				Debug.LogWarning("AudioSourceWrapper->Play: No such sound.");
				return;
			}
			Source.volume = audioClip.Volume * Settings.SFXVolume;
			MilMo_AudioUtils.SetRollOffFactor(this, audioClip.Rolloff);
			Source.pitch = audioClip.Pitch;
			Source.loop = audioClip.Looping;
			Source.Play();
		}
	}

	public void Pause()
	{
		if ((bool)Source)
		{
			Source.Pause();
		}
	}

	public void Stop()
	{
		if ((bool)Source)
		{
			Source.Stop();
		}
	}
}
