using System;

namespace Code.Core.Network.messages.server;

public class ServerCreatureAggro : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 146;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreatureAggro(reader);
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

	private const int OPCODE = 146;

	private int creatureId;

	private string playerId;

	private ServerCreatureAggro(MessageReader reader)
	{
		creatureId = reader.ReadInt32();
		playerId = reader.ReadString();
	}

	public ServerCreatureAggro(int creatureId, string playerId)
	{
		this.creatureId = creatureId;
		this.playerId = playerId;
	}

	public int getCreatureId()
	{
		return creatureId;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(146);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(creatureId);
		messageWriter.WriteString(playerId);
		return messageWriter.GetData();
	}
}
