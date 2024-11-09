using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerCreatureAttackInstant : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 85;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreatureAttackInstant(reader);
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

	private const int OPCODE = 85;

	private string PlayerID;

	private int CreatureID;

	private sbyte IsHit;

	private float HealthDamage;

	private float ArmorDamage;

	private float HealthLeft;

	private TemplateReference Attack;

	private ServerCreatureAttackInstant(MessageReader reader)
	{
		PlayerID = reader.ReadString();
		CreatureID = reader.ReadInt32();
		IsHit = reader.ReadInt8();
		HealthDamage = reader.ReadFloat();
		ArmorDamage = reader.ReadFloat();
		HealthLeft = reader.ReadFloat();
		Attack = new TemplateReference(reader);
	}

	public ServerCreatureAttackInstant(string PlayerID, int CreatureID, sbyte IsHit, float HealthDamage, float ArmorDamage, float HealthLeft, TemplateReference Attack)
	{
		this.PlayerID = PlayerID;
		this.CreatureID = CreatureID;
		this.IsHit = IsHit;
		this.HealthDamage = HealthDamage;
		this.ArmorDamage = ArmorDamage;
		this.HealthLeft = HealthLeft;
		this.Attack = Attack;
	}

	public string getPlayerID()
	{
		return PlayerID;
	}

	public int getCreatureID()
	{
		return CreatureID;
	}

	public sbyte getIsHit()
	{
		return IsHit;
	}

	public float getHealthDamage()
	{
		return HealthDamage;
	}

	public float getArmorDamage()
	{
		return ArmorDamage;
	}

	public float getHealthLeft()
	{
		return HealthLeft;
	}

	public TemplateReference getAttack()
	{
		return Attack;
	}

	public byte[] GetData()
	{
		int num = 23;
		num += MessageWriter.GetSize(PlayerID);
		num += Attack.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(85);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerID);
		messageWriter.WriteInt32(CreatureID);
		messageWriter.WriteInt8(IsHit);
		messageWriter.WriteFloat(HealthDamage);
		messageWriter.WriteFloat(ArmorDamage);
		messageWriter.WriteFloat(HealthLeft);
		Attack.Write(messageWriter);
		return messageWriter.GetData();
	}
}
