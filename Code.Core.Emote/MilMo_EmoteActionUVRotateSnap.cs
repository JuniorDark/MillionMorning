using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteActionUVRotateSnap : MilMo_EmoteAction
{
	private string _name;

	private float _rotation;

	public MilMo_EmoteActionUVRotateSnap()
	{
	}

	private MilMo_EmoteActionUVRotateSnap(MilMo_EmoteActionUVRotateSnap action)
	{
		_name = action._name;
		_rotation = action._rotation;
	}

	public override bool Read(MilMo_SFFile file)
	{
		_name = file.GetString();
		_rotation = file.GetFloat();
		return true;
	}

	public override MilMo_EmoteMover GetMover(MilMo_EmoteManager manager)
	{
		return manager.GetRotationMover(_name);
	}

	public override void UpdateDefaultValues(MilMo_EmoteManager manager)
	{
		GetMover(manager).SetDefaultValues(new Vector2(_rotation, 0f));
	}

	protected override void ExecuteInternal(MilMo_EmoteManager manager)
	{
		MilMo_RotationMover obj = (MilMo_RotationMover)GetMover(manager);
		obj.Snapping = true;
		obj.Mover.Target.x = _rotation;
		obj.Mover.Val.x = _rotation;
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionUVRotateSnap(this);
	}
}
