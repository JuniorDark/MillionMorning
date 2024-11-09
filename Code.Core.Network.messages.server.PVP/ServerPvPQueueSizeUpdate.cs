namespace Code.Core.Network.messages.server.PVP;

public class ServerPvPQueueSizeUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 411;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPvPQueueSizeUpdate(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = QueueInfo.size();
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 411;

	private QueueInfo queueInfo;

	public QueueInfo QueueInfo => queueInfo;

	private ServerPvPQueueSizeUpdate(MessageReader reader)
	{
		queueInfo = new QueueInfo(reader);
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2 + QueueInfo.size());
		messageWriter.WriteOpCode(411);
		queueInfo.write(messageWriter);
		return messageWriter.GetData();
	}
}
