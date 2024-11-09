using Code.Core.ResourceSystem;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteActionEnd : MilMo_EmoteAction
{
	public override bool Read(MilMo_SFFile file)
	{
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
		manager.AbortEmote();
	}

	public override MilMo_EmoteAction Instantiate()
	{
		return new MilMo_EmoteActionEnd();
	}
}
