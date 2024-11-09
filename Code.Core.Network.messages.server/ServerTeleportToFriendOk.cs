using System;

namespace Code.Core.Network.messages.server;

public class ServerTeleportToFriendOk : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 265;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerTeleportToFriendOk(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 2;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt16();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 265;

	private int friendIdentifier;

	private string fullLevelName;

	private string token;

	private int homeOwnerId;

	private string host;

	private int port;

	private ServerTeleportToFriendOk(MessageReader reader)
	{
		friendIdentifier = reader.ReadInt32();
		fullLevelName = reader.ReadString();
		token = reader.ReadString();
		homeOwnerId = reader.ReadInt32();
		host = reader.ReadString();
		port = reader.ReadInt32();
	}

	public ServerTeleportToFriendOk(int friendIdentifier, string fullLevelName, string token, int homeOwnerId, string host, int port)
	{
		this.friendIdentifier = friendIdentifier;
		this.fullLevelName = fullLevelName;
		this.token = token;
		this.homeOwnerId = homeOwnerId;
		this.host = host;
		this.port = port;
	}

	public int getFriendIdentifier()
	{
		return friendIdentifier;
	}

	public string getFullLevelName()
	{
		return fullLevelName;
	}

	public string getToken()
	{
		return token;
	}

	public int getHomeOwnerId()
	{
		return homeOwnerId;
	}

	public string getHost()
	{
		return host;
	}

	public int getPort()
	{
		return port;
	}

	public byte[] GetData()
	{
		int num = 22;
		num += MessageWriter.GetSize(fullLevelName);
		num += MessageWriter.GetSize(token);
		num += MessageWriter.GetSize(host);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(265);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(friendIdentifier);
		messageWriter.WriteString(fullLevelName);
		messageWriter.WriteString(token);
		messageWriter.WriteInt32(homeOwnerId);
		messageWriter.WriteString(host);
		messageWriter.WriteInt32(port);
		return messageWriter.GetData();
	}
}
