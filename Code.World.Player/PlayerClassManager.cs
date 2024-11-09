using System;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Core;
using UnityEngine;

namespace Code.World.Player;

public class PlayerClassManager
{
	private const int CLASS_LEVELS = 4;

	private bool[] ClassSelection { get; }

	public event Action OnClassSelectionChanged;

	public PlayerClassManager()
	{
		ClassSelection = new bool[4];
		MilMo_EventSystem.Listen("player_class_level_update", PlayerClassLevelUpdate).Repeating = true;
	}

	public bool HasAvailableClassSelection()
	{
		return ClassSelection.Any((bool cl) => cl);
	}

	public void CheckForAvailableSelections()
	{
		Singleton<GameNetwork>.Instance.RequestClassLevelUpdate();
	}

	public int GetFirstAvailableClassSelection()
	{
		for (int i = 0; i < 4; i++)
		{
			if (ClassSelection[i])
			{
				return i;
			}
		}
		return -1;
	}

	private bool IsValidLevel(int level)
	{
		if (level >= 0)
		{
			return level < 4;
		}
		return false;
	}

	private void EnableSelection(int level)
	{
		if (IsValidLevel(level))
		{
			ClassSelection[level] = true;
			this.OnClassSelectionChanged?.Invoke();
		}
	}

	public void DisableSelection(int level)
	{
		if (IsValidLevel(level))
		{
			ClassSelection[level] = false;
			this.OnClassSelectionChanged?.Invoke();
		}
	}

	private void ClearSelectionsAfter(int level)
	{
		if (IsValidLevel(level))
		{
			for (int num = 3; num > level; num--)
			{
				ClassSelection[num] = false;
			}
		}
	}

	private void PlayerClassLevelUpdate(object msgAsObject)
	{
		if (msgAsObject is ServerClassLevelUp serverClassLevelUp)
		{
			int level = serverClassLevelUp.getLevel();
			Debug.Log($"Got some class levels to select: {level}");
			ClearSelectionsAfter(level);
			EnableSelection(level);
			MilMo_EventSystem.Instance.PostEvent("tutorial_ClassQuests", "");
		}
	}
}
