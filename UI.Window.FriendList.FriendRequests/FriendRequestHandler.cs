using System.Collections.Generic;
using System.Linq;
using Code.Core.BuddyBackend;
using Code.Core.Network.nexus;
using Core;
using Core.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Window.FriendList.FriendRequests;

[RequireComponent(typeof(FriendList))]
public class FriendRequestHandler : MonoBehaviour
{
	[SerializeField]
	private AssetReference buddyRequestPrefab;

	[SerializeField]
	public List<BuddyRequestUIFriend> friendRequests;

	[SerializeField]
	private Transform friendRequestsContainer;

	private MilMo_BuddyBackend _backend;

	private FriendList _friendList;

	private void Awake()
	{
		if (buddyRequestPrefab == null)
		{
			Debug.LogError(base.name + ": Unable to get buddyRequestPrefab");
		}
		else if (friendRequestsContainer == null)
		{
			Debug.LogError(base.name + ": Unable to get friendRequestsContainer");
		}
	}

	private void Start()
	{
		_friendList = GetComponent<FriendList>();
		if (_friendList == null)
		{
			Debug.LogError(base.name + ": Unable to get _friendList");
			return;
		}
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
		if (_backend == null)
		{
			Debug.LogError(base.name + ": Unable to get _backend");
			return;
		}
		_backend.OnConnected += Connected;
		_backend.OnDisconnected += Disconnected;
		_backend.OnFriendRequestAdded += AddFriendRequest;
		_backend.OnFriendRequestRemoved += RemoveFriendRequest;
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
			_backend.OnFriendRequestAdded -= AddFriendRequest;
			_backend.OnFriendRequestRemoved -= RemoveFriendRequest;
		}
	}

	private void Connected()
	{
		ICollection<FriendRequest> buddyRequests = _backend.GetBuddyRequests();
		Debug.Log($"{base.name}: Connected. Getting {buddyRequests.Count} friend requests");
		foreach (FriendRequest item2 in buddyRequests)
		{
			BuddyRequestUIFriend item = InstantiateFriendRequestPrefab(item2);
			friendRequests.Add(item);
		}
		TriggerRebuild();
	}

	private void Disconnected()
	{
		Debug.Log(base.name + ": Disconnected. Clear friend requests");
		foreach (BuddyRequestUIFriend item in friendRequests.ToList())
		{
			friendRequests.Remove(item);
			Object.Destroy(item.gameObject);
		}
		TriggerRebuild();
	}

	private void AddFriendRequest(FriendRequest request)
	{
		Debug.Log(base.name + ": Add FriendRequest: " + request.Name);
		if (friendRequests.FirstOrDefault((BuddyRequestUIFriend f) => f.IsChosenIdentity(request)) != null)
		{
			Debug.LogWarning("FriendRequest already added...");
			return;
		}
		BuddyRequestUIFriend item = InstantiateFriendRequestPrefab(request);
		friendRequests.Add(item);
		TriggerRebuild();
	}

	private void RemoveFriendRequest(IIdentity request)
	{
		Debug.LogWarning(base.name + ": Remove FriendRequest: " + request.Name);
		BuddyRequestUIFriend buddyRequestUIFriend = friendRequests.FirstOrDefault((BuddyRequestUIFriend f) => f.IsChosenIdentity(request));
		if (!(buddyRequestUIFriend == null))
		{
			friendRequests.Remove(buddyRequestUIFriend);
			Object.Destroy(buddyRequestUIFriend.gameObject);
			TriggerRebuild();
		}
	}

	private BuddyRequestUIFriend InstantiateFriendRequestPrefab(FriendRequest request)
	{
		BuddyRequestSO buddyRequestSO = ScriptableObject.CreateInstance<BuddyRequestSO>();
		buddyRequestSO.Init(request);
		Transform targetTransform = friendRequestsContainer;
		BuddyRequestUIFriend buddyRequestUIFriend = Instantiator.Instantiate<BuddyRequestUIFriend>(buddyRequestPrefab, targetTransform);
		buddyRequestUIFriend.Init(buddyRequestSO);
		return buddyRequestUIFriend;
	}

	private void TriggerRebuild()
	{
		_friendList.TriggerRebuild();
	}
}
