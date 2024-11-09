using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestThumbnailAvatarInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 122;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestThumbnailAvatarInfo(reader);
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

	private const int OPCODE = 122;

	private string userId;

	private ClientRequestThumbnailAvatarInfo(MessageReader reader)
	{
		userId = reader.ReadString();
	}

	public ClientRequestThumbnailAvatarInfo(string userId)
	{
		this.userId = userId;
	}

	public string getUserId()
	{
		return userId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(userId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(122);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(userId);
		return messageWriter.GetData();
	}
}
