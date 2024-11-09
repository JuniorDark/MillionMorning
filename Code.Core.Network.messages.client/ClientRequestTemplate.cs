using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestTemplate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 28;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestTemplate(reader);
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

	private const int OPCODE = 28;

	private string category;

	private string path;

	private ClientRequestTemplate(MessageReader reader)
	{
		category = reader.ReadString();
		path = reader.ReadString();
	}

	public ClientRequestTemplate(string category, string path)
	{
		this.category = category;
		this.path = path;
	}

	public string getCategory()
	{
		return category;
	}

	public string getPath()
	{
		return path;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(category);
		num += MessageWriter.GetSize(path);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(28);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(category);
		messageWriter.WriteString(path);
		return messageWriter.GetData();
	}
}
