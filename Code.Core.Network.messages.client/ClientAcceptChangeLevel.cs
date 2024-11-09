namespace Code.Core.Network.messages.client;

public class ClientAcceptChangeLevel : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 13;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientAcceptChangeLevel(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 13;

	private int npcID;

	private int levelIndex;

	private ClientAcceptChangeLevel(MessageReader reader)
	{
		npcID = reader.ReadInt32();
		levelIndex = reader.ReadInt32();
	}

	public ClientAcceptChangeLevel(int npcID, int levelIndex)
	{
		this.npcID = npcID;
		this.levelIndex = levelIndex;
	}

	public int getNpcID()
	{
		return npcID;
	}

	public int getLevelIndex()
	{
		return levelIndex;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(13);
		messageWriter.WriteInt32(npcID);
		messageWriter.WriteInt32(levelIndex);
		return messageWriter.GetData();
	}
}
