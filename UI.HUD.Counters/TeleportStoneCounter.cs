using Core.State.Basic;
using UnityEngine;

namespace UI.HUD.Counters;

public class TeleportStoneCounter : Counter
{
	[Header("States")]
	[SerializeField]
	private IntState playerTelepods;

	private void OnEnable()
	{
		SyncWithState(0);
		playerTelepods.OnChange += SyncWithState;
	}

	private void OnDisable()
	{
		playerTelepods.OnChange -= SyncWithState;
	}

	private void SyncWithState(int newValue)
	{
		int num = playerTelepods.Get();
		SetText($"{num}");
	}
}
