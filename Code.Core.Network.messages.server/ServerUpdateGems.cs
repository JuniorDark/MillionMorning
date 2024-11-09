namespace Code.Core.Network.messages.server;

public class ServerUpdateGems : IMessage
{
	public enum EventTypes
	{
		GemReceived = 1,
		GemSpent,
		GemStolen
	}

	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 164;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateGems(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 7;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 164;

	private int gemCount;

	private sbyte eventType;

	private short bonusGems;

	private ServerUpdateGems(MessageReader reader)
	{
		gemCount = reader.ReadInt32();
		eventType = reader.ReadInt8();
		bonusGems = reader.ReadInt16();
	}

	public ServerUpdateGems(int gemCount, sbyte eventType, short bonusGems)
	{
		this.gemCount = gemCount;
		this.eventType = eventType;
		this.bonusGems = bonusGems;
	}

	public int getGemCount()
	{
		return gemCount;
	}

	public EventTypes getEventType()
	{
		return (EventTypes)eventType;
	}

	public short getBonusGems()
	{
		return bonusGems;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(9);
		messageWriter.WriteOpCode(164);
		messageWriter.WriteInt32(gemCount);
		messageWriter.WriteInt8(eventType);
		messageWriter.WriteInt16(bonusGems);
		return messageWriter.GetData();
	}
}
