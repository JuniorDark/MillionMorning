using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestMoveHomeEquipment : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 250;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestMoveHomeEquipment(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 2;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt16();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 250;

	private long id;

	private string gridCell;

	private float rotation;

	private sbyte inStorage;

	private long inRoom;

	private ClientRequestMoveHomeEquipment(MessageReader reader)
	{
		id = reader.ReadInt64();
		gridCell = reader.ReadString();
		rotation = reader.ReadFloat();
		inStorage = reader.ReadInt8();
		inRoom = reader.ReadInt64();
	}

	public ClientRequestMoveHomeEquipment(long id, string gridCell, float rotation, sbyte inStorage, long inRoom)
	{
		this.id = id;
		this.gridCell = gridCell;
		this.rotation = rotation;
		this.inStorage = inStorage;
		this.inRoom = inRoom;
	}

	public long getId()
	{
		return id;
	}

	public string getGridCell()
	{
		return gridCell;
	}

	public float getRotation()
	{
		return rotation;
	}

	public sbyte getInStorage()
	{
		return inStorage;
	}

	public long getInRoom()
	{
		return inRoom;
	}

	public byte[] GetData()
	{
		int num = 27;
		num += MessageWriter.GetSize(gridCell);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(250);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt64(id);
		messageWriter.WriteString(gridCell);
		messageWriter.WriteFloat(rotation);
		messageWriter.WriteInt8(inStorage);
		messageWriter.WriteInt64(inRoom);
		return messageWriter.GetData();
	}
}
