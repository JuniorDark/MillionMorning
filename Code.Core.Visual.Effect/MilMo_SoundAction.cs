using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public class MilMo_SoundAction : MilMo_EffectAction
{
	private AudioClip _sound;

	private string _soundFileName;

	private bool _haveStartedLoadingSound;

	private float _rolloffFactor = 0.1f;

	public virtual AudioClip Sound
	{
		get
		{
			if (_haveStartedLoadingSound)
			{
				return _sound;
			}
			_haveStartedLoadingSound = true;
			LoadClipAsync();
			return _sound;
		}
	}

	public float RolloffFactor => _rolloffFactor;

	public string Name { get; internal set; }

	public bool Looping { get; internal set; }

	private async void LoadClipAsync()
	{
		_sound = await MilMo_ResourceManager.Instance.LoadAudioAsync(_soundFileName);
		if (_sound == null)
		{
			Debug.LogWarning("Trying to load unknown audio clip '" + _soundFileName + "' for sound action " + Name);
		}
	}

	public override MilMo_SubEffect CreateSubEffect(GameObject parent, float staticYPos)
	{
		return new MilMo_SoundSubEffect(this, parent, staticYPos);
	}

	public override MilMo_SubEffect CreateSubEffect(GameObject parent, Vector3 dynamicOffset)
	{
		return new MilMo_SoundSubEffect(this, parent, dynamicOffset);
	}

	public override MilMo_SubEffect CreateSubEffect(Vector3 position)
	{
		return new MilMo_SoundSubEffect(this, position);
	}

	protected override void ReadToken(MilMo_SFFile file)
	{
		if (file.IsNext("RolloffFactor"))
		{
			_rolloffFactor = file.GetFloat();
		}
		else
		{
			base.ReadToken(file);
		}
	}

	public new static MilMo_SoundAction Load(MilMo_SFFile file)
	{
		MilMo_SoundAction milMo_SoundAction = new MilMo_SoundAction();
		string soundFileName = (milMo_SoundAction.Name = file.GetString());
		milMo_SoundAction._soundFileName = soundFileName;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Looping"))
			{
				milMo_SoundAction.Looping = file.GetBool();
			}
			else
			{
				milMo_SoundAction.ReadToken(file);
			}
		}
		return milMo_SoundAction;
	}
}
