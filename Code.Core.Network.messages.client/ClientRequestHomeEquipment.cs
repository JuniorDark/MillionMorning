using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestHomeEquipment : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 244;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestHomeEquipment(reader);
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

	private const int OPCODE = 244;

	private string homeOwnerId;

	private long roomId;

	private ClientRequestHomeEquipment(MessageReader reader)
	{
		homeOwnerId = reader.ReadString();
		roomId = reader.ReadInt64();
	}

	public ClientRequestHomeEquipment(string homeOwnerId, long roomId)
	{
		this.homeOwnerId = homeOwnerId;
		this.roomId = roomId;
	}

	public string getHomeOwnerId()
	{
		return homeOwnerId;
	}

	public long getRoomId()
	{
		return roomId;
	}

	public byte[] GetData()
	{
		int num = 14;
		num += MessageWriter.GetSize(homeOwnerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(244);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(homeOwnerId);
		messageWriter.WriteInt64(roomId);
		return messageWriter.GetData();
	}
}
