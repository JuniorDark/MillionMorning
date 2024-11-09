using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements.TabSystem;

public class TabStrip : MonoBehaviour
{
	[SerializeField]
	protected TabPair[] tabCollection;

	[SerializeField]
	protected Toggle defaultTab;

	private int _currentTabIndex;

	protected virtual void Start()
	{
		for (int i = 0; i < tabCollection.Length; i++)
		{
			int index = i;
			tabCollection[i].tabButton.onValueChanged.AddListener(delegate
			{
				PickTab(index);
			});
		}
		for (int j = 0; j < tabCollection.Length; j++)
		{
			SetTabState(j, picked: false);
		}
		if (tabCollection.Length != 0)
		{
			int? num = FindTabIndex(defaultTab).GetValueOrDefault();
			_currentTabIndex = num.Value;
			SetTabState(_currentTabIndex, picked: true);
		}
	}

	protected virtual void SetTabState(int index, bool picked)
	{
		TabPair tabPair = tabCollection[index];
		if (tabPair.tabContent != null)
		{
			tabPair.tabContent.gameObject.SetActive(picked);
		}
		if (tabPair.tabButton != null)
		{
			tabPair.tabButton.SetIsOnWithoutNotify(picked);
		}
	}

	protected void PickTab(int index)
	{
		SetTabState(_currentTabIndex, picked: false);
		_currentTabIndex = index;
		SetTabState(_currentTabIndex, picked: true);
	}

	public void ActivateTabContaining(Selectable selectable)
	{
		for (int i = 0; i < tabCollection.Length; i++)
		{
			if (!(tabCollection[i].tabButton != selectable))
			{
				PickTab(i);
				break;
			}
		}
	}

	private int? FindTabIndex(Toggle tabButton)
	{
		TabPair tabPair = tabCollection.FirstOrDefault((TabPair x) => x.tabButton == tabButton);
		if (tabPair != null)
		{
			return Array.IndexOf(tabCollection, tabPair);
		}
		Debug.LogWarning("The tab " + tabButton.name + " does not belong to the tab strip");
		return null;
	}
}
