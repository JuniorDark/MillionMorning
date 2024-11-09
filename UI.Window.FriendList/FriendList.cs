using System.Collections;
using Code.Core.BuddyBackend;
using Code.World.Player;
using Core;
using UI.Elements;
using UI.Notification;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Window.FriendList;

public class FriendList : Panel
{
	[SerializeField]
	private RectTransform friendListRoot;

	private bool _isRebuilding;

	private MilMo_BuddyBackend _backend;

	private NotificationManager _notificationManager;

	private MilMo_Player _player;

	protected override void Start()
	{
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
		if (_backend == null)
		{
			Debug.LogError(base.name + ": Unable to get _backend");
			return;
		}
		_notificationManager = Singleton<NotificationManager>.Instance;
		if (_notificationManager == null)
		{
			Debug.LogError(base.name + ": Unable to get _notificationManager");
			return;
		}
		_player = MilMo_Player.Instance;
		base.Start();
	}

	public override void Open()
	{
		if (!_player.InHub)
		{
			base.Open();
		}
	}

	public void TriggerRebuild()
	{
		if (base.gameObject.activeSelf && !_isRebuilding)
		{
			_isRebuilding = true;
			RebuildFriendsList();
		}
	}

	private void RebuildFriendsList()
	{
		StartCoroutine(RebuildFriendListCoroutine());
	}

	private IEnumerator RebuildFriendListCoroutine()
	{
		yield return new WaitForFixedUpdate();
		LayoutRebuilder.ForceRebuildLayoutImmediate(friendListRoot);
		_isRebuilding = false;
	}
}
