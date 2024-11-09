using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Audio;

public class MilMo_RandomSound : MilMo_Randomizer.IRandomElement
{
	public string Path;

	private AudioClip _clip;

	private bool _haveStartedLoadingClip;

	public float Probability { get; set; }

	public float NormalizedProbability { get; set; }

	public AudioClip Clip
	{
		get
		{
			if (_haveStartedLoadingClip)
			{
				return _clip;
			}
			_haveStartedLoadingClip = true;
			LoadClipAsync();
			return _clip;
		}
	}

	private async void LoadClipAsync()
	{
		_clip = await MilMo_ResourceManager.Instance.LoadAudioAsync(Path);
	}

	public MilMo_RandomSound()
	{
		Probability = 1f;
	}
}
