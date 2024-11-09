using System;

namespace Code.Core.Network.messages.client;

public class ClientGroupKick : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 351;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientGroupKick(reader);
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

	private const int OPCODE = 351;

	private string toKick;

	private ClientGroupKick(MessageReader reader)
	{
		toKick = reader.ReadString();
	}

	public ClientGroupKick(string toKick)
	{
		this.toKick = toKick;
	}

	public string getToKick()
	{
		return toKick;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(toKick);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(351);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(toKick);
		return messageWriter.GetData();
	}
}
