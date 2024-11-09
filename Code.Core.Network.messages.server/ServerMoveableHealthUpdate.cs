namespace Code.Core.Network.messages.server;

public class ServerMoveableHealthUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 67;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerMoveableHealthUpdate(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 67;

	private int moveableID;

	private float health;

	private ServerMoveableHealthUpdate(MessageReader reader)
	{
		moveableID = reader.ReadInt32();
		health = reader.ReadFloat();
	}

	public ServerMoveableHealthUpdate(int moveableID, float health)
	{
		this.moveableID = moveableID;
		this.health = health;
	}

	public int getMoveableID()
	{
		return moveableID;
	}

	public float getHealth()
	{
		return health;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(67);
		messageWriter.WriteInt32(moveableID);
		messageWriter.WriteFloat(health);
		return messageWriter.GetData();
	}
}
