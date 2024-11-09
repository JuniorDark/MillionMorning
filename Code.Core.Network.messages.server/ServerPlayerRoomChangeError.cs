namespace Code.Core.Network.messages.server;

public class ServerPlayerRoomChangeError : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 385;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerRoomChangeError(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 385;

	private sbyte reson;

	private ServerPlayerRoomChangeError(MessageReader reader)
	{
		reson = reader.ReadInt8();
	}

	public ServerPlayerRoomChangeError(sbyte reson)
	{
		this.reson = reson;
	}

	public sbyte getReson()
	{
		return reson;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(385);
		messageWriter.WriteInt8(reson);
		return messageWriter.GetData();
	}
}
