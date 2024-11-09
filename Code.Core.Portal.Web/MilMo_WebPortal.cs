using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Monetization;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI;

namespace Code.Core.Portal.Web;

public sealed class MilMo_WebPortal : MilMo_Portal
{
	public MilMo_WebPortal()
	{
		MilMo_Monetization.Instance.Initialize(MilMo_JuneCash.Instance);
	}

	public override void ShowInviteInterface()
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Info);
		MilMo_Dialog dialog = new MilMo_Dialog(MilMo_GlobalUI.GetSystemUI);
		dialog.DoOK("Batch01/Textures/Dialog/Warning", MilMo_Localization.GetLocString("World_389"), MilMo_Localization.GetNotLocalizedLocString("Not implemented"), delegate
		{
			dialog.CloseAndRemove(null);
			dialog = null;
		}, impulse: true);
		MilMo_GlobalUI.GetSystemUI.AddChild(dialog);
	}
}
