using Code.Core.BuddyBackend;
using Code.Core.Network.nexus;
using Code.World.Player;
using Core;
using UI.Window.FriendList;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Profile;

public class ProfileContact : MonoBehaviour
{
	[SerializeField]
	private Button sendInstantMessage;

	[SerializeField]
	private Button sendFriendRequest;

	private ProfilePanel _profilePanel;

	private MilMo_Profile _profile;

	private MilMo_BuddyBackend _backend;

	private void Awake()
	{
		_profilePanel = GetComponentInParent<ProfilePanel>();
		if (!_profilePanel)
		{
			Debug.LogWarning(base.gameObject.name + ": Could not find ProfilePanel");
		}
	}

	private void Start()
	{
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
	}

	private void OnEnable()
	{
		_profile = ((_profilePanel != null) ? _profilePanel.profile : null);
		RefreshButtons();
	}

	private void RefreshButtons()
	{
		if (sendInstantMessage != null)
		{
			sendInstantMessage.gameObject.SetActive(_profile?.isFriend ?? false);
		}
		if (sendFriendRequest != null)
		{
			GameObject obj = sendFriendRequest.gameObject;
			MilMo_Profile profile = _profile;
			obj.SetActive(profile != null && !profile.isFriend && !profile.isMe);
		}
	}

	public void SendInstantMessage()
	{
		MilMo_Profile profile = _profile;
		if (profile != null && !profile.isFriend)
		{
			return;
		}
		string text = _profile?.playerId;
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (!_backend.IsBuddy(text))
		{
			Debug.LogWarning("Unable to find friend with Id: " + text);
			return;
		}
		Friend buddy = _backend.GetBuddy(text);
		if (buddy != null)
		{
			InstantMessage.OpenSendIMDialog(buddy.UserIdentifier.ToString(), buddy.Name);
		}
	}

	public void SendFriendRequest()
	{
		MilMo_Profile profile = _profile;
		if (profile != null && profile.isFriend && !profile.isMe)
		{
			return;
		}
		string text = _profile?.playerId;
		if (!string.IsNullOrEmpty(text))
		{
			if (_backend.IsRequestingFriend(text))
			{
				_backend.SendApproveFriendRequest(text);
			}
			else
			{
				_backend.SendFriendRequest(text);
			}
		}
	}
}
