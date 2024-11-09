using Code.World.GUI.FriendInvites;
using UnityEngine;

namespace UI.Window.FriendList;

public class RecruitAFriendHandler : MonoBehaviour
{
	[SerializeField]
	private FriendListButton recruitButton;

	private void Awake()
	{
		if (recruitButton == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to get leaveButton");
		}
	}

	private void Start()
	{
		recruitButton.Init("Messenger_FriendInvites_12595", Toggle);
	}

	public void Toggle()
	{
		MilMoFriendInviteDialog.GetInstance().Toggle();
	}
}
