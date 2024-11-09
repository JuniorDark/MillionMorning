using System;

namespace Code.Core.Network.messages.server;

public class ServerPremiumTokenFound : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 198;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPremiumTokenFound(reader);
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

	private const int OPCODE = 198;

	private string playerId;

	private int value;

	private ServerPremiumTokenFound(MessageReader reader)
	{
		playerId = reader.ReadString();
		value = reader.ReadInt32();
	}

	public ServerPremiumTokenFound(string playerId, int value)
	{
		this.playerId = playerId;
		this.value = value;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public int getValue()
	{
		return value;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(198);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt32(value);
		return messageWriter.GetData();
	}
}
