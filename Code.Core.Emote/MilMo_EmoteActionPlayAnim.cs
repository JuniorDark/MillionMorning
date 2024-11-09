using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteActionPlayAnim : MilMo_EmoteAction
{
	private string _anim;

	public MilMo_EmoteActionPlayAnim()
	{
	}

	private MilMo_EmoteActionPlayAnim(MilMo_EmoteActionPlayAnim action)
	{
		_anim = action._anim;
	}

	public override bool Read(MilMo_SFFile file)
	{
		_anim = file.GetString();
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
		if (MilMo_EmoteAction.DevMode)
		{
			Debug.Log("MilMo_EmoteActionPlayAnim: Playing animation " + _anim + " for emote");
		}
		manager.PlayAnimation(_anim);
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionPlayAnim(this);
	}
}
