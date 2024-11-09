namespace Code.Core.Network.messages.server;

public class ServerGameplayObjectChangeDirection : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 176;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGameplayObjectChangeDirection(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 176;

	private int objectId;

	private int newDirection;

	private ServerGameplayObjectChangeDirection(MessageReader reader)
	{
		objectId = reader.ReadInt32();
		newDirection = reader.ReadInt32();
	}

	public ServerGameplayObjectChangeDirection(int objectId, int newDirection)
	{
		this.objectId = objectId;
		this.newDirection = newDirection;
	}

	public int getObjectId()
	{
		return objectId;
	}

	public int getNewDirection()
	{
		return newDirection;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(176);
		messageWriter.WriteInt32(objectId);
		messageWriter.WriteInt32(newDirection);
		return messageWriter.GetData();
	}
}
