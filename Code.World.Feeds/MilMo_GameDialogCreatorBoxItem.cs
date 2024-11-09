using System.Collections.Generic;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.World.GUI.GameDialog;

namespace Code.World.Feeds;

public class MilMo_GameDialogCreatorBoxItem : MilMo_GameDialogCreator
{
	private readonly IMilMo_OpenableBox _box;

	private readonly List<MilMo_BoxLoot> _items;

	public MilMo_GameDialogCreatorBoxItem(List<MilMo_BoxLoot> receivedItems, IMilMo_OpenableBox box, MilMo_UserInterface ui)
		: base(ui)
	{
		foreach (MilMo_BoxLoot receivedItem in receivedItems)
		{
			receivedItem.Item = receivedItem.ItemTemplate.Instantiate(new Dictionary<string, string>());
		}
		_items = receivedItems;
		_box = box;
	}

	protected override void CreateDialog()
	{
		if (UI != null && (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI) || MilMo_GameDialogCreator.TheDialogs[UI] == null))
		{
			if (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI))
			{
				MilMo_GameDialogCreator.TheDialogs.Add(UI, null);
			}
			MilMo_GameDialogBox milMo_GameDialogBox = new MilMo_GameDialogBox(UI, CloseDialog, _box);
			UI.AddChild(milMo_GameDialogBox);
			milMo_GameDialogBox.Show(_items.GetEnumerator());
			MilMo_GameDialogCreator.TheDialogs[UI] = new OpenDialog(milMo_GameDialogBox, this);
		}
	}
}
