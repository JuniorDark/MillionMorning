using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteActionSnapUV : MilMo_EmoteAction
{
	private string _name;

	private float _uOffset;

	private float _vOffset;

	public MilMo_EmoteActionSnapUV()
	{
	}

	private MilMo_EmoteActionSnapUV(MilMo_EmoteActionSnapUV action)
	{
		_name = action._name;
		_uOffset = action._uOffset;
		_vOffset = action._vOffset;
	}

	public override bool Read(MilMo_SFFile file)
	{
		string @string = file.GetString();
		string string2 = file.GetString();
		_name = @string;
		if (!MilMo_EmoteSystem.UVPresets.TryGetValue(string2, out var value))
		{
			Debug.LogWarning("Trying to use non existing preset '" + string2 + "' in SnapUV action at line " + file.GetLineNumber());
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
		obj.Snapping = true;
		obj.Mover.Target.x = _uOffset;
		obj.Mover.Target.y = _vOffset;
		obj.Mover.Val.x = _uOffset;
		obj.Mover.Val.y = _vOffset;
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionSnapUV(this);
	}
}
