using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientUpdatePosition : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 49;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUpdatePosition(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 24;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 49;

	private vector3 position;

	private vector3 velocity;

	private ClientUpdatePosition(MessageReader reader)
	{
		position = new vector3(reader);
		velocity = new vector3(reader);
	}

	public ClientUpdatePosition(vector3 position, vector3 velocity)
	{
		this.position = position;
		this.velocity = velocity;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public vector3 getVelocity()
	{
		return velocity;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(26);
		messageWriter.WriteOpCode(49);
		position.Write(messageWriter);
		velocity.Write(messageWriter);
		return messageWriter.GetData();
	}
}
