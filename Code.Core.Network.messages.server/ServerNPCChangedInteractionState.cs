namespace Code.Core.Network.messages.server;

public class ServerNPCChangedInteractionState : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 292;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerNPCChangedInteractionState(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 5;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 292;

	private int npcId;

	private sbyte newState;

	private ServerNPCChangedInteractionState(MessageReader reader)
	{
		npcId = reader.ReadInt32();
		newState = reader.ReadInt8();
	}

	public ServerNPCChangedInteractionState(int npcId, sbyte newState)
	{
		this.npcId = npcId;
		this.newState = newState;
	}

	public int getNpcId()
	{
		return npcId;
	}

	public sbyte getNewState()
	{
		return newState;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(7);
		messageWriter.WriteOpCode(292);
		messageWriter.WriteInt32(npcId);
		messageWriter.WriteInt8(newState);
		return messageWriter.GetData();
	}

	public int GetOPCode()
	{
		return 292;
	}

	public IMessageFactory GetFactory()
	{
		return new Factory();
	}
}
