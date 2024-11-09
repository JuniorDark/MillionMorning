using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientUseConsumable : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 148;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUseConsumable(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 28;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 148;

	private int inventoryId;

	private vector3 position;

	private vector3 rotation;

	private ClientUseConsumable(MessageReader reader)
	{
		inventoryId = reader.ReadInt32();
		position = new vector3(reader);
		rotation = new vector3(reader);
	}

	public ClientUseConsumable(int inventoryId, vector3 position, vector3 rotation)
	{
		this.inventoryId = inventoryId;
		this.position = position;
		this.rotation = rotation;
	}

	public int getInventoryId()
	{
		return inventoryId;
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
		messageWriter.WriteOpCode(148);
		messageWriter.WriteInt32(inventoryId);
		position.Write(messageWriter);
		rotation.Write(messageWriter);
		return messageWriter.GetData();
	}
}
