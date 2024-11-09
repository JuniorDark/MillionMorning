using System;

namespace Code.Core.Network.messages.client;

public class ClientAbortSteamTransaction : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 427;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientAbortSteamTransaction(reader);
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

	private const int OPCODE = 427;

	private string orderId;

	private ClientAbortSteamTransaction(MessageReader reader)
	{
		orderId = reader.ReadString();
	}

	public ClientAbortSteamTransaction(string orderId)
	{
		this.orderId = orderId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(orderId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(427);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(orderId);
		return messageWriter.GetData();
	}
}
