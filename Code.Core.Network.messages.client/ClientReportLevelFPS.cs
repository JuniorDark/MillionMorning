using System;

namespace Code.Core.Network.messages.client;

public class ClientReportLevelFPS : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 370;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientReportLevelFPS(reader);
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

	private const int OPCODE = 370;

	private string levelTemplateIdentifier;

	private float fps;

	private ClientReportLevelFPS(MessageReader reader)
	{
		levelTemplateIdentifier = reader.ReadString();
		fps = reader.ReadFloat();
	}

	public ClientReportLevelFPS(string levelTemplateIdentifier, float fps)
	{
		this.levelTemplateIdentifier = levelTemplateIdentifier;
		this.fps = fps;
	}

	public string getLevelTemplateIdentifier()
	{
		return levelTemplateIdentifier;
	}

	public float getFps()
	{
		return fps;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(levelTemplateIdentifier);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(370);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(levelTemplateIdentifier);
		messageWriter.WriteFloat(fps);
		return messageWriter.GetData();
	}
}
