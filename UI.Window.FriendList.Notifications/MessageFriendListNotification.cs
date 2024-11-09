using UI.Window.FriendList.Friends;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Window.FriendList.Notifications;

public class MessageFriendListNotification : FriendListNotification
{
	[SerializeField]
	private BuddyUIFriend friend;

	protected override void Awake()
	{
		base.Awake();
		if (friend == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to find UIFriend");
		}
	}

	protected override void Action()
	{
		BuddySO buddy = friend.GetBuddy();
		if (buddy.HasUnreadMessage())
		{
			buddy.OpenFirstMessage();
		}
	}

	protected override void OnHoverShow(PointerEventData data)
	{
		if (data.pointerEnter == base.gameObject && base.gameObject.activeSelf)
		{
			Debug.LogWarning("Show MessageNotification hover");
		}
	}

	protected override void OnHoverHide()
	{
		if (base.gameObject.activeSelf)
		{
			Debug.LogWarning("Hide MessageNotification hover");
		}
	}
}
