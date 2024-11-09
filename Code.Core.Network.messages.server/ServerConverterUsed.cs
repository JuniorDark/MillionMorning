using System;

namespace Code.Core.Network.messages.server;

public class ServerConverterUsed : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 332;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerConverterUsed(reader);
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

	private const int OPCODE = 332;

	private string converterIdentifier;

	private string itemIdentifier;

	private ServerConverterUsed(MessageReader reader)
	{
		converterIdentifier = reader.ReadString();
		itemIdentifier = reader.ReadString();
	}

	public ServerConverterUsed(string converterIdentifier, string itemIdentifier)
	{
		this.converterIdentifier = converterIdentifier;
		this.itemIdentifier = itemIdentifier;
	}

	public string getConverterIdentifier()
	{
		return converterIdentifier;
	}

	public string getItemIdentifier()
	{
		return itemIdentifier;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(converterIdentifier);
		num += MessageWriter.GetSize(itemIdentifier);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(332);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(converterIdentifier);
		messageWriter.WriteString(itemIdentifier);
		return messageWriter.GetData();
	}
}
