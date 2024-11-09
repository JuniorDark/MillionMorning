using System;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.GUI.GameDialog;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Feeds;

public class MilMo_GameDialogCreatorBecameMember : MilMo_GameDialogCreator
{
	private const string ICON_PATH = "Content/GUI/Batch01/Textures/NPCDialog/NPCDialogPremiumPortrait";

	private readonly int _numberOfDays;

	private readonly bool _wasExtended;

	private readonly int _membershipDaysLeft;

	public MilMo_GameDialogCreatorBecameMember(int numberOfMemberDays, bool wasExtended, MilMo_UserInterface ui)
		: base(ui)
	{
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
		IconPathIngame = "Content/GUI/Batch01/Textures/NPCDialog/NPCDialogPremiumPortrait";
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
		MilMo_LocString locString = MilMo_Localization.GetLocString("World_4765");
		MilMo_LocString eventDescription = ((!_wasExtended) ? MilMo_Localization.GetLocString("World_4766") : MilMo_Localization.GetLocString("World_5588"));
		MilMo_LocString locString2;
		MilMo_LocString locString3;
		if (_numberOfDays > 30000)
		{
			locString2 = MilMo_Localization.GetLocString("World_4767");
			locString3 = MilMo_Localization.GetLocString("World_4768");
		}
		else
		{
			locString2 = MilMo_Localization.GetLocString("World_4769");
			locString2.SetFormatArgs(_numberOfDays / 30);
			if (_wasExtended)
			{
				locString3 = MilMo_Localization.GetLocString("World_5589");
				locString3.SetFormatArgs(DateTime.Today.AddDays(_membershipDaysLeft).Date.ToString("yyyy-MM-dd"));
			}
			else
			{
				locString3 = MilMo_Localization.GetLocString("World_4770");
				locString3.SetFormatArgs(_numberOfDays);
			}
		}
		MilMo_GameDialog milMo_GameDialog = new MilMo_GameDialogItem(UI, CloseDialog, locString, eventDescription, locString2, locString3, MilMo_GameDialog.HudDestination.Map);
		UI.AddChild(milMo_GameDialog);
		milMo_GameDialog.Show("Content/GUI/Batch01/Textures/NPCDialog/NPCDialogPremiumPortrait");
		MilMo_GameDialogCreator.TheDialogs[UI] = new OpenDialog(milMo_GameDialog, this);
	}
}
