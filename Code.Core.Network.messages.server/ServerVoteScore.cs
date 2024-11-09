namespace Code.Core.Network.messages.server;

public class ServerVoteScore : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 379;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerVoteScore(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 9;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 379;

	private float score;

	private sbyte voteType;

	private int numVotes;

	private ServerVoteScore(MessageReader reader)
	{
		score = reader.ReadFloat();
		voteType = reader.ReadInt8();
		numVotes = reader.ReadInt32();
	}

	public ServerVoteScore(float score, sbyte voteType, int numVotes)
	{
		this.score = score;
		this.voteType = voteType;
		this.numVotes = numVotes;
	}

	public float getScore()
	{
		return score;
	}

	public sbyte getVoteType()
	{
		return voteType;
	}

	public int getNumVotes()
	{
		return numVotes;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(11);
		messageWriter.WriteOpCode(379);
		messageWriter.WriteFloat(score);
		messageWriter.WriteInt8(voteType);
		messageWriter.WriteInt32(numVotes);
		return messageWriter.GetData();
	}
}
