using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestCurrencyInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 234;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestCurrencyInfo(reader);
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

	private const int OPCODE = 234;

	private string currencyId;

	private ClientRequestCurrencyInfo(MessageReader reader)
	{
		currencyId = reader.ReadString();
	}

	public ClientRequestCurrencyInfo(string currencyId)
	{
		this.currencyId = currencyId;
	}

	public string getCurrencyId()
	{
		return currencyId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(currencyId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(234);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(currencyId);
		return messageWriter.GetData();
	}
}
