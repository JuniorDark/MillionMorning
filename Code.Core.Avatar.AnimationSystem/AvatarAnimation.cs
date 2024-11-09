using System;
using UnityEngine;

namespace Code.Core.Avatar.AnimationSystem;

[Serializable]
public class AvatarAnimation
{
	[SerializeField]
	private string name;

	[SerializeField]
	private float blendTime;

	public string Name => name;

	public float BlendTime => blendTime;

	public AvatarAnimation(string name, float blendTime)
	{
		this.name = name;
		this.blendTime = blendTime;
	}
}
