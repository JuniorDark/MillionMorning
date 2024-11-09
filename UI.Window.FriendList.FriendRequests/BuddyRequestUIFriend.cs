using Code.Core.Network.nexus;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Window.FriendList.FriendRequests;

public class BuddyRequestUIFriend : UIFriend
{
	[SerializeField]
	private Button acceptButton;

	[SerializeField]
	private Button declineButton;

	private BuddyRequestSO _so;

	public override void Init(IIdentity identity)
	{
		if (!(identity is BuddyRequestSO so))
		{
			Debug.LogError(base.name + ": so is of wrong type.");
			return;
		}
		_so = so;
		base.Init(identity);
	}

	protected override void Awake()
	{
		if (acceptButton == null)
		{
			Debug.LogError(base.name + ": Unable to find acceptButton");
		}
		else if (declineButton == null)
		{
			Debug.LogError(base.name + ": Unable to find declineButton");
		}
	}

	protected override void SetupListeners()
	{
		acceptButton.onClick.AddListener(Accept);
		declineButton.onClick.AddListener(Decline);
	}

	protected override void RemoveListeners()
	{
		acceptButton.onClick.RemoveListener(Accept);
		declineButton.onClick.RemoveListener(Decline);
	}

	private void Accept()
	{
		_so.Accept();
	}

	private void Decline()
	{
		_so.Decline();
	}
}
