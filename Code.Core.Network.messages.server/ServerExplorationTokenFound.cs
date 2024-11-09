using System;

namespace Code.Core.Network.messages.server;

public class ServerExplorationTokenFound : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 161;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerExplorationTokenFound(reader);
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

	private const int OPCODE = 161;

	private readonly string _playerId;

	private readonly sbyte _tokenIndex;

	private readonly string _level;

	private ServerExplorationTokenFound(MessageReader reader)
	{
		_playerId = reader.ReadString();
		_tokenIndex = reader.ReadInt8();
		_level = reader.ReadString();
	}

	public ServerExplorationTokenFound(string playerId, sbyte tokenIndex, string level)
	{
		_playerId = playerId;
		_tokenIndex = tokenIndex;
		_level = level;
	}

	public string GetPlayerId()
	{
		return _playerId;
	}

	public sbyte GetTokenIndex()
	{
		return _tokenIndex;
	}

	public string GetLevel()
	{
		return _level;
	}

	public byte[] GetData()
	{
		int num = 9;
		num += MessageWriter.GetSize(_playerId);
		num += MessageWriter.GetSize(_level);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(161);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(_playerId);
		messageWriter.WriteInt8(_tokenIndex);
		messageWriter.WriteString(_level);
		return messageWriter.GetData();
	}
}
