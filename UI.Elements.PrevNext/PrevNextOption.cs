using System;
using UnityEngine.Localization;

namespace UI.Elements.PrevNext;

[Serializable]
public struct PrevNextOption
{
	public string identifier;

	public string displayText;

	public LocalizedString localizedString;
}
