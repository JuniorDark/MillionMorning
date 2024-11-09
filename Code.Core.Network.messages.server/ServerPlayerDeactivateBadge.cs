using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerDeactivateBadge : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 196;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerDeactivateBadge(reader);
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

	private const int OPCODE = 196;

	private string playerID;

	private ServerPlayerDeactivateBadge(MessageReader reader)
	{
		playerID = reader.ReadString();
	}

	public ServerPlayerDeactivateBadge(string playerID)
	{
		this.playerID = playerID;
	}

	public string getPlayerID()
	{
		return playerID;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(playerID);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(196);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerID);
		return messageWriter.GetData();
	}
}
