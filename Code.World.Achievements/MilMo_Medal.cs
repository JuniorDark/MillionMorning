using System.Collections.Generic;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Code.World.Achievements;

public class MilMo_Medal : MilMo_Item
{
	private bool _acquired;

	public bool IsMine = true;

	public bool Acquired
	{
		get
		{
			return _acquired;
		}
		set
		{
			_acquired = value;
			this.OnAcquired?.Invoke();
		}
	}

	public new MilMo_AchievementTemplate Template => base.Template as MilMo_AchievementTemplate;

	public event UnityAction OnAcquired;

	public MilMo_Medal(MilMo_ItemTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override MilMo_LocString GetDescription()
	{
		if (!IsMine)
		{
			return Template.FeedDescriptionExternal;
		}
		return Template.Description;
	}

	public override bool IsWearable()
	{
		return false;
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public void CalculateProgress(out int target, out int current)
	{
		target = 0;
		current = 0;
		MilMo_AchievementHandler milMo_AchievementHandler = MilMo_AchievementHandler.Get();
		foreach (MilMo_AchievementObjective objective in Template.Objectives)
		{
			if (objective.Type == "FindAllSecrets")
			{
				string @object = objective.Object;
				MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(@object);
				if (levelInfoData == null)
				{
					Debug.LogWarning("Trying to get objective 'FindAllSecrets' data for unknown level " + @object);
					continue;
				}
				target += levelInfoData.ExplorationTokens.Count;
				current += levelInfoData.NumberOfExplorationTokensFound;
			}
			else if (objective.Type == "CompleteMedal")
			{
				if (objective.Object == "Any")
				{
					target += objective.Count;
					current += Mathf.Min(milMo_AchievementHandler.NumberCompleted, objective.Count);
				}
				else
				{
					target++;
					current += (milMo_AchievementHandler.IsCompleted(objective.Object) ? 1 : 0);
				}
			}
			else if (objective.Type == "CompleteQuest")
			{
				if (MilMo_Player.Instance != null && MilMo_Player.Instance.Quests != null)
				{
					if (objective.Object == "Any")
					{
						target += objective.Count;
						current += Mathf.Min(MilMo_Player.Instance.Quests.CompletedQuests.Count, objective.Count);
					}
					else
					{
						target++;
						current += (MilMo_Player.Instance.Quests.IsCompleted(objective.Object) ? 1 : 0);
					}
				}
			}
			else
			{
				target += objective.Count;
				MilMo_AchievementCounter counter = milMo_AchievementHandler.GetCounter(objective.Type, objective.Object);
				if (counter == null)
				{
					Debug.LogWarning("Trying to get non existing achievement counter " + objective.Type + ":" + objective.Object);
				}
				else
				{
					current += Mathf.Min(counter.Count, objective.Count);
				}
			}
		}
		if (target == 0)
		{
			target = 1;
			current = (Acquired ? 1 : 0);
		}
	}
}
