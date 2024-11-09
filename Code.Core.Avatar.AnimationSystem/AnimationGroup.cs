using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Core.Avatar.AnimationSystem;

public class AnimationGroup
{
	public class PartialBone
	{
		public string Bone;

		public string Postfix;
	}

	private readonly List<AvatarAnimation> _animations = new List<AvatarAnimation>();

	private readonly Dictionary<string, List<PartialBone>> _partials = new Dictionary<string, List<PartialBone>>();

	private readonly AnimationGroups _context;

	public string Name { get; private set; }

	public bool Loop { get; private set; } = true;


	public int Layer { get; private set; }

	public IEnumerable<AvatarAnimation> Animations => _animations;

	public Dictionary<string, List<PartialBone>> Partials => _partials;

	public AnimationGroup(AnimationGroups context)
	{
		_context = context;
		Layer = 0;
	}

	public AvatarAnimation GetAnimation(string animationName)
	{
		return _animations.FirstOrDefault((AvatarAnimation anim) => anim.Name.Equals(animationName, StringComparison.InvariantCultureIgnoreCase));
	}

	public bool Contains(string animation)
	{
		return GetAnimation(animation) != null;
	}

	public float GetBlendTime(string animation)
	{
		float num = GetAnimation(animation)?.BlendTime ?? 0f;
		if (!(num > 0.01f))
		{
			return 0.3f;
		}
		return num;
	}

	public bool TryGetPartial(string animationGroupPlaying, out List<PartialBone> bones)
	{
		return _partials.TryGetValue(animationGroupPlaying, out bones);
	}

	public bool Load(AnimationGroupSO group)
	{
		Name = group.name;
		foreach (AvatarAnimation animation in group.GetAnimations())
		{
			AnimationGroup group2 = _context.GetGroup(animation.Name);
			if (group2 != null)
			{
				Debug.LogWarning("Trying to add animation " + animation?.ToString() + " to group " + Name + " when already in group " + group2.Name);
			}
			else
			{
				_animations.Add(animation);
			}
		}
		Loop = group.IsLooping();
		Layer = group.GetLayer();
		foreach (MixingRule mixingRule in group.GetMixingRules())
		{
			string group3 = mixingRule.group;
			string postfix = mixingRule.postfix;
			string bone = mixingRule.bone;
			if (!_partials.TryGetValue(group3, out var value))
			{
				value = new List<PartialBone>();
				_partials.Add(group3, value);
			}
			PartialBone item = new PartialBone
			{
				Postfix = postfix,
				Bone = bone
			};
			value.Add(item);
		}
		return true;
	}
}
