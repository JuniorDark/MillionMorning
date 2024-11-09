using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.messages.server;
using Code.World.GUI;
using Code.World.Player;
using Core;
using Core.GameEvent;
using Localization;
using UI.HUD.Dialogues;
using UI.Sprites;
using UnityEngine;

namespace Player;

public class GroupManager : Singleton<GroupManager>
{
	private enum EInviteResponse
	{
		Success,
		AlreadyInGroup,
		GroupIsFull,
		UnknownError,
		IgnoredRequest,
		IsNotLeader,
		AlreadyInvited
	}

	private enum TravelResponse
	{
		Success,
		AlreadyTraveling,
		GotPendingInvites,
		GotUnavailableGroupMembers,
		NoGroup,
		FailedToLoadLevel,
		LevelNotUnlocked,
		CanNotPay,
		TravelAborted
	}

	private delegate void GroupCreatedCallback(bool success);

	private readonly List<GroupMember> _group = new List<GroupMember>();

	private bool _playerIsInGroup;

	private string _groupLeader = "";

	private readonly List<GroupMember> _groupInvites = new List<GroupMember>();

	private MilMoGroupTravelDialog _groupTravelDialog;

	private GroupCreatedCallback _groupCreatedCallback;

	private MilMo_GenericReaction _joinListener;

	private MilMo_GenericReaction _createListener;

	private MilMo_GenericReaction _leaveListener;

	private MilMo_GenericReaction _invitedListener;

	private MilMo_GenericReaction _memberUpdateListener;

	private MilMo_GenericReaction _memberKickedListener;

	private MilMo_GenericReaction _leaderChangedListener;

	private MilMo_GenericReaction _inviteResponseListener;

	private MilMo_GenericReaction _travelInitiatedListener;

	private MilMo_GenericReaction _travelResponseListener;

	private MilMo_GenericReaction _travelFailedListener;

	private MilMo_Player PlayerInstance => MilMo_Player.Instance;

	public bool PlayerIsInGroup
	{
		get
		{
			if (_playerIsInGroup)
			{
				return _group.Count > 1;
			}
			return false;
		}
	}

	public int NrOfGroupMembers => _group.Count;

	public bool HasInvitations
	{
		get
		{
			if (!_playerIsInGroup)
			{
				return _groupInvites.Count != 0;
			}
			return false;
		}
	}

	public int HasAcceptedTravel { get; private set; }

	public event Action<GroupMember> OnLeaveGroup = delegate
	{
	};

	public event Action<GroupMember> OnJoinGroup = delegate
	{
	};

	public event Action OnGroupDisband = delegate
	{
	};

	public event Action OnLeaderChange = delegate
	{
	};

	public event Action OnGroupChanged = delegate
	{
	};

	public static GroupManager Get()
	{
		return Singleton<GroupManager>.Instance;
	}

	public void OnEnable()
	{
		_joinListener = MilMo_EventSystem.Listen("player_join_group", JoinGroup);
		_joinListener.Repeating = true;
		_createListener = MilMo_EventSystem.Listen("player_create_group", CreateGroup);
		_createListener.Repeating = true;
		_leaveListener = MilMo_EventSystem.Listen("player_leave_group", LeaveGroup);
		_leaveListener.Repeating = true;
		_invitedListener = MilMo_EventSystem.Listen("group_invited", InvitedToGroup);
		_invitedListener.Repeating = true;
		_inviteResponseListener = MilMo_EventSystem.Listen("group_invite_response", InviteResponse);
		_inviteResponseListener.Repeating = true;
		_memberUpdateListener = MilMo_EventSystem.Listen("group_member_update", GroupMembersUpdate);
		_memberUpdateListener.Repeating = true;
		_memberKickedListener = MilMo_EventSystem.Listen("group_member_kicked", KickedFromGroup);
		_memberKickedListener.Repeating = true;
		_leaderChangedListener = MilMo_EventSystem.Listen("group_leader_changed", LeaderChanged);
		_leaderChangedListener.Repeating = true;
		_travelInitiatedListener = MilMo_EventSystem.Listen("group_travel_initiated", TravelInitiated);
		_travelInitiatedListener.Repeating = true;
		_travelResponseListener = MilMo_EventSystem.Listen("group_travel_response", GroupTravelResponse);
		_travelResponseListener.Repeating = true;
		_travelFailedListener = MilMo_EventSystem.Listen("group_travel_failed", GroupTravelFailed);
		_travelFailedListener.Repeating = true;
		OnLeaveGroup += GroupChanged;
		OnJoinGroup += GroupChanged;
		OnGroupDisband += GroupChanged;
	}

	private void OnDisable()
	{
		MilMo_EventSystem.RemoveReaction(_joinListener);
		_joinListener = null;
		MilMo_EventSystem.RemoveReaction(_createListener);
		_createListener = null;
		MilMo_EventSystem.RemoveReaction(_leaveListener);
		_leaveListener = null;
		MilMo_EventSystem.RemoveReaction(_invitedListener);
		_invitedListener = null;
		MilMo_EventSystem.RemoveReaction(_inviteResponseListener);
		_inviteResponseListener = null;
		MilMo_EventSystem.RemoveReaction(_memberUpdateListener);
		_memberUpdateListener = null;
		MilMo_EventSystem.RemoveReaction(_memberKickedListener);
		_memberKickedListener = null;
		MilMo_EventSystem.RemoveReaction(_leaderChangedListener);
		_leaderChangedListener = null;
		MilMo_EventSystem.RemoveReaction(_travelInitiatedListener);
		_travelInitiatedListener = null;
		MilMo_EventSystem.RemoveReaction(_travelResponseListener);
		_travelResponseListener = null;
		MilMo_EventSystem.RemoveReaction(_travelFailedListener);
		_travelFailedListener = null;
		OnLeaveGroup -= GroupChanged;
		OnJoinGroup -= GroupChanged;
		OnGroupDisband -= GroupChanged;
	}

	private void GroupChanged(GroupMember member)
	{
		this.OnGroupChanged?.Invoke();
	}

	private void GroupChanged()
	{
		this.OnGroupChanged?.Invoke();
	}

	public bool InGroup(string id)
	{
		if (_playerIsInGroup)
		{
			return _group.Any((GroupMember m) => m.Id == id);
		}
		return false;
	}

	public bool LocalPlayerIsLeader()
	{
		return IsGroupLeader(PlayerInstance.Id);
	}

	public bool IsGroupLeader(string id)
	{
		if (_playerIsInGroup)
		{
			return _group.Any((GroupMember m) => m.Id == id && m.IsLeader);
		}
		return false;
	}

	private void SetGroupLeader(string groupMemberId)
	{
		_groupLeader = groupMemberId;
		this.OnLeaderChange?.Invoke();
	}

	public List<GroupMember> GetGroupMembers()
	{
		return _group.ToList();
	}

	private GroupMember GetGroupMember(string groupMemberId)
	{
		GroupMember groupMember = GetGroupMembers().FirstOrDefault((GroupMember b) => b.Id == groupMemberId);
		if (groupMember == null)
		{
			Debug.LogWarning("Error. Trying to fetch a group member that do not exist in group.");
		}
		return groupMember;
	}

	private void AddToGroup(string id, string playerName)
	{
		if (!GetGroupMembers().Any((GroupMember m) => m.Id == id))
		{
			GroupMember groupMember = new GroupMember(id, playerName, id == _groupLeader);
			_group.Add(groupMember);
			this.OnJoinGroup?.Invoke(groupMember);
		}
	}

	private void RemoveFromGroup(string id)
	{
		for (int num = _group.Count - 1; num >= 0; num--)
		{
			if (!(_group[num].Id != id))
			{
				this.OnLeaveGroup?.Invoke(_group[num]);
				_group.RemoveAt(num);
			}
		}
	}

	private void SetInGroup(bool val)
	{
		if (val != _playerIsInGroup)
		{
			GameEvent.OnGroupChangeEvent.RaiseEvent(val);
		}
		_playerIsInGroup = val;
	}

	public void LeaveGroup()
	{
		SendGroupLeave();
	}

	public void MemberLevelUp(string id, int level)
	{
		GroupMember groupMember = GetGroupMember(id);
		if (groupMember != null)
		{
			DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("World_11380", groupMember.Name, level));
		}
	}

	private bool GroupInvitesContains(string id)
	{
		return _groupInvites.Any((GroupMember m) => m.Id == id);
	}

	public void InviteToGroup(string playerId, string playerName)
	{
		if (!_playerIsInGroup)
		{
			InviteToNewGroup(playerId, playerName);
		}
		else
		{
			InviteToExistingGroup(playerId, playerName);
		}
	}

	private void InviteToNewGroup(string playerId, string playerName)
	{
		_groupCreatedCallback = delegate(bool success)
		{
			if (success)
			{
				InviteToExistingGroup(playerId, playerName);
			}
			_groupCreatedCallback = null;
		};
		SendGroupForm();
	}

	private void InviteToExistingGroup(string playerId, string playerName)
	{
		if (!string.IsNullOrEmpty(playerName))
		{
			SendGroupInvite(playerId);
			DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10250", playerName));
		}
	}

	public void DeclineInvitation(string inviterId)
	{
		if (!GroupInvitesContains(inviterId))
		{
			return;
		}
		for (int num = _groupInvites.Count - 1; num >= 0; num--)
		{
			if (!(_groupInvites[num].Id != inviterId))
			{
				_groupInvites.RemoveAt(num);
				break;
			}
		}
		SendGroupJoinDecline(inviterId);
		this.OnGroupDisband?.Invoke();
	}

	public void AcceptInvitation(string inviterId)
	{
		SendGroupJoinAccept(inviterId);
	}

	private void SendGroupLeave()
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupLeave());
	}

	private void SendGroupForm()
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupForm());
	}

	private void SendGroupInvite(string inviterId)
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupInvite(inviterId));
	}

	public void SendGroupJoinDecline(string inviterId)
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupJoin(inviterId, 0));
	}

	public void SendGroupJoinAccept(string inviterId)
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupJoin(inviterId, 1));
	}

	private void JoinGroup(object msg)
	{
		ServerGroupJoin joinMessage = msg as ServerGroupJoin;
		if (joinMessage == null)
		{
			return;
		}
		MilMo_ProfileManager.RequestPlayerName(joinMessage.getPlayer(), delegate(string playerName, string id)
		{
			if (joinMessage.getAccepted() == 0)
			{
				Debug.LogWarning("JoinGroup Accepted = false");
				if (!(id == PlayerInstance.Id))
				{
					DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_11406", playerName));
				}
			}
			else if (joinMessage.getAccepted() == 1)
			{
				Debug.LogWarning("JoinGroup Accepted = true");
				if (joinMessage.getPlayer() == PlayerInstance.Id && !_playerIsInGroup)
				{
					SetInGroup(val: true);
					DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10249"));
				}
				else
				{
					DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10248", playerName));
				}
				AddToGroup(id, playerName);
			}
		});
	}

	private void CreateGroup(object msgAsObject)
	{
		if (msgAsObject is ServerGroupForm serverGroupForm)
		{
			if (serverGroupForm.getSucceeded() == 1)
			{
				_group.Clear();
				_groupInvites.Clear();
				SetInGroup(val: true);
				SetGroupLeader(PlayerInstance.Id);
				AddToGroup(PlayerInstance.Id, PlayerInstance.Avatar.Name);
				_groupCreatedCallback?.Invoke(success: true);
			}
			else
			{
				_groupCreatedCallback?.Invoke(success: false);
				DialogueSpawner.SpawnErrorModalDialogue(new LocalizedStringWithArgument("Generic_ERROR"), new LocalizedStringWithArgument("Messenger_FriendList_10251"));
			}
		}
	}

	private void LeaveGroup(object msg)
	{
		if (!(msg is ServerGroupLeave serverGroupLeave))
		{
			return;
		}
		string player = serverGroupLeave.getPlayer();
		if (player == PlayerInstance.Id)
		{
			Debug.LogWarning("LocalPlayer leave");
			if (_group.Count > 1)
			{
				DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10252"));
			}
			_group.Clear();
			SetInGroup(val: false);
			SetGroupLeader("");
			this.OnGroupDisband?.Invoke();
		}
		else
		{
			Debug.LogWarning("RemotePlayer leave");
			GroupMember groupMember = GetGroupMember(player);
			if (groupMember == null)
			{
				Debug.LogWarning("Leave group: Unable to find member");
				return;
			}
			DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10253", groupMember.Name));
			RemoveFromGroup(player);
		}
	}

	private void GroupMembersUpdate(object msgAsObject)
	{
		if (!(msgAsObject is ServerGroupMembers serverGroupMembers))
		{
			return;
		}
		IList<string> members = serverGroupMembers.GetMembers();
		members.Add(PlayerInstance.Id);
		if (!_playerIsInGroup)
		{
			Debug.LogWarning("GroupMembersUpdate: Set in group: true");
			SetInGroup(val: true);
		}
		SetGroupLeader(serverGroupMembers.GetLeader());
		_group.Clear();
		foreach (string item in members)
		{
			MilMo_ProfileManager.RequestPlayerName(item, delegate(string playerName, string id)
			{
				AddToGroup(id, playerName);
			});
		}
	}

	private void KickedFromGroup(object msg)
	{
		if (!(msg is ServerGroupKicked serverGroupKicked))
		{
			return;
		}
		string player = serverGroupKicked.getPlayer();
		if (player == PlayerInstance.Id)
		{
			Debug.LogWarning("LocalPlayer was kicked");
			DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10256"));
			SetInGroup(val: false);
			SetGroupLeader("");
			_group.Clear();
			this.OnGroupDisband?.Invoke();
			return;
		}
		Debug.LogWarning("RemotePlayer was kicked");
		GroupMember groupMember = GetGroupMember(player);
		if (groupMember == null)
		{
			Debug.LogWarning("KickedFromGroup: Unable to find member");
			return;
		}
		DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10257", groupMember.Name));
		RemoveFromGroup(player);
	}

	private void LeaderChanged(object msg)
	{
		if (!(msg is ServerGroupNewLeader serverGroupNewLeader) || serverGroupNewLeader.getSucceeded() != 1)
		{
			return;
		}
		GroupMember groupMember = GetGroupMember(serverGroupNewLeader.getNewLeader());
		if (groupMember == null)
		{
			Debug.LogWarning("Unable to find new leader");
			return;
		}
		foreach (GroupMember item in _group)
		{
			item.ChangeLeader(groupMember);
		}
		SetGroupLeader(groupMember.Id);
		DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10258", groupMember.Name));
	}

	private void InvitedToGroup(object msg)
	{
		if (msg is ServerGroupInvite serverGroupInvite)
		{
			if (_playerIsInGroup)
			{
				Debug.LogWarning("Player is already in group");
				return;
			}
			SpawnGroupRequestDialogue(serverGroupInvite.getInviter());
			MilMo_EventSystem.Instance.PostEvent("tutorial_Groups", null);
		}
	}

	private void SpawnGroupRequestDialogue(string userId)
	{
		MilMo_ProfileManager.RequestPlayerName(userId, Callback);
		void Callback(string playerName, string id)
		{
			LocalizedStringWithArgument localizedStringWithArgument = new LocalizedStringWithArgument("Messenger_FriendList_10259", playerName);
			DialogueSpawner.SpawnAcceptDeclineModalDialogue("Messenger_FriendList_10116", localizedStringWithArgument.GetMessage(), delegate
			{
				SendGroupJoinAccept(id);
			}, delegate
			{
				SendGroupJoinDecline(id);
			}, new AddressableSpriteLoader("GroupIcon"));
		}
	}

	private void InviteResponse(object msg)
	{
		if (msg is ServerGroupInviteResponse serverGroupInviteResponse)
		{
			if (serverGroupInviteResponse.getSucceeded() == 1 || serverGroupInviteResponse.getSucceeded() == 6)
			{
				Debug.LogWarning("InviteResponse: Already in group/Already invited");
				DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_11395"));
			}
			else if (serverGroupInviteResponse.getSucceeded() == 2)
			{
				Debug.LogWarning("InviteResponse: Group is full");
				DialogueSpawner.SpawnGroupQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_11396"));
			}
			else if (serverGroupInviteResponse.getSucceeded() == 3)
			{
				Debug.LogWarning("InviteResponse: Unknown error");
				DialogueSpawner.SpawnErrorModalDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10116"), new LocalizedStringWithArgument("Messenger_FriendList_11397"));
			}
			else if (serverGroupInviteResponse.getSucceeded() == 4)
			{
				Debug.LogWarning("Player declined invite");
			}
		}
	}

	private void TravelInitiated(object msg)
	{
		if (msg is ServerGroupInitiateTravel serverGroupInitiateTravel)
		{
			HasAcceptedTravel = 0;
			_groupTravelDialog = new MilMoGroupTravelDialog(MilMo_GlobalUI.GetSystemUI, 30, serverGroupInitiateTravel.getLevel(), serverGroupInitiateTravel.getPlayer());
		}
	}

	private void GroupTravelResponse(object msg)
	{
		if (msg is ServerGroupTravelResponse serverGroupTravelResponse)
		{
			if (serverGroupTravelResponse.getAccepted() == 1)
			{
				HasAcceptedTravel++;
			}
			_groupTravelDialog?.Refresh();
		}
		if (_group.Count == HasAcceptedTravel && _groupTravelDialog != null)
		{
			_groupTravelDialog.Close(null);
			_groupTravelDialog = null;
			PlayerInstance.Teleporting = true;
		}
	}

	private void GroupTravelFailed(object msg)
	{
		PlayerInstance.Teleporting = false;
		if (msg is ServerGroupTravelFailed serverGroupTravelFailed && serverGroupTravelFailed.getResponse() != 1)
		{
			LocalizedStringWithArgument message = new LocalizedStringWithArgument("World_11379");
			switch (serverGroupTravelFailed.getResponse())
			{
			case 1:
				message = new LocalizedStringWithArgument("Messenger_FriendList_11404");
				break;
			case 7:
				message = new LocalizedStringWithArgument("Messenger_FriendList_11403");
				break;
			case 5:
				message = new LocalizedStringWithArgument("Messenger_FriendList_11402");
				break;
			case 2:
				message = new LocalizedStringWithArgument("Messenger_FriendList_11401");
				break;
			case 3:
				message = new LocalizedStringWithArgument("Messenger_FriendList_11400");
				break;
			case 6:
				message = new LocalizedStringWithArgument("Messenger_FriendList_11399");
				break;
			case 4:
				Debug.LogError("Error. Group travel failed due to player not being in group.");
				break;
			case 8:
				message = new LocalizedStringWithArgument("Messenger_FriendList_11398");
				break;
			default:
				message = new LocalizedStringWithArgument("Messenger_FriendList_11398");
				break;
			}
			if (_groupTravelDialog != null)
			{
				_groupTravelDialog.Close(null);
				_groupTravelDialog = null;
			}
			DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("Interact_Travel"), message, "GroupIcon");
		}
	}
}
