using System;
using Code.Core.Network.types;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Gameplay;

public abstract class MilMo_GameplayTriggerReaction
{
	public abstract void Activate(MilMo_GameplayObject obj, MilMo_Player player);

	public abstract void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player);

	public virtual bool MayActivate()
	{
		return true;
	}

	public virtual void SetPositionAndRoom(Vector3 position, string room)
	{
	}

	public static MilMo_GameplayTriggerReaction GetReaction(GameplayTriggerReaction reaction)
	{
		if (reaction.GetTemplateType().Equals("Spline", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionSplineRide(reaction.GetName());
		}
		if (reaction.GetTemplateType().Equals("ObjectEffect", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionObjectEffect(reaction.GetName());
		}
		if (reaction.GetTemplateType().Equals("LocalObjectEffect", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionLocalObjectEffect(reaction.GetName());
		}
		if (reaction.GetTemplateType().Equals("Camera", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionCamera(reaction.GetName());
		}
		if (reaction.GetTemplateType().Equals("Teleport", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionTeleport(reaction.GetName());
		}
		if (reaction.GetTemplateType().Equals("PlayerEffect", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionPlayerEffect(reaction.GetName());
		}
		if (reaction.GetTemplateType().Equals("Mover", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionMover(reaction.GetName());
		}
		if (reaction.GetTemplateType().Equals("EnterTown", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionEnterTown();
		}
		if (reaction.GetTemplateType().Equals("EnterShop", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionEnterShop();
		}
		if (reaction.GetTemplateType().Equals("EnterNavigator", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionEnterNavigator();
		}
		if (reaction.GetTemplateType().Equals("EnterMakeoverStudio", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionEnterMakeoverStudio();
		}
		if (reaction.GetTemplateType().Equals("Capture", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionCapture();
		}
		if (reaction.GetTemplateType().Equals("Deliver", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_GameplayTriggerReactionDeliver();
		}
		return null;
	}
}
