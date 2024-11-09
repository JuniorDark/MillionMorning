using System;

namespace Code.Core.Network.messages.server;

public class ServerUpdateMood : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 59;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateMood(reader);
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

	private const int OPCODE = 59;

	private string playerID;

	private string moodName;

	private ServerUpdateMood(MessageReader reader)
	{
		playerID = reader.ReadString();
		moodName = reader.ReadString();
	}

	public ServerUpdateMood(string playerID, string moodName)
	{
		this.playerID = playerID;
		this.moodName = moodName;
	}

	public string getPlayerID()
	{
		return playerID;
	}

	public string getMoodName()
	{
		return moodName;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(playerID);
		num += MessageWriter.GetSize(moodName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(59);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerID);
		messageWriter.WriteString(moodName);
		return messageWriter.GetData();
	}
}
