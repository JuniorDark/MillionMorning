namespace Code.Core.Network.messages.server;

public class ServerSteamTransactionError : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 405;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerSteamTransactionError(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 405;

	private sbyte reason;

	private ServerSteamTransactionError(MessageReader reader)
	{
		reason = reader.ReadInt8();
	}

	public ServerSteamTransactionError(sbyte reason)
	{
		this.reason = reason;
	}

	public sbyte getReson()
	{
		return reason;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(405);
		messageWriter.WriteInt8(reason);
		return messageWriter.GetData();
	}
}
