namespace Code.Core.Network.messages.client.pvp;

public class ClientRequestEnterPvPQueue : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 431;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestEnterPvPQueue(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 431;

	private sbyte matchMode;

	private ClientRequestEnterPvPQueue(MessageReader reader)
	{
		matchMode = reader.ReadInt8();
	}

	public ClientRequestEnterPvPQueue(sbyte matchMode)
	{
		this.matchMode = matchMode;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(431);
		messageWriter.WriteInt8(matchMode);
		return messageWriter.GetData();
	}
}
