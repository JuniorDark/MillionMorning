namespace Code.Core.Network.messages.server;

public class ServerDeactivateAbility : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 108;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerDeactivateAbility(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 108;

	private int AbilityInventoryId;

	private ServerDeactivateAbility(MessageReader reader)
	{
		AbilityInventoryId = reader.ReadInt32();
	}

	public ServerDeactivateAbility(int AbilityInventoryId)
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
		messageWriter.WriteOpCode(108);
		messageWriter.WriteInt32(AbilityInventoryId);
		return messageWriter.GetData();
	}
}
