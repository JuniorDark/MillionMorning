using System;

namespace Code.Core.Network.messages.server;

public class ServerNewNotification : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 210;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerNewNotification(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 2;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt16();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 210;

	private int notificationId;

	private string notificationType;

	private string notificationCustomData;

	private ServerNewNotification(MessageReader reader)
	{
		notificationId = reader.ReadInt32();
		notificationType = reader.ReadString();
		notificationCustomData = reader.ReadString();
	}

	public ServerNewNotification(int notificationId, string notificationType, string notificationCustomData)
	{
		this.notificationId = notificationId;
		this.notificationType = notificationType;
		this.notificationCustomData = notificationCustomData;
	}

	public int getNotificationId()
	{
		return notificationId;
	}

	public string getNotificationType()
	{
		return notificationType;
	}

	public string getNotificationCustomData()
	{
		return notificationCustomData;
	}

	public byte[] GetData()
	{
		int num = 12;
		num += MessageWriter.GetSize(notificationType);
		num += MessageWriter.GetSize(notificationCustomData);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(210);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(notificationId);
		messageWriter.WriteString(notificationType);
		messageWriter.WriteString(notificationCustomData);
		return messageWriter.GetData();
	}
}
