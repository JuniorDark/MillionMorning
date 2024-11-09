using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerObjectCreate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 53;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerObjectCreate(reader);
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

	private const int OPCODE = 53;

	private LevelItem item;

	private ServerObjectCreate(MessageReader reader)
	{
		item = new LevelItem(reader);
	}

	public ServerObjectCreate(LevelItem item)
	{
		this.item = item;
	}

	public LevelItem getItem()
	{
		return item;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += item.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(53);
		messageWriter.WriteInt16((short)(num - 4));
		item.Write(messageWriter);
		return messageWriter.GetData();
	}
}
