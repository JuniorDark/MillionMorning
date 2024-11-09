namespace Code.Core.Network.nexus;

public static class ConnectionFactory
{
	public static Connection CreateTcpConnection(INexusListener listener)
	{
		return new ConnectionImplementation(listener);
	}
}
