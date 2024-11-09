using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientRequestCreateAvatar : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 25;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestCreateAvatar(reader);
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

	private const int OPCODE = 25;

	private Code.Core.Network.types.Avatar avatar;

	private string token;

	private ClientRequestCreateAvatar(MessageReader reader)
	{
		avatar = new Code.Core.Network.types.Avatar(reader);
		token = reader.ReadString();
	}

	public ClientRequestCreateAvatar(Code.Core.Network.types.Avatar avatar, string token)
	{
		this.avatar = avatar;
		this.token = token;
	}

	public Code.Core.Network.types.Avatar getAvatar()
	{
		return avatar;
	}

	public string getToken()
	{
		return token;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += avatar.Size();
		num += MessageWriter.GetSize(token);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(25);
		messageWriter.WriteInt16((short)(num - 4));
		avatar.Write(messageWriter);
		messageWriter.WriteString(token);
		return messageWriter.GetData();
	}
}
