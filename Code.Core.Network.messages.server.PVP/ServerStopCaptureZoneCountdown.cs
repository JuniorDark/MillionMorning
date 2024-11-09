namespace Code.Core.Network.messages.server.PVP;

public class ServerStopCaptureZoneCountdown : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 415;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerStopCaptureZoneCountdown(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 415;

	private int gampleyObjectId;

	private ServerStopCaptureZoneCountdown(MessageReader reader)
	{
		gampleyObjectId = reader.ReadInt32();
	}

	public ServerStopCaptureZoneCountdown(int gampleyObjectId)
	{
		this.gampleyObjectId = gampleyObjectId;
	}

	public int getGampleyObjectId()
	{
		return gampleyObjectId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(415);
		messageWriter.WriteInt32(gampleyObjectId);
		return messageWriter.GetData();
	}
}
