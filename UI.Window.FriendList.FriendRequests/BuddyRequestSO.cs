using Code.Core.Network.nexus;
using UnityEngine;

namespace UI.Window.FriendList.FriendRequests;

[CreateAssetMenu(menuName = "Buddy/New BuddyRequest", fileName = "buddyRequest")]
public class BuddyRequestSO : ScriptableObject, IIdentity
{
	[SerializeField]
	private int buddyId;

	[SerializeField]
	private string buddyName;

	private FriendRequest _request;

	int IIdentity.UserIdentifier => buddyId;

	string IIdentity.Name => buddyName;

	public void Init(FriendRequest request)
	{
		_request = request;
		buddyId = request.UserIdentifier;
		buddyName = request.Name;
	}

	public void Accept()
	{
		_request?.Accept();
	}

	public void Decline()
	{
		_request?.Decline();
	}
}
