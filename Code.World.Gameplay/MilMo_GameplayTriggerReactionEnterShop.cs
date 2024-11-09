using Code.World.Player;

namespace Code.World.Gameplay;

public class MilMo_GameplayTriggerReactionEnterShop : MilMo_GameplayTriggerReaction
{
	public override bool MayActivate()
	{
		return MilMo_Player.Instance.OkToEnterShop();
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		MilMo_Player.Instance.RequestEnterShop();
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
	}
}
