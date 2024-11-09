using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Monetization;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.World.GUI;
using Code.World.GUI.Steam;
using Core;
using Core.Analytics;
using Localization;
using Steamworks;
using UI.HUD.Dialogues;
using UI.Sprites;
using UnityEngine;

namespace Code.Core.Portal.Steam;

public class MilMo_SteamPortal : MilMo_Portal
{
	private static Action _dialogCallback;

	protected Callback<MicroTxnAuthorizationResponse_t> microTxnAuthorizationResponseT;

	private static JuneCashItem _item;

	public MilMo_SteamPortal()
	{
		new GameObject("SteamWorks").AddComponent<SteamManager>();
		MilMo_EventSystem.Listen("steam_transaction_error", OnSteamTransactionError).Repeating = true;
		microTxnAuthorizationResponseT = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicroTxnAuthorizationResponse);
		InitializeMonetization();
	}

	public override void ShowInviteInterface()
	{
		int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
		List<CSteamID> list = new List<CSteamID>();
		for (int i = 0; i < friendCount; i++)
		{
			CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
			list.Add(friendByIndex);
		}
		SteamInviteDialog steamInviteDialog = new SteamInviteDialog(MilMo_GlobalUI.GetSystemUI, list);
		MilMo_GlobalUI.GetSystemUI.AddChild(steamInviteDialog);
		steamInviteDialog.Open();
	}

	public static Texture2D GetSmallAvatar(CSteamID user)
	{
		int smallFriendAvatar = SteamFriends.GetSmallFriendAvatar(user);
		if (SteamUtils.GetImageSize(smallFriendAvatar, out var pnWidth, out var pnHeight) && pnWidth != 0 && pnHeight != 0)
		{
			byte[] array = new byte[pnWidth * pnHeight * 4];
			Texture2D texture2D = new Texture2D((int)pnWidth, (int)pnHeight, TextureFormat.RGBA32, mipChain: false, linear: true);
			if (!SteamUtils.GetImageRGBA(smallFriendAvatar, array, (int)(pnWidth * pnHeight * 4)))
			{
				return texture2D;
			}
			texture2D.LoadRawTextureData(array);
			texture2D.Apply();
			return FlipTexture(texture2D);
		}
		Debug.LogError("Couldn't get avatar.");
		return new Texture2D(0, 0);
	}

	private static Texture2D FlipTexture(Texture2D original, bool upSideDown = true)
	{
		Texture2D texture2D = new Texture2D(original.width, original.height);
		int width = original.width;
		int height = original.height;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (upSideDown)
				{
					texture2D.SetPixel(j, width - i - 1, original.GetPixel(j, i));
				}
				else
				{
					texture2D.SetPixel(width - i - 1, j, original.GetPixel(i, j));
				}
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	private static void InitializeMonetization()
	{
		MilMo_Monetization.Instance.Initialize(MilMo_JuneCash.Instance);
	}

	public static void InitiateMicroTransaction(JuneCashItem item, Action callback)
	{
		_dialogCallback = callback;
		_item = item;
		CSteamID steamID = SteamUser.GetSteamID();
		string currentGameLanguage = SteamApps.GetCurrentGameLanguage();
		int quantity = item.GetQuantity();
		Singleton<GameNetwork>.Instance.RequestSteamPurchase(steamID.ToString(), currentGameLanguage, quantity);
	}

	private static void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t param)
	{
		if (param.m_bAuthorized == 1)
		{
			Singleton<GameNetwork>.Instance.FinalizeSteamPurchase(param.m_ulOrderID.ToString(), param.m_unAppID.ToString());
			if (_dialogCallback != null)
			{
				_dialogCallback();
			}
			Analytics.Transaction(_item.GetProduct(), (int)_item.GetPrice());
		}
		else
		{
			Singleton<GameNetwork>.Instance.AbortSteamPurchase(param.m_ulOrderID.ToString());
		}
	}

	private static void OnSteamTransactionError(object message)
	{
		if (_dialogCallback != null)
		{
			_dialogCallback();
		}
		DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("Generic_4771"), new LocalizedStringWithArgument("Steam_0001"), new AddressableSpriteLoader("ErrorIcon"), null);
	}
}
