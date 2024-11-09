using Code.World.Player;

namespace Code.World.Gameplay;

public class MilMo_GameplayTriggerReactionEnterNavigator : MilMo_GameplayTriggerReaction
{
	public override bool MayActivate()
	{
		return MilMo_Player.Instance.OkToEnterNavigator();
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		MilMo_Player.Instance.RequestEnterNavigator();
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
	}
}
