namespace Code.Core.Network.messages.server;

public class ServerHubResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 289;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerHubResponse(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 289;

	private sbyte hubResult;

	private ServerHubResponse(MessageReader reader)
	{
		hubResult = reader.ReadInt8();
	}

	public ServerHubResponse(sbyte hubResult)
	{
		this.hubResult = hubResult;
	}

	public sbyte getHubResult()
	{
		return hubResult;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(289);
		messageWriter.WriteInt8(hubResult);
		return messageWriter.GetData();
	}
}
