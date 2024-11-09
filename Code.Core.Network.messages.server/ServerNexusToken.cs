using System;

namespace Code.Core.Network.messages.server;

public class ServerNexusToken : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 376;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerNexusToken(reader);
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

	private const int OPCODE = 376;

	private string host;

	private int port;

	private string token;

	private ServerNexusToken(MessageReader reader)
	{
		host = reader.ReadString();
		port = reader.ReadInt32();
		token = reader.ReadString();
	}

	public ServerNexusToken(string host, int port, string token)
	{
		this.host = host;
		this.port = port;
		this.token = token;
	}

	public string getHost()
	{
		return host;
	}

	public int getPort()
	{
		return port;
	}

	public string getToken()
	{
		return token;
	}

	public byte[] GetData()
	{
		int num = 12;
		num += MessageWriter.GetSize(host);
		num += MessageWriter.GetSize(token);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(376);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(host);
		messageWriter.WriteInt32(port);
		messageWriter.WriteString(token);
		return messageWriter.GetData();
	}
}
