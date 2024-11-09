using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientRequestUpdateAvatar : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 219;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestUpdateAvatar(reader);
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

	private const int OPCODE = 219;

	private Code.Core.Network.types.Avatar avatar;

	private ClientRequestUpdateAvatar(MessageReader reader)
	{
		avatar = new Code.Core.Network.types.Avatar(reader);
	}

	public ClientRequestUpdateAvatar(Code.Core.Network.types.Avatar avatar)
	{
		this.avatar = avatar;
	}

	public Code.Core.Network.types.Avatar getAvatar()
	{
		return avatar;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += avatar.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(219);
		messageWriter.WriteInt16((short)(num - 4));
		avatar.Write(messageWriter);
		return messageWriter.GetData();
	}
}
