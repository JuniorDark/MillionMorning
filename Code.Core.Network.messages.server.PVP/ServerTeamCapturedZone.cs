namespace Code.Core.Network.messages.server.PVP;

public class ServerTeamCapturedZone : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 416;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerTeamCapturedZone(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 416;

	private int teamId;

	private int gampleyObjectId;

	private ServerTeamCapturedZone(MessageReader reader)
	{
		teamId = reader.ReadInt32();
		gampleyObjectId = reader.ReadInt32();
	}

	public ServerTeamCapturedZone(int teamId, int gampleyObjectId)
	{
		this.teamId = teamId;
		this.gampleyObjectId = gampleyObjectId;
	}

	public int getTeamId()
	{
		return teamId;
	}

	public int getGampleyObjectId()
	{
		return gampleyObjectId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(416);
		messageWriter.WriteInt32(teamId);
		messageWriter.WriteInt32(gampleyObjectId);
		return messageWriter.GetData();
	}
}
