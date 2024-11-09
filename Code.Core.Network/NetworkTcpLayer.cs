using System.Text;
using Code.Apps.LoginScreen;
using Code.Core.EventSystem;
using UnityEngine;

namespace Code.Core.Network;

public class NetworkTcpLayer : ZenLayer, INetworkLayer
{
	private class ClientResponseMessage : IMessage
	{
		private readonly byte[] _response;

		public ClientResponseMessage(byte[] r)
		{
			_response = r;
		}

		public byte[] GetData()
		{
			MessageWriter messageWriter = new MessageWriter(8 + _response.Length);
			messageWriter.WriteOpCode(0);
			messageWriter.WriteInt16((short)(4 + _response.Length));
			messageWriter.WriteOpCode(1);
			messageWriter.WriteBytes(_response);
			return messageWriter.GetData();
		}
	}

	private const int SERVER_CHALLENGE = 0;

	private const int CLIENT_RESPONSE = 1;

	private const int LOGIN_OK = 2;

	private const int BAD_USER_OR_PASSWORD = 3;

	private const int UNKNOWN_ERROR = 4;

	private const int USER_ALREADY_LOGGED_ON = 5;

	private const int STEAM_LOGIN = 6;

	private const int USER_IS_BANNED = 7;

	private readonly string _token;

	public NetworkTcpLayer(IZenListener listener, string token)
		: base(listener)
	{
		_token = token;
	}

	protected override void OnSessionMessage(int opcode, MessageReader reader)
	{
		if (LoggedIn)
		{
			base.OnSessionMessage(opcode, reader);
			return;
		}
		opcode = reader.ReadOpCode();
		switch (opcode)
		{
		case 0:
			if (_token == null)
			{
				MilMo_SteamLogin.GetAuthSessionTicket();
			}
			else
			{
				SendClientResponseMessage();
			}
			break;
		case 2:
			Debug.Log("Login was successful.");
			OnLogin();
			break;
		case 3:
			Debug.Log("Failed to login, bad user or password");
			OnLoginFail(LoginResponse.BadUserOrPassword);
			break;
		case 4:
			Debug.Log("Failed to login, unknown error");
			OnLoginFail(LoginResponse.UnknownError);
			break;
		case 5:
			Debug.Log("Failed to login, user already logged on");
			OnLoginFail(LoginResponse.UserAlreadyLoggedOn);
			break;
		case 6:
		{
			string @string = Encoding.UTF8.GetString(reader.ReadBytes());
			MilMo_EventSystem.Instance.AsyncPostEvent("steam_token_recieved", @string);
			break;
		}
		case 7:
		{
			string banReason = reader.ReadString();
			string banExpiration = reader.ReadString();
			MilMo_EventSystem.Instance.AsyncPostEvent("login_failed", new MilMo_LoginInfo(LoginResponse.Banned, banReason, banExpiration));
			OnLoginFail(LoginResponse.Banned);
			break;
		}
		default:
			Debug.Log("Failed to login, unknown error (default)");
			OnLoginFail(LoginResponse.UnknownError);
			break;
		}
	}

	private void SendClientResponseMessage()
	{
		IMessage message = new ClientResponseMessage(Encoding.UTF8.GetBytes(_token));
		if (!SendMessage(message))
		{
			Debug.Log("Failed to login, failed to send client response");
			OnLoginFail(LoginResponse.Disconnected);
		}
	}
}
