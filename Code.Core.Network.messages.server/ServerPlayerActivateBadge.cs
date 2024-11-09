using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerActivateBadge : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 195;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerActivateBadge(reader);
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

	private const int OPCODE = 195;

	private string playerID;

	private ServerPlayerActivateBadge(MessageReader reader)
	{
		playerID = reader.ReadString();
	}

	public ServerPlayerActivateBadge(string playerID)
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
		messageWriter.WriteOpCode(195);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerID);
		return messageWriter.GetData();
	}
}
