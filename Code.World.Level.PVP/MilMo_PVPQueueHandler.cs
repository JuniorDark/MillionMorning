using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Network.messages.server.PVP;
using Code.Core.ResourceSystem;
using Code.World.GUI.Hub;
using Code.World.GUI.LoadingScreen;
using Code.World.GUI.PVP;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Code.World.WorldMap;
using UnityEngine;

namespace Code.World.Level.PVP;

public static class MilMo_PVPQueueHandler
{
	private static MilMoPvpQueueStatusDialog _pvpQueueStatusDialog;

	private static MilMoPvpJoinQueueWindow _pvpJoinQueueWindow;

	private static MilMo_UserInterface _pvpUI;

	private static bool _created;

	public static void Create()
	{
		if (!_created)
		{
			InitGUI();
			MilMo_EventSystem.Listen("pvp_queue_size_update", QueueSizeUpdate).Repeating = true;
			MilMo_EventSystem.Listen("pvp_queues", QueueInfoArrived).Repeating = true;
			MilMo_EventSystem.Listen("pvp_ok_to_join", OkToJoin).Repeating = true;
			_created = true;
		}
	}

	private static void InitGUI()
	{
		_pvpUI = MilMo_UserInterfaceManager.CreateUserInterface("_pvpUI");
		MilMo_UserInterfaceManager.SetUserInterfaceDepth(_pvpUI, -500);
		_pvpJoinQueueWindow = new MilMoPvpJoinQueueWindow(_pvpUI);
		_pvpUI.AddChild(_pvpJoinQueueWindow);
		_pvpQueueStatusDialog = new MilMoPvpQueueStatusDialog(_pvpUI);
		_pvpUI.AddChild(_pvpQueueStatusDialog);
	}

	private static void ShowPVPQueueStatus(MilMo_LocString matchModeName, int queueSize, int maxSize)
	{
		_pvpQueueStatusDialog.Open(matchModeName, queueSize, maxSize);
		_pvpQueueStatusDialog.BringToFront();
	}

	private static void ClosePVPQueueStatus()
	{
		_pvpQueueStatusDialog.Close(null);
	}

	private static void OpenJoinPVPQueueWindow(ICollection<QueueInfo> queueInfoList)
	{
		_pvpJoinQueueWindow.Open(queueInfoList);
	}

	private static void QueueSizeUpdate(object msgAsObject)
	{
		if (!(msgAsObject is ServerPvPQueueSizeUpdate serverPvPQueueSizeUpdate))
		{
			Debug.LogWarning("PVP queue size update message was null");
		}
		else
		{
			ShowPVPQueueStatus(MilMo_Localization.GetLocString(serverPvPQueueSizeUpdate.QueueInfo.MatchMode.Title()), serverPvPQueueSizeUpdate.QueueInfo.QueueSize, serverPvPQueueSizeUpdate.QueueInfo.MaxQueueSize);
		}
	}

	private static void QueueInfoArrived(object msgAsObject)
	{
		if (!(msgAsObject is ServerPvPQueues serverPvPQueues))
		{
			Debug.LogWarning("PVP queues message was null");
		}
		else
		{
			OpenJoinPVPQueueWindow(serverPvPQueues.GetQueues());
		}
	}

	private static void OkToJoin(object msgAsObject)
	{
		ServerPvPOkToJoin msg = msgAsObject as ServerPvPOkToJoin;
		if (msg == null)
		{
			Debug.LogWarning("PvP ok to join message was null");
		}
		else if (MilMo_LoadingScreen.Instance.LoadingState != 0)
		{
			MilMo_EventSystem.At(1f, delegate
			{
				OkToJoin(msg);
			});
		}
		else
		{
			ClosePVPQueueStatus();
			PrepareLevelJoin(msg.LevelToken);
		}
	}

	private static void PrepareLevelJoin(string levelToken)
	{
		if (MilMo_Player.Instance.InNavigator)
		{
			MilMo_WorldMap.WasTravelClosed = true;
			MilMo_WorldMap.TravelClosedFullLevelName = levelToken;
			MilMo_Player.Instance.RequestLeaveNavigator();
		}
		else if (MilMo_Player.Instance.InHub)
		{
			MilMo_Hub.WasTravelClosed = true;
			MilMo_Hub.TravelClosedFullLevelName = levelToken;
			MilMo_Player.Instance.RequestLeaveHub();
		}
		else
		{
			MilMo_LevelInfo.Travel(levelToken);
		}
	}
}
