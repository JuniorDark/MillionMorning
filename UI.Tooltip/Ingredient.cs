using Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltip;

public class Ingredient : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text text;

	[SerializeField]
	private Image image;

	public void SetText(string newText)
	{
		text.text = newText;
	}

	public void SetIcon(Texture2D newTexture)
	{
		if (!(newTexture == null))
		{
			Core.Utilities.UI.SetIcon(image, newTexture);
		}
	}
}
