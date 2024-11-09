using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Elements.TabSystem;

public class TabStripWithTransparency : TabStrip
{
	protected override void Start()
	{
		base.Start();
		defaultTab.isOn = true;
		EventSystem.current.SetSelectedGameObject(defaultTab.gameObject);
	}

	protected override void SetTabState(int index, bool picked)
	{
		TabPair tabPair = tabCollection[index];
		if (tabPair.tabContent != null)
		{
			tabPair.tabContent.gameObject.SetActive(picked);
		}
		if (tabPair.tabButton != null)
		{
			Image image = tabPair.tabButton.image;
			Color color = image.color;
			float a = (picked ? 1f : 0.5f);
			color.a = a;
			image.color = color;
			tabPair.tabButton.image = image;
		}
	}
}
