using System;
using Core.Audio.AudioData;
using Core.Audio.SoundEmitters;
using Core.GameEvent;
using Core.Settings;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio;

public class AudioManager : MonoBehaviour
{
	[Header("SoundEmitters pool")]
	[SerializeField]
	private SoundEmitterPoolSO pool;

	[SerializeField]
	private int initialSize = 10;

	[Header("Listening on channels")]
	[Tooltip("The SoundManager listens to this event, fired by objects in any scene, to play SFXs")]
	[SerializeField]
	private AudioCueEventChannelSO sfxEventChannel;

	[Tooltip("The SoundManager listens to this event, fired by objects in any scene, to play Music")]
	[SerializeField]
	private AudioCueEventChannelSO musicEventChannel;

	[Header("Audio control")]
	[SerializeField]
	private AudioMixer audioMixer;

	[Range(0f, 1f)]
	[SerializeField]
	private float masterVolume = 1f;

	[Range(0f, 1f)]
	[SerializeField]
	private float musicVolume = 1f;

	[Range(0f, 1f)]
	[SerializeField]
	private float sfxVolume = 1f;

	[Range(0f, 1f)]
	[SerializeField]
	private float ambienceVolume = 1f;

	private SoundEmitterVault _soundEmitterVault;

	private SoundEmitter _musicSoundEmitter;

	private void Awake()
	{
		_soundEmitterVault = new SoundEmitterVault();
	}

	private void Start()
	{
		pool.Prewarm(initialSize);
		pool.SetParent(base.transform);
		Core.GameEvent.GameEvent.MasterVolumeEvent.RegisterAction(ChangeMasterVolume);
		Core.GameEvent.GameEvent.SFXVolumeEvent.RegisterAction(ChangeSFXVolume);
		Core.GameEvent.GameEvent.MusicVolumeEvent.RegisterAction(ChangeMusicVolume);
		Core.GameEvent.GameEvent.AmbienceVolumeEvent.RegisterAction(ChangeAmbienceVolume);
		if (sfxEventChannel != null)
		{
			AudioCueEventChannelSO audioCueEventChannelSO = sfxEventChannel;
			audioCueEventChannelSO.OnAudioCuePlayRequested = (AudioCuePlayAction)Delegate.Combine(audioCueEventChannelSO.OnAudioCuePlayRequested, new AudioCuePlayAction(PlayAudioCue));
			AudioCueEventChannelSO audioCueEventChannelSO2 = sfxEventChannel;
			audioCueEventChannelSO2.OnAudioCueStopRequested = (AudioCueStopAction)Delegate.Combine(audioCueEventChannelSO2.OnAudioCueStopRequested, new AudioCueStopAction(StopAudioCue));
			AudioCueEventChannelSO audioCueEventChannelSO3 = sfxEventChannel;
			audioCueEventChannelSO3.OnAudioCueFinishRequested = (AudioCueFinishAction)Delegate.Combine(audioCueEventChannelSO3.OnAudioCueFinishRequested, new AudioCueFinishAction(FinishAudioCue));
		}
		if (musicEventChannel != null)
		{
			AudioCueEventChannelSO audioCueEventChannelSO4 = musicEventChannel;
			audioCueEventChannelSO4.OnAudioCuePlayRequested = (AudioCuePlayAction)Delegate.Combine(audioCueEventChannelSO4.OnAudioCuePlayRequested, new AudioCuePlayAction(PlayMusicTrack));
			AudioCueEventChannelSO audioCueEventChannelSO5 = musicEventChannel;
			audioCueEventChannelSO5.OnAudioCueStopRequested = (AudioCueStopAction)Delegate.Combine(audioCueEventChannelSO5.OnAudioCueStopRequested, new AudioCueStopAction(StopMusic));
		}
		ChangeMasterVolume(Core.Settings.Settings.MasterVolume);
		ChangeSFXVolume(Core.Settings.Settings.SFXVolume);
		ChangeMusicVolume(Core.Settings.Settings.MusicVolume);
		ChangeAmbienceVolume(Core.Settings.Settings.AmbienceVolume);
	}

	private void OnDestroy()
	{
		if (sfxEventChannel != null)
		{
			AudioCueEventChannelSO audioCueEventChannelSO = sfxEventChannel;
			audioCueEventChannelSO.OnAudioCuePlayRequested = (AudioCuePlayAction)Delegate.Remove(audioCueEventChannelSO.OnAudioCuePlayRequested, new AudioCuePlayAction(PlayAudioCue));
			AudioCueEventChannelSO audioCueEventChannelSO2 = sfxEventChannel;
			audioCueEventChannelSO2.OnAudioCueStopRequested = (AudioCueStopAction)Delegate.Remove(audioCueEventChannelSO2.OnAudioCueStopRequested, new AudioCueStopAction(StopAudioCue));
			AudioCueEventChannelSO audioCueEventChannelSO3 = sfxEventChannel;
			audioCueEventChannelSO3.OnAudioCueFinishRequested = (AudioCueFinishAction)Delegate.Remove(audioCueEventChannelSO3.OnAudioCueFinishRequested, new AudioCueFinishAction(FinishAudioCue));
		}
		if (musicEventChannel != null)
		{
			AudioCueEventChannelSO audioCueEventChannelSO4 = musicEventChannel;
			audioCueEventChannelSO4.OnAudioCuePlayRequested = (AudioCuePlayAction)Delegate.Remove(audioCueEventChannelSO4.OnAudioCuePlayRequested, new AudioCuePlayAction(PlayMusicTrack));
		}
		Core.GameEvent.GameEvent.MasterVolumeEvent.UnregisterAction(ChangeMasterVolume);
		Core.GameEvent.GameEvent.SFXVolumeEvent.UnregisterAction(ChangeSFXVolume);
		Core.GameEvent.GameEvent.MusicVolumeEvent.UnregisterAction(ChangeMusicVolume);
		Core.GameEvent.GameEvent.AmbienceVolumeEvent.UnregisterAction(ChangeAmbienceVolume);
	}

	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			SetGroupVolume("MasterVolume", masterVolume);
			SetGroupVolume("SFXVolume", sfxVolume);
			SetGroupVolume("MusicVolume", musicVolume);
			SetGroupVolume("AmbienceVolume", ambienceVolume);
		}
	}

	private void ChangeMasterVolume(float newVolume)
	{
		masterVolume = newVolume;
		SetGroupVolume("MasterVolume", newVolume);
	}

	private void ChangeSFXVolume(float newVolume)
	{
		sfxVolume = newVolume;
		SetGroupVolume("SFXVolume", newVolume);
	}

	private void ChangeMusicVolume(float newVolume)
	{
		musicVolume = newVolume;
		SetGroupVolume("MusicVolume", newVolume);
	}

	private void ChangeAmbienceVolume(float newVolume)
	{
		ambienceVolume = newVolume;
		SetGroupVolume("AmbienceVolume", newVolume);
	}

	private void SetGroupVolume(string parameterName, float normalizedVolume)
	{
		if (!audioMixer.SetFloat(parameterName, NormalizedToMixerValue(normalizedVolume)))
		{
			Debug.LogError("The AudioMixer parameter was not found");
		}
	}

	private float NormalizedToMixerValue(float normalizedValue)
	{
		return Mathf.Log10(normalizedValue) * 20f;
	}

	private AudioCueKey PlayMusicTrack(AudioCueSO audioCue, AudioConfigurationSO audioConfiguration, Vector3 positionInSpace)
	{
		float duration = 2f;
		float startTime = 0f;
		if (_musicSoundEmitter != null && _musicSoundEmitter.IsPlaying())
		{
			AudioClip audioClip = audioCue.GetClips()[0];
			if (_musicSoundEmitter.GetClip() == audioClip)
			{
				return AudioCueKey.Invalid;
			}
			startTime = _musicSoundEmitter.FadeMusicOut(duration);
		}
		_musicSoundEmitter = pool.Request();
		_musicSoundEmitter.FadeMusicIn(audioCue.GetClips()[0], audioConfiguration, 1f, startTime);
		_musicSoundEmitter.OnSoundFinishedPlaying += StopMusicEmitter;
		return AudioCueKey.Invalid;
	}

	private bool StopMusic(AudioCueKey key)
	{
		if (_musicSoundEmitter == null || !_musicSoundEmitter.IsPlaying())
		{
			return false;
		}
		_musicSoundEmitter.Stop();
		return true;
	}

	public void TimelineInterruptsMusic()
	{
		StopMusic(AudioCueKey.Invalid);
	}

	public AudioCueKey PlayAudioCue(AudioCueSO audioCue, AudioConfigurationSO settings, Vector3 position = default(Vector3))
	{
		AudioClip[] clips = audioCue.GetClips();
		SoundEmitter[] array = new SoundEmitter[clips.Length];
		int num = clips.Length;
		for (int i = 0; i < num; i++)
		{
			array[i] = pool.Request();
			if (array[i] != null)
			{
				array[i].PlayAudioClip(clips[i], settings, audioCue.looping, position);
				if (!audioCue.looping)
				{
					array[i].OnSoundFinishedPlaying += OnSoundEmitterFinishedPlaying;
				}
			}
		}
		return _soundEmitterVault.Add(audioCue, array);
	}

	public bool FinishAudioCue(AudioCueKey audioCueKey)
	{
		SoundEmitter[] emitter;
		bool flag = _soundEmitterVault.Get(audioCueKey, out emitter);
		if (flag)
		{
			SoundEmitter[] array = emitter;
			foreach (SoundEmitter obj in array)
			{
				obj.Finish();
				obj.OnSoundFinishedPlaying += OnSoundEmitterFinishedPlaying;
			}
		}
		else
		{
			Debug.LogWarning("Finishing an AudioCue was requested, but the AudioCue was not found.");
		}
		return flag;
	}

	public bool StopAudioCue(AudioCueKey audioCueKey)
	{
		SoundEmitter[] emitter;
		bool flag = _soundEmitterVault.Get(audioCueKey, out emitter);
		if (flag)
		{
			SoundEmitter[] array = emitter;
			foreach (SoundEmitter soundEmitter in array)
			{
				StopAndCleanEmitter(soundEmitter);
			}
			_soundEmitterVault.Remove(audioCueKey);
		}
		return flag;
	}

	private void OnSoundEmitterFinishedPlaying(SoundEmitter soundEmitter)
	{
		StopAndCleanEmitter(soundEmitter);
	}

	private void StopAndCleanEmitter(SoundEmitter soundEmitter)
	{
		if (!soundEmitter.IsLooping())
		{
			soundEmitter.OnSoundFinishedPlaying -= OnSoundEmitterFinishedPlaying;
		}
		soundEmitter.Stop();
		pool.Return(soundEmitter);
	}

	private void StopMusicEmitter(SoundEmitter soundEmitter)
	{
		soundEmitter.OnSoundFinishedPlaying -= StopMusicEmitter;
		pool.Return(soundEmitter);
	}

	public AudioMixer GetMixer()
	{
		return audioMixer;
	}
}
