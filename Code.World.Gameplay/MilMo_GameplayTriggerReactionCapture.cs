using Code.World.Player;

namespace Code.World.Gameplay;

public sealed class MilMo_GameplayTriggerReactionCapture : MilMo_GameplayTriggerReaction
{
	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		obj.PlayerCapturedObject(player.Id);
		obj.SetCapturerAsParent();
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
		obj.PlayerCapturedObject(player.Id);
		obj.SetCapturerAsParent();
	}
}
