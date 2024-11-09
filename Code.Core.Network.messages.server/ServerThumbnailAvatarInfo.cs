using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerThumbnailAvatarInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 125;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerThumbnailAvatarInfo(reader);
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

	private const int OPCODE = 125;

	private string userId;

	private Code.Core.Network.types.Avatar avatar;

	private ServerThumbnailAvatarInfo(MessageReader reader)
	{
		userId = reader.ReadString();
		avatar = new Code.Core.Network.types.Avatar(reader);
	}

	public ServerThumbnailAvatarInfo(string userId, Code.Core.Network.types.Avatar avatar)
	{
		this.userId = userId;
		this.avatar = avatar;
	}

	public string getUserId()
	{
		return userId;
	}

	public Code.Core.Network.types.Avatar getAvatar()
	{
		return avatar;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(userId);
		num += avatar.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(125);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(userId);
		avatar.Write(messageWriter);
		return messageWriter.GetData();
	}
}
