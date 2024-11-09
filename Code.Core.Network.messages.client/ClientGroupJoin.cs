using System;

namespace Code.Core.Network.messages.client;

public class ClientGroupJoin : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 349;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientGroupJoin(reader);
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

	private const int OPCODE = 349;

	private string inviter;

	private sbyte accepted;

	private ClientGroupJoin(MessageReader reader)
	{
		inviter = reader.ReadString();
		accepted = reader.ReadInt8();
	}

	public ClientGroupJoin(string inviter, sbyte accepted)
	{
		this.inviter = inviter;
		this.accepted = accepted;
	}

	public string getInviter()
	{
		return inviter;
	}

	public sbyte getAccepted()
	{
		return accepted;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(inviter);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(349);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(inviter);
		messageWriter.WriteInt8(accepted);
		return messageWriter.GetData();
	}
}
