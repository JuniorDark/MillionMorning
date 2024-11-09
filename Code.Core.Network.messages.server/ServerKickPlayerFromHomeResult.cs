namespace Code.Core.Network.messages.server;

public class ServerKickPlayerFromHomeResult : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 260;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerKickPlayerFromHomeResult(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 260;

	private sbyte result;

	private ServerKickPlayerFromHomeResult(MessageReader reader)
	{
		result = reader.ReadInt8();
	}

	public ServerKickPlayerFromHomeResult(sbyte result)
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
		messageWriter.WriteOpCode(260);
		messageWriter.WriteInt8(result);
		return messageWriter.GetData();
	}
}
