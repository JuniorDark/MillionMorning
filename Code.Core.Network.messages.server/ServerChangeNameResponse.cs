using System;

namespace Code.Core.Network.messages.server;

public class ServerChangeNameResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 334;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerChangeNameResponse(reader);
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

	private const int OPCODE = 334;

	private string newName;

	private sbyte result;

	private ServerChangeNameResponse(MessageReader reader)
	{
		newName = reader.ReadString();
		result = reader.ReadInt8();
	}

	public ServerChangeNameResponse(string newName, sbyte result)
	{
		this.newName = newName;
		this.result = result;
	}

	public string getNewName()
	{
		return newName;
	}

	public sbyte getResult()
	{
		return result;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(newName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(334);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(newName);
		messageWriter.WriteInt8(result);
		return messageWriter.GetData();
	}
}
