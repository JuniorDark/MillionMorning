using Code.Core.Network.nexus;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Window.FriendList.Group;

public class GroupUIFriend : UIFriend
{
	[SerializeField]
	private Image leaderIcon;

	private GroupMemberSO _so;

	public override void Init(IIdentity identity)
	{
		if (!(identity is GroupMemberSO groupMemberSO))
		{
			Debug.LogError(base.name + ": so is of wrong type.");
			return;
		}
		_so = groupMemberSO;
		base.Init(groupMemberSO);
		UpdateLeaderCrown(_so.IsLeader);
	}

	protected override void Awake()
	{
		if (leaderIcon == null)
		{
			Debug.LogError(base.name + ": Unable to find leaderIcon");
		}
	}

	protected override void SetupListeners()
	{
		_so.OnLeaderChange += UpdateLeaderCrown;
	}

	protected override void RemoveListeners()
	{
		_so.OnLeaderChange -= UpdateLeaderCrown;
	}

	private void UpdateLeaderCrown(bool show)
	{
		if (leaderIcon != null)
		{
			leaderIcon.gameObject.SetActive(show);
		}
	}
}
