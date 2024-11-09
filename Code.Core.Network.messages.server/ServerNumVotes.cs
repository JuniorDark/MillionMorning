namespace Code.Core.Network.messages.server;

public class ServerNumVotes : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 380;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerNumVotes(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 5;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 380;

	private int numVotes;

	private sbyte voteType;

	private ServerNumVotes(MessageReader reader)
	{
		numVotes = reader.ReadInt32();
		voteType = reader.ReadInt8();
	}

	public ServerNumVotes(int numVotes, sbyte voteType)
	{
		this.numVotes = numVotes;
		this.voteType = voteType;
	}

	public int getNumVotes()
	{
		return numVotes;
	}

	public sbyte getVoteType()
	{
		return voteType;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(7);
		messageWriter.WriteOpCode(380);
		messageWriter.WriteInt32(numVotes);
		messageWriter.WriteInt8(voteType);
		return messageWriter.GetData();
	}
}
