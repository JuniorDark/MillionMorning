using System;

namespace Code.Core.Network.messages.server;

public class ServerUpdateEmote : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 57;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateEmote(reader);
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

	private const int OPCODE = 57;

	private string playerID;

	private string emoteName;

	private ServerUpdateEmote(MessageReader reader)
	{
		playerID = reader.ReadString();
		emoteName = reader.ReadString();
	}

	public ServerUpdateEmote(string playerID, string emoteName)
	{
		this.playerID = playerID;
		this.emoteName = emoteName;
	}

	public string getPlayerID()
	{
		return playerID;
	}

	public string getEmoteName()
	{
		return emoteName;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(playerID);
		num += MessageWriter.GetSize(emoteName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(57);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerID);
		messageWriter.WriteString(emoteName);
		return messageWriter.GetData();
	}
}
