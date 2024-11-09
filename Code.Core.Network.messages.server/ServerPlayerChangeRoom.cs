using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerPlayerChangeRoom : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 166;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerChangeRoom(reader);
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

	private const int OPCODE = 166;

	private string playerId;

	private string room;

	private vector3 position;

	private float rotation;

	private ServerPlayerChangeRoom(MessageReader reader)
	{
		playerId = reader.ReadString();
		room = reader.ReadString();
		position = new vector3(reader);
		rotation = reader.ReadFloat();
	}

	public ServerPlayerChangeRoom(string playerId, string room, vector3 position, float rotation)
	{
		this.playerId = playerId;
		this.room = room;
		this.position = position;
		this.rotation = rotation;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public string getRoom()
	{
		return room;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public float getRotation()
	{
		return rotation;
	}

	public byte[] GetData()
	{
		int num = 24;
		num += MessageWriter.GetSize(playerId);
		num += MessageWriter.GetSize(room);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(166);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteString(room);
		position.Write(messageWriter);
		messageWriter.WriteFloat(rotation);
		return messageWriter.GetData();
	}
}
