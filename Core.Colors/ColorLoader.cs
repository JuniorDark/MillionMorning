using UnityEngine;
using UnityEngine.UI;

namespace Core.Colors;

public class ColorLoader : MonoBehaviour, IIdentifier
{
	[SerializeField]
	private ScriptableColor color;

	[SerializeField]
	private Image target;

	private void Start()
	{
		if (color != null)
		{
			SetTargetColor();
		}
	}

	public void SetColor(ScriptableColor newColor)
	{
		color = newColor;
		SetTargetColor();
	}

	public string GetIdentifier()
	{
		if (!(color != null))
		{
			return null;
		}
		return color.GetIdentifier();
	}

	private void SetTargetColor()
	{
		target.color = color.GetIconColor();
	}
}
