using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerItemWieldOK : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 31;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerItemWieldOK(reader);
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

	private const int OPCODE = 31;

	private string playerID;

	private Item item;

	private ServerItemWieldOK(MessageReader reader)
	{
		playerID = reader.ReadString();
		item = Item.Create(reader.ReadTypeCode(), reader);
	}

	public ServerItemWieldOK(string playerID, Item item)
	{
		this.playerID = playerID;
		this.item = item;
	}

	public string getPlayerID()
	{
		return playerID;
	}

	public Item getItem()
	{
		return item;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(playerID);
		num += item.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(31);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerID);
		messageWriter.WriteTypeCode(item.GetTypeId());
		item.Write(messageWriter);
		return messageWriter.GetData();
	}
}
