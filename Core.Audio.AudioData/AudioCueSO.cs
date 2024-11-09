using UnityEngine;

namespace Core.Audio.AudioData;

[CreateAssetMenu(fileName = "newAudioCue", menuName = "Audio/Audio Cue")]
public class AudioCueSO : ScriptableObject
{
	public bool looping;

	[SerializeField]
	private AudioClipsGroup[] _audioClipGroups;

	public AudioClip[] GetClips()
	{
		int num = _audioClipGroups.Length;
		AudioClip[] array = new AudioClip[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = _audioClipGroups[i].GetNextClip();
		}
		return array;
	}
}
