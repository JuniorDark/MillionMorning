using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.World.GUI.GameDialog;

namespace Code.World.Feeds;

public class MilMo_GameDialogCreatorGiftTelepods : MilMo_GameDialogCreator
{
	private static MilMo_FeedDefaultTemplate _theDefaults;

	private MilMo_ItemTemplate.MilMo_FeedMode _feedMode;

	private readonly int _amount;

	private readonly string _giverName;

	public MilMo_GameDialogCreatorGiftTelepods(int amount, string giverName, MilMo_UserInterface ui)
		: base(ui)
	{
		_amount = amount;
		_giverName = giverName;
	}

	protected override void CreateDialog()
	{
		if (UI != null && (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI) || MilMo_GameDialogCreator.TheDialogs[UI] == null))
		{
			if (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI))
			{
				MilMo_GameDialogCreator.TheDialogs.Add(UI, null);
			}
			MilMo_LocString copy = MilMo_Localization.GetLocString("World_6098").GetCopy();
			copy.SetFormatArgs(_amount);
			MilMo_GameDialog milMo_GameDialog = new MilMo_GameDialogGift(UI, CloseDialog, MilMo_Localization.GetNotLocalizedLocString(_giverName), copy, MilMo_Localization.GetLocString("World_6099"), MilMo_GameDialog.HudDestination.Map);
			UI.AddChild(milMo_GameDialog);
			milMo_GameDialog.Show("Content/Items/Batch01/SpecialItems/IconTeleportStone");
			MilMo_GameDialogCreator.TheDialogs[UI] = new OpenDialog(milMo_GameDialog, this);
		}
	}
}
