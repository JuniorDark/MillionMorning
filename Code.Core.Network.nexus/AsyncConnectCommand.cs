using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Code.Core.Network.nexus;

public class AsyncConnectCommand
{
	private readonly string _hostname;

	private readonly int _port;

	private readonly string _loginToken;

	private readonly TcpClient _client;

	private readonly Thread _receiveThread;

	public AsyncConnectCommand(string hostname, int port, string loginToken, TcpClient client, Thread receiveThread)
	{
		_hostname = hostname;
		_port = port;
		_loginToken = loginToken;
		_client = client;
		_receiveThread = receiveThread;
	}

	public void Connect()
	{
		try
		{
			_client.Connect(_hostname, _port);
			StreamWriter streamWriter = new StreamWriter(_client.GetStream());
			streamWriter.WriteLine(_loginToken);
			streamWriter.Flush();
		}
		catch (Exception)
		{
		}
		finally
		{
			_receiveThread.Start();
		}
	}
}
