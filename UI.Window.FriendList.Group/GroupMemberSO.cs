using System;
using Code.Core.Network.nexus;
using Player;
using UnityEngine;

namespace UI.Window.FriendList.Group;

[CreateAssetMenu(menuName = "Buddy/New GroupMember", fileName = "testing")]
public class GroupMemberSO : ScriptableObject, IIdentity
{
	[SerializeField]
	private int buddyId;

	[SerializeField]
	private string buddyName;

	[SerializeField]
	private bool isLeader;

	private GroupMember _groupMember;

	int IIdentity.UserIdentifier => buddyId;

	string IIdentity.Name => buddyName;

	public bool IsLeader => isLeader;

	public event Action<bool> OnLeaderChange;

	public void Init(GroupMember memberObject)
	{
		_groupMember = memberObject;
		buddyId = _groupMember.UserIdentifier;
		buddyName = _groupMember.Name;
		isLeader = _groupMember.IsLeader;
		_groupMember.OnLeaderChanged += UpdateLeader;
	}

	private void UpdateLeader(bool leaderStatus)
	{
		isLeader = leaderStatus;
		this.OnLeaderChange?.Invoke(isLeader);
	}
}
