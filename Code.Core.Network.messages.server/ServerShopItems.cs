using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerShopItems : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 71;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerShopItems(reader);
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

	private const int OPCODE = 71;

	private ShopItems items;

	private ServerShopItems(MessageReader reader)
	{
		items = new ShopItems(reader);
	}

	public ServerShopItems(ShopItems items)
	{
		this.items = items;
	}

	public ShopItems getItems()
	{
		return items;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += items.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(71);
		messageWriter.WriteInt16((short)(num - 4));
		items.Write(messageWriter);
		return messageWriter.GetData();
	}
}
