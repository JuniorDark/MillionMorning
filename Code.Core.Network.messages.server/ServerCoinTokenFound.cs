using System;

namespace Code.Core.Network.messages.server;

public class ServerCoinTokenFound : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 163;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCoinTokenFound(reader);
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

	private const int OPCODE = 163;

	private string playerId;

	private sbyte tokenIndex;

	private ServerCoinTokenFound(MessageReader reader)
	{
		playerId = reader.ReadString();
		tokenIndex = reader.ReadInt8();
	}

	public ServerCoinTokenFound(string playerId, sbyte tokenIndex)
	{
		this.playerId = playerId;
		this.tokenIndex = tokenIndex;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public sbyte getTokenIndex()
	{
		return tokenIndex;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(163);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt8(tokenIndex);
		return messageWriter.GetData();
	}
}
