using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Core.Avatar.AnimationSystem;

public class AnimationGroups
{
	private readonly Dictionary<string, AnimationGroup> _groups = new Dictionary<string, AnimationGroup>();

	public AnimationGroups()
	{
		LoadGroups();
	}

	public List<AnimationGroup> GetGroups()
	{
		return _groups.Values.ToList();
	}

	public AnimationGroup GetGroup(string animation)
	{
		return _groups.Values.FirstOrDefault((AnimationGroup group) => group.Contains(animation));
	}

	private void LoadGroups()
	{
		AnimationGroupSO[] array = Resources.LoadAll<AnimationGroupSO>("Avatar/AnimationGroups");
		foreach (AnimationGroupSO animationGroupSO in array)
		{
			AnimationGroup animationGroup = new AnimationGroup(this);
			if (animationGroup.Load(animationGroupSO))
			{
				_groups.Add(animationGroupSO.name, animationGroup);
			}
			else
			{
				Debug.LogWarning("Failed to load animation group " + animationGroupSO.name);
			}
			Resources.UnloadAsset(animationGroupSO);
		}
	}
}
