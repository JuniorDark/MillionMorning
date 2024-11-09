using Code.World.Player;
using UnityEngine;

namespace Code.World.Gameplay;

public sealed class MilMo_GameplayTriggerReactionDeliver : MilMo_GameplayTriggerReaction
{
	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		Debug.Log("local player delivered flag!");
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
		Debug.Log("remote player delivered flag!");
	}
}
