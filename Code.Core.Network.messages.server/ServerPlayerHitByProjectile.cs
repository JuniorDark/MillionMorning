using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerHitByProjectile : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 88;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerHitByProjectile(reader);
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

	private const int OPCODE = 88;

	private string PlayerID;

	private int ProjectileID;

	private float Health;

	private float ArmorDurability;

	private ServerPlayerHitByProjectile(MessageReader reader)
	{
		PlayerID = reader.ReadString();
		ProjectileID = reader.ReadInt32();
		Health = reader.ReadFloat();
		ArmorDurability = reader.ReadFloat();
	}

	public ServerPlayerHitByProjectile(string PlayerID, int ProjectileID, float Health, float ArmorDurability)
	{
		this.PlayerID = PlayerID;
		this.ProjectileID = ProjectileID;
		this.Health = Health;
		this.ArmorDurability = ArmorDurability;
	}

	public string getPlayerID()
	{
		return PlayerID;
	}

	public int getProjectileID()
	{
		return ProjectileID;
	}

	public float getHealth()
	{
		return Health;
	}

	public float getArmorDurability()
	{
		return ArmorDurability;
	}

	public byte[] GetData()
	{
		int num = 18;
		num += MessageWriter.GetSize(PlayerID);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(88);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerID);
		messageWriter.WriteInt32(ProjectileID);
		messageWriter.WriteFloat(Health);
		messageWriter.WriteFloat(ArmorDurability);
		return messageWriter.GetData();
	}
}
