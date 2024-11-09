namespace Code.Core.Network.messages.client;

public class ClientRequestLadderPage : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 388;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestLadderPage(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 9;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 388;

	private int perPage;

	private sbyte type;

	private int pageNumber;

	private ClientRequestLadderPage(MessageReader reader)
	{
		perPage = reader.ReadInt32();
		type = reader.ReadInt8();
		pageNumber = reader.ReadInt32();
	}

	public ClientRequestLadderPage(int perPage, sbyte type, int pageNumber)
	{
		this.perPage = perPage;
		this.type = type;
		this.pageNumber = pageNumber;
	}

	public int getPerPage()
	{
		return perPage;
	}

	public sbyte getType()
	{
		return type;
	}

	public int getPageNumber()
	{
		return pageNumber;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(11);
		messageWriter.WriteOpCode(388);
		messageWriter.WriteInt32(perPage);
		messageWriter.WriteInt8(type);
		messageWriter.WriteInt32(pageNumber);
		return messageWriter.GetData();
	}
}
