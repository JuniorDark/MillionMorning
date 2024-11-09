using System;

namespace Code.Core.Network.messages.client;

public class ClientHomeBoxPosition : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 293;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientHomeBoxPosition(reader);
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

	private const int OPCODE = 293;

	private long inRoom;

	private string gridCell;

	private float rotation;

	private sbyte inStorage;

	private ClientHomeBoxPosition(MessageReader reader)
	{
		inRoom = reader.ReadInt64();
		gridCell = reader.ReadString();
		rotation = reader.ReadFloat();
		inStorage = reader.ReadInt8();
	}

	public ClientHomeBoxPosition(long inRoom, string gridCell, float rotation, sbyte inStorage)
	{
		this.inRoom = inRoom;
		this.gridCell = gridCell;
		this.rotation = rotation;
		this.inStorage = inStorage;
	}

	public long getInRoom()
	{
		return inRoom;
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

	public byte[] GetData()
	{
		int num = 19;
		num += MessageWriter.GetSize(gridCell);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(293);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt64(inRoom);
		messageWriter.WriteString(gridCell);
		messageWriter.WriteFloat(rotation);
		messageWriter.WriteInt8(inStorage);
		return messageWriter.GetData();
	}
}
