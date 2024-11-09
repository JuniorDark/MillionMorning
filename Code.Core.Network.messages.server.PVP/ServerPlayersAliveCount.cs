namespace Code.Core.Network.messages.server.PVP;

public class ServerPlayersAliveCount : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 422;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayersAliveCount(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 422;

	private int playerCount;

	public int PlayerCount => playerCount;

	private ServerPlayersAliveCount(MessageReader reader)
	{
		playerCount = reader.ReadInt32();
	}

	public ServerPlayersAliveCount(int playerCount)
	{
		this.playerCount = playerCount;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(422);
		messageWriter.WriteInt32(playerCount);
		return messageWriter.GetData();
	}
}
