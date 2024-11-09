using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerPlayerRespawn : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 93;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerRespawn(reader);
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

	private const int OPCODE = 93;

	private string PlayerID;

	private vector3 Position;

	private float Rotation;

	private float Health;

	private sbyte isRespawnInvulnerable;

	private ServerPlayerRespawn(MessageReader reader)
	{
		PlayerID = reader.ReadString();
		Position = new vector3(reader);
		Rotation = reader.ReadFloat();
		Health = reader.ReadFloat();
		isRespawnInvulnerable = reader.ReadInt8();
	}

	public ServerPlayerRespawn(string PlayerID, vector3 Position, float Rotation, float Health, sbyte isRespawnInvulnerable)
	{
		this.PlayerID = PlayerID;
		this.Position = Position;
		this.Rotation = Rotation;
		this.Health = Health;
		this.isRespawnInvulnerable = isRespawnInvulnerable;
	}

	public string getPlayerID()
	{
		return PlayerID;
	}

	public vector3 getPosition()
	{
		return Position;
	}

	public float getRotation()
	{
		return Rotation;
	}

	public float getHealth()
	{
		return Health;
	}

	public sbyte getIsRespawnInvulnerable()
	{
		return isRespawnInvulnerable;
	}

	public byte[] GetData()
	{
		int num = 27;
		num += MessageWriter.GetSize(PlayerID);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(93);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerID);
		Position.Write(messageWriter);
		messageWriter.WriteFloat(Rotation);
		messageWriter.WriteFloat(Health);
		messageWriter.WriteInt8(isRespawnInvulnerable);
		return messageWriter.GetData();
	}
}
