using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerShopResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 134;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerShopResponse(reader);
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

	private const int OPCODE = 134;

	private sbyte shopResult;

	private Currency currencyInfo;

	private long currentTimestampGMT;

	private ServerShopResponse(MessageReader reader)
	{
		shopResult = reader.ReadInt8();
		if (reader.ReadInt8() == 1)
		{
			currencyInfo = new Currency(reader);
		}
		currentTimestampGMT = reader.ReadInt64();
	}

	public ServerShopResponse(sbyte shopResult, Currency currencyInfo, long currentTimestampGMT)
	{
		this.shopResult = shopResult;
		this.currencyInfo = currencyInfo;
		this.currentTimestampGMT = currentTimestampGMT;
	}

	public sbyte getShopResult()
	{
		return shopResult;
	}

	public Currency getCurrencyInfo()
	{
		return currencyInfo;
	}

	public long getCurrentTimestampGMT()
	{
		return currentTimestampGMT;
	}

	public byte[] GetData()
	{
		int num = 14;
		if (currencyInfo != null)
		{
			num += currencyInfo.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(134);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt8(shopResult);
		if (currencyInfo == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			currencyInfo.Write(messageWriter);
		}
		messageWriter.WriteInt64(currentTimestampGMT);
		return messageWriter.GetData();
	}
}
