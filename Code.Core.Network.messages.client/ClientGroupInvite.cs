using System;

namespace Code.Core.Network.messages.client;

public class ClientGroupInvite : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 348;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientGroupInvite(reader);
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

	private const int OPCODE = 348;

	private string invitee;

	private ClientGroupInvite(MessageReader reader)
	{
		invitee = reader.ReadString();
	}

	public ClientGroupInvite(string invitee)
	{
		this.invitee = invitee;
	}

	public string getInvitee()
	{
		return invitee;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(invitee);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(348);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(invitee);
		return messageWriter.GetData();
	}
}
