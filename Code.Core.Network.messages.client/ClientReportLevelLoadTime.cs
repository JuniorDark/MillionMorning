using System;

namespace Code.Core.Network.messages.client;

public class ClientReportLevelLoadTime : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 371;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientReportLevelLoadTime(reader);
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

	private const int OPCODE = 371;

	private string levelTemplateIdentifier;

	private float loadTime;

	private ClientReportLevelLoadTime(MessageReader reader)
	{
		levelTemplateIdentifier = reader.ReadString();
		loadTime = reader.ReadFloat();
	}

	public ClientReportLevelLoadTime(string levelTemplateIdentifier, float loadTime)
	{
		this.levelTemplateIdentifier = levelTemplateIdentifier;
		this.loadTime = loadTime;
	}

	public string getLevelTemplateIdentifier()
	{
		return levelTemplateIdentifier;
	}

	public float getLoadTime()
	{
		return loadTime;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(levelTemplateIdentifier);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(371);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(levelTemplateIdentifier);
		messageWriter.WriteFloat(loadTime);
		return messageWriter.GetData();
	}
}
