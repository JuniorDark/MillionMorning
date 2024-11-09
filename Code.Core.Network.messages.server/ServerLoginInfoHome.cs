using System;

namespace Code.Core.Network.messages.server;

public class ServerLoginInfoHome : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 241;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLoginInfoHome(reader);
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

	private const int OPCODE = 241;

	private string host;

	private string homeOwnerId;

	private sbyte homeHasNewItemsInDB;

	private string token;

	private ServerLoginInfoHome(MessageReader reader)
	{
		host = reader.ReadString();
		homeOwnerId = reader.ReadString();
		homeHasNewItemsInDB = reader.ReadInt8();
		token = reader.ReadString();
	}

	public ServerLoginInfoHome(string host, string homeOwnerId, sbyte homeHasNewItemsInDB, string token)
	{
		this.host = host;
		this.homeOwnerId = homeOwnerId;
		this.homeHasNewItemsInDB = homeHasNewItemsInDB;
		this.token = token;
	}

	public string getHost()
	{
		return host;
	}

	public string getHomeOwnerId()
	{
		return homeOwnerId;
	}

	public sbyte getHomeHasNewItemsInDB()
	{
		return homeHasNewItemsInDB;
	}

	public string getToken()
	{
		return token;
	}

	public byte[] GetData()
	{
		int num = 11;
		num += MessageWriter.GetSize(host);
		num += MessageWriter.GetSize(homeOwnerId);
		num += MessageWriter.GetSize(token);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(241);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(host);
		messageWriter.WriteString(homeOwnerId);
		messageWriter.WriteInt8(homeHasNewItemsInDB);
		messageWriter.WriteString(token);
		return messageWriter.GetData();
	}
}
