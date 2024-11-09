using Code.Core.Network.types;

namespace Code.Core.Network.messages.debug;

public class DebugServerCreatureSync : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 152;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new DebugServerCreatureSync(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 20;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 152;

	private int creatureId;

	private float sqrDistance;

	private vector3 position;

	private DebugServerCreatureSync(MessageReader reader)
	{
		creatureId = reader.ReadInt32();
		sqrDistance = reader.ReadFloat();
		position = new vector3(reader);
	}

	public DebugServerCreatureSync(int creatureId, float sqrDistance, vector3 position)
	{
		this.creatureId = creatureId;
		this.sqrDistance = sqrDistance;
		this.position = position;
	}

	public int getCreatureId()
	{
		return creatureId;
	}

	public float getSqrDistance()
	{
		return sqrDistance;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(22);
		messageWriter.WriteOpCode(152);
		messageWriter.WriteInt32(creatureId);
		messageWriter.WriteFloat(sqrDistance);
		position.Write(messageWriter);
		return messageWriter.GetData();
	}
}
