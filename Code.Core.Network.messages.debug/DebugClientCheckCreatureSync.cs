using Code.Core.Network.types;

namespace Code.Core.Network.messages.debug;

public class DebugClientCheckCreatureSync : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 151;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new DebugClientCheckCreatureSync(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 16;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 151;

	private int creatureId;

	private vector3 position;

	private DebugClientCheckCreatureSync(MessageReader reader)
	{
		creatureId = reader.ReadInt32();
		position = new vector3(reader);
	}

	public DebugClientCheckCreatureSync(int creatureId, vector3 position)
	{
		this.creatureId = creatureId;
		this.position = position;
	}

	public int getCreatureId()
	{
		return creatureId;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(18);
		messageWriter.WriteOpCode(151);
		messageWriter.WriteInt32(creatureId);
		position.Write(messageWriter);
		return messageWriter.GetData();
	}
}
