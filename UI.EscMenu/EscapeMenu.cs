using UI.Elements.Window;
using UnityEngine;

namespace UI.EscMenu;

public class EscapeMenu : UIWindow
{
	public void ExitGame()
	{
		Application.Quit();
	}
}
