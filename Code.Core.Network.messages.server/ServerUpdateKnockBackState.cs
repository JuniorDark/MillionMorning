namespace Code.Core.Network.messages.server;

public class ServerUpdateKnockBackState : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 311;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateKnockBackState(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 5;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 311;

	private int playerID;

	private sbyte knockBackState;

	private ServerUpdateKnockBackState(MessageReader reader)
	{
		playerID = reader.ReadInt32();
		knockBackState = reader.ReadInt8();
	}

	public ServerUpdateKnockBackState(int playerID, sbyte knockBackState)
	{
		this.playerID = playerID;
		this.knockBackState = knockBackState;
	}

	public int getPlayerID()
	{
		return playerID;
	}

	public sbyte getKnockBackState()
	{
		return knockBackState;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(7);
		messageWriter.WriteOpCode(311);
		messageWriter.WriteInt32(playerID);
		messageWriter.WriteInt8(knockBackState);
		return messageWriter.GetData();
	}
}
