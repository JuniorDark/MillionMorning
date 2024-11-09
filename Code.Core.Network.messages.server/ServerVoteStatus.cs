using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerVoteStatus : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 377;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerVoteStatus(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 11;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 377;

	private sbyte canVote;

	private float currentScore;

	private VoteData voteData;

	private ServerVoteStatus(MessageReader reader)
	{
		canVote = reader.ReadInt8();
		currentScore = reader.ReadFloat();
		voteData = new VoteData(reader);
	}

	public ServerVoteStatus(sbyte canVote, float currentScore, VoteData voteData)
	{
		this.canVote = canVote;
		this.currentScore = currentScore;
		this.voteData = voteData;
	}

	public sbyte getCanVote()
	{
		return canVote;
	}

	public float getCurrentScore()
	{
		return currentScore;
	}

	public VoteData getVoteData()
	{
		return voteData;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(13);
		messageWriter.WriteOpCode(377);
		messageWriter.WriteInt8(canVote);
		messageWriter.WriteFloat(currentScore);
		voteData.Write(messageWriter);
		return messageWriter.GetData();
	}
}
