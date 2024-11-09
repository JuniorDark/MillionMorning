using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Visual;
using Code.World.Level;
using Code.World.Player;
using UnityEngine;

namespace Code.World;

public sealed class MilMo_RemotePlayerCollisionHandler
{
	private readonly List<IMilMoNoPlayerCollisionArea> _mNoPlayerCollisionAreas = new List<IMilMoNoPlayerCollisionArea>();

	public static MilMo_RemotePlayerCollisionHandler Instance { get; private set; }

	private MilMo_RemotePlayerCollisionHandler()
	{
		MilMo_EventSystem.Listen("no_playercollision_area_created", AddNoCollisionArea).Repeating = true;
		MilMo_EventSystem.Listen("no_playercollision_area_destroyed", RemoveNoCollisionArea).Repeating = true;
	}

	static MilMo_RemotePlayerCollisionHandler()
	{
		Instance = new MilMo_RemotePlayerCollisionHandler();
	}

	public void Update(bool playerIsCloseToClimbingSurface)
	{
		if (MilMo_Instance.CurrentInstance == null || MilMo_Player.Instance.Avatar == null || MilMo_Player.Instance.Avatar.GameObject == null)
		{
			return;
		}
		Vector3 position = MilMo_Player.Instance.Avatar.Position;
		bool enable = true;
		if (MilMo_World.Instance.PlayerController is MilMo_PlayerControllerSocial || MilMo_Player.Instance.IsClimbing || playerIsCloseToClimbingSurface || MilMo_Player.Instance.IsOnAnyMovingPlatform || MilMo_Player.Instance.IsCloseToMovingPlatform)
		{
			enable = false;
		}
		else
		{
			foreach (IMilMoNoPlayerCollisionArea mNoPlayerCollisionArea in _mNoPlayerCollisionAreas)
			{
				if (mNoPlayerCollisionArea.Enabled && Vector3.SqrMagnitude(mNoPlayerCollisionArea.Position - position) < mNoPlayerCollisionArea.SqrRadius)
				{
					enable = false;
					break;
				}
			}
		}
		MilMo_Instance.CurrentInstance.RemotePlayerCollision(enable);
	}

	private void AddNoCollisionArea(object areaAsObj)
	{
		if (areaAsObj is IMilMoNoPlayerCollisionArea item)
		{
			_mNoPlayerCollisionAreas.Add(item);
		}
	}

	private void RemoveNoCollisionArea(object areaAsObj)
	{
		if (areaAsObj is IMilMoNoPlayerCollisionArea item)
		{
			_mNoPlayerCollisionAreas.Remove(item);
		}
	}

	public void Clear()
	{
		_mNoPlayerCollisionAreas.Clear();
	}
}
