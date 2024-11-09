using Core.Settings;
using UI.Elements.Window;

namespace UI.OptionsMenu;

public class OptionsMenu : UIWindow
{
	public override void Close()
	{
		Settings.Save();
		base.Close();
	}
}
