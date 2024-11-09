namespace Code.Core.Network.messages.server;

public class ServerEnterCharBuilderResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 216;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerEnterCharBuilderResponse(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 216;

	private sbyte enterResult;

	private ServerEnterCharBuilderResponse(MessageReader reader)
	{
		enterResult = reader.ReadInt8();
	}

	public ServerEnterCharBuilderResponse(sbyte enterResult)
	{
		this.enterResult = enterResult;
	}

	public sbyte getEnterResult()
	{
		return enterResult;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(216);
		messageWriter.WriteInt8(enterResult);
		return messageWriter.GetData();
	}
}
