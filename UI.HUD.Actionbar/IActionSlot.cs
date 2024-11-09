using UI.HUD.Actionbar.Hotkeys;

namespace UI.HUD.Actionbar;

public interface IActionSlot
{
	HotkeyType GetHotkey();

	void RefreshServerUpdateAbilityHotkey();
}
