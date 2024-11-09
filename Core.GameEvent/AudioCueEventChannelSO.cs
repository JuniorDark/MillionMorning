using Core.Audio.AudioData;
using UnityEngine;

namespace Core.GameEvent;

[CreateAssetMenu(menuName = "GameEvents/AudioCue Event Channel")]
public class AudioCueEventChannelSO : ScriptableObject
{
	public AudioCuePlayAction OnAudioCuePlayRequested;

	public AudioCueStopAction OnAudioCueStopRequested;

	public AudioCueFinishAction OnAudioCueFinishRequested;

	public AudioCueKey RaisePlayEvent(AudioCueSO audioCue, AudioConfigurationSO audioConfiguration, Vector3 positionInSpace = default(Vector3))
	{
		AudioCueKey result = AudioCueKey.Invalid;
		if (OnAudioCuePlayRequested != null)
		{
			result = OnAudioCuePlayRequested(audioCue, audioConfiguration, positionInSpace);
		}
		else
		{
			Debug.LogWarning("An AudioCue play event was requested  for " + audioCue.name + ", but nobody picked it up. Check why there is no AudioManager already loaded, and make sure it's listening on this AudioCue Event channel.");
		}
		return result;
	}

	public bool RaiseStopEvent(AudioCueKey audioCueKey)
	{
		bool result = false;
		if (OnAudioCueStopRequested != null)
		{
			result = OnAudioCueStopRequested(audioCueKey);
		}
		else
		{
			Debug.LogWarning("An AudioCue stop event was requested, but nobody picked it up. Check why there is no AudioManager already loaded, and make sure it's listening on this AudioCue Event channel.");
		}
		return result;
	}

	public bool RaiseFinishEvent(AudioCueKey audioCueKey)
	{
		bool result = false;
		if (OnAudioCueStopRequested != null)
		{
			result = OnAudioCueFinishRequested(audioCueKey);
		}
		else
		{
			Debug.LogWarning("An AudioCue finish event was requested, but nobody picked it up. Check why there is no AudioManager already loaded, and make sure it's listening on this AudioCue Event channel.");
		}
		return result;
	}
}
