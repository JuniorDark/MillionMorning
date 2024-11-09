namespace Code.Core.Network.messages.server;

public class ServerEnterHomeFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 243;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerEnterHomeFail(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 243;

	private sbyte failCode;

	private ServerEnterHomeFail(MessageReader reader)
	{
		failCode = reader.ReadInt8();
	}

	public ServerEnterHomeFail(sbyte failCode)
	{
		this.failCode = failCode;
	}

	public sbyte getFailCode()
	{
		return failCode;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(243);
		messageWriter.WriteInt8(failCode);
		return messageWriter.GetData();
	}
}
