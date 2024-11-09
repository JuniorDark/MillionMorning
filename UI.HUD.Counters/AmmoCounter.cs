using System.Collections.Generic;
using System.Linq;
using Core.State.Basic;
using UnityEngine;

namespace UI.HUD.Counters;

public class AmmoCounter : Counter
{
	[Header("States")]
	[SerializeField]
	private IntState ammoAmount;

	[SerializeField]
	private StringState ammoType;

	[Header("Extra")]
	[SerializeField]
	private List<Texture2D> ammoTextures;

	private void OnEnable()
	{
		SyncWithState(0);
		SyncWithState("");
		ammoAmount.OnChange += SyncWithState;
		ammoType.OnChange += SyncWithState;
	}

	private void OnDisable()
	{
		ammoAmount.OnChange -= SyncWithState;
		ammoType.OnChange -= SyncWithState;
	}

	private void SyncWithState(int newValue)
	{
		int num = ammoAmount.Get();
		SetText($"{num:0}");
	}

	private void SyncWithState(string newValue)
	{
		string currentAmmoType = ammoType.Get();
		Texture2D texture2D = ammoTextures.FirstOrDefault((Texture2D i) => i.name == "Hud" + currentAmmoType);
		SetIcon(texture2D);
	}
}
