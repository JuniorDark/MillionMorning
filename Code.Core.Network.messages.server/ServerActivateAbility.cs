namespace Code.Core.Network.messages.server;

public class ServerActivateAbility : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 106;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerActivateAbility(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 106;

	private int AbilityInventoryId;

	private ServerActivateAbility(MessageReader reader)
	{
		AbilityInventoryId = reader.ReadInt32();
	}

	public ServerActivateAbility(int AbilityInventoryId)
	{
		this.AbilityInventoryId = AbilityInventoryId;
	}

	public int getAbilityInventoryId()
	{
		return AbilityInventoryId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(106);
		messageWriter.WriteInt32(AbilityInventoryId);
		return messageWriter.GetData();
	}
}
