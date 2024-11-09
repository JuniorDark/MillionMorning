using System;

namespace UI.Marker.Combat;

[Serializable]
public struct BadgeData
{
	public string iconPath;

	public string text;

	public string tooltipText;

	public string identifier;

	public bool isEarned;
}
