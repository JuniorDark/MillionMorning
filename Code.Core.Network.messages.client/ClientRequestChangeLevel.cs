using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestChangeLevel : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 46;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestChangeLevel(reader);
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

	private const int OPCODE = 46;

	private string worldName;

	private string levelName;

	private string language;

	private ClientRequestChangeLevel(MessageReader reader)
	{
		worldName = reader.ReadString();
		levelName = reader.ReadString();
		language = reader.ReadString();
	}

	public ClientRequestChangeLevel(string worldName, string levelName, string language)
	{
		this.worldName = worldName;
		this.levelName = levelName;
		this.language = language;
	}

	public string getWorldName()
	{
		return worldName;
	}

	public string getLevelName()
	{
		return levelName;
	}

	public string getLanguage()
	{
		return language;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(worldName);
		num += MessageWriter.GetSize(levelName);
		num += MessageWriter.GetSize(language);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(46);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(worldName);
		messageWriter.WriteString(levelName);
		messageWriter.WriteString(language);
		return messageWriter.GetData();
	}
}
