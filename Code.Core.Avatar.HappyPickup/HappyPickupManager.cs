using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Code.World.Player;
using UnityEngine;

namespace Code.Core.Avatar.HappyPickup;

public class HappyPickupManager : MonoBehaviour
{
	private readonly Queue<HappyPickup> _queue = new Queue<HappyPickup>();

	private HappyPickup _happyAbout;

	private MilMo_GenericReaction _receiveObjectHappyReaction;

	private MilMo_Player PlayerInstance => MilMo_Player.Instance;

	private MilMo_Instance LevelInstance => MilMo_Instance.CurrentInstance;

	protected void OnEnable()
	{
		_receiveObjectHappyReaction = MilMo_EventSystem.Listen("receive_object_happy", ReceiveObjectHappy);
		_receiveObjectHappyReaction.Repeating = true;
	}

	protected void OnDisable()
	{
		MilMo_EventSystem.RemoveReaction(_receiveObjectHappyReaction);
		_receiveObjectHappyReaction = null;
	}

	private void Update()
	{
		if (_happyAbout == null && _queue.Count >= 1 && CanBeHappy())
		{
			_happyAbout = _queue.Dequeue();
			_happyAbout.BeHappy(StopBeingHappy);
		}
	}

	private void ReceiveObjectHappy(object msgAsObj)
	{
		if (!(msgAsObj is ServerReceiveObjectHappy serverReceiveObjectHappy))
		{
			return;
		}
		string playerId = serverReceiveObjectHappy.GetPlayerID();
		TemplateReference reference = serverReceiveObjectHappy.GetReference();
		new MilMo_LevelItem(spawnEffect: false).ReadGeneric(reference, delegate(bool success, MilMo_LevelObject obj)
		{
			if (!success || !(obj is MilMo_LevelItem item))
			{
				Debug.LogWarning(base.gameObject.name + ": Failed to load level item");
			}
			else
			{
				HappyPickupItem happyPickupItem = HappyPickupItem.Create(item);
				if (happyPickupItem == null || happyPickupItem.GameObject == null || happyPickupItem.HappyPickupTemplate == null)
				{
					Debug.LogWarning(base.gameObject.name + ": Failed to create happy pickup item");
				}
				else
				{
					HappyPickup happyPickup = GetHappyPickup(playerId, happyPickupItem);
					ProcessHappyPickup(happyPickup);
				}
			}
		});
	}

	private void ProcessHappyPickup(HappyPickup happyPickup)
	{
		if (!(happyPickup is HappyPickupLocal))
		{
			if (happyPickup is HappyPickupRemote)
			{
				if (CanSeeHappy())
				{
					happyPickup.BeHappy();
				}
			}
			else
			{
				Debug.LogWarning(base.gameObject.name + ": Failed to get happy pickup type");
			}
		}
		else
		{
			_queue.Enqueue(happyPickup);
		}
	}

	private HappyPickup GetHappyPickup(string playerId, HappyPickupItem happyPickupItem)
	{
		if (PlayerInstance == null)
		{
			return null;
		}
		if (playerId != PlayerInstance.Id)
		{
			MilMo_RemotePlayer milMo_RemotePlayer = LevelInstance?.GetRemotePlayer(playerId);
			if (milMo_RemotePlayer == null || milMo_RemotePlayer.Avatar == null)
			{
				return null;
			}
			return new HappyPickupRemote(milMo_RemotePlayer.Avatar, happyPickupItem);
		}
		MilMo_Player playerInstance = PlayerInstance;
		if (playerInstance == null || playerInstance.Avatar == null)
		{
			return null;
		}
		return new HappyPickupLocal(PlayerInstance.Avatar, happyPickupItem);
	}

	private void StopBeingHappy()
	{
		_happyAbout = null;
	}

	private bool CanSeeHappy()
	{
		if (PlayerInstance.InShop)
		{
			return false;
		}
		return true;
	}

	private bool CanBeHappy()
	{
		if (PlayerInstance?.Avatar == null)
		{
			return false;
		}
		if (PlayerInstance.InShop)
		{
			return false;
		}
		if (PlayerInstance.Avatar.InHappyPickup)
		{
			return false;
		}
		if (PlayerInstance.Avatar.InCombat || PlayerInstance.InSpline)
		{
			return false;
		}
		if (PlayerInstance.IsTalking)
		{
			return false;
		}
		return true;
	}
}
