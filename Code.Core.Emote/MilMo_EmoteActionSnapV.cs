using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Emote;

public class MilMo_EmoteActionSnapV : MilMo_EmoteAction
{
	private string _name;

	private float _vOffset;

	public MilMo_EmoteActionSnapV()
	{
	}

	private MilMo_EmoteActionSnapV(MilMo_EmoteActionSnapV action)
	{
		_name = action._name;
		_vOffset = action._vOffset;
	}

	public override bool Read(MilMo_SFFile file)
	{
		string @string = file.GetString();
		string string2 = file.GetString();
		_name = @string;
		if (!MilMo_EmoteSystem.UVPresets.TryGetValue(string2, out var value))
		{
			Debug.LogWarning("Trying to use non existing preset '" + string2 + "' in SnapV action at line " + file.GetLineNumber());
			return false;
		}
		_vOffset = value.y;
		return true;
	}

	public override MilMo_EmoteMover GetMover(MilMo_EmoteManager manager)
	{
		return manager.GetUVMover(_name);
	}

	public override void UpdateDefaultValues(MilMo_EmoteManager manager)
	{
		GetMover(manager).SetDefaultValues(new Vector2(0f, _vOffset));
	}

	protected override void ExecuteInternal(MilMo_EmoteManager manager)
	{
		MilMo_UVMover obj = (MilMo_UVMover)GetMover(manager);
		obj.Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		obj.Mover.Target.y = _vOffset;
		obj.Mover.Val.y = _vOffset;
		obj.Mover.Pull = new Vector2(0.005f, 0.005f);
		obj.Mover.Drag = new Vector2(0.95f, 0.95f);
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionSnapV(this);
	}
}
