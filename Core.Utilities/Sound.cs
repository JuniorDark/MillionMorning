using Core.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Utilities;

public static class Sound
{
	public static void SetOutputMixer(AudioSource source, string outputMixer)
	{
		AudioManager audioManager = Object.FindObjectOfType<AudioManager>();
		if (audioManager == null)
		{
			Debug.LogWarning("Unable to find audio manager");
			return;
		}
		AudioMixer mixer = audioManager.GetMixer();
		if (mixer.FindMatchingGroups(outputMixer).Length == 0)
		{
			Debug.LogWarning("Unable to set mixer group: " + outputMixer);
		}
		else
		{
			source.outputAudioMixerGroup = mixer.FindMatchingGroups(outputMixer)[0];
		}
	}
}
