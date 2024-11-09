using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerLevelInstanceInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 274;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLevelInstanceInfo(reader);
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

	private const int OPCODE = 274;

	private string worldName;

	private string levelName;

	private string language;

	private string instanceId;

	private vector3 entryPoint;

	private ServerLevelInstanceInfo(MessageReader reader)
	{
		worldName = reader.ReadString();
		levelName = reader.ReadString();
		language = reader.ReadString();
		instanceId = reader.ReadString();
		entryPoint = new vector3(reader);
	}

	public ServerLevelInstanceInfo(string worldName, string levelName, string language, string instanceId, vector3 entryPoint)
	{
		this.worldName = worldName;
		this.levelName = levelName;
		this.language = language;
		this.instanceId = instanceId;
		this.entryPoint = entryPoint;
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

	public vector3 getEntryPoint()
	{
		return entryPoint;
	}

	public byte[] GetData()
	{
		int num = 24;
		num += MessageWriter.GetSize(worldName);
		num += MessageWriter.GetSize(levelName);
		num += MessageWriter.GetSize(language);
		num += MessageWriter.GetSize(instanceId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(274);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(worldName);
		messageWriter.WriteString(levelName);
		messageWriter.WriteString(language);
		messageWriter.WriteString(instanceId);
		entryPoint.Write(messageWriter);
		return messageWriter.GetData();
	}
}
