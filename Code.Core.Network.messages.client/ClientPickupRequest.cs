using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientPickupRequest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 11;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientPickupRequest(reader);
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

	private const int OPCODE = 11;

	private vector3 position;

	private IList<int> itemIDs;

	private ClientPickupRequest(MessageReader reader)
	{
		position = new vector3(reader);
		itemIDs = new List<int>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			itemIDs.Add(reader.ReadInt32());
		}
	}

	public ClientPickupRequest(vector3 position, IList<int> itemIDs)
	{
		this.position = position;
		this.itemIDs = itemIDs;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public IList<int> getItemIDs()
	{
		return itemIDs;
	}

	public byte[] GetData()
	{
		int num = 18;
		num += (short)(itemIDs.Count * 4);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(11);
		messageWriter.WriteInt16((short)(num - 4));
		position.Write(messageWriter);
		messageWriter.WriteInt16((short)itemIDs.Count);
		foreach (int itemID in itemIDs)
		{
			messageWriter.WriteInt32(itemID);
		}
		return messageWriter.GetData();
	}
}
