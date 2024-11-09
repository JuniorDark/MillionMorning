using System.Collections.Generic;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Audio;

public class MilMo_RandomSoundTemplate : MilMo_Template
{
	private readonly List<MilMo_Randomizer.IRandomElement> _mSounds = new List<MilMo_Randomizer.IRandomElement>();

	private MilMo_Randomizer _mRandomizer;

	public MilMo_RandomSoundTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "RandomSound")
	{
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("Sound"))
		{
			MilMo_RandomSound milMo_RandomSound = new MilMo_RandomSound();
			milMo_RandomSound.Path = file.GetString();
			if (file.HasMoreTokens())
			{
				milMo_RandomSound.Probability = file.GetFloat();
			}
			_mSounds.Add(milMo_RandomSound);
			return true;
		}
		return base.ReadLine(file);
	}

	public override bool FinishLoading()
	{
		if (!base.FinishLoading())
		{
			return false;
		}
		_mRandomizer = new MilMo_Randomizer(_mSounds);
		return true;
	}

	public AudioClip GetClip()
	{
		if (_mRandomizer != null && _mRandomizer.Next() is MilMo_RandomSound milMo_RandomSound)
		{
			return milMo_RandomSound.Clip;
		}
		return null;
	}

	public static MilMo_RandomSoundTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_RandomSoundTemplate(category, path, filePath);
	}
}
