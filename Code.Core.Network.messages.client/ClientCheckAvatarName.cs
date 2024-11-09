using System;

namespace Code.Core.Network.messages.client;

public class ClientCheckAvatarName : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 178;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientCheckAvatarName(reader);
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

	private const int OPCODE = 178;

	private string avatarName;

	private ClientCheckAvatarName(MessageReader reader)
	{
		avatarName = reader.ReadString();
	}

	public ClientCheckAvatarName(string avatarName)
	{
		this.avatarName = avatarName;
	}

	public string getAvatarName()
	{
		return avatarName;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(avatarName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(178);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(avatarName);
		return messageWriter.GetData();
	}
}
