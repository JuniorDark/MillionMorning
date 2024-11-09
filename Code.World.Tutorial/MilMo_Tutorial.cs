using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.Network;
using Core;
using Core.GameEvent;
using UI.HUD.Dialogues;
using UnityEngine;

namespace Code.World.Tutorial;

public sealed class MilMo_Tutorial : IMilMo_Tutorial
{
	private readonly MilMo_TutorialTemplate _template;

	private readonly List<MilMo_TutorialTrigger> _activationTriggers = new List<MilMo_TutorialTrigger>();

	private readonly List<MilMo_TutorialTrigger> _closeTriggers = new List<MilMo_TutorialTrigger>();

	public bool IsComplete;

	public string Name => _template.Name;

	public Action OnCloseTriggered { get; set; }

	public ITutorialData GetTemplate()
	{
		return _template;
	}

	public string GetIdentifier()
	{
		return _template.Name;
	}

	public MilMo_Tutorial(MilMo_TutorialTemplate template)
	{
		_template = template;
		foreach (MilMo_TutorialTemplate.Trigger activationTrigger in template.ActivationTriggers)
		{
			_activationTriggers.Add(new MilMo_TutorialTrigger(activationTrigger.Evt, activationTrigger.Obj, activationTrigger.Activations, _template.World, _template.Level));
		}
		foreach (MilMo_TutorialTemplate.Trigger closeTrigger in template.CloseTriggers)
		{
			_closeTriggers.Add(new MilMo_TutorialTrigger(closeTrigger.Evt, closeTrigger.Obj, closeTrigger.Activations, _template.World, _template.Level));
		}
	}

	public void SetupTriggers()
	{
		foreach (MilMo_TutorialTrigger activationTrigger in _activationTriggers)
		{
			activationTrigger.StartListening(ActivationCallback);
		}
		foreach (MilMo_TutorialTrigger closeTrigger in _closeTriggers)
		{
			closeTrigger.StartListening(CloseCallback);
		}
	}

	private async void ActivationCallback()
	{
		if (!IsComplete)
		{
			IsComplete = true;
			Singleton<GameNetwork>.Instance.SendTutorialCompleted(_template.Name);
			RemoveActivationTriggers();
			if (!string.IsNullOrEmpty(_template.Dialog))
			{
				SpawnDialogue();
				return;
			}
			await Task.Delay((int)(_template.Delay * 1000f));
			DialogueSpawner.SpawnTutorialDialogue(this);
		}
	}

	private void SpawnDialogue()
	{
		Debug.LogWarning("Spawning dialogue");
		GameEvent.ShowControllerChoiceEvent.RaiseEvent();
	}

	private void CloseCallback()
	{
		OnCloseTriggered?.Invoke();
	}

	private void RemoveActivationTriggers()
	{
		foreach (MilMo_TutorialTrigger activationTrigger in _activationTriggers)
		{
			activationTrigger.StopListening();
		}
	}

	public void RemoveCloseTriggers()
	{
		foreach (MilMo_TutorialTrigger closeTrigger in _closeTriggers)
		{
			closeTrigger.StopListening();
		}
	}
}
