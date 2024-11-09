using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.Portal;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Core;
using UnityEngine;

namespace Code.World.GUI.FriendInvites;

public class MilMoFriendInviteDialog : MilMo_Window
{
	private enum EType
	{
		Invites,
		Accepted
	}

	private MilMo_Button _inviteButton;

	private MilMo_InviteMeter _inviteMeter;

	private int _totalInvites;

	private int _totalAcceptedInvites;

	private static MilMoFriendInviteDialog _theDialog;

	private int _acceptedItemsDone;

	private int _sentItemsDone;

	public List<MilMo_InviteItem[]> InviteItems { get; private set; }

	public int TotalInvites => _totalInvites;

	public int TotalAcceptedInvites => _totalAcceptedInvites;

	public static MilMoFriendInviteDialog GetInstance()
	{
		if (_theDialog == null)
		{
			_theDialog = new MilMoFriendInviteDialog(MilMo_GlobalUI.GetSystemUI);
			MilMo_GlobalUI.GetSystemUI.AddChild(_theDialog);
		}
		return _theDialog;
	}

	public void SetItems(IList<InviteItem> inviteItems, IList<InviteItem> acceptedInviteItems)
	{
		InviteItems = new List<MilMo_InviteItem[]>
		{
			new MilMo_InviteItem[10],
			new MilMo_InviteItem[10]
		};
		for (int i = 0; i < inviteItems.Count; i++)
		{
			int index = i;
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(inviteItems[i].GetItem().GetTemplate(), delegate(MilMo_Template template, bool timeOut)
			{
				int step2 = (int)MilMo_InviteMeter.GetStep(inviteItems[index].GetInvitesNeeded());
				if (template is MilMo_ItemTemplate milMo_ItemTemplate2)
				{
					InviteItems[0][step2] = new MilMo_InviteItem(milMo_ItemTemplate2.Instantiate(MilMo_Item.ReadModifiers(inviteItems[index].GetItem().GetModifiers())), inviteItems[index].GetAmount());
					_sentItemsDone++;
					if (_acceptedItemsDone == acceptedInviteItems.Count && _sentItemsDone == inviteItems.Count)
					{
						_inviteMeter.SetItems(InviteItems[0], InviteItems[1]);
					}
				}
			});
		}
		for (int j = 0; j < acceptedInviteItems.Count; j++)
		{
			int index = j;
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(acceptedInviteItems[j].GetItem().GetTemplate(), delegate(MilMo_Template template, bool timeOut)
			{
				int step = (int)MilMo_InviteMeter.GetStep(acceptedInviteItems[index].GetInvitesNeeded());
				if (template is MilMo_ItemTemplate milMo_ItemTemplate)
				{
					InviteItems[1][step] = new MilMo_InviteItem(milMo_ItemTemplate.Instantiate(MilMo_Item.ReadModifiers(acceptedInviteItems[index].GetItem().GetModifiers())), acceptedInviteItems[index].GetAmount());
					_acceptedItemsDone++;
					if (_acceptedItemsDone == acceptedInviteItems.Count && _sentItemsDone == inviteItems.Count)
					{
						_inviteMeter.SetItems(InviteItems[0], InviteItems[1]);
					}
				}
			});
		}
	}

	private MilMoFriendInviteDialog(MilMo_UserInterface ui)
		: base(ui)
	{
		Initialize();
		Close(null);
	}

	public void UpdateValues(int invites, int accepted)
	{
		_totalInvites = invites;
		_totalAcceptedInvites = accepted;
		_inviteMeter.UpdateValues(invites, accepted);
	}

	private void Initialize()
	{
		base.Text = MilMo_Localization.GetLocString("Messenger_FriendInvites_12595");
		HasCloseButton = true;
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetTextureBlackTransparent();
		milMo_Widget.SetAlpha(0.4f);
		milMo_Widget.UseParentAlpha = false;
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetPosition(21f, 48f);
		milMo_Widget.SetScale(318f, 147f);
		AddChild(milMo_Widget);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.SetTexture("Batch01/Textures/FriendList/Invite");
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.BottomLeft);
		milMo_Widget2.UseParentAlpha = false;
		milMo_Widget2.SetScale(140f, 181f);
		milMo_Widget2.SetPosition(milMo_Widget.Pos.x - 8f, milMo_Widget.Pos.y + milMo_Widget.Scale.y);
		AddChild(milMo_Widget2);
		MilMo_Widget milMo_Widget3 = new MilMo_Widget(UI);
		milMo_Widget3.SetFont(MilMo_GUI.Font.ArialRounded);
		milMo_Widget3.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget3.SetAlignment(MilMo_GUI.Align.TopRight);
		milMo_Widget3.SetWordWrap(w: true);
		milMo_Widget3.SetScale(175f, 55f);
		milMo_Widget3.SetPosition(milMo_Widget.Scale.x, 0f);
		milMo_Widget3.UseParentAlpha = false;
		milMo_Widget3.SetText(MilMo_Localization.GetLocString("Messenger_FriendInvites_12596"));
		milMo_Widget.AddChild(milMo_Widget3);
		_inviteButton = new MilMo_Button(UI);
		_inviteButton.SetTexture("Batch01/Textures/GameDialog/GameDialogButtonGreen");
		_inviteButton.SetHoverTexture("Batch01/Textures/GameDialog/GameDialogButtonGreenMO");
		_inviteButton.SetPressedTexture("Batch01/Textures/GameDialog/GameDialogButtonGreenPressed");
		_inviteButton.SetScale(145f, 34f);
		_inviteButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_inviteButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		_inviteButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_inviteButton.SetText(MilMo_Localization.GetLocString("Messenger_FriendInvites_12595"));
		_inviteButton.SetPosition(182f, 110f);
		_inviteButton.SetFontPreset(MilMo_GUI.FontPreset.DropShadow);
		_inviteButton.Function = InviteCallback;
		AddChild(_inviteButton);
		_inviteMeter = new MilMo_InviteMeter(UI);
		_inviteMeter.SetAlignment(MilMo_GUI.Align.TopCenter);
		_inviteMeter.SetPosition(180f, 195f);
		AddChild(_inviteMeter);
		RefreshUI();
	}

	private void InviteCallback(object o)
	{
		MilMo_Portal.Instance.ShowInviteInterface();
	}

	public void RefreshUI()
	{
		SpawnScale = new Vector2(360f, 342f);
		TargetScale = new Vector2(360f, 342f);
	}

	public override void Close(object obj)
	{
		MilMo_EventSystem.Instance.PostEvent("tutorial_InviteFriends", null);
		base.Close(obj);
	}
}
