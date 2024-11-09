using System;

namespace Code.Core.Network.messages.client;

public class ClientUpdateTrackedQuest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 439;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUpdateTrackedQuest(reader);
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

	private const int OPCODE = 439;

	private int questId;

	private bool track;

	private ClientUpdateTrackedQuest(MessageReader reader)
	{
		questId = reader.ReadInt32();
		track = reader.ReadInt8() == 1;
	}

	public ClientUpdateTrackedQuest(int questId, bool track)
	{
		this.questId = questId;
		this.track = track;
	}

	public byte[] GetData()
	{
		int num = 9;
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(439);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(questId);
		messageWriter.WriteInt8((sbyte)(track ? 1 : 0));
		return messageWriter.GetData();
	}
}
