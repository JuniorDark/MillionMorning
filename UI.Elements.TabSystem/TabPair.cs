using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements.TabSystem;

[Serializable]
public class TabPair
{
	public Toggle tabButton;

	public CanvasGroup tabContent;
}
