using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.Core.Visual.Audio;
using UnityEngine;

namespace Code.World.AmbientSound;

public class MilMo_AmbientSoundRandom : MilMo_AmbientSound
{
	private readonly List<MilMo_Randomizer.IRandomElement> _sounds = new List<MilMo_Randomizer.IRandomElement>();

	private MilMo_Randomizer _randomizer;

	private Vector2 _interval;

	public MilMo_AmbientSoundRandom(int channel)
		: base(channel)
	{
	}

	protected override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("Sound"))
		{
			MilMo_RandomSound milMo_RandomSound = new MilMo_RandomSound
			{
				Path = file.GetString()
			};
			if (file.HasMoreTokens())
			{
				milMo_RandomSound.Probability = file.GetFloat();
			}
			_sounds.Add(milMo_RandomSound);
		}
		else
		{
			if (!file.IsNext("Interval"))
			{
				return base.ReadLine(file);
			}
			_interval.x = file.GetFloat();
			_interval.y = file.GetFloat();
		}
		return true;
	}

	protected override void FinishReading()
	{
		base.FinishReading();
		_randomizer = new MilMo_Randomizer(_sounds);
	}

	public override void Play()
	{
		MilMo_EventSystem.At(Random.Range(_interval.x, _interval.y), delegate
		{
			if (!(base.Wrapper == null))
			{
				AudioClip clip = GetClip();
				if (clip != null && base.Wrapper.Volume > 0f)
				{
					base.Wrapper.Clip = clip;
					base.Wrapper.Loop = false;
					base.Wrapper.Play();
				}
				Play();
			}
		});
	}

	private AudioClip GetClip()
	{
		return (_randomizer?.Next() as MilMo_RandomSound)?.Clip;
	}
}
