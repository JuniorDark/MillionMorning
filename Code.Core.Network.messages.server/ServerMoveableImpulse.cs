using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerMoveableImpulse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 142;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerMoveableImpulse(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 33;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 142;

	private int creatureId;

	private vector3 position;

	private vector3 target;

	private float impulse;

	private sbyte deathImpulse;

	private ServerMoveableImpulse(MessageReader reader)
	{
		creatureId = reader.ReadInt32();
		position = new vector3(reader);
		target = new vector3(reader);
		impulse = reader.ReadFloat();
		deathImpulse = reader.ReadInt8();
	}

	public ServerMoveableImpulse(int creatureId, vector3 position, vector3 target, float impulse, sbyte deathImpulse)
	{
		this.creatureId = creatureId;
		this.position = position;
		this.target = target;
		this.impulse = impulse;
		this.deathImpulse = deathImpulse;
	}

	public int getCreatureId()
	{
		return creatureId;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public vector3 getTarget()
	{
		return target;
	}

	public float getImpulse()
	{
		return impulse;
	}

	public sbyte getDeathImpulse()
	{
		return deathImpulse;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(35);
		messageWriter.WriteOpCode(142);
		messageWriter.WriteInt32(creatureId);
		position.Write(messageWriter);
		target.Write(messageWriter);
		messageWriter.WriteFloat(impulse);
		messageWriter.WriteInt8(deathImpulse);
		return messageWriter.GetData();
	}
}
