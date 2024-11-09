namespace Code.Core.Network;

public interface IZenListener
{
	void OnConnect();

	void OnConnectFail(ConnectResponse response);

	void OnDisconnect(bool wasConnected);

	void OnLogin(LoginResponse response);

	void OnLoginFail(LoginResponse response);
}
