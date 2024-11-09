using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteActionScale : MilMo_EmoteAction
{
	private string _bone;

	private float _scale;

	private MilMo_FaceMover _mover = new MilMo_FaceMover();

	public MilMo_EmoteActionScale()
	{
	}

	private MilMo_EmoteActionScale(MilMo_EmoteActionScale action)
	{
		_bone = action._bone;
		_scale = action._scale;
		_mover = action._mover;
	}

	public override bool Read(MilMo_SFFile file)
	{
		if (!file.HasMoreTokens())
		{
			Debug.LogWarning("Scale action does not have a bone");
			return false;
		}
		_bone = file.GetString();
		if (file.HasMoreTokens())
		{
			_scale = file.GetFloat();
		}
		if (file.HasMoreTokens())
		{
			string @string = file.GetString();
			_mover = MilMo_FaceMover.GetFaceMover(@string);
			if (_mover == null)
			{
				Debug.LogWarning("Could not find face mover " + @string + ". It is used in scale at line " + file.GetLineNumber());
				_mover = new MilMo_FaceMover();
			}
		}
		return true;
	}

	public override MilMo_EmoteMover GetMover(MilMo_EmoteManager manager)
	{
		return manager.GetBoneScaleMover(_bone);
	}

	public override void UpdateDefaultValues(MilMo_EmoteManager manager)
	{
		GetMover(manager).SetDefaultValues(new Vector2(1f, _scale));
	}

	protected override void ExecuteInternal(MilMo_EmoteManager manager)
	{
		MilMo_BoneScaleMover obj = (MilMo_BoneScaleMover)GetMover(manager);
		obj.Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		obj.Mover.Target.y = _scale;
		obj.Mover.Pull = _mover.Pull;
		obj.Mover.Drag = _mover.Drag;
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionScale(this);
	}
}
