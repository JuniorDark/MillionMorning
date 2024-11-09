using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerShopCategories : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 70;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerShopCategories(reader);
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

	private const int OPCODE = 70;

	private ShopCategory root;

	private int numberOfItemMessages;

	private ServerShopCategories(MessageReader reader)
	{
		root = new ShopCategory(reader);
		numberOfItemMessages = reader.ReadInt32();
	}

	public ServerShopCategories(ShopCategory root, int numberOfItemMessages)
	{
		this.root = root;
		this.numberOfItemMessages = numberOfItemMessages;
	}

	public ShopCategory getRoot()
	{
		return root;
	}

	public int getNumberOfItemMessages()
	{
		return numberOfItemMessages;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += root.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(70);
		messageWriter.WriteInt16((short)(num - 4));
		root.Write(messageWriter);
		messageWriter.WriteInt32(numberOfItemMessages);
		return messageWriter.GetData();
	}
}
