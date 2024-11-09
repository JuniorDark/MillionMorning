using Code.Core.Collision;
using Code.Core.EventSystem;
using Code.Core.Utility;
using Code.World.Chat.ChatRoom;
using UnityEngine;

namespace Code.World.Player;

public class MilMo_PlayerControllerChat : MilMo_PlayerControllerBase
{
	private readonly MilMo_GenericReaction _attackButtonReaction;

	private readonly MilMo_GenericReaction _useButtonReaction;

	private bool _enterFurnishingModeOnLeave;

	public override ControllerType Type => ControllerType.Chat;

	public static MilMo_Transform ExitPoint { get; set; }

	public MilMo_PlayerControllerChat()
	{
		MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation(MilMo_PlayerControllerBase.Player.Avatar.SitPose);
		MilMo_PlayerControllerBase.Player.SetWieldableModeFood();
		if (MilMo_Player.InMyHome)
		{
			MilMo_EventSystem.Listen("enter_furnishing_mode", TryEnterFurnishingMode);
		}
	}

	public override void UpdatePlayer()
	{
		base.UpdatePlayer();
		Vector3 vector = Vector3.zero;
		if (MilMo_PlayerControllerBase.Player.Avatar.Ass != null)
		{
			vector = MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.position - MilMo_PlayerControllerBase.Player.Avatar.Ass.position;
		}
		MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.position = MilMo_PlayerControllerBase.Player.Avatar.SitPoint.position + vector;
		MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = Quaternion.Euler(MilMo_PlayerControllerBase.Player.Avatar.SitPoint.eulerAngles);
	}

	public override void Exit()
	{
		base.Exit();
		Vector3 position = ExitPoint.Position;
		float distanceToGround = MilMo_Physics.GetDistanceToGround(position);
		position.y -= distanceToGround;
		MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.position = position;
		MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = Quaternion.Euler(ExitPoint.EulerRotation);
		MilMo_PlayerControllerBase.TargetRotation = MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation;
		MilMo_EventSystem.RemoveReaction(_attackButtonReaction);
		MilMo_EventSystem.RemoveReaction(_useButtonReaction);
		MilMo_PlayerControllerBase.Player.EnableWieldables(ignoreControllerType: true);
		if (_enterFurnishingModeOnLeave && MilMo_Player.InMyHome)
		{
			MilMo_EventSystem.NextFrame(delegate
			{
				MilMo_EventSystem.Instance.PostEvent("enter_furnishing_mode", null);
			});
		}
	}

	private void TryEnterFurnishingMode(object o)
	{
		if (MilMo_Player.InMyHome)
		{
			_enterFurnishingModeOnLeave = true;
			MilMo_ChatRoomManager.Instance.RequestLeave(null);
		}
	}
}
