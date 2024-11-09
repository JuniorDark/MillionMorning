using System;
using UI.Elements.Window;

namespace UI.Window.Home;

public class HomeSettingsMenu : UIWindow
{
	public event Action Persist;

	public override void Close()
	{
		PersistSettings();
		base.Close();
	}

	public void PersistSettings()
	{
		this.Persist?.Invoke();
	}
}
