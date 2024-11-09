namespace Code.Core.Network.messages.client;

public class ClientNotificationRead : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 209;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientNotificationRead(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 209;

	private int notificationId;

	private ClientNotificationRead(MessageReader reader)
	{
		notificationId = reader.ReadInt32();
	}

	public ClientNotificationRead(int notificationId)
	{
		this.notificationId = notificationId;
	}

	public int getNotificationId()
	{
		return notificationId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(209);
		messageWriter.WriteInt32(notificationId);
		return messageWriter.GetData();
	}
}
