using System;

namespace Code.Core.Network.messages.client;

public class ClientGrabFurniture : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 254;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientGrabFurniture(reader);
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

	private const int OPCODE = 254;

	private long furnitureId;

	private string playerTile;

	private ClientGrabFurniture(MessageReader reader)
	{
		furnitureId = reader.ReadInt64();
		playerTile = reader.ReadString();
	}

	public ClientGrabFurniture(long furnitureId, string playerTile)
	{
		this.furnitureId = furnitureId;
		this.playerTile = playerTile;
	}

	public long getFurnitureId()
	{
		return furnitureId;
	}

	public string getPlayerTile()
	{
		return playerTile;
	}

	public byte[] GetData()
	{
		int num = 14;
		num += MessageWriter.GetSize(playerTile);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(254);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt64(furnitureId);
		messageWriter.WriteString(playerTile);
		return messageWriter.GetData();
	}
}
