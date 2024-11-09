using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_BoneScaleMover : MilMo_EmoteMover
{
	private readonly Transform _boneTransform;

	public MilMo_BoneScaleMover(string name, MilMo_EmoteManager manager)
		: base(name)
	{
		if (manager == null || manager.Renderer == null)
		{
			return;
		}
		DefaultValues.Set(1f, 1f);
		Transform[] bones = manager.Renderer.bones;
		foreach (Transform transform in bones)
		{
			if (!(transform.name != name))
			{
				_boneTransform = transform;
				break;
			}
		}
	}

	public override void ResetDefaultValues()
	{
		DefaultValues.Set(1f, 1f);
	}

	public override void LateUpdate(MilMo_EmoteManager manager)
	{
		if (!(_boneTransform == null))
		{
			_boneTransform.localScale.Set(1f, base.Mover.Val.y, 1f);
		}
	}
}
