using Core.GameEvent;
using UnityEngine;

namespace Core.Audio.AudioData;

[CreateAssetMenu(fileName = "newUIAudioCue", menuName = "Audio/UI Audio Cue")]
public class UIAudioCueSO : AudioCueSO
{
	[Header("Configuration")]
	[SerializeField]
	private AudioCueEventChannelSO _audioCueEventChannel;

	[SerializeField]
	private AudioConfigurationSO _audioConfiguration;

	private AudioCueKey controlKey = AudioCueKey.Invalid;

	public void PlayAudioCue()
	{
		controlKey = _audioCueEventChannel.RaisePlayEvent(this, _audioConfiguration, Vector3.zero);
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
