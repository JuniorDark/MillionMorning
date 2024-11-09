using System;

namespace Code.Core.Network.messages.server;

public class ServerGroupInvite : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 357;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGroupInvite(reader);
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

	private const int OPCODE = 357;

	private string inviter;

	private ServerGroupInvite(MessageReader reader)
	{
		inviter = reader.ReadString();
	}

	public ServerGroupInvite(string inviter)
	{
		this.inviter = inviter;
	}

	public string getInviter()
	{
		return inviter;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(inviter);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(357);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(inviter);
		return messageWriter.GetData();
	}
}
