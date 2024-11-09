using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerQuestConditionUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 305;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerQuestConditionUpdate(reader);
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

	private const int OPCODE = 305;

	private int questId;

	private short index;

	private Condition condition;

	private ServerQuestConditionUpdate(MessageReader reader)
	{
		questId = reader.ReadInt32();
		index = reader.ReadInt16();
		condition = Condition.Create(reader.ReadTypeCode(), reader);
	}

	public ServerQuestConditionUpdate(int questId, short index, Condition condition)
	{
		this.questId = questId;
		this.index = index;
		this.condition = condition;
	}

	public int getQuestId()
	{
		return questId;
	}

	public short getIndex()
	{
		return index;
	}

	public Condition getCondition()
	{
		return condition;
	}

	public byte[] GetData()
	{
		int num = 12;
		num += condition.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(305);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(questId);
		messageWriter.WriteInt16(index);
		messageWriter.WriteTypeCode(condition.GetTypeId());
		condition.Write(messageWriter);
		return messageWriter.GetData();
	}
}
