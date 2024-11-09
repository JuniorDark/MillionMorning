using System.Collections.Generic;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.GUI.GameDialog;
using Core;
using UnityEngine;

namespace Code.World.Feeds;

public class MilMo_GameDialogCreatorItemNotification : MilMo_GameDialogCreator
{
	private static MilMo_FeedDefaultTemplate _theDefaults;

	private readonly string _notificationType;

	private readonly MilMo_Item _item;

	private readonly int _amount;

	public MilMo_GameDialogCreatorItemNotification(string notificationType, MilMo_ItemTemplate item, int amount, string modifiers, MilMo_UserInterface ui)
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
		IconPathIngame = item.IconPath;
		FeedDescriptionIngame = item.FeedDescriptionIngame;
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
		_notificationType = notificationType;
		_item = item.Instantiate(dictionary);
		_amount = amount;
	}

	protected override void CreateDialog()
	{
		if (UI != null && (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI) || MilMo_GameDialogCreator.TheDialogs[UI] == null))
		{
			if (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI))
			{
				MilMo_GameDialogCreator.TheDialogs.Add(UI, null);
			}
			MilMo_LocString headline = MilMo_LocString.Empty;
			MilMo_LocString eventDescription = MilMo_LocString.Empty;
			if (_notificationType == "subscribeitem")
			{
				headline = MilMo_Localization.GetLocString("World_3721");
				eventDescription = MilMo_Localization.GetLocString("World_3722");
			}
			else if (_notificationType == "memberitem")
			{
				headline = MilMo_Localization.GetLocString("World_3724");
				eventDescription = MilMo_Localization.GetLocString("World_3725");
			}
			else if (_notificationType == "monthlysubscribeitem")
			{
				headline = MilMo_FeedExclamations.GetExclamation();
				eventDescription = MilMo_Localization.GetLocString("World_3728");
			}
			else if (_notificationType == "monthlymemberitem")
			{
				headline = MilMo_Localization.GetLocString("World_3726");
				eventDescription = MilMo_Localization.GetLocString("World_3727");
			}
			MilMo_LocString milMo_LocString;
			if (_amount > 1)
			{
				milMo_LocString = MilMo_Localization.GetLocString("World_3723");
				milMo_LocString.SetFormatArgs(ObjectName, _amount);
			}
			else
			{
				milMo_LocString = ObjectName;
			}
			MilMo_GameDialog milMo_GameDialog = new MilMo_GameDialogItem(UI, CloseDialog, headline, eventDescription, milMo_LocString, FeedDescriptionIngame, MilMo_GameDialog.HudDestination.Bag);
			UI.AddChild(milMo_GameDialog);
			milMo_GameDialog.Show(_item);
			MilMo_GameDialogCreator.TheDialogs[UI] = new OpenDialog(milMo_GameDialog, this);
		}
	}
}
