using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteActionGotoUV : MilMo_EmoteAction
{
	private string _name;

	private float _uOffset;

	private float _vOffset;

	private MilMo_FaceMover _faceMover = new MilMo_FaceMover();

	public MilMo_EmoteActionGotoUV()
	{
	}

	private MilMo_EmoteActionGotoUV(MilMo_EmoteActionGotoUV action)
	{
		_name = action._name;
		_uOffset = action._uOffset;
		_vOffset = action._vOffset;
		_faceMover = action._faceMover;
	}

	public override bool Read(MilMo_SFFile file)
	{
		string @string = file.GetString();
		string string2 = file.GetString();
		if (file.HasMoreTokens())
		{
			string string3 = file.GetString();
			_faceMover = MilMo_FaceMover.GetFaceMover(string3);
			if (_faceMover == null)
			{
				Debug.LogWarning("Could not find face mover " + string3 + ". It is used in goto uv at line " + file.GetLineNumber());
			}
		}
		if (_faceMover == null)
		{
			_faceMover = new MilMo_FaceMover();
		}
		_name = @string;
		if (!MilMo_EmoteSystem.UVPresets.TryGetValue(string2, out var value))
		{
			Debug.LogWarning("Trying to use non existing preset '" + string2 + "' in GotoUV action at line " + file.GetLineNumber());
			return false;
		}
		_uOffset = value.x;
		_vOffset = value.y;
		return true;
	}

	public override MilMo_EmoteMover GetMover(MilMo_EmoteManager manager)
	{
		return manager.GetUVMover(_name);
	}

	public override void UpdateDefaultValues(MilMo_EmoteManager manager)
	{
		GetMover(manager).SetDefaultValues(new Vector2(_uOffset, _vOffset));
	}

	protected override void ExecuteInternal(MilMo_EmoteManager manager)
	{
		MilMo_UVMover obj = (MilMo_UVMover)GetMover(manager);
		obj.Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		obj.Mover.Target.x = _uOffset;
		obj.Mover.Target.y = _vOffset;
		obj.Mover.Pull = _faceMover.Pull;
		obj.Mover.Drag = _faceMover.Drag;
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionGotoUV(this);
	}
}
