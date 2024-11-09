using Code.Core.Network.types;

namespace Code.Core.Network.messages.debug;

public class DebugServerCapsuleVolumeSync : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 177;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new DebugServerCapsuleVolumeSync(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 44;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 177;

	private vector3 position;

	private vector3 direction;

	private float length;

	private float radius;

	private vector3 creaturePosition;

	private DebugServerCapsuleVolumeSync(MessageReader reader)
	{
		position = new vector3(reader);
		direction = new vector3(reader);
		length = reader.ReadFloat();
		radius = reader.ReadFloat();
		creaturePosition = new vector3(reader);
	}

	public DebugServerCapsuleVolumeSync(vector3 position, vector3 direction, float length, float radius, vector3 creaturePosition)
	{
		this.position = position;
		this.direction = direction;
		this.length = length;
		this.radius = radius;
		this.creaturePosition = creaturePosition;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public vector3 getDirection()
	{
		return direction;
	}

	public float getLength()
	{
		return length;
	}

	public float getRadius()
	{
		return radius;
	}

	public vector3 getCreaturePosition()
	{
		return creaturePosition;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(46);
		messageWriter.WriteOpCode(177);
		position.Write(messageWriter);
		direction.Write(messageWriter);
		messageWriter.WriteFloat(length);
		messageWriter.WriteFloat(radius);
		creaturePosition.Write(messageWriter);
		return messageWriter.GetData();
	}
}
