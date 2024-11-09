using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.World.Tutorial;
using Core;
using UnityEngine;

namespace UI.LockState;

public class LockStateManager : Singleton<LockStateManager>
{
	[Header("Found elements")]
	[SerializeField]
	private bool hideGems;

	[SerializeField]
	private bool hideCoins;

	[SerializeField]
	private bool hideTokens;

	[SerializeField]
	private bool hideActionBar;

	[Header("LockStates")]
	public LockState profileButton;

	public LockState friendListButton;

	public LockState questLogButton;

	public LockState shopButton;

	public LockState townButton;

	public LockState navigatorButton;

	public LockState bagButton;

	private readonly List<MilMo_GenericReaction> _reactions = new List<MilMo_GenericReaction>();

	public bool HasFoundGems
	{
		get
		{
			return !hideGems;
		}
		set
		{
			hideGems = !value;
			this.OnChange?.Invoke();
		}
	}

	public bool HasFoundCoins
	{
		get
		{
			return !hideCoins;
		}
		set
		{
			hideCoins = !value;
			this.OnChange?.Invoke();
		}
	}

	public bool HasFoundTokens
	{
		get
		{
			return !hideTokens;
		}
		set
		{
			hideTokens = !value;
			this.OnChange?.Invoke();
		}
	}

	public bool HasFoundActionBar
	{
		get
		{
			return !hideActionBar;
		}
		set
		{
			hideActionBar = !value;
			this.OnChange?.Invoke();
		}
	}

	public bool HasUnlockedProfile
	{
		get
		{
			return !profileButton.IsLocked();
		}
		set
		{
			UpdateLockState(profileButton, value);
		}
	}

	public bool HasUnlockedFriendList
	{
		get
		{
			return !friendListButton.IsLocked();
		}
		set
		{
			UpdateLockState(friendListButton, value);
		}
	}

	public bool HasUnlockedQuestLog
	{
		get
		{
			return !questLogButton.IsLocked();
		}
		set
		{
			UpdateLockState(questLogButton, value);
		}
	}

	public bool HasUnlockedShop
	{
		get
		{
			return !shopButton.IsLocked();
		}
		set
		{
			UpdateLockState(shopButton, value);
		}
	}

	public bool HasUnlockedTown
	{
		get
		{
			return !townButton.IsLocked();
		}
		set
		{
			UpdateLockState(townButton, value);
		}
	}

	public bool HasUnlockedNavigator
	{
		get
		{
			return !navigatorButton.IsLocked();
		}
		set
		{
			UpdateLockState(navigatorButton, value);
		}
	}

	public bool HasUnlockedBag
	{
		get
		{
			return !bagButton.IsLocked();
		}
		set
		{
			UpdateLockState(bagButton, value);
		}
	}

	public event Action OnChange = delegate
	{
	};

	private void OnValidate()
	{
		this.OnChange?.Invoke();
	}

	public void Refresh()
	{
		Singleton<MilMo_TutorialManager>.Instance.RefreshLockStates(this);
	}

	public void UnlockAll()
	{
		SetAll(shouldLock: false);
	}

	private void SetAll(bool shouldLock)
	{
		hideGems = shouldLock;
		hideCoins = shouldLock;
		hideTokens = shouldLock;
		hideActionBar = shouldLock;
		HasUnlockedProfile = !shouldLock;
		HasUnlockedFriendList = !shouldLock;
		HasUnlockedShop = !shouldLock;
		HasUnlockedTown = !shouldLock;
		HasUnlockedNavigator = !shouldLock;
		HasUnlockedBag = !shouldLock;
	}

	private void UpdateLockState(LockState lockState, bool shouldUnlock)
	{
		if (shouldUnlock)
		{
			lockState.Unlock();
		}
		else
		{
			lockState.Lock();
		}
	}

	public void ListenForUnlockingEvents()
	{
		Debug.Log("Start listening for unlocking events");
		AddReaction("tutorial_ReceiveMedal", UnlockProfile);
		AddReaction("tutorial_TalkTo", UnlockQuestLog);
		AddReaction("tutorial_UseCoins", UnlockShop);
		AddReaction("tutorial_EnterTown", UnlockTown);
		AddReaction("tutorial_WorldMap", UnlockNavigator);
		AddReaction("tutorial_ReceiveItem", UnlockBag);
		AddReaction("consumable_or_ability_added", UnlockBag);
		AddReaction("avatarlevel_updated", EnableActionBar);
		AddReaction("tutorial_ReceiveConsumable", EnableActionBar);
		AddReaction("consumable_or_ability_added", EnableActionBar);
	}

	private void AddReaction(string ev, MilMo_EventSystem.MilMo_EventCallback action)
	{
		MilMo_GenericReaction item = MilMo_EventSystem.Listen(ev, action);
		_reactions.Add(item);
	}

	public void StopListeningForUnlockingEvents()
	{
		Debug.Log("Stop listening for unlocking events");
		foreach (MilMo_GenericReaction reaction in _reactions)
		{
			MilMo_EventSystem.RemoveReaction(reaction);
		}
		_reactions.Clear();
	}

	private void UnlockProfile(object _)
	{
		Debug.Log("UNLOCKED PROFILE!");
		HasUnlockedProfile = true;
	}

	private void UnlockQuestLog(object _)
	{
		Debug.Log("UNLOCKED QUEST LOG!");
		HasUnlockedQuestLog = true;
	}

	private void UnlockShop(object _)
	{
		Debug.Log("UNLOCKED SHOP!");
		HasUnlockedShop = true;
	}

	private void UnlockTown(object _)
	{
		Debug.Log("UNLOCKED TOWN!");
		HasUnlockedTown = true;
	}

	private void UnlockNavigator(object _)
	{
		Debug.Log("UNLOCKED NAVIGATOR!");
		HasUnlockedNavigator = true;
	}

	private void UnlockBag(object _)
	{
		Debug.Log("UNLOCKED BAG!");
		HasUnlockedBag = true;
	}

	private void EnableActionBar(object _)
	{
		Debug.Log("UNLOCKED ACTION BAR!");
		HasFoundActionBar = true;
	}
}
