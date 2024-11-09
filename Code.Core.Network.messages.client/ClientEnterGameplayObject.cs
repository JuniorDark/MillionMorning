namespace Code.Core.Network.messages.client;

public class ClientEnterGameplayObject : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 100;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientEnterGameplayObject(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 100;

	private int gameplayObjectId;

	private ClientEnterGameplayObject(MessageReader reader)
	{
		gameplayObjectId = reader.ReadInt32();
	}

	public ClientEnterGameplayObject(int gameplayObjectId)
	{
		this.gameplayObjectId = gameplayObjectId;
	}

	public int getGameplayObjectId()
	{
		return gameplayObjectId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(100);
		messageWriter.WriteInt32(gameplayObjectId);
		return messageWriter.GetData();
	}
}
