using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerHealthUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 90;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerHealthUpdate(reader);
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

	private const int OPCODE = 90;

	private string PlayerID;

	private float Health;

	private float MaxHealth;

	private float ArmorDurability;

	private ServerPlayerHealthUpdate(MessageReader reader)
	{
		PlayerID = reader.ReadString();
		Health = reader.ReadFloat();
		MaxHealth = reader.ReadFloat();
		ArmorDurability = reader.ReadFloat();
	}

	public ServerPlayerHealthUpdate(string PlayerID, float Health, float MaxHealth, float ArmorDurability)
	{
		this.PlayerID = PlayerID;
		this.Health = Health;
		this.MaxHealth = MaxHealth;
		this.ArmorDurability = ArmorDurability;
	}

	public string getPlayerID()
	{
		return PlayerID;
	}

	public float getHealth()
	{
		return Health;
	}

	public float getMaxHealth()
	{
		return MaxHealth;
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
		messageWriter.WriteOpCode(90);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerID);
		messageWriter.WriteFloat(Health);
		messageWriter.WriteFloat(MaxHealth);
		messageWriter.WriteFloat(ArmorDurability);
		return messageWriter.GetData();
	}
}
