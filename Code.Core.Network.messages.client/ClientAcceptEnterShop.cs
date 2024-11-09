namespace Code.Core.Network.messages.client;

public class ClientAcceptEnterShop : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 15;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientAcceptEnterShop(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 15;

	private int npcID;

	private ClientAcceptEnterShop(MessageReader reader)
	{
		npcID = reader.ReadInt32();
	}

	public ClientAcceptEnterShop(int npcID)
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
		messageWriter.WriteOpCode(15);
		messageWriter.WriteInt32(npcID);
		return messageWriter.GetData();
	}
}
