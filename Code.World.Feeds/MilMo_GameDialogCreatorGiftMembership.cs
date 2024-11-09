using System;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.GUI.GameDialog;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Feeds;

public class MilMo_GameDialogCreatorGiftMembership : MilMo_GameDialogCreator
{
	private static string _iconPath = "Content/Items/Batch01/SpecialItems/IconMembership1Month";

	private readonly string _giverName;

	private readonly int _numberOfDays;

	private readonly bool _wasExtended;

	private readonly int _membershipDaysLeft;

	public MilMo_GameDialogCreatorGiftMembership(string giverName, int numberOfMemberDays, bool wasExtended, MilMo_UserInterface ui)
		: base(ui)
	{
		_giverName = giverName;
		_wasExtended = wasExtended;
		_numberOfDays = numberOfMemberDays;
		_membershipDaysLeft = MilMo_Player.Instance.MembershipDaysLeft;
		if (_membershipDaysLeft < 0)
		{
			_membershipDaysLeft = _numberOfDays;
		}
		else if (_wasExtended && Time.time - MilMo_Player.Instance.MembershipDaysLastSetTime > 60f)
		{
			_membershipDaysLeft += _numberOfDays;
		}
		switch (_numberOfDays)
		{
		case 30:
			_iconPath = "Content/Items/Batch01/SpecialItems/IconMembership1Month";
			break;
		case 90:
			_iconPath = "Content/Items/Batch01/SpecialItems/IconMembership3Month";
			break;
		case 180:
			_iconPath = "Content/Items/Batch01/SpecialItems/IconMembership6Month";
			break;
		case 72000:
			_iconPath = "Content/Items/Batch01/SpecialItems/IconMembershipLifetime";
			break;
		}
		IconPathIngame = _iconPath;
	}

	protected override void CreateDialog()
	{
		if (UI == null || (MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI) && MilMo_GameDialogCreator.TheDialogs[UI] != null))
		{
			return;
		}
		if (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI))
		{
			MilMo_GameDialogCreator.TheDialogs.Add(UI, null);
		}
		MilMo_LocString locString;
		MilMo_LocString locString2;
		if (_numberOfDays > 30000)
		{
			locString = MilMo_Localization.GetLocString("World_4767");
			locString2 = MilMo_Localization.GetLocString("World_4768");
		}
		else
		{
			locString = MilMo_Localization.GetLocString("World_4769");
			locString.SetFormatArgs(_numberOfDays / 30);
			if (_wasExtended)
			{
				locString2 = MilMo_Localization.GetLocString("World_5589");
				locString2.SetFormatArgs(DateTime.Today.AddDays(_membershipDaysLeft).Date.ToString("yyyy-MM-dd"));
			}
			else
			{
				locString2 = MilMo_Localization.GetLocString("World_4770");
				locString2.SetFormatArgs(_numberOfDays);
			}
		}
		MilMo_GameDialog milMo_GameDialog = new MilMo_GameDialogGift(UI, CloseDialog, MilMo_Localization.GetNotLocalizedLocString(_giverName), locString, locString2, MilMo_GameDialog.HudDestination.Map);
		UI.AddChild(milMo_GameDialog);
		milMo_GameDialog.Show(_iconPath);
		MilMo_GameDialogCreator.TheDialogs[UI] = new OpenDialog(milMo_GameDialog, this);
	}
}
