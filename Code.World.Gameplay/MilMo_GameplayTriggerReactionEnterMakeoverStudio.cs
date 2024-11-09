using Code.World.Player;

namespace Code.World.Gameplay;

public class MilMo_GameplayTriggerReactionEnterMakeoverStudio : MilMo_GameplayTriggerReaction
{
	public override bool MayActivate()
	{
		return MilMo_Player.Instance.OkToEnterCharBuilder();
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		MilMo_Player.Instance.RequestEnterCharBuilder();
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
	}
}
