using System.Collections.Generic;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.Items.Home;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.GUI.GameDialog;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.Feeds;

public class MilMo_GameDialogCreatorGiftItem : MilMo_GameDialogCreator
{
	private static MilMo_FeedDefaultTemplate _theDefaults;

	private readonly MilMo_Item _item;

	private readonly string _giverName;

	public MilMo_GameDialogCreatorGiftItem(MilMo_ItemTemplate item, string modifiers, string giverName, int amount, MilMo_UserInterface ui)
		: base(ui)
	{
		if (_theDefaults == null)
		{
			_theDefaults = Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("FeedDefault", "Item") as MilMo_FeedDefaultTemplate;
		}
		if (_theDefaults != null)
		{
			FeedEventIngame = _theDefaults.FeedEventIngame;
		}
		ObjectName = item.DisplayName;
		if (amount > 1 && !(item is MilMo_AmmoTemplate))
		{
			ObjectName = MilMo_Localization.GetLocString("World_8003").GetCopy();
			ObjectName.SetFormatArgs(amount.ToString(), item.DisplayName);
		}
		IconPathIngame = item.IconPath;
		FeedDescriptionIngame = item.FeedDescriptionIngame;
		string.IsNullOrEmpty(item.FeedEventExternal);
		if (item.FeedEventIngame != null && !string.IsNullOrEmpty(item.FeedEventIngame.String))
		{
			FeedEventIngame = item.FeedEventIngame;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] array = modifiers.Split("#".ToCharArray());
		if (array.Length % 2 != 0)
		{
			Debug.LogWarning("Got invalid number of modifier parts (" + array.Length + ") in notification message.");
		}
		else
		{
			for (int i = 0; i < array.Length; i += 2)
			{
				dictionary.Add(array[i], array[i + 1]);
			}
		}
		_item = item.Instantiate(dictionary);
		_giverName = giverName;
	}

	protected override async void CreateDialog()
	{
		if (UI != null && (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI) || MilMo_GameDialogCreator.TheDialogs[UI] == null))
		{
			if (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI))
			{
				MilMo_GameDialogCreator.TheDialogs.Add(UI, null);
			}
			MilMo_GameDialog dialog = new MilMo_GameDialogGift(hudDestination: (!(_item is MilMo_HomeEquipment)) ? MilMo_GameDialog.HudDestination.Bag : (MilMo_Player.InMyHome ? MilMo_GameDialog.HudDestination.Storage : ((!MilMo_Player.Instance.InNavigator) ? MilMo_GameDialog.HudDestination.Home : MilMo_GameDialog.HudDestination.HomeInNav)), ui: UI, okayFunction: CloseDialog, giver: MilMo_Localization.GetNotLocalizedLocString(_giverName), itemName: ObjectName, itemDescription: FeedDescriptionIngame);
			UI.AddChild(dialog);
			await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Shop/IconGift");
			dialog.Show(_item);
			MilMo_GameDialogCreator.TheDialogs[UI] = new OpenDialog(dialog, this);
		}
	}
}
