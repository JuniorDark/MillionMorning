using System.Collections.Generic;
using System.Linq;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.nexus;
using Core;
using Core.Utilities;
using Player;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Window.FriendList.Group;

[RequireComponent(typeof(FriendList))]
public class GroupRequestHandler : MonoBehaviour
{
	[SerializeField]
	private AssetReference groupMemberPrefab;

	[SerializeField]
	private FriendListButton leaveButton;

	[SerializeField]
	public List<GroupUIFriend> groupMembers;

	[SerializeField]
	private Transform groupContainer;

	private GroupManager _manager;

	private FriendList _friendList;

	private void Awake()
	{
		if (groupMemberPrefab == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to get groupPrefab");
		}
		else if (groupContainer == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to get groupContainer");
		}
	}

	public void Start()
	{
		_manager = Singleton<GroupManager>.Instance;
		_friendList = GetComponent<FriendList>();
		if (_friendList == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find _friendList");
			return;
		}
		InitGroupLeaveButton();
		_manager.OnJoinGroup += MemberJoined;
		_manager.OnLeaveGroup += MemberLeft;
		_manager.OnGroupDisband += GroupDisband;
		CreateInitialMembers();
	}

	private void CreateInitialMembers()
	{
		if (!_manager.PlayerIsInGroup)
		{
			return;
		}
		foreach (GroupMember groupMember in _manager.GetGroupMembers())
		{
			MemberJoined(groupMember);
		}
	}

	private void OnDestroy()
	{
		if (!(_manager == null))
		{
			_manager.OnJoinGroup -= MemberJoined;
			_manager.OnLeaveGroup -= MemberLeft;
			_manager.OnGroupDisband -= GroupDisband;
		}
	}

	private void MemberJoined(GroupMember groupMember)
	{
		if (!CheckExists(groupMember))
		{
			GroupUIFriend item = InstantiateGroupPrefab(groupMember);
			groupMembers.Add(item);
			leaveButton.Show();
			TriggerRebuild();
		}
	}

	private void MemberLeft(GroupMember groupMember)
	{
		GroupUIFriend groupUIFriend = groupMembers.FirstOrDefault((GroupUIFriend m) => m.IsChosenIdentity(groupMember));
		if (!(groupUIFriend == null))
		{
			groupMembers.Remove(groupUIFriend);
			Object.Destroy(groupUIFriend.gameObject);
			TriggerRebuild();
		}
	}

	private void GroupDisband()
	{
		foreach (GroupUIFriend groupMember in groupMembers)
		{
			Object.Destroy(groupMember.gameObject);
		}
		groupMembers.Clear();
		leaveButton.Hide();
		TriggerRebuild();
	}

	private GroupUIFriend InstantiateGroupPrefab(GroupMember member)
	{
		GroupMemberSO groupMemberSO = ScriptableObject.CreateInstance<GroupMemberSO>();
		groupMemberSO.Init(member);
		Transform targetTransform = groupContainer;
		GroupUIFriend groupUIFriend = Instantiator.Instantiate<GroupUIFriend>(groupMemberPrefab, targetTransform);
		groupUIFriend.Init(groupMemberSO);
		return groupUIFriend;
	}

	private void InitGroupLeaveButton()
	{
		leaveButton.Init("Messenger_FriendList_10120", LeaveGroup);
		leaveButton.Hide();
	}

	private void LeaveGroup()
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupLeave());
	}

	private bool CheckExists(IIdentity member)
	{
		return groupMembers.Any((GroupUIFriend uiFriend) => uiFriend.IsChosenIdentity(member));
	}

	private void TriggerRebuild()
	{
		_friendList.TriggerRebuild();
	}
}
