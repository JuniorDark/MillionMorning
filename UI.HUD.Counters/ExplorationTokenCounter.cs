using Core.State;
using UnityEngine;

namespace UI.HUD.Counters;

public class ExplorationTokenCounter : Counter
{
	[Header("States")]
	[SerializeField]
	private LevelTokenState levelTokenState;

	private void OnEnable()
	{
		SyncWithState(0);
		levelTokenState.tokensFound.OnChange += SyncWithState;
		levelTokenState.tokensMax.OnChange += SyncWithState;
	}

	private void OnDisable()
	{
		levelTokenState.tokensFound.OnChange -= SyncWithState;
		levelTokenState.tokensMax.OnChange -= SyncWithState;
	}

	private void SyncWithState(int newValue)
	{
		int num = levelTokenState.tokensFound.Get();
		int num2 = levelTokenState.tokensMax.Get();
		SetText($"{num:0}/{num2:0}");
	}
}
