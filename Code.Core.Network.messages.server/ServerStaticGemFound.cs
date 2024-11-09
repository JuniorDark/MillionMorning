namespace Code.Core.Network.messages.server;

public class ServerStaticGemFound : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 204;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerStaticGemFound(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 204;

	private sbyte gemIndex;

	private ServerStaticGemFound(MessageReader reader)
	{
		gemIndex = reader.ReadInt8();
	}

	public ServerStaticGemFound(sbyte gemIndex)
	{
		this.gemIndex = gemIndex;
	}

	public sbyte getGemIndex()
	{
		return gemIndex;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(204);
		messageWriter.WriteInt8(gemIndex);
		return messageWriter.GetData();
	}
}
