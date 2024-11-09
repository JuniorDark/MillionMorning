using Code.Core.Template;
using Code.World.Player;
using Core;
using Core.Analytics;
using UnityEngine;

namespace Code.World.Gameplay;

public class MilMo_GameplayTriggerReactionTeleport : MilMo_GameplayTriggerReaction
{
	private MilMo_TeleportTemplate _teleport;

	private Vector3 _fromPosition;

	private string _fromRoom = "NOT_SET";

	public MilMo_GameplayTriggerReactionTeleport(string templatePath)
	{
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("Teleport", templatePath, TemplateLoaded);
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		if (_teleport != null)
		{
			Analytics.Teleport(player.Avatar.Position);
			_teleport.Activate(player);
		}
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
	}

	public override void SetPositionAndRoom(Vector3 position, string room)
	{
		_fromPosition = position;
		_fromRoom = room;
		RegisterAsDoor();
	}

	public override bool MayActivate()
	{
		if (_teleport != null)
		{
			return base.MayActivate();
		}
		return false;
	}

	private void TemplateLoaded(MilMo_Template t, bool timeout)
	{
		if (timeout)
		{
			Debug.LogWarning("Failed to load teleport template: timeout");
			return;
		}
		if (!(t is MilMo_TeleportTemplate teleport))
		{
			Debug.LogWarning("Failed to load teleport template: template is null");
			return;
		}
		_teleport = teleport;
		RegisterAsDoor();
	}

	private void RegisterAsDoor()
	{
		if (!(_fromRoom == "NOT_SET") && _teleport is MilMo_IntraTeleportTemplate milMo_IntraTeleportTemplate && !string.IsNullOrEmpty(milMo_IntraTeleportTemplate.Room) && !milMo_IntraTeleportTemplate.NoPlop)
		{
			MilMo_RoomPlopManager.Instance.AddDoor(_fromRoom, milMo_IntraTeleportTemplate.Room, _fromPosition);
		}
	}
}
