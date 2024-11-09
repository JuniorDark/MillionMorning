using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.HUD.TopMenu;

public class TopMenuButton : MonoBehaviour
{
	public void SetSelectedGameObject()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}
}
