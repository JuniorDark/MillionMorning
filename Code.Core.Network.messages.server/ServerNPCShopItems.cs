using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerNPCShopItems : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 16;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerNPCShopItems(reader);
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

	private const int OPCODE = 16;

	private readonly IList<InGameShopItem> _inGameShopItems;

	private ServerNPCShopItems(MessageReader reader)
	{
		_inGameShopItems = new List<InGameShopItem>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_inGameShopItems.Add(new InGameShopItem(reader));
		}
	}

	public ServerNPCShopItems(IList<InGameShopItem> inGameShopItems)
	{
		_inGameShopItems = inGameShopItems;
	}

	public IList<InGameShopItem> GetInGameShopItems()
	{
		return _inGameShopItems;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (InGameShopItem inGameShopItem in _inGameShopItems)
		{
			num += inGameShopItem.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(16);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)_inGameShopItems.Count);
		foreach (InGameShopItem inGameShopItem2 in _inGameShopItems)
		{
			inGameShopItem2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
