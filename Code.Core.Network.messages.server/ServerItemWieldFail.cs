namespace Code.Core.Network.messages.server;

public class ServerItemWieldFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 32;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerItemWieldFail(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 5;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 32;

	private int itemID;

	private sbyte lowLevel;

	private ServerItemWieldFail(MessageReader reader)
	{
		itemID = reader.ReadInt32();
		lowLevel = reader.ReadInt8();
	}

	public ServerItemWieldFail(int itemID, sbyte lowLevel)
	{
		this.itemID = itemID;
		this.lowLevel = lowLevel;
	}

	public int getItemID()
	{
		return itemID;
	}

	public sbyte getLowLevel()
	{
		return lowLevel;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(7);
		messageWriter.WriteOpCode(32);
		messageWriter.WriteInt32(itemID);
		messageWriter.WriteInt8(lowLevel);
		return messageWriter.GetData();
	}
}
