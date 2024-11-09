using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerLevelObjectCreatureSpawn : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 131;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLevelObjectCreatureSpawn(reader);
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

	private const int OPCODE = 131;

	private LevelItem item;

	private int creature;

	private ServerLevelObjectCreatureSpawn(MessageReader reader)
	{
		item = new LevelItem(reader);
		creature = reader.ReadInt32();
	}

	public ServerLevelObjectCreatureSpawn(LevelItem item, int creature)
	{
		this.item = item;
		this.creature = creature;
	}

	public LevelItem getItem()
	{
		return item;
	}

	public int getCreature()
	{
		return creature;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += item.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(131);
		messageWriter.WriteInt16((short)(num - 4));
		item.Write(messageWriter);
		messageWriter.WriteInt32(creature);
		return messageWriter.GetData();
	}
}
