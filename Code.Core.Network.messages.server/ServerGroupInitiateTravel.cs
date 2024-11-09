using System;

namespace Code.Core.Network.messages.server;

public class ServerGroupInitiateTravel : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 363;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGroupInitiateTravel(reader);
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

	private const int OPCODE = 363;

	private string player;

	private string level;

	private ServerGroupInitiateTravel(MessageReader reader)
	{
		player = reader.ReadString();
		level = reader.ReadString();
	}

	public ServerGroupInitiateTravel(string player, string level)
	{
		this.player = player;
		this.level = level;
	}

	public string getPlayer()
	{
		return player;
	}

	public string getLevel()
	{
		return level;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(player);
		num += MessageWriter.GetSize(level);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(363);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(player);
		messageWriter.WriteString(level);
		return messageWriter.GetData();
	}
}
