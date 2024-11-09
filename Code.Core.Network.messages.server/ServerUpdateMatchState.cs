namespace Code.Core.Network.messages.server;

public class ServerUpdateMatchState : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 314;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateMatchState(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 5;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 314;

	private sbyte matchState;

	private int secondsToNextMatchState;

	private ServerUpdateMatchState(MessageReader reader)
	{
		matchState = reader.ReadInt8();
		secondsToNextMatchState = reader.ReadInt32();
	}

	public ServerUpdateMatchState(sbyte matchState, int secondsToNextMatchState)
	{
		this.matchState = matchState;
		this.secondsToNextMatchState = secondsToNextMatchState;
	}

	public sbyte getMatchState()
	{
		return matchState;
	}

	public int getSecondsToNextMatchState()
	{
		return secondsToNextMatchState;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(7);
		messageWriter.WriteOpCode(314);
		messageWriter.WriteInt8(matchState);
		messageWriter.WriteInt32(secondsToNextMatchState);
		return messageWriter.GetData();
	}
}
