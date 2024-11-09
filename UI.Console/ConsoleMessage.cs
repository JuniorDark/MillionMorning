using TMPro;
using UnityEngine;

namespace UI.Console;

public class ConsoleMessage : MonoBehaviour
{
	[SerializeField]
	private TMP_Text text;

	public void SetMessage(string message)
	{
		if (message != null)
		{
			text.text = "";
			if (!string.IsNullOrEmpty(message))
			{
				text.text = message;
			}
		}
	}
}
