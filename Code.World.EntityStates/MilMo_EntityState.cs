using System.Collections.Generic;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Template;
using Core;
using UnityEngine;

namespace Code.World.EntityStates;

internal class MilMo_EntityState
{
	private readonly Dictionary<int, MilMo_EntityStateEffect> _effects;

	public Dictionary<int, MilMo_EntityStateEffect>.ValueCollection Effects => _effects.Values;

	internal MilMo_EntityState(ServerEntityStateEffectAdded message, IMilMo_Entity entity)
	{
		_effects = new Dictionary<int, MilMo_EntityStateEffect>();
		if (message != null)
		{
			IList<EntityStateEffectReference> effects = message.getEffects();
			LoadEffects(entity, effects);
		}
	}

	internal MilMo_EntityState()
	{
		_effects = new Dictionary<int, MilMo_EntityStateEffect>();
		Activate();
	}

	private async void LoadEffects(IMilMo_Entity entity, IList<EntityStateEffectReference> effects)
	{
		foreach (EntityStateEffectReference effect in effects)
		{
			int id = effect.GetId();
			if (!ContainsEffect(id))
			{
				float modifier = effect.GetModifier();
				bool isPerm = effect.GetIsPermanent() == 1;
				TemplateReference templateReference = effect.GetTemplate();
				if (!(await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(templateReference) is MilMo_EntityStateEffectTemplate milMo_EntityStateEffectTemplate))
				{
					string text = templateReference?.GetIdentifier();
					Debug.LogWarning("Could not load MilMo_EntityStateEffectTemplate from " + text + ".");
				}
				else
				{
					MilMo_EntityStateEffect value = new MilMo_EntityStateEffect(milMo_EntityStateEffectTemplate.ParticleEffect, milMo_EntityStateEffectTemplate.EffectType, id, modifier, entity, isPerm, milMo_EntityStateEffectTemplate.StackCount);
					_effects.Add(id, value);
				}
			}
		}
		Activate();
	}

	private void Activate()
	{
		foreach (KeyValuePair<int, MilMo_EntityStateEffect> effect in _effects)
		{
			effect.Value.Activate();
		}
	}

	internal void DeactivateEffect(int effectId)
	{
		if (ContainsEffect(effectId))
		{
			_effects[effectId].Deactivate();
		}
	}

	internal void AddEffect(MilMo_EntityStateEffect effectToAdd)
	{
		if (!ContainsEffect(effectToAdd.Id))
		{
			_effects.Add(effectToAdd.Id, effectToAdd);
			_effects[effectToAdd.Id].Activate();
		}
	}

	internal bool ContainsEffect(int effectId)
	{
		return _effects.ContainsKey(effectId);
	}

	internal void Deactivate()
	{
		foreach (KeyValuePair<int, MilMo_EntityStateEffect> effect in _effects)
		{
			effect.Value.Deactivate();
		}
	}
}
