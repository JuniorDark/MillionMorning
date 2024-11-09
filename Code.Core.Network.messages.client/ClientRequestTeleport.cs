using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestTeleport : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 266;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestTeleport(reader);
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

	private const int OPCODE = 266;

	private string token;

	private string host;

	private int port;

	private ClientRequestTeleport(MessageReader reader)
	{
		token = reader.ReadString();
		host = reader.ReadString();
		port = reader.ReadInt32();
	}

	public ClientRequestTeleport(string token, string host, int port)
	{
		this.token = token;
		this.host = host;
		this.port = port;
	}

	public string getToken()
	{
		return token;
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
		int num = 12;
		num += MessageWriter.GetSize(token);
		num += MessageWriter.GetSize(host);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(266);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(token);
		messageWriter.WriteString(host);
		messageWriter.WriteInt32(port);
		return messageWriter.GetData();
	}
}
