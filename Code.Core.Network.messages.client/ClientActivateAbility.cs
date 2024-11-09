using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientActivateAbility : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 107;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientActivateAbility(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 28;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 107;

	private int AbilityInventoryId;

	private vector3 position;

	private vector3 rotation;

	private ClientActivateAbility(MessageReader reader)
	{
		AbilityInventoryId = reader.ReadInt32();
		position = new vector3(reader);
		rotation = new vector3(reader);
	}

	public ClientActivateAbility(int AbilityInventoryId, vector3 position, vector3 rotation)
	{
		this.AbilityInventoryId = AbilityInventoryId;
		this.position = position;
		this.rotation = rotation;
	}

	public int getAbilityInventoryId()
	{
		return AbilityInventoryId;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public vector3 getRotation()
	{
		return rotation;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(30);
		messageWriter.WriteOpCode(107);
		messageWriter.WriteInt32(AbilityInventoryId);
		position.Write(messageWriter);
		rotation.Write(messageWriter);
		return messageWriter.GetData();
	}
}
