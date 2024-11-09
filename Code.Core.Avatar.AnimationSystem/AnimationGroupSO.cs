using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Core.Avatar.AnimationSystem;

[CreateAssetMenu(menuName = "Create AnimationGroupSO", fileName = "AnimationGroupSO", order = 0)]
public class AnimationGroupSO : ScriptableObject
{
	[SerializeField]
	private List<AvatarAnimation> animations = new List<AvatarAnimation>();

	[SerializeField]
	private bool loop;

	[SerializeField]
	private int layer;

	[SerializeField]
	private MixingRule[] mixingRules;

	public IEnumerable<AvatarAnimation> GetAnimations()
	{
		return animations.ToList();
	}

	public bool IsLooping()
	{
		return loop;
	}

	public int GetLayer()
	{
		return layer;
	}

	public IEnumerable<MixingRule> GetMixingRules()
	{
		return mixingRules.ToList();
	}

	public void AddAnimation(AvatarAnimation animationInfo)
	{
		animations.Add(animationInfo);
	}
}
