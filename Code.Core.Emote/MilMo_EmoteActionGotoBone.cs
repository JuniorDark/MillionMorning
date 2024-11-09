using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteActionGotoBone : MilMo_EmoteAction
{
	private string _bone;

	private float _angle;

	private MilMo_FaceMover _faceMover = new MilMo_FaceMover();

	public MilMo_EmoteActionGotoBone()
	{
	}

	private MilMo_EmoteActionGotoBone(MilMo_EmoteActionGotoBone action)
	{
		_bone = action._bone;
		_angle = action._angle;
		_faceMover = action._faceMover;
	}

	public override bool Read(MilMo_SFFile file)
	{
		_bone = file.GetString();
		_angle = file.GetFloat();
		if (file.HasMoreTokens())
		{
			string @string = file.GetString();
			_faceMover = MilMo_FaceMover.GetFaceMover(@string);
			if (_faceMover == null)
			{
				Debug.LogWarning("Could not find face mover " + @string + ". It is used in goto bone at line " + file.GetLineNumber());
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
		return manager.GetBoneMover(_bone);
	}

	public override void UpdateDefaultValues(MilMo_EmoteManager manager)
	{
		GetMover(manager).SetDefaultValues(new Vector2(_angle, 0f));
	}

	protected override void ExecuteInternal(MilMo_EmoteManager manager)
	{
		MilMo_BoneMover obj = (MilMo_BoneMover)GetMover(manager);
		obj.Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		obj.Mover.Target.x = _angle;
		obj.Mover.Pull = _faceMover.Pull;
		obj.Mover.Drag = _faceMover.Drag;
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionGotoBone(this);
	}
}
