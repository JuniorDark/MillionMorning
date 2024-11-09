using System;
using System.Net.Sockets;
using System.Threading;
using Code.Core.EventSystem;
using UnityEngine;

namespace Code.Core.Network;

public class ZenLayer
{
	private const int IO_BUFFER_SIZE = 1048576;

	private TcpClient _client;

	private Thread _rcvThread;

	private readonly IZenListener _zenListener;

	protected volatile bool LoggedIn;

	public string Hostname { get; private set; }

	public int HostPort { get; private set; }

	public bool IsConnected
	{
		get
		{
			if (_client != null)
			{
				return _client.Connected;
			}
			return false;
		}
	}

	public bool IsLoggedIn => LoggedIn;

	protected ZenLayer(IZenListener listener)
	{
		_zenListener = listener;
	}

	public void Connect(string host, int port)
	{
		if (IsConnected)
		{
			Debug.LogWarning("Already connected when calling Connect with host " + host);
			_zenListener.OnConnectFail(ConnectResponse.AlreadyConnected);
			return;
		}
		if (host == null)
		{
			Debug.LogWarning("Got null host name when calling Connect");
			_zenListener.OnConnectFail(ConnectResponse.BadHostName);
			return;
		}
		if (_client != null)
		{
			Debug.LogWarning("Client is not null when calling Connect and being marked as not logged in. This is a bug.");
			_zenListener.OnConnectFail(ConnectResponse.UnknownError);
			return;
		}
		try
		{
			_client = new TcpClient();
		}
		catch (SocketException ex)
		{
			Debug.LogWarning("Failed to create tcp-client" + ex.Message);
			_zenListener.OnConnectFail(ConnectResponse.UnknownError);
			return;
		}
		catch (Exception ex2)
		{
			Debug.LogWarning("Failed to create tcp-client: " + ex2.Message);
			_zenListener.OnConnectFail(ConnectResponse.UnknownError);
			return;
		}
		try
		{
			Debug.Log("Connecting to host " + host + " on port " + port);
			_client.Connect(host, port);
		}
		catch (SocketException ex3)
		{
			Debug.LogWarning("Got SocketException: " + ex3.Message);
			_client.Close();
			_client = null;
			_zenListener.OnConnectFail(ConnectResponse.Failed);
			return;
		}
		catch (NullReferenceException ex4)
		{
			Debug.LogWarning("Got NullReferenceException: " + ex4.Message);
			_client.Close();
			_client = null;
			_zenListener.OnConnectFail(ConnectResponse.Failed);
			return;
		}
		catch (Exception ex5)
		{
			Debug.LogWarning("Got Exception: " + ex5.Message);
			_client.Close();
			_client = null;
			_zenListener.OnConnectFail(ConnectResponse.Failed);
			return;
		}
		if (!_client.Connected)
		{
			Debug.LogWarning("Client not connected!");
			try
			{
				_client.Client.Shutdown(SocketShutdown.Both);
			}
			catch (Exception ex6)
			{
				Debug.LogWarning("Got exception when shutting down socket after a failed connect: " + ex6.Message);
			}
			_client.Client.Close();
			_client.Close();
			_client = null;
			_zenListener.OnConnectFail(ConnectResponse.Failed);
		}
		else
		{
			Debug.Log("Client is connected!");
			Hostname = host;
			HostPort = port;
			_rcvThread = new Thread(WaitForData);
			_rcvThread.Start(_client);
			_zenListener.OnConnect();
		}
	}

	public void Disconnect()
	{
		if (_client == null)
		{
			Debug.LogWarning("Calling disconnect when client is null. Disconnect will be aborted.");
			_zenListener.OnDisconnect(wasConnected: false);
			return;
		}
		try
		{
			_client.Client.Shutdown(SocketShutdown.Both);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Got exception when shutting down socket: " + ex.Message);
		}
		_client.Client.Close();
		_client.Close();
		_client = null;
		Debug.Log("Disconnected from host " + Hostname);
		_zenListener.OnDisconnect(wasConnected: true);
	}

	protected virtual void OnSessionMessage(int opcode, MessageReader reader)
	{
		MessageDispatcher.Dispatch(opcode, reader, _zenListener);
	}

	protected void OnLogin()
	{
		LoggedIn = true;
		_zenListener.OnLogin(LoginResponse.Success);
	}

	protected void OnLoginFail(LoginResponse response)
	{
		Debug.Log("Login failed");
		Disconnect();
		_zenListener.OnLoginFail(response);
	}

	public bool SendMessage(IMessage message)
	{
		if (!IsConnected)
		{
			return false;
		}
		try
		{
			_client.Client.Send(message.GetData());
		}
		catch (SocketException)
		{
			Disconnect();
			return false;
		}
		return true;
	}

	private void WaitForData(object client)
	{
		byte[] array = new byte[1048576];
		ByteBuffer byteBuffer = new ByteBuffer(2097152);
		TcpClient tcpClient = client as TcpClient;
		while (true)
		{
			int num = 0;
			try
			{
				if (tcpClient != null)
				{
					num = tcpClient.Client.Receive(array, SocketFlags.None);
				}
			}
			catch (Exception ex)
			{
				Debug.Log("ZenLayer Thread was aborted with exception: " + ex.Message);
				if (tcpClient != null)
				{
					MilMo_EventSystem.Listen("Disconnect" + tcpClient.GetHashCode(), delegate
					{
						Disconnect();
					});
					MilMo_EventSystem.Instance.AsyncPostEvent("Disconnect" + tcpClient.GetHashCode());
				}
				break;
			}
			if (num <= 0)
			{
				continue;
			}
			byteBuffer.Put(array, num);
			byteBuffer.Flip();
			byte[] array2 = new byte[2];
			while (byteBuffer.HasRemaining && byteBuffer.Remaining() >= 2)
			{
				Array.Copy(byteBuffer.Bytes, byteBuffer.Pos, array2, 0, 2);
				int num2 = new MessageReader(array2).ReadOpCode();
				if (!MessageDispatcher.GetMessageSize(num2, byteBuffer, out var length, out var lengthSize))
				{
					break;
				}
				byte[] array3 = new byte[length];
				Array.Copy(byteBuffer.Bytes, byteBuffer.Pos + lengthSize + 2, array3, 0, length);
				try
				{
					OnSessionMessage(num2, new MessageReader(array3));
				}
				catch (ArgumentOutOfRangeException ex2)
				{
					Debug.LogWarning("Got exception when shutting down sgs socket: " + ex2.Message + " (opCode:" + num2 + ")");
				}
				catch (NullReferenceException ex3)
				{
					Debug.LogWarning("Got exception when shutting down sgs socket: " + ex3.Message + " (opCode:" + num2 + ")");
				}
				catch (Exception ex4)
				{
					Debug.LogWarning("Got exception when shutting down sgs socket: " + ex4.Message + " (opCode:" + num2 + ")");
				}
				byteBuffer.Skip(length + lengthSize + 2);
			}
			byteBuffer.Compact();
		}
	}
}
