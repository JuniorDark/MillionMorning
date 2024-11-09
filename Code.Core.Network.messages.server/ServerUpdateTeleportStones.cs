namespace Code.Core.Network.messages.server;

public class ServerUpdateTeleportStones : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 165;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateTeleportStones(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 165;

	private int teleportStoneCount;

	private ServerUpdateTeleportStones(MessageReader reader)
	{
		teleportStoneCount = reader.ReadInt32();
	}

	public ServerUpdateTeleportStones(int teleportStoneCount)
	{
		this.teleportStoneCount = teleportStoneCount;
	}

	public int getTeleportStoneCount()
	{
		return teleportStoneCount;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(165);
		messageWriter.WriteInt32(teleportStoneCount);
		return messageWriter.GetData();
	}
}
