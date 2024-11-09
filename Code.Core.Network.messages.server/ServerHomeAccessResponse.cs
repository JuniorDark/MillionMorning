namespace Code.Core.Network.messages.server;

public class ServerHomeAccessResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 397;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerHomeAccessResponse(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 397;

	private sbyte result;

	private ServerHomeAccessResponse(MessageReader reader)
	{
		result = reader.ReadInt8();
	}

	public ServerHomeAccessResponse(sbyte result)
	{
		this.result = result;
	}

	public sbyte getResult()
	{
		return result;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(397);
		messageWriter.WriteInt8(result);
		return messageWriter.GetData();
	}
}
