using System;

namespace Code.Core.Network.messages.client;

public class ClientInviteRequest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 104;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientInviteRequest(reader);
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

	private const int OPCODE = 104;

	private string email;

	private ClientInviteRequest(MessageReader reader)
	{
		email = reader.ReadString();
	}

	public ClientInviteRequest(string email)
	{
		this.email = email;
	}

	public string getEmail()
	{
		return email;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(email);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(104);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(email);
		return messageWriter.GetData();
	}
}
