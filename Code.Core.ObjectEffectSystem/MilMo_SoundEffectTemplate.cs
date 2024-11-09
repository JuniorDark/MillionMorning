using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_SoundEffectTemplate : MilMo_ObjectEffectTemplate
{
	private AudioClip _sound;

	private readonly string _soundFileName;

	private bool _haveStartedLoadingSound;

	public float RolloffFactor { get; private set; }

	public float MinDistanceAdd { get; private set; }

	public bool Loop { get; private set; }

	public AudioClip Sound
	{
		get
		{
			if (_haveStartedLoadingSound)
			{
				return _sound;
			}
			_haveStartedLoadingSound = true;
			LoadSoundAsync();
			return _sound;
		}
	}

	private async void LoadSoundAsync()
	{
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(_soundFileName);
		if (audioClip == null)
		{
			Debug.LogWarning("Trying to load unknown audio clip '" + _soundFileName + "' for sound effect template " + base.Name);
		}
		else
		{
			_sound = audioClip;
		}
	}

	public MilMo_SoundEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		RolloffFactor = 0.35f;
		_soundFileName = file.GetString();
		while (file.HasMoreTokens())
		{
			if (file.IsNext("RolloffFactor"))
			{
				RolloffFactor = file.GetFloat();
			}
			else if (file.IsNext("MinDistanceAdd"))
			{
				MinDistanceAdd = file.GetFloat();
			}
			else if (file.IsNext("Loop"))
			{
				Loop = file.GetBool();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_SoundEffect(gameObject, this);
	}
}
