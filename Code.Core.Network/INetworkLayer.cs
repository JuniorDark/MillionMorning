namespace Code.Core.Network;

internal interface INetworkLayer
{
	bool IsLoggedIn { get; }

	bool IsConnected { get; }

	int HostPort { get; }

	string Hostname { get; }

	bool SendMessage(IMessage message);

	void Disconnect();
}
