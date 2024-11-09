using System;
using Code.Core.Config;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Network;
using Core;
using Core.Settings;
using Steamworks;
using UnityEngine;

namespace Code.Apps.LoginScreen;

public class MilMo_SteamLogin : MilMo_Login
{
	private static byte[] _ticket;

	private static uint _pcbTicket;

	private static HAuthTicket _authTicket;

	protected Callback<GetAuthSessionTicketResponse_t> GetAuthSessionTicketResponseT;

	private new void Awake()
	{
		string text = Settings.Server.ToLower();
		if (!(text == "en") && !(text == "br"))
		{
			Debug.LogWarning("Missing configuration for SteamLogin on this server (" + text + "), using default 'en'");
			text = "en";
		}
		MilMo_Global.SteamLoginServer = MilMo_Config.Instance.GetValue("Steam.LoginServer." + text);
		base.Awake();
		GetAuthSessionTicketResponseT = Callback<GetAuthSessionTicketResponse_t>.Create(OnGetAuthSessionTicketResponse);
		_ticket = new byte[1024];
		MilMo_EventSystem.Listen("steam_token_recieved", SetSteamToken);
		Singleton<GameNetwork>.Instance.ConnectToGameServer(MilMo_Global.SteamLoginServer, null);
	}

	public static void GetAuthSessionTicket()
	{
		_authTicket = SteamUser.GetAuthSessionTicket(_ticket, 1024, out _pcbTicket);
	}

	private static void SetSteamToken(object token)
	{
		MilMo_Login.SetUserToken((string)token);
		SteamUser.CancelAuthTicket(_authTicket);
	}

	private static void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t callback)
	{
		byte[] array = new byte[_pcbTicket];
		Array.Copy(_ticket, array, _pcbTicket);
		Singleton<GameNetwork>.Instance.SendSteamLogin(array);
	}
}
