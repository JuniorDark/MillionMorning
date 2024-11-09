using Core.GameEvent;
using UnityEngine;

namespace Core.Audio.AudioData;

[CreateAssetMenu(fileName = "newMusicAudioCue", menuName = "Audio/Music Audio Cue")]
public class MusicAudioCueSO : AudioCueSO
{
	[Header("Configuration")]
	[SerializeField]
	private AudioCueEventChannelSO _audioCueEventChannel;

	[SerializeField]
	private AudioConfigurationSO _audioConfiguration;

	private AudioCueKey controlKey = AudioCueKey.Invalid;

	public void PlayAudioCue()
	{
		_audioCueEventChannel.RaisePlayEvent(this, _audioConfiguration, Vector3.zero);
	}

	public void StopAudioCue()
	{
		if (!_audioCueEventChannel.RaiseStopEvent(controlKey))
		{
			controlKey = AudioCueKey.Invalid;
		}
	}

	public void FinishAudioCue()
	{
		if (!_audioCueEventChannel.RaiseFinishEvent(controlKey))
		{
			controlKey = AudioCueKey.Invalid;
		}
	}
}
