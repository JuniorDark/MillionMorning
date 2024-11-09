using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.World.Inventory;
using Code.World.Player;
using Code.World.Player.Skills;
using UI.Elements.Slot;
using UnityEngine;

namespace Player;

public class SkillManager : MonoBehaviour
{
	private readonly Dictionary<MilMo_Skill, MilMo_SkillEntry> _skillEntries = new Dictionary<MilMo_Skill, MilMo_SkillEntry>();

	private MilMo_GenericReaction _availableSkillsUpdate;

	private MilMo_GenericReaction _abilityActivated;

	private MilMo_GenericReaction _abilityDeactivated;

	private MilMo_GenericReaction _skillActivated;

	private MilMo_GenericReaction _skillActivationFailed;

	private MilMo_GenericReaction _skillItemActivated;

	private MilMo_GenericReaction _skillItemActivationFailed;

	public event Action<MilMo_SkillEntry> OnEntryAdded;

	public event Action<MilMo_SkillEntry> OnEntryRemoved;

	private void OnEnable()
	{
		_availableSkillsUpdate = MilMo_EventSystem.Listen("player_available_skills_update", SkillsUpdated);
		_availableSkillsUpdate.Repeating = true;
		_abilityActivated = MilMo_EventSystem.Listen("ability_activated", AbilityActivated);
		_abilityActivated.Repeating = true;
		_abilityDeactivated = MilMo_EventSystem.Listen("ability_deactivated", AbilityDeactivated);
		_abilityDeactivated.Repeating = true;
		_skillActivated = MilMo_EventSystem.Listen("skill_activated", SkillActivated);
		_skillActivated.Repeating = true;
		_skillActivationFailed = MilMo_EventSystem.Listen("skill_activation_failed", SkillActivationFailed);
		_skillActivationFailed.Repeating = true;
		_skillItemActivated = MilMo_EventSystem.Listen("skill_item_activated", SkillItemActivated);
		_skillItemActivated.Repeating = true;
		_skillItemActivationFailed = MilMo_EventSystem.Listen("skill_item_activation_failed", SkillItemActivationFailed);
		_skillItemActivationFailed.Repeating = true;
	}

	private void OnDisable()
	{
		MilMo_EventSystem.RemoveReaction(_availableSkillsUpdate);
		_availableSkillsUpdate = null;
		MilMo_EventSystem.RemoveReaction(_abilityActivated);
		_abilityActivated = null;
		MilMo_EventSystem.RemoveReaction(_abilityDeactivated);
		_abilityDeactivated = null;
		MilMo_EventSystem.RemoveReaction(_skillActivated);
		_skillActivated = null;
		MilMo_EventSystem.RemoveReaction(_skillActivationFailed);
		_skillActivationFailed = null;
		MilMo_EventSystem.RemoveReaction(_skillItemActivated);
		_skillItemActivated = null;
		MilMo_EventSystem.RemoveReaction(_skillItemActivationFailed);
		_skillItemActivationFailed = null;
	}

	private void SkillItemActivated(object o)
	{
		if (o is ServerSkillItemActivated serverSkillItemActivated)
		{
			MilMo_InventoryEntry entry = MilMo_Player.Instance.Inventory.GetEntry(serverSkillItemActivated.getSkillInventoryId());
			if (entry?.Item != null)
			{
				(entry.Item as MilMo_SkillItem)?.Activate();
			}
		}
	}

	private void SkillItemActivationFailed(object o)
	{
		if (o is ServerSkillItemActivationFailed serverSkillItemActivationFailed && MilMo_Player.Instance.Inventory.GetEntry(serverSkillItemActivationFailed.getSkillInventoryId())?.Item != null)
		{
			Debug.Log("SkillItemActivationFailed: " + serverSkillItemActivationFailed.getSkillInventoryId());
		}
	}

	private static void AbilityActivated(object o)
	{
		if (o is ServerActivateAbility serverActivateAbility && MilMo_Player.Instance.Inventory.GetEntry(serverActivateAbility.getAbilityInventoryId())?.Item is MilMo_Ability milMo_Ability)
		{
			milMo_Ability.WasActivated();
		}
	}

	private static void AbilityDeactivated(object o)
	{
		if (o is ServerDeactivateAbility serverDeactivateAbility && MilMo_Player.Instance.Inventory.GetEntry(serverDeactivateAbility.getAbilityInventoryId())?.Item is MilMo_Ability milMo_Ability)
		{
			milMo_Ability.Deactivate();
		}
	}

	private void SkillActivated(object o)
	{
		ServerSkillActivated msg = o as ServerSkillActivated;
		if (msg == null)
		{
			return;
		}
		MilMo_Skill milMo_Skill = _skillEntries.Keys.FirstOrDefault((MilMo_Skill skill) => skill.ClassName == msg.getClassName() && skill.Level == msg.getLevel() && skill.ModeId == msg.getMode());
		if (milMo_Skill == null)
		{
			return;
		}
		float cooldown = milMo_Skill.Cooldown;
		foreach (MilMo_Skill item in _skillEntries.Keys.Where((MilMo_Skill skill) => skill.ClassName == msg.getClassName() && skill.Level == msg.getLevel()))
		{
			item.WasActivated(cooldown);
		}
	}

	private void SkillActivationFailed(object o)
	{
		ServerSkillActivationFailed msg = o as ServerSkillActivationFailed;
		if (msg != null)
		{
			_skillEntries.Keys.FirstOrDefault((MilMo_Skill skill) => skill.ClassName == msg.getClassName() && skill.Level == msg.getLevel() && skill.ModeId == msg.getMode())?.FailedToActivate();
		}
	}

	public IEnumerable<MilMo_Skill> GetSkills()
	{
		return _skillEntries.Keys;
	}

	public List<MilMo_SkillEntry> GetAllEntries()
	{
		return _skillEntries.Values.ToList();
	}

	private MilMo_Skill GetSkillById(string id)
	{
		return GetSkills().FirstOrDefault((MilMo_Skill skill) => skill.GetSaveString() == id);
	}

	public ISlotItemEntry GetEntryBySId(string id)
	{
		MilMo_Skill skillById = GetSkillById(id);
		if (skillById == null)
		{
			Debug.LogWarning("Unable to find skill with id: " + id);
			return null;
		}
		if (!_skillEntries.TryGetValue(skillById, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddEntry(MilMo_Skill skill)
	{
		MilMo_SkillEntry milMo_SkillEntry = new MilMo_SkillEntry();
		milMo_SkillEntry.Skill = skill;
		_skillEntries.Add(skill, milMo_SkillEntry);
		this.OnEntryAdded?.Invoke(milMo_SkillEntry);
	}

	private void RemoveEntry(MilMo_Skill skill)
	{
		_skillEntries.TryGetValue(skill, out var value);
		if (value != null)
		{
			this.OnEntryRemoved?.Invoke(value);
			_skillEntries.Remove(skill);
		}
	}

	public void CreateSkills(IList<SkillTemplate> skillTemplates)
	{
		List<MilMo_Skill> newSkills = GenerateNewSkills(skillTemplates);
		RefreshSkills(newSkills);
	}

	private static List<MilMo_Skill> GenerateNewSkills(IList<SkillTemplate> skillTemplates)
	{
		List<MilMo_Skill> list = new List<MilMo_Skill>();
		foreach (SkillTemplate skillTemplate in skillTemplates)
		{
			Code.World.Player.Skills.MilMo_SkillTemplate milMo_SkillTemplate = new Code.World.Player.Skills.MilMo_SkillTemplate(skillTemplate);
			list.AddRange(milMo_SkillTemplate.SkillModes);
		}
		return list;
	}

	private void RefreshSkills(List<MilMo_Skill> newSkills)
	{
		ClearRemovedSkills(newSkills);
		foreach (MilMo_Skill newSkill in newSkills)
		{
			if (!_skillEntries.Keys.Contains(newSkill))
			{
				AddEntry(newSkill);
			}
		}
	}

	private void ClearRemovedSkills(List<MilMo_Skill> newSkills)
	{
		foreach (MilMo_Skill item in _skillEntries.Keys.ToList())
		{
			if (!newSkills.Contains(item))
			{
				RemoveEntry(item);
			}
		}
	}

	private void SkillsUpdated(object skillMessageAsObj)
	{
		if (skillMessageAsObj is ServerSkillsAvailableUpdate serverSkillsAvailableUpdate)
		{
			CreateSkills(serverSkillsAvailableUpdate.getAvailableSkills());
			MilMo_EventSystem.Instance.PostEvent("tutorial_ClassAbility", null);
		}
	}

	public static SkillManager GetPlayerSkillManager()
	{
		GameObject gameObject = GameObject.FindWithTag("Player");
		if (!(gameObject == null))
		{
			return gameObject.GetComponent<SkillManager>();
		}
		return null;
	}
}
