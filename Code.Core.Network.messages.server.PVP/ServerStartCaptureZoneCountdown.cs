namespace Code.Core.Network.messages.server.PVP;

public class ServerStartCaptureZoneCountdown : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 414;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerStartCaptureZoneCountdown(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 16;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 414;

	private int teamId;

	private int gampleyObjectId;

	private long captureTimeStamp;

	private ServerStartCaptureZoneCountdown(MessageReader reader)
	{
		teamId = reader.ReadInt32();
		gampleyObjectId = reader.ReadInt32();
		captureTimeStamp = reader.ReadInt64();
	}

	public ServerStartCaptureZoneCountdown(int teamId, int gampleyObjectId, long captureTimeStamp)
	{
		this.teamId = teamId;
		this.gampleyObjectId = gampleyObjectId;
		this.captureTimeStamp = captureTimeStamp;
	}

	public int getTeamId()
	{
		return teamId;
	}

	public int getGampleyObjectId()
	{
		return gampleyObjectId;
	}

	public long getCaptureTimeStamp()
	{
		return captureTimeStamp;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(18);
		messageWriter.WriteOpCode(414);
		messageWriter.WriteInt32(teamId);
		messageWriter.WriteInt32(gampleyObjectId);
		messageWriter.WriteInt64(captureTimeStamp);
		return messageWriter.GetData();
	}
}
