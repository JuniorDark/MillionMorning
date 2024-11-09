using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientCastVote : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 378;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientCastVote(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 6;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 378;

	private VoteData voteData;

	private ClientCastVote(MessageReader reader)
	{
		voteData = new VoteData(reader);
	}

	public ClientCastVote(VoteData voteData)
	{
		this.voteData = voteData;
	}

	public VoteData getVoteData()
	{
		return voteData;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(8);
		messageWriter.WriteOpCode(378);
		voteData.Write(messageWriter);
		return messageWriter.GetData();
	}
}
