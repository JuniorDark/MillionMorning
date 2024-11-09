using Code.Core.Sound;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_SoundEffect : MilMo_ObjectEffect
{
	private float _time;

	private AudioSourceWrapper _audioSource;

	private MilMo_SoundEffectTemplate Template => EffectTemplate as MilMo_SoundEffectTemplate;

	public override float Duration
	{
		get
		{
			if (Template.Sound == null)
			{
				return 0f;
			}
			return Template.Sound.length;
		}
	}

	public MilMo_SoundEffect(GameObject gameObject, MilMo_ObjectEffectTemplate template)
		: base(gameObject, template)
	{
	}

	public override bool Update()
	{
		if (GameObject == null)
		{
			Destroy();
			return false;
		}
		if (_audioSource == null && Template.Sound != null)
		{
			_audioSource = GameObject.AddComponent<AudioSourceWrapper>();
			if (_audioSource != null)
			{
				MilMo_AudioUtils.SetRollOffFactor(_audioSource, Template.RolloffFactor, Template.MinDistanceAdd);
				_audioSource.Loop = Template.Loop;
				_audioSource.enabled = true;
				_audioSource.Clip = Template.Sound;
				_audioSource.Play();
			}
		}
		_time += Time.deltaTime;
		if (Template.Loop || _time <= Duration)
		{
			return true;
		}
		Destroy();
		return false;
	}

	public override void Destroy()
	{
		if (_audioSource != null)
		{
			_audioSource.Stop();
			Object.Destroy(_audioSource);
		}
		base.Destroy();
	}
}
