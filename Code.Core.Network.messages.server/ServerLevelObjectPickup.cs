using System;

namespace Code.Core.Network.messages.server;

public class ServerLevelObjectPickup : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 65;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLevelObjectPickup(reader);
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

	private const int OPCODE = 65;

	private int itemID;

	private string playerID;

	private ServerLevelObjectPickup(MessageReader reader)
	{
		itemID = reader.ReadInt32();
		playerID = reader.ReadString();
	}

	public ServerLevelObjectPickup(int itemID, string playerID)
	{
		this.itemID = itemID;
		this.playerID = playerID;
	}

	public int getItemID()
	{
		return itemID;
	}

	public string getPlayerID()
	{
		return playerID;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(playerID);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(65);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(itemID);
		messageWriter.WriteString(playerID);
		return messageWriter.GetData();
	}
}
