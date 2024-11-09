namespace Code.Core.Network.messages.client;

public class ClientUntargetedAttack : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 119;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUntargetedAttack(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 119;

	private int WeaponInventoryId;

	private ClientUntargetedAttack(MessageReader reader)
	{
		WeaponInventoryId = reader.ReadInt32();
	}

	public ClientUntargetedAttack(int WeaponInventoryId)
	{
		this.WeaponInventoryId = WeaponInventoryId;
	}

	public int getWeaponInventoryId()
	{
		return WeaponInventoryId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(119);
		messageWriter.WriteInt32(WeaponInventoryId);
		return messageWriter.GetData();
	}
}
