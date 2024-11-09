namespace Code.Core.Network.messages.server;

public class ServerGroupInviteResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 358;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGroupInviteResponse(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 358;

	private sbyte succeeded;

	private ServerGroupInviteResponse(MessageReader reader)
	{
		succeeded = reader.ReadInt8();
	}

	public ServerGroupInviteResponse(sbyte succeeded)
	{
		this.succeeded = succeeded;
	}

	public sbyte getSucceeded()
	{
		return succeeded;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(358);
		messageWriter.WriteInt8(succeeded);
		return messageWriter.GetData();
	}
}
