using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public abstract class MilMo_EffectAction
{
	public float StartTime { get; private set; }

	public float Duration { get; internal set; }

	protected MilMo_EffectAction()
	{
		StartTime = 0f;
		Duration = 0f;
	}

	public abstract MilMo_SubEffect CreateSubEffect(Vector3 position);

	public abstract MilMo_SubEffect CreateSubEffect(GameObject parent, Vector3 dynamicOffset);

	public abstract MilMo_SubEffect CreateSubEffect(GameObject parent, float staticYPos);

	public static MilMo_EffectAction Load(MilMo_SFFile file)
	{
		file.GetString();
		float @float = file.GetFloat();
		MilMo_EffectAction milMo_EffectAction;
		if (file.IsNext("Particle"))
		{
			milMo_EffectAction = MilMo_ParticleAction.Load(file);
		}
		else if (file.IsNext("Sound"))
		{
			milMo_EffectAction = MilMo_SoundAction.Load(file);
		}
		else
		{
			if (!file.IsNext("RandomSound"))
			{
				return null;
			}
			milMo_EffectAction = MilMo_RandomSoundAction.Load(file);
		}
		if (milMo_EffectAction == null)
		{
			return null;
		}
		milMo_EffectAction.StartTime = @float;
		return milMo_EffectAction;
	}

	protected virtual void ReadToken(MilMo_SFFile file)
	{
		if (file.IsNext("Duration"))
		{
			Duration = file.GetFloat();
		}
		else
		{
			file.NextToken();
		}
	}
}
