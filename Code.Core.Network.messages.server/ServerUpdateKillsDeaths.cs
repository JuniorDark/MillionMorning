namespace Code.Core.Network.messages.server;

public class ServerUpdateKillsDeaths : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 318;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateKillsDeaths(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 318;

	private int kills;

	private int deaths;

	private ServerUpdateKillsDeaths(MessageReader reader)
	{
		kills = reader.ReadInt32();
		deaths = reader.ReadInt32();
	}

	public ServerUpdateKillsDeaths(int kills, int deaths)
	{
		this.kills = kills;
		this.deaths = deaths;
	}

	public int getKills()
	{
		return kills;
	}

	public int getDeaths()
	{
		return deaths;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(318);
		messageWriter.WriteInt32(kills);
		messageWriter.WriteInt32(deaths);
		return messageWriter.GetData();
	}
}
