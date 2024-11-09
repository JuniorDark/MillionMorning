using System;

namespace Code.Core.Network.messages.server;

public class ServerAvatarCreated : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 26;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerAvatarCreated(reader);
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

	private const int OPCODE = 26;

	private string token;

	private ServerAvatarCreated(MessageReader reader)
	{
		token = reader.ReadString();
	}

	public ServerAvatarCreated(string token)
	{
		this.token = token;
	}

	public string getToken()
	{
		return token;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(token);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(26);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(token);
		return messageWriter.GetData();
	}
}