using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteActionUVRotate : MilMo_EmoteAction
{
	private string _name;

	private float _rotation;

	private MilMo_FaceMover _faceMover = new MilMo_FaceMover();

	public MilMo_EmoteActionUVRotate()
	{
	}

	private MilMo_EmoteActionUVRotate(MilMo_EmoteActionUVRotate action)
	{
		_name = action._name;
		_rotation = action._rotation;
		_faceMover = action._faceMover;
	}

	public override bool Read(MilMo_SFFile file)
	{
		_name = file.GetString();
		_rotation = file.GetFloat();
		if (file.HasMoreTokens())
		{
			string @string = file.GetString();
			_faceMover = MilMo_FaceMover.GetFaceMover(@string);
			if (_faceMover == null)
			{
				Debug.LogWarning("Could not find face mover " + @string + ". It is used in uv rotate at line " + file.GetLineNumber());
			}
		}
		if (_faceMover == null)
		{
			_faceMover = new MilMo_FaceMover();
		}
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
		obj.Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		obj.Mover.Target.x = _rotation;
		obj.Mover.Pull = _faceMover.Pull;
		obj.Mover.Drag = _faceMover.Drag;
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionUVRotate(this);
	}
}
