namespace Code.Core.Network.messages.client;

public class ClientGetLadderFromPosition : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 321;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientGetLadderFromPosition(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 321;

	private int position;

	private int numberOfEntries;

	private ClientGetLadderFromPosition(MessageReader reader)
	{
		position = reader.ReadInt32();
		numberOfEntries = reader.ReadInt32();
	}

	public ClientGetLadderFromPosition(int position, int numberOfEntries)
	{
		this.position = position;
		this.numberOfEntries = numberOfEntries;
	}

	public int getPosition()
	{
		return position;
	}

	public int getNumberOfEntries()
	{
		return numberOfEntries;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(321);
		messageWriter.WriteInt32(position);
		messageWriter.WriteInt32(numberOfEntries);
		return messageWriter.GetData();
	}
}
