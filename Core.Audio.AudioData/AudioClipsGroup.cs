using System;
using UnityEngine;

namespace Core.Audio.AudioData;

[Serializable]
public class AudioClipsGroup
{
	public enum SequenceMode
	{
		Random,
		RandomNoImmediateRepeat,
		Sequential
	}

	public SequenceMode sequenceMode = SequenceMode.RandomNoImmediateRepeat;

	public AudioClip[] audioClips;

	private int _nextClipToPlay = -1;

	private int _lastClipPlayed = -1;

	public AudioClip GetNextClip()
	{
		if (audioClips.Length == 1)
		{
			return audioClips[0];
		}
		if (_nextClipToPlay == -1)
		{
			_nextClipToPlay = ((sequenceMode != SequenceMode.Sequential) ? UnityEngine.Random.Range(0, audioClips.Length) : 0);
		}
		else
		{
			switch (sequenceMode)
			{
			case SequenceMode.Random:
				_nextClipToPlay = UnityEngine.Random.Range(0, audioClips.Length);
				break;
			case SequenceMode.RandomNoImmediateRepeat:
				do
				{
					_nextClipToPlay = UnityEngine.Random.Range(0, audioClips.Length);
				}
				while (_nextClipToPlay == _lastClipPlayed);
				break;
			case SequenceMode.Sequential:
				_nextClipToPlay = (int)Mathf.Repeat(++_nextClipToPlay, audioClips.Length);
				break;
			}
		}
		_lastClipPlayed = _nextClipToPlay;
		return audioClips[_nextClipToPlay];
	}
}
