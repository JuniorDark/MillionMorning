using System;

namespace Code.Core.Network.messages.server;

public class ServerLoginInfoLevel : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 47;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLoginInfoLevel(reader);
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

	private const int OPCODE = 47;

	private string host;

	private string token;

	private string worldName;

	private string levelName;

	private string language;

	private string instanceId;

	private ServerLoginInfoLevel(MessageReader reader)
	{
		host = reader.ReadString();
		token = reader.ReadString();
		worldName = reader.ReadString();
		levelName = reader.ReadString();
		language = reader.ReadString();
		instanceId = reader.ReadString();
	}

	public ServerLoginInfoLevel(string host, string token, string worldName, string levelName, string language, string instanceId)
	{
		this.host = host;
		this.token = token;
		this.worldName = worldName;
		this.levelName = levelName;
		this.language = language;
		this.instanceId = instanceId;
	}

	public string getHost()
	{
		return host;
	}

	public string getToken()
	{
		return token;
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

	public string getInstanceId()
	{
		return instanceId;
	}

	public byte[] GetData()
	{
		int num = 16;
		num += MessageWriter.GetSize(host);
		num += MessageWriter.GetSize(token);
		num += MessageWriter.GetSize(worldName);
		num += MessageWriter.GetSize(levelName);
		num += MessageWriter.GetSize(language);
		num += MessageWriter.GetSize(instanceId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(47);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(host);
		messageWriter.WriteString(token);
		messageWriter.WriteString(worldName);
		messageWriter.WriteString(levelName);
		messageWriter.WriteString(language);
		messageWriter.WriteString(instanceId);
		return messageWriter.GetData();
	}
}
