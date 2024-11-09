using System.Collections;
using Core.Audio.AudioData;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Audio.SoundEmitters;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
	private AudioSource _audioSource;

	public event UnityAction<SoundEmitter> OnSoundFinishedPlaying;

	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		_audioSource.playOnAwake = false;
	}

	public void PlayAudioClip(AudioClip clip, AudioConfigurationSO settings, bool hasToLoop, Vector3 position = default(Vector3))
	{
		_audioSource.clip = clip;
		settings.ApplyTo(_audioSource);
		_audioSource.transform.position = position;
		_audioSource.loop = hasToLoop;
		_audioSource.time = 0f;
		_audioSource.Play();
		if (!hasToLoop)
		{
			StartCoroutine(FinishedPlaying(clip.length));
		}
	}

	public void FadeMusicIn(AudioClip musicClip, AudioConfigurationSO settings, float duration, float startTime = 0f)
	{
		PlayAudioClip(musicClip, settings, hasToLoop: true);
		_audioSource.volume = 0f;
		if (startTime <= _audioSource.clip.length)
		{
			_audioSource.time = startTime;
		}
		LeanTween.value(_audioSource.gameObject, 0f, settings.Volume, duration).setOnUpdate(delegate(float t)
		{
			_audioSource.volume = t;
		});
	}

	public float FadeMusicOut(float duration)
	{
		LeanTween.value(_audioSource.gameObject, _audioSource.volume, 0f, duration).setOnUpdate(delegate(float t)
		{
			_audioSource.volume = t;
		});
		return _audioSource.time;
	}

	private void OnFadeOutComplete()
	{
		NotifyBeingDone();
	}

	public AudioClip GetClip()
	{
		return _audioSource.clip;
	}

	public void Resume()
	{
		_audioSource.Play();
	}

	public void Pause()
	{
		_audioSource.Pause();
	}

	public void Stop()
	{
		_audioSource.Stop();
	}

	public void Finish()
	{
		if (_audioSource.loop)
		{
			_audioSource.loop = false;
			float clipLength = _audioSource.clip.length - _audioSource.time;
			StartCoroutine(FinishedPlaying(clipLength));
		}
	}

	public bool IsPlaying()
	{
		return _audioSource.isPlaying;
	}

	public bool IsLooping()
	{
		return _audioSource.loop;
	}

	private IEnumerator FinishedPlaying(float clipLength)
	{
		yield return new WaitForSeconds(clipLength);
		NotifyBeingDone();
	}

	private void NotifyBeingDone()
	{
		this.OnSoundFinishedPlaying(this);
	}
}
