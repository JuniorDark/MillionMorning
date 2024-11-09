using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using Code.World.GUI.ShopPopups;
using UnityEngine;

namespace Code.World.GUI.FriendInvites;

public class MilMo_InviteMeter : MilMo_Widget
{
	public enum ERewardSteps
	{
		One,
		Three,
		Five,
		Ten,
		Twenty,
		ThirtyFive,
		Fifty,
		Hundred,
		HundredFifty,
		ThreeHundred,
		NrOfSteps
	}

	private MilMo_Widget _meterTexture;

	private MilMo_ProgressBar _invitesProgressBar;

	private MilMo_ProgressBar _acceptedProgressBar;

	private MilMo_Widget _topText;

	private MilMo_Widget _botText;

	private readonly int[] _inviteSteps;

	private readonly MilMo_Button[] _inviteItems;

	private readonly MilMo_Button[] _acceptItems;

	private readonly MilMo_ShopItemPopup[] _inviteItemPopups;

	private readonly MilMo_ShopItemPopup[] _acceptedInviteItemPopups;

	internal static ERewardSteps GetStep(int value)
	{
		return value switch
		{
			1 => ERewardSteps.One, 
			3 => ERewardSteps.Three, 
			5 => ERewardSteps.Five, 
			10 => ERewardSteps.Ten, 
			20 => ERewardSteps.Twenty, 
			35 => ERewardSteps.ThirtyFive, 
			50 => ERewardSteps.Fifty, 
			100 => ERewardSteps.Hundred, 
			150 => ERewardSteps.HundredFifty, 
			300 => ERewardSteps.ThreeHundred, 
			_ => ERewardSteps.NrOfSteps, 
		};
	}

	public MilMo_InviteMeter(MilMo_UserInterface ui)
		: base(ui)
	{
		_inviteItemPopups = new MilMo_ShopItemPopup[10];
		_acceptedInviteItemPopups = new MilMo_ShopItemPopup[10];
		_inviteItems = new MilMo_Button[10];
		_acceptItems = new MilMo_Button[10];
		_inviteSteps = new int[10];
		_inviteSteps[0] = 1;
		_inviteSteps[1] = 3;
		_inviteSteps[2] = 5;
		_inviteSteps[3] = 10;
		_inviteSteps[4] = 20;
		_inviteSteps[5] = 35;
		_inviteSteps[6] = 50;
		_inviteSteps[7] = 100;
		_inviteSteps[8] = 150;
		_inviteSteps[9] = 300;
		float num = 32f;
		for (int i = 0; i < 10; i++)
		{
			_inviteItems[i] = new MilMo_Button(UI);
			_inviteItems[i].SetTextureWhite();
			_inviteItems[i].SetScale(27f, 27f);
			_inviteItems[i].UseParentAlpha = false;
			_inviteItems[i].FadeToDefaultColor = false;
			_inviteItems[i].SetPosition(25f + (float)i * num, 50f);
			_inviteItems[i].SetAlignment(MilMo_GUI.Align.BottomCenter);
			_acceptItems[i] = new MilMo_Button(UI);
			_acceptItems[i].SetTextureWhite();
			_acceptItems[i].SetScale(27f, 27f);
			_acceptItems[i].UseParentAlpha = false;
			_acceptItems[i].FadeToDefaultColor = false;
			_acceptItems[i].SetPosition(25f + (float)i * num, 90f);
			_acceptItems[i].SetAlignment(MilMo_GUI.Align.TopCenter);
			AddChild(_acceptItems[i]);
			AddChild(_inviteItems[i]);
		}
		Initialize();
	}

	public void SetItems(MilMo_InviteItem[] sentInvites, MilMo_InviteItem[] acceptedInvites)
	{
		for (int i = 0; i < 10; i++)
		{
			int index = i;
			if (sentInvites[i] != null)
			{
				sentInvites[i].Item.AsyncGetIcon(delegate(Texture2D tex)
				{
					_inviteItems[index].SetAllTextures(tex);
					_inviteItems[index].Info = index;
					MilMo_LocString milMo_LocString2 = sentInvites[index].Item.Template.DisplayName;
					if (sentInvites[index].Amount > 1)
					{
						milMo_LocString2 = MilMo_Localization.GetNotLocalizedLocString("{0} x {1}").GetCopy();
						milMo_LocString2.SetFormatArgs(sentInvites[index].Amount, sentInvites[index].Item.Template.DisplayName);
					}
					_inviteItemPopups[index] = new MilMo_ShopItemPopup(UI, milMo_LocString2, sentInvites[index].Item.Template.Description, new MilMo_Texture(tex));
					_inviteItemPopups[index].Close(null);
					UI.AddChild(_inviteItemPopups[index]);
					int index2 = index;
					_inviteItems[index].PointerHoverFunction = delegate
					{
						_inviteItemPopups[index2].Open();
						_inviteItemPopups[index2].BringToFront();
						_inviteItemPopups[index2].SetPosition(MilMo_Pointer.Position.x - _inviteItemPopups[index2].Scale.x * 0.5f, MilMo_Pointer.Position.y - _inviteItemPopups[index2].Scale.y);
					};
					_inviteItems[index].PointerLeaveFunction = delegate
					{
						_inviteItemPopups[index2].Close(null);
					};
				});
			}
			if (acceptedInvites[i] == null)
			{
				continue;
			}
			acceptedInvites[i].Item.AsyncGetIcon(delegate(Texture2D tex)
			{
				_acceptItems[index].SetAllTextures(tex);
				_acceptItems[index].Info = index;
				MilMo_LocString milMo_LocString = acceptedInvites[index].Item.Template.DisplayName;
				if (acceptedInvites[index].Amount > 1)
				{
					milMo_LocString = MilMo_Localization.GetNotLocalizedLocString("{0} x {1}").GetCopy();
					milMo_LocString.SetFormatArgs(acceptedInvites[index].Amount, acceptedInvites[index].Item.Template.DisplayName);
				}
				_acceptedInviteItemPopups[index] = new MilMo_ShopItemPopup(UI, milMo_LocString, acceptedInvites[index].Item.Template.Description, new MilMo_Texture(tex));
				_acceptedInviteItemPopups[index].Close(null);
				UI.AddChild(_acceptedInviteItemPopups[index]);
				int index2 = index;
				_acceptItems[index].PointerHoverFunction = delegate
				{
					_acceptedInviteItemPopups[index2].Open();
					_acceptedInviteItemPopups[index2].BringToFront();
					_acceptedInviteItemPopups[index2].SetPosition(MilMo_Pointer.Position.x - _acceptedInviteItemPopups[index2].Scale.x * 0.5f, MilMo_Pointer.Position.y - _acceptedInviteItemPopups[index2].Scale.y);
				};
				_acceptItems[index].PointerLeaveFunction = delegate
				{
					_acceptedInviteItemPopups[index2].Close(null);
				};
			});
		}
	}

	private void Initialize()
	{
		_meterTexture = new MilMo_Widget(UI);
		_meterTexture.SetTexture("Batch01/Textures/FriendList/InviteMeter");
		_meterTexture.SetAlignment(MilMo_GUI.Align.CenterCenter);
		AddChild(_meterTexture);
		_invitesProgressBar = new MilMo_ProgressBar(UI, default(Vector2), 295f, 7f, 1f, Color.black, Color.green, 0f);
		_invitesProgressBar.UseBarShade = true;
		_acceptedProgressBar = new MilMo_ProgressBar(UI, default(Vector2), 295f, 7f, 1f, Color.black, Color.green, 0f);
		_acceptedProgressBar.UseBarShade = true;
		_invitesProgressBar.SetPosition(19f, 60f);
		_acceptedProgressBar.SetPosition(19f, 79f);
		_invitesProgressBar.CurrentProgress = 0f;
		_acceptedProgressBar.CurrentProgress = 0f;
		_invitesProgressBar.SetAlignment(MilMo_GUI.Align.TopLeft);
		_acceptedProgressBar.SetAlignment(MilMo_GUI.Align.TopLeft);
		_invitesProgressBar.AllowPointerFocus = false;
		_acceptedProgressBar.AllowPointerFocus = false;
		AddChild(_invitesProgressBar);
		AddChild(_acceptedProgressBar);
		_topText = new MilMo_Widget(UI);
		_topText.SetAlignment(MilMo_GUI.Align.TopCenter);
		_topText.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_topText.SetFont(MilMo_GUI.Font.ArialRounded);
		_topText.SetText(MilMo_Localization.GetLocString("Messenger_FriendInvites_12594"));
		_topText.AllowPointerFocus = false;
		AddChild(_topText);
		_botText = new MilMo_Widget(UI);
		_botText.SetAlignment(MilMo_GUI.Align.BottomCenter);
		_botText.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_botText.SetFont(MilMo_GUI.Font.ArialRounded);
		_botText.SetText(MilMo_Localization.GetLocString("Messenger_FriendInvites_12599"));
		_botText.AllowPointerFocus = false;
		AddChild(_botText);
		RefreshUI();
		UpdateValues(0, 0);
	}

	internal void UpdateValues(int totalInvites, int totalAcceptedInvites)
	{
		for (int i = 0; i < 10; i++)
		{
			_inviteItems[i].RemoveAllChildren();
			_acceptItems[i].RemoveAllChildren();
			if (totalInvites < _inviteSteps[i])
			{
				_inviteItems[i].SetAlpha(0.3f);
			}
			else
			{
				_inviteItems[i].SetAlpha(1f);
				MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
				milMo_Widget.SetTexture("Batch01/Textures/Homes/IconCheck");
				milMo_Widget.SetScale(14f, 14f);
				milMo_Widget.SetPosition(27f, 27f);
				milMo_Widget.SetAlignment(MilMo_GUI.Align.BottomRight);
				_inviteItems[i].AddChild(milMo_Widget);
			}
			if (totalAcceptedInvites < _inviteSteps[i])
			{
				_acceptItems[i].SetAlpha(0.3f);
				continue;
			}
			_acceptItems[i].SetAlpha(1f);
			MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
			milMo_Widget2.SetTexture("Batch01/Textures/Homes/IconCheck");
			milMo_Widget2.SetScale(14f, 14f);
			milMo_Widget2.SetPosition(27f, 27f);
			milMo_Widget2.SetAlignment(MilMo_GUI.Align.BottomRight);
			_acceptItems[i].AddChild(milMo_Widget2);
		}
		_invitesProgressBar.CurrentProgress = GetProgressValue(totalInvites);
		_acceptedProgressBar.CurrentProgress = GetProgressValue(totalAcceptedInvites);
	}

	private float GetProgressValue(int value)
	{
		if (value <= 0)
		{
			return 0f;
		}
		if (value >= 300)
		{
			return 1f;
		}
		float num = 0f;
		int num2 = 0;
		float num3 = 0f;
		if (value <= 3)
		{
			num3 = 0f;
			num = 0.5f;
			num2 = value - 1;
		}
		else if (value <= 5)
		{
			num3 = 11f;
			num = 0.5f;
			num2 = value - 3;
		}
		else if (value <= 10)
		{
			num3 = 22f;
			num = 0.2f;
			num2 = value - 5;
		}
		else if (value <= 20)
		{
			num3 = 33f;
			num = 0.1f;
			num2 = value - 10;
		}
		else if (value <= 35)
		{
			num3 = 44f;
			num = 0.066f;
			num2 = value - 20;
		}
		else if (value <= 50)
		{
			num3 = 55f;
			num = 0.066f;
			num2 = value - 35;
		}
		else if (value <= 100)
		{
			num3 = 66f;
			num = 0.02f;
			num2 = value - 50;
		}
		else if (value <= 150)
		{
			num3 = 77f;
			num = 0.02f;
			num2 = value - 100;
		}
		else if (value <= 300)
		{
			num3 = 88f;
			num = 0.0066f;
			num2 = value - 150;
		}
		float num4 = (float)num2 * num;
		return (num3 + num4 * 11f) / 100f;
	}

	private void RefreshUI()
	{
		SetScale(335f, 140f);
		_meterTexture.SetScale(325f, 36f);
		_meterTexture.SetPosition(Scale.x * 0.5f, Scale.y * 0.5f);
		_topText.SetScale(150f, 20f);
		_topText.SetPosition(167f, 2f);
		_botText.SetScale(150f, 20f);
		_botText.SetPosition(167f, 138f);
	}
}
