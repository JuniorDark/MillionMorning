using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerCurrencyInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 235;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCurrencyInfo(reader);
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

	private const int OPCODE = 235;

	private Currency currency;

	private ServerCurrencyInfo(MessageReader reader)
	{
		currency = new Currency(reader);
	}

	public ServerCurrencyInfo(Currency currency)
	{
		this.currency = currency;
	}

	public Currency getCurrency()
	{
		return currency;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += currency.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(235);
		messageWriter.WriteInt16((short)(num - 4));
		currency.Write(messageWriter);
		return messageWriter.GetData();
	}
}
