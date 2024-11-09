using System.Collections;
using Core.Audio.AudioData;
using Core.GameEvent;
using UnityEngine;

namespace Core.Audio;

public class AudioCue : MonoBehaviour
{
	[Header("Sound definition")]
	[SerializeField]
	private AudioCueSO _audioCue;

	[SerializeField]
	private bool _playOnStart;

	[Header("Configuration")]
	[SerializeField]
	private AudioCueEventChannelSO _audioCueEventChannel;

	[SerializeField]
	private AudioConfigurationSO _audioConfiguration;

	private AudioCueKey controlKey = AudioCueKey.Invalid;

	private void Start()
	{
		if (_playOnStart)
		{
			StartCoroutine(PlayDelayed());
		}
	}

	private void OnDisable()
	{
		Debug.LogWarning(base.gameObject.name + ": OnDisable");
		_playOnStart = false;
		FinishAudioCue();
	}

	private void OnDestroy()
	{
		Debug.LogWarning(base.gameObject.name + ": OnDestroy");
		_playOnStart = false;
		StopAudioCue();
	}

	private IEnumerator PlayDelayed()
	{
		yield return new WaitForSeconds(1f);
		if (_playOnStart)
		{
			PlayAudioCue();
		}
	}

	public void PlayAudioCue()
	{
		controlKey = _audioCueEventChannel.RaisePlayEvent(_audioCue, _audioConfiguration, base.transform.position);
	}

	public void StopAudioCue()
	{
		if (controlKey != AudioCueKey.Invalid && !_audioCueEventChannel.RaiseStopEvent(controlKey))
		{
			controlKey = AudioCueKey.Invalid;
		}
	}

	public void FinishAudioCue()
	{
		if (controlKey != AudioCueKey.Invalid && !_audioCueEventChannel.RaiseFinishEvent(controlKey))
		{
			controlKey = AudioCueKey.Invalid;
		}
	}
}
