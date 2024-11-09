using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public class MilMo_EffectTemplate
{
	private readonly List<MilMo_EffectAction> _actions = new List<MilMo_EffectAction>();

	public List<MilMo_EffectAction> Actions => _actions;

	public string Name { get; private set; }

	public float Duration { get; private set; }

	private MilMo_EffectTemplate(string name)
	{
		Duration = 0f;
		Name = name;
	}

	public static MilMo_EffectTemplate Load(MilMo_SFFile file)
	{
		if (file == null)
		{
			return null;
		}
		MilMo_EffectTemplate milMo_EffectTemplate = new MilMo_EffectTemplate(file.Name);
		while (file.NextRow())
		{
			MilMo_EffectAction milMo_EffectAction = MilMo_EffectAction.Load(file);
			if (milMo_EffectAction != null)
			{
				milMo_EffectTemplate._actions.Add(milMo_EffectAction);
				continue;
			}
			Debug.LogWarning("Failed to load particle effect action #" + (milMo_EffectTemplate._actions.Count + 1) + " in file '" + file.Path + "'.");
		}
		foreach (MilMo_EffectAction action in milMo_EffectTemplate._actions)
		{
			float num = action.StartTime + action.Duration;
			if (num > milMo_EffectTemplate.Duration)
			{
				milMo_EffectTemplate.Duration = num;
			}
		}
		return milMo_EffectTemplate;
	}
}
