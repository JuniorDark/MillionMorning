namespace Code.Core.Network.messages.client;

public class ClientReportSessionFPSAndPing : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 369;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientReportSessionFPSAndPing(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 369;

	private float fps;

	private float ping;

	private ClientReportSessionFPSAndPing(MessageReader reader)
	{
		fps = reader.ReadFloat();
		ping = reader.ReadFloat();
	}

	public ClientReportSessionFPSAndPing(float fps, float ping)
	{
		this.fps = fps;
		this.ping = ping;
	}

	public float getFps()
	{
		return fps;
	}

	public float getPing()
	{
		return ping;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(369);
		messageWriter.WriteFloat(fps);
		messageWriter.WriteFloat(ping);
		return messageWriter.GetData();
	}
}
