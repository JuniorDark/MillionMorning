using Code.Core.ResourceSystem;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteActionWield : MilMo_EmoteAction
{
	private bool _wield = true;

	public MilMo_EmoteActionWield()
	{
	}

	private MilMo_EmoteActionWield(MilMo_EmoteActionWield action)
	{
		_wield = action._wield;
	}

	public override bool Read(MilMo_SFFile file)
	{
		if (file.HasMoreTokens())
		{
			_wield = file.GetBool();
		}
		return true;
	}

	public override MilMo_EmoteMover GetMover(MilMo_EmoteManager manager)
	{
		return null;
	}

	public override void UpdateDefaultValues(MilMo_EmoteManager manager)
	{
	}

	protected override void ExecuteInternal(MilMo_EmoteManager manager)
	{
		if (_wield)
		{
			manager.WieldWeapon();
		}
		else
		{
			manager.UnwieldWeapon();
		}
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionWield(this);
	}
}
