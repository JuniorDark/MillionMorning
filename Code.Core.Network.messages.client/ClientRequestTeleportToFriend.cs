namespace Code.Core.Network.messages.client;

public class ClientRequestTeleportToFriend : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 264;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestTeleportToFriend(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 264;

	private int playerId;

	private ClientRequestTeleportToFriend(MessageReader reader)
	{
		playerId = reader.ReadInt32();
	}

	public ClientRequestTeleportToFriend(int playerId)
	{
		this.playerId = playerId;
	}

	public int getPlayerId()
	{
		return playerId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(264);
		messageWriter.WriteInt32(playerId);
		return messageWriter.GetData();
	}
}
