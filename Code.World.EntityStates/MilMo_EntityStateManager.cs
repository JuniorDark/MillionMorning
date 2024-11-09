using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Template;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.EntityStates;

public class MilMo_EntityStateManager
{
	private string _entityId;

	private readonly Dictionary<int, MilMo_EntityState> _states;

	private readonly IMilMo_Entity _entity;

	private readonly MilMo_EntityState _remoteEntityState;

	private MilMo_GenericReaction _entityStateEffectAddedListener;

	private MilMo_GenericReaction _entityStateUpdateListener;

	public MilMo_EntityStateManager(IMilMo_Entity entity)
	{
		_entity = entity;
		_states = new Dictionary<int, MilMo_EntityState>();
		_remoteEntityState = new MilMo_EntityState();
		StartListeners();
	}

	private void StartListeners()
	{
		_entityStateEffectAddedListener = MilMo_EventSystem.Listen("entity_state_effect_added", EntityStateEffectAdded);
		_entityStateEffectAddedListener.Repeating = true;
		_entityStateUpdateListener = MilMo_EventSystem.Listen("entity_state_update", EntityStateUpdated);
		_entityStateUpdateListener.Repeating = true;
	}

	private void StopListeners()
	{
		MilMo_EventSystem.RemoveReaction(_entityStateEffectAddedListener);
		_entityStateEffectAddedListener = null;
		MilMo_EventSystem.RemoveReaction(_entityStateUpdateListener);
		_entityStateUpdateListener = null;
	}

	private void EntityStateEffectAdded(object msg)
	{
		if (msg is ServerEntityStateEffectAdded state)
		{
			AddState(state);
		}
	}

	private void EntityStateUpdated(object msg)
	{
		if (!(msg is ServerEntityStateUpdate serverEntityStateUpdate))
		{
			return;
		}
		foreach (int item in serverEntityStateUpdate.getEffectsToTurnOff())
		{
			RemoveEffect(item);
		}
		foreach (int item2 in serverEntityStateUpdate.getStatesToTurnOff())
		{
			RemoveState(item2);
		}
	}

	private void AddState(ServerEntityStateEffectAdded state)
	{
		if (_states.ContainsKey(state.getStateId()))
		{
			Debug.LogWarning("Trying to add a state that is already in the list.");
		}
		else
		{
			_states.Add(state.getStateId(), new MilMo_EntityState(state, _entity));
		}
	}

	public int GetActiveInStatesSum(string particleEffect)
	{
		if (_entity is MilMo_Avatar { IsTheLocalPlayer: false })
		{
			return _remoteEntityState.Effects.Count((MilMo_EntityStateEffect fx) => fx.IsActive && fx.ParticleEffect == particleEffect);
		}
		return _states.Sum((KeyValuePair<int, MilMo_EntityState> state) => state.Value.Effects.Count((MilMo_EntityStateEffect fx) => fx.IsActive && fx.ParticleEffect == particleEffect));
	}

	private void RemoveState(int stateId)
	{
		if (_states.ContainsKey(stateId))
		{
			_states[stateId].Deactivate();
			_states.Remove(stateId);
		}
		else
		{
			Debug.LogWarning("Trying to remove a state that does not exist in current entity.");
		}
	}

	private void RemoveEffect(int effectId)
	{
		foreach (KeyValuePair<int, MilMo_EntityState> item in _states.Where((KeyValuePair<int, MilMo_EntityState> states) => states.Value.ContainsEffect(effectId)))
		{
			item.Value.DeactivateEffect(effectId);
		}
	}

	public void AddEffectToRemoteEntity(EntityStateEffectReference templateReference)
	{
		int effectId = templateReference.GetId();
		float modifier = templateReference.GetModifier();
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(templateReference.GetTemplate(), delegate(MilMo_Template template, bool timeOut)
		{
			MilMo_EntityStateEffectTemplate milMo_EntityStateEffectTemplate = template as MilMo_EntityStateEffectTemplate;
			if (!(milMo_EntityStateEffectTemplate == null || timeOut))
			{
				_remoteEntityState.AddEffect(new MilMo_EntityStateEffect(milMo_EntityStateEffectTemplate.ParticleEffect, milMo_EntityStateEffectTemplate.EffectType, effectId, modifier, _entity, isPermanent: false, milMo_EntityStateEffectTemplate.StackCount));
			}
		});
	}

	public void RemoveEffectFromRemoteEntity(EntityStateEffectReference templateReference)
	{
		int id = templateReference.GetId();
		if (_remoteEntityState.ContainsEffect(id))
		{
			_remoteEntityState.DeactivateEffect(id);
		}
	}

	public void Destroy()
	{
		_remoteEntityState.Deactivate();
		foreach (KeyValuePair<int, MilMo_EntityState> state in _states)
		{
			state.Value.Deactivate();
		}
		StopListeners();
	}

	private static bool ShouldRun(MilMo_EntityStateEffect effect)
	{
		if (effect.IsPermanent && MilMo_Level.CurrentLevel != null)
		{
			return !MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName);
		}
		return true;
	}

	public float GetModifier(string type)
	{
		float num = 1f;
		if (!MilMo_Player.Instance.InWorld || MilMo_Player.Instance.InRoom)
		{
			return num;
		}
		num += _states.Sum((KeyValuePair<int, MilMo_EntityState> state) => state.Value.Effects.Where((MilMo_EntityStateEffect effect) => ShouldRun(effect) && string.Equals(effect.EffectType, type, StringComparison.CurrentCultureIgnoreCase) && effect.IsActive).Sum((MilMo_EntityStateEffect effect) => effect.Modifier - 1f));
		if (!(num < 0f))
		{
			return num;
		}
		return 0f;
	}
}
