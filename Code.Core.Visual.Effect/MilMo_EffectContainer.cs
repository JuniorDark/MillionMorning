using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public static class MilMo_EffectContainer
{
	private static readonly Dictionary<string, MilMo_EffectTemplate> Effects = new Dictionary<string, MilMo_EffectTemplate>();

	public static MilMo_Effect GetEffect(string name, Vector3 position, bool warnIfNotFound = true)
	{
		MilMo_EffectTemplate effectTemplate = GetEffectTemplate(name, warnIfNotFound);
		if (effectTemplate != null)
		{
			return new MilMo_Effect(effectTemplate, position);
		}
		return null;
	}

	public static MilMo_Effect GetEffect(string name, GameObject parent, bool warnIfNotFound = true)
	{
		MilMo_EffectTemplate effectTemplate = GetEffectTemplate(name, warnIfNotFound);
		if (effectTemplate != null)
		{
			return new MilMo_Effect(effectTemplate, parent);
		}
		return null;
	}

	private static MilMo_EffectTemplate GetEffectTemplate(string name, bool warnIfNotFound)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (Effects.TryGetValue(name, out var value))
		{
			return value;
		}
		string text = "Particles/" + name;
		value = MilMo_EffectTemplate.Load(MilMo_SimpleFormat.LoadLocal(text));
		if (value == null && warnIfNotFound)
		{
			Debug.LogWarning("Failed to load effect " + text);
		}
		Effects.Add(name, value);
		return value;
	}

	public static void UnloadAll()
	{
		Effects.Clear();
	}
}
