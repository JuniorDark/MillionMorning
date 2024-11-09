namespace Code.Core.Network.messages.client;

public class ClientRequestLadderPageContainingSelf : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 389;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestLadderPageContainingSelf(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 2;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 389;

	private sbyte type;

	private sbyte perPage;

	private ClientRequestLadderPageContainingSelf(MessageReader reader)
	{
		type = reader.ReadInt8();
		perPage = reader.ReadInt8();
	}

	public ClientRequestLadderPageContainingSelf(sbyte type, sbyte perPage)
	{
		this.type = type;
		this.perPage = perPage;
	}

	public sbyte getType()
	{
		return type;
	}

	public sbyte getPerPage()
	{
		return perPage;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(4);
		messageWriter.WriteOpCode(389);
		messageWriter.WriteInt8(type);
		messageWriter.WriteInt8(perPage);
		return messageWriter.GetData();
	}
}
