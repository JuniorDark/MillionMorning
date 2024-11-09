using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerHitAttackOnPlayer : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 116;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerHitAttackOnPlayer(reader);
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

	private const int OPCODE = 116;

	private string attackerId;

	private string targetId;

	private float damage;

	private sbyte killingBlow;

	private sbyte usingWeapon;

	private string weaponTemplate;

	private ServerPlayerHitAttackOnPlayer(MessageReader reader)
	{
		attackerId = reader.ReadString();
		targetId = reader.ReadString();
		damage = reader.ReadFloat();
		killingBlow = reader.ReadInt8();
		usingWeapon = reader.ReadInt8();
		weaponTemplate = reader.ReadString();
	}

	public ServerPlayerHitAttackOnPlayer(string attackerId, string targetId, float damage, sbyte killingBlow, sbyte usingWeapon, string weaponTemplate)
	{
		this.attackerId = attackerId;
		this.targetId = targetId;
		this.damage = damage;
		this.killingBlow = killingBlow;
		this.usingWeapon = usingWeapon;
		this.weaponTemplate = weaponTemplate;
	}

	public string getAttackerId()
	{
		return attackerId;
	}

	public string getTargetId()
	{
		return targetId;
	}

	public float getDamage()
	{
		return damage;
	}

	public sbyte getKillingBlow()
	{
		return killingBlow;
	}

	public sbyte getUsingWeapon()
	{
		return usingWeapon;
	}

	public string getWeaponTemplate()
	{
		return weaponTemplate;
	}

	public byte[] GetData()
	{
		int num = 16;
		num += MessageWriter.GetSize(attackerId);
		num += MessageWriter.GetSize(targetId);
		num += MessageWriter.GetSize(weaponTemplate);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(116);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(attackerId);
		messageWriter.WriteString(targetId);
		messageWriter.WriteFloat(damage);
		messageWriter.WriteInt8(killingBlow);
		messageWriter.WriteInt8(usingWeapon);
		messageWriter.WriteString(weaponTemplate);
		return messageWriter.GetData();
	}
}
