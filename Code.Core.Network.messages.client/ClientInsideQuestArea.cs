using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientInsideQuestArea : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 172;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientInsideQuestArea(reader);
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

	private const int OPCODE = 172;

	private string questAreaName;

	private vector3 playerPosition;

	private ClientInsideQuestArea(MessageReader reader)
	{
		questAreaName = reader.ReadString();
		playerPosition = new vector3(reader);
	}

	public ClientInsideQuestArea(string questAreaName, vector3 playerPosition)
	{
		this.questAreaName = questAreaName;
		this.playerPosition = playerPosition;
	}

	public string getQuestAreaName()
	{
		return questAreaName;
	}

	public vector3 getPlayerPosition()
	{
		return playerPosition;
	}

	public byte[] GetData()
	{
		int num = 18;
		num += MessageWriter.GetSize(questAreaName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(172);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(questAreaName);
		playerPosition.Write(messageWriter);
		return messageWriter.GetData();
	}
}
