using Core.GameEvent;
using UnityEngine;

namespace UI;

public class DebugGUI : MonoBehaviour
{
	private void OnGUI()
	{
		GUI.Box(new Rect(10f, 10f, 150f, 300f), "Debug Menu");
		if (GUI.Button(new Rect(35f, 40f, 100f, 20f), "ToggleInventory"))
		{
			GameEvent.ToggleInventoryEvent.RaiseEvent();
		}
	}
}
