using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Colors;

[CreateAssetMenu(menuName = "Color/Color", fileName = "new Color")]
public class ScriptableColor : ScriptableObject
{
	[SerializeField]
	private string identifier;

	[SerializeField]
	private string colorName;

	[SerializeField]
	private Color iconColor;

	[SerializeField]
	private int saturation;

	[SerializeField]
	private Color[] overlay;

	[SerializeField]
	private Color[] softlight;

	public string GetIdentifier()
	{
		return identifier;
	}

	public string GetColorName()
	{
		return colorName;
	}

	public Color GetIconColor()
	{
		return iconColor;
	}

	public int GetSaturation()
	{
		return saturation;
	}

	public List<Color> GetOverlays()
	{
		return overlay.ToList();
	}

	public List<Color> GetSoftLights()
	{
		return softlight.ToList();
	}
}
