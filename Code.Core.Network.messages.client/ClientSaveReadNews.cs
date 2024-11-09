namespace Code.Core.Network.messages.client;

public class ClientSaveReadNews : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 298;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientSaveReadNews(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 298;

	private int newsHashCode;

	private ClientSaveReadNews(MessageReader reader)
	{
		newsHashCode = reader.ReadInt32();
	}

	public ClientSaveReadNews(int newsHashCode)
	{
		this.newsHashCode = newsHashCode;
	}

	public int getNewsHashCode()
	{
		return newsHashCode;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(298);
		messageWriter.WriteInt32(newsHashCode);
		return messageWriter.GetData();
	}
}
