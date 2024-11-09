namespace Code.Core.Network.messages.server;

public class ServerMemberUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 188;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerMemberUpdate(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 188;

	private int membershipTimeLeftInDays;

	private int gemBonusTimeLeftInSeconds;

	private ServerMemberUpdate(MessageReader reader)
	{
		membershipTimeLeftInDays = reader.ReadInt32();
		gemBonusTimeLeftInSeconds = reader.ReadInt32();
	}

	public ServerMemberUpdate(int membershipTimeLeftInDays, int gemBonusTimeLeftInSeconds)
	{
		this.membershipTimeLeftInDays = membershipTimeLeftInDays;
		this.gemBonusTimeLeftInSeconds = gemBonusTimeLeftInSeconds;
	}

	public int getMembershipTimeLeftInDays()
	{
		return membershipTimeLeftInDays;
	}

	public int getGemBonusTimeLeftInSeconds()
	{
		return gemBonusTimeLeftInSeconds;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(188);
		messageWriter.WriteInt32(membershipTimeLeftInDays);
		messageWriter.WriteInt32(gemBonusTimeLeftInSeconds);
		return messageWriter.GetData();
	}
}
