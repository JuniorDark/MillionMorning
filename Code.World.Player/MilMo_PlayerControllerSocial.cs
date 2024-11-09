using Code.Core.Avatar;
using Code.Core.EventSystem;

namespace Code.World.Player;

public class MilMo_PlayerControllerSocial : MilMo_PlayerControllerGame
{
	private readonly MilMo_GenericReaction _avatarLoadedReaction;

	public override ControllerType Type => ControllerType.Social;

	public MilMo_PlayerControllerSocial()
	{
		MilMo_EventSystem.RemoveReaction(RunWalkButtonReaction);
		if (MilMo_PlayerControllerBase.RunMode)
		{
			MilMo_PlayerControllerBase.ToggleRunWalk();
		}
		_avatarLoadedReaction = MilMo_EventSystem.Listen("avatar_loaded", DisableCharacterController);
		_avatarLoadedReaction.Repeating = true;
	}

	public override void Exit()
	{
		base.Exit();
		MilMo_EventSystem.RemoveReaction(_avatarLoadedReaction);
		if (!MilMo_PlayerControllerBase.RunMode)
		{
			MilMo_PlayerControllerBase.ToggleRunWalk();
		}
	}

	private static void DisableCharacterController(object avatarAsObj)
	{
		if (avatarAsObj is MilMo_Avatar milMo_Avatar)
		{
			milMo_Avatar.ShouldCollideWithPlayers = false;
		}
	}
}
