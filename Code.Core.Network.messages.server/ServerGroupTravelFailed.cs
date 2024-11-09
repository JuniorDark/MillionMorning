namespace Code.Core.Network.messages.server;

public class ServerGroupTravelFailed : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 365;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGroupTravelFailed(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 365;

	private sbyte response;

	private ServerGroupTravelFailed(MessageReader reader)
	{
		response = reader.ReadInt8();
	}

	public ServerGroupTravelFailed(sbyte response)
	{
		this.response = response;
	}

	public sbyte getResponse()
	{
		return response;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(365);
		messageWriter.WriteInt8(response);
		return messageWriter.GetData();
	}
}
