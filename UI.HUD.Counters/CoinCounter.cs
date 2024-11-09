using Core.State;
using Core.State.Basic;
using UnityEngine;

namespace UI.HUD.Counters;

public class CoinCounter : Counter
{
	[Header("States")]
	[SerializeField]
	private LevelTokenState levelTokenState;

	[SerializeField]
	private IntState playerTotalCoins;

	private void OnEnable()
	{
		SyncWithState(0);
		levelTokenState.tokensFound.OnChange += SyncWithState;
		levelTokenState.tokensMax.OnChange += SyncWithState;
		playerTotalCoins.OnChange += SyncWithState;
	}

	private void OnDisable()
	{
		levelTokenState.tokensFound.OnChange -= SyncWithState;
		levelTokenState.tokensMax.OnChange -= SyncWithState;
		playerTotalCoins.OnChange -= SyncWithState;
	}

	private void SyncWithState(int newValue)
	{
		int num = levelTokenState.tokensFound.Get();
		int num2 = levelTokenState.tokensMax.Get();
		int num3 = playerTotalCoins.Get();
		SetText((num3 > num2) ? $"{num:0}/{num2:0} : {num3:0}" : $"{num:0}/{num2:0}");
	}
}
