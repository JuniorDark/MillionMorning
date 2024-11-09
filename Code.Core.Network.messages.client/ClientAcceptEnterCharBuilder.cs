namespace Code.Core.Network.messages.client;

public class ClientAcceptEnterCharBuilder : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 214;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientAcceptEnterCharBuilder(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 214;

	private int npcID;

	private ClientAcceptEnterCharBuilder(MessageReader reader)
	{
		npcID = reader.ReadInt32();
	}

	public ClientAcceptEnterCharBuilder(int npcID)
	{
		this.npcID = npcID;
	}

	public int getNpcID()
	{
		return npcID;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(214);
		messageWriter.WriteInt32(npcID);
		return messageWriter.GetData();
	}
}
