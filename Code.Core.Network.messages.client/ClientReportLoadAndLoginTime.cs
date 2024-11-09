namespace Code.Core.Network.messages.client;

public class ClientReportLoadAndLoginTime : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 368;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientReportLoadAndLoginTime(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 368;

	private float totalTimeSeconds;

	private float loginTimeSeconds;

	private ClientReportLoadAndLoginTime(MessageReader reader)
	{
		totalTimeSeconds = reader.ReadFloat();
		loginTimeSeconds = reader.ReadFloat();
	}

	public ClientReportLoadAndLoginTime(float totalTimeSeconds, float loginTimeSeconds)
	{
		this.totalTimeSeconds = totalTimeSeconds;
		this.loginTimeSeconds = loginTimeSeconds;
	}

	public float getTotalTimeSeconds()
	{
		return totalTimeSeconds;
	}

	public float getLoginTimeSeconds()
	{
		return loginTimeSeconds;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(368);
		messageWriter.WriteFloat(totalTimeSeconds);
		messageWriter.WriteFloat(loginTimeSeconds);
		return messageWriter.GetData();
	}
}
