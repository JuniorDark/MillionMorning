namespace Code.Core.Network.messages.client;

public class ClientUpdateKnockBackState : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 310;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUpdateKnockBackState(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 310;

	private sbyte knockBackState;

	private ClientUpdateKnockBackState(MessageReader reader)
	{
		knockBackState = reader.ReadInt8();
	}

	public ClientUpdateKnockBackState(sbyte knockBackState)
	{
		this.knockBackState = knockBackState;
	}

	public sbyte getKnockBackState()
	{
		return knockBackState;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(310);
		messageWriter.WriteInt8(knockBackState);
		return messageWriter.GetData();
	}
}
