using Core.Audio.AudioData;
using UnityEngine;

namespace Core.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioConfigApplier : MonoBehaviour
{
	public AudioConfigurationSO config;

	private void OnValidate()
	{
		ConfigureAudioSource();
	}

	private void Start()
	{
		ConfigureAudioSource();
	}

	private void ConfigureAudioSource()
	{
		if (config != null)
		{
			AudioSource component = GetComponent<AudioSource>();
			config.ApplyTo(component);
		}
	}
}
