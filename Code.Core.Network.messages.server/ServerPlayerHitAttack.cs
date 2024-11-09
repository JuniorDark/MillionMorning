using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerHitAttack : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 115;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerHitAttack(reader);
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

	private const int OPCODE = 115;

	private string playerId;

	private int targetCreatureId;

	private float damage;

	private sbyte killingBlow;

	private sbyte usingWeapon;

	private ServerPlayerHitAttack(MessageReader reader)
	{
		playerId = reader.ReadString();
		targetCreatureId = reader.ReadInt32();
		damage = reader.ReadFloat();
		killingBlow = reader.ReadInt8();
		usingWeapon = reader.ReadInt8();
	}

	public ServerPlayerHitAttack(string playerId, int targetCreatureId, float damage, sbyte killingBlow, sbyte usingWeapon)
	{
		this.playerId = playerId;
		this.targetCreatureId = targetCreatureId;
		this.damage = damage;
		this.killingBlow = killingBlow;
		this.usingWeapon = usingWeapon;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public int getTargetCreatureId()
	{
		return targetCreatureId;
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

	public byte[] GetData()
	{
		int num = 16;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(115);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt32(targetCreatureId);
		messageWriter.WriteFloat(damage);
		messageWriter.WriteInt8(killingBlow);
		messageWriter.WriteInt8(usingWeapon);
		return messageWriter.GetData();
	}
}
