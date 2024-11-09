namespace Code.Core.Network.messages.client;

public class ClientActivateGameplayTrigger : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 155;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientActivateGameplayTrigger(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 155;

	private int gameplayObjectId;

	private int triggerId;

	private ClientActivateGameplayTrigger(MessageReader reader)
	{
		gameplayObjectId = reader.ReadInt32();
		triggerId = reader.ReadInt32();
	}

	public ClientActivateGameplayTrigger(int gameplayObjectId, int triggerId)
	{
		this.gameplayObjectId = gameplayObjectId;
		this.triggerId = triggerId;
	}

	public int getGameplayObjectId()
	{
		return gameplayObjectId;
	}

	public int getTriggerId()
	{
		return triggerId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(155);
		messageWriter.WriteInt32(gameplayObjectId);
		messageWriter.WriteInt32(triggerId);
		return messageWriter.GetData();
	}
}
