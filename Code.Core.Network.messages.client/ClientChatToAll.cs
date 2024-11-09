using System;

namespace Code.Core.Network.messages.client;

public class ClientChatToAll : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 3;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientChatToAll(reader);
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

	private const int OPCODE = 3;

	private string message;

	private sbyte chanel;

	private ClientChatToAll(MessageReader reader)
	{
		message = reader.ReadString();
		chanel = reader.ReadInt8();
	}

	public ClientChatToAll(string message, sbyte chanel)
	{
		this.message = message;
		this.chanel = chanel;
	}

	public string getMessage()
	{
		return message;
	}

	public sbyte getChanel()
	{
		return chanel;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(message);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(3);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(message);
		messageWriter.WriteInt8(chanel);
		return messageWriter.GetData();
	}
}
