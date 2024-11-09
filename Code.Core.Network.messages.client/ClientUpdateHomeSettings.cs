using System;

namespace Code.Core.Network.messages.client;

public class ClientUpdateHomeSettings : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 386;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUpdateHomeSettings(reader);
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

	private const int OPCODE = 386;

	private string name;

	private sbyte accessLevel;

	private ClientUpdateHomeSettings(MessageReader reader)
	{
		name = reader.ReadString();
		accessLevel = reader.ReadInt8();
	}

	public ClientUpdateHomeSettings(string name, sbyte accessLevel)
	{
		this.name = name;
		this.accessLevel = accessLevel;
	}

	public string getName()
	{
		return name;
	}

	public sbyte getAccessLevel()
	{
		return accessLevel;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(name);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(386);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(name);
		messageWriter.WriteInt8(accessLevel);
		return messageWriter.GetData();
	}
}
