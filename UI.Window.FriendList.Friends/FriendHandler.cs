using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.BuddyBackend;
using Code.Core.Network.nexus;
using Core;
using Core.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace UI.Window.FriendList.Friends;

[RequireComponent(typeof(FriendList))]
public class FriendHandler : MonoBehaviour
{
	[SerializeField]
	private AssetReference buddyPrefab;

	[SerializeField]
	public List<UIFriend> friends;

	[SerializeField]
	private LocalizeStringEvent onlineHeader;

	[SerializeField]
	private LocalizeStringEvent offlineHeader;

	[SerializeField]
	private Transform onlineContainer;

	[SerializeField]
	private Transform offlineContainer;

	private MilMo_BuddyBackend _backend;

	private FriendList _friendList;

	private void Awake()
	{
		if (buddyPrefab == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to get buddyPrefab");
		}
		else if (onlineContainer == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to get onlineContainer");
		}
		else if (offlineContainer == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to get offlineContainer");
		}
		else
		{
			HookupLocalizationStuff();
		}
	}

	protected void Start()
	{
		_friendList = GetComponent<FriendList>();
		if (_friendList == null)
		{
			Debug.LogError(base.name + ": Unable to get _friendList");
		}
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
		if (_backend == null)
		{
			Debug.LogError(base.name + ": Unable to get _backend");
			return;
		}
		_backend.OnConnected += Connected;
		_backend.OnDisconnected += Disconnected;
		_backend.OnFriendAdded += AddFriend;
		_backend.OnFriendRemoved += RemoveFriend;
		_backend.OnStatusChange += UpdateFriendStatus;
		if (_backend.IsConnected)
		{
			Connected();
		}
	}

	private void OnDestroy()
	{
		if (!(_backend == null))
		{
			_backend.OnConnected -= Connected;
			_backend.OnDisconnected -= Disconnected;
			_backend.OnFriendAdded -= AddFriend;
			_backend.OnFriendRemoved -= RemoveFriend;
			_backend.OnStatusChange -= UpdateFriendStatus;
		}
	}

	private void Connected()
	{
		ICollection<Friend> buddies = _backend.GetBuddies();
		Debug.Log($"{base.name}: Connected. Getting {buddies.Count} buddies");
		foreach (Friend item2 in buddies)
		{
			BuddyUIFriend item = InstantiateUIFriend(item2);
			friends.Add(item);
		}
		TriggerRebuild();
	}

	private void Disconnected()
	{
		Debug.Log(base.name + ": Disconnected. Clear buddies");
		foreach (UIFriend item in friends.ToList())
		{
			friends.Remove(item);
			UnityEngine.Object.Destroy(item.gameObject);
		}
		TriggerRebuild();
	}

	private void AddFriend(Friend friend)
	{
		Debug.Log(base.name + ": Add Friend: " + friend.Name);
		if (friends.FirstOrDefault((UIFriend f) => f.IsChosenIdentity(friend)) != null)
		{
			Debug.LogWarning("Friend already added...");
			return;
		}
		BuddyUIFriend item = InstantiateUIFriend(friend);
		friends.Add(item);
		TriggerRebuild();
	}

	private void RemoveFriend(IIdentity friend)
	{
		Debug.Log(base.name + ": Remove Friend: " + friend.Name);
		UIFriend uIFriend = friends.FirstOrDefault((UIFriend f) => f.IsChosenIdentity(friend));
		if (uIFriend == null)
		{
			Debug.LogWarning("Friend not found...");
			return;
		}
		friends.Remove(uIFriend);
		UnityEngine.Object.Destroy(uIFriend.gameObject);
		TriggerRebuild();
	}

	private void UpdateFriendStatus(Friend friend, MilMo_BuddyBackend.FriendStatus status)
	{
		Debug.Log(base.name + ": Update Friend Status: " + friend.Name);
		UIFriend uIFriend = friends.FirstOrDefault((UIFriend f) => f.IsChosenIdentity(friend));
		if (uIFriend == null)
		{
			Debug.LogWarning("Friend not found...");
			return;
		}
		switch (status)
		{
		case MilMo_BuddyBackend.FriendStatus.Online:
			uIFriend.transform.SetParent(onlineContainer);
			break;
		case MilMo_BuddyBackend.FriendStatus.Offline:
			uIFriend.transform.SetParent(offlineContainer);
			break;
		default:
			throw new ArgumentOutOfRangeException("status", status, null);
		case MilMo_BuddyBackend.FriendStatus.Away:
		case MilMo_BuddyBackend.FriendStatus.Etc:
			break;
		}
		TriggerRebuild();
	}

	private BuddyUIFriend InstantiateUIFriend(Friend friend)
	{
		BuddySO buddySO = ScriptableObject.CreateInstance<BuddySO>();
		buddySO.Init(friend);
		Transform targetTransform = (buddySO.IsOnline ? onlineContainer : offlineContainer);
		BuddyUIFriend buddyUIFriend = Instantiator.Instantiate<BuddyUIFriend>(buddyPrefab, targetTransform);
		buddyUIFriend.Init(buddySO);
		return buddyUIFriend;
	}

	private void HookupLocalizationStuff()
	{
		if (!onlineHeader.StringReference.TryGetValue("1", out var value))
		{
			onlineHeader.StringReference.Add("1", new IntVariable());
		}
		if (!offlineHeader.StringReference.TryGetValue("1", out value))
		{
			offlineHeader.StringReference.Add("1", new IntVariable());
		}
	}

	private void UpdateOnlineHeaderText()
	{
		if (onlineHeader.StringReference.TryGetValue("1", out var value) && value is IntVariable intVariable)
		{
			intVariable.Value = friends.Count((UIFriend f) => f is BuddyUIFriend buddyUIFriend && buddyUIFriend.GetBuddy().IsOnline);
		}
	}

	private void UpdateOfflineHeaderText()
	{
		if (offlineHeader.StringReference.TryGetValue("1", out var value) && value is IntVariable intVariable)
		{
			intVariable.Value = friends.Count((UIFriend f) => f is BuddyUIFriend buddyUIFriend && !buddyUIFriend.GetBuddy().IsOnline);
		}
	}

	private void TriggerRebuild()
	{
		UpdateOnlineHeaderText();
		UpdateOfflineHeaderText();
		_friendList.TriggerRebuild();
	}
}
