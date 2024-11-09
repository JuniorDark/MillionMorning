using System;

namespace Code.Core.Network.messages.server;

public class ServerLadderPositionFor : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 320;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLadderPositionFor(reader);
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

	private const int OPCODE = 320;

	private string playerId;

	private int rank;

	private ServerLadderPositionFor(MessageReader reader)
	{
		playerId = reader.ReadString();
		rank = reader.ReadInt32();
	}

	public ServerLadderPositionFor(string playerId, int rank)
	{
		this.playerId = playerId;
		this.rank = rank;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public int getRank()
	{
		return rank;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(320);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt32(rank);
		return messageWriter.GetData();
	}
}
