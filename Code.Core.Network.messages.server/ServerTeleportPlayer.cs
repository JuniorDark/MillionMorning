using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerTeleportPlayer : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 157;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerTeleportPlayer(reader);
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

	private const int OPCODE = 157;

	private string playerId;

	private vector3 position;

	private float rotation;

	private ServerTeleportPlayer(MessageReader reader)
	{
		playerId = reader.ReadString();
		position = new vector3(reader);
		rotation = reader.ReadFloat();
	}

	public ServerTeleportPlayer(string playerId, vector3 position, float rotation)
	{
		this.playerId = playerId;
		this.position = position;
		this.rotation = rotation;
	}

	public string getPlayerId()
	{
		return playerId;
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
		int num = 22;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(157);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		position.Write(messageWriter);
		messageWriter.WriteFloat(rotation);
		return messageWriter.GetData();
	}
}
