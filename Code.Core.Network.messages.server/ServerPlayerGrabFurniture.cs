using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerGrabFurniture : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 255;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerGrabFurniture(reader);
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

	private const int OPCODE = 255;

	private string playerId;

	private long furnitureId;

	private string playerTile;

	private ServerPlayerGrabFurniture(MessageReader reader)
	{
		playerId = reader.ReadString();
		furnitureId = reader.ReadInt64();
		playerTile = reader.ReadString();
	}

	public ServerPlayerGrabFurniture(string playerId, long furnitureId, string playerTile)
	{
		this.playerId = playerId;
		this.furnitureId = furnitureId;
		this.playerTile = playerTile;
	}

	public string getPlayerId()
	{
		return playerId;
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
		int num = 16;
		num += MessageWriter.GetSize(playerId);
		num += MessageWriter.GetSize(playerTile);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(255);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt64(furnitureId);
		messageWriter.WriteString(playerTile);
		return messageWriter.GetData();
	}
}
