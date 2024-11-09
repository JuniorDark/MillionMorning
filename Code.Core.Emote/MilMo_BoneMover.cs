using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_BoneMover : MilMo_EmoteMover
{
	private readonly Transform _mBoneTransform;

	public MilMo_BoneMover(string name, MilMo_EmoteManager manager)
		: base(name)
	{
		if (manager == null || manager.Renderer == null)
		{
			return;
		}
		Transform[] bones = manager.Renderer.bones;
		foreach (Transform transform in bones)
		{
			if (transform.name == name)
			{
				_mBoneTransform = transform;
			}
		}
	}

	public override void LateUpdate(MilMo_EmoteManager manager)
	{
		if (!(_mBoneTransform == null))
		{
			Vector3 localEulerAngles = _mBoneTransform.localEulerAngles;
			_mBoneTransform.localEulerAngles = new Vector3(base.Mover.Val.x, localEulerAngles.y, localEulerAngles.z);
		}
	}
}
