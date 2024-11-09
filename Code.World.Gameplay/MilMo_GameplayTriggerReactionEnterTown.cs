using Code.World.Player;

namespace Code.World.Gameplay;

public sealed class MilMo_GameplayTriggerReactionEnterTown : MilMo_GameplayTriggerReaction
{
	public override bool MayActivate()
	{
		return MilMo_Player.Instance.OkToEnterHub();
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		MilMo_Player.Instance.RequestEnterHub();
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
	}
}
