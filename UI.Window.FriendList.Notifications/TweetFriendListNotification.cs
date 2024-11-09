using UI.Window.FriendList.Friends;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Window.FriendList.Notifications;

public class TweetFriendListNotification : FriendListNotification
{
	[SerializeField]
	private BuddyUIFriend friend;

	protected override void Awake()
	{
		if (friend == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find friend");
		}
	}

	protected override void Action()
	{
	}

	protected override void OnHoverShow(PointerEventData data)
	{
		if (data.pointerEnter == base.gameObject && base.gameObject.activeSelf)
		{
			Debug.LogWarning(friend.GetBuddy().Tweet ?? "");
		}
	}

	protected override void OnHoverHide()
	{
		Debug.Log("Hide TweetNotification hover");
	}
}
