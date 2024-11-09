using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientPlayerStop : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 51;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientPlayerStop(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 16;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 51;

	private vector3 position;

	private float rotation;

	private ClientPlayerStop(MessageReader reader)
	{
		position = new vector3(reader);
		rotation = reader.ReadFloat();
	}

	public ClientPlayerStop(vector3 position, float rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public float getRotation()
	{
		return rotation;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(18);
		messageWriter.WriteOpCode(51);
		position.Write(messageWriter);
		messageWriter.WriteFloat(rotation);
		return messageWriter.GetData();
	}
}
