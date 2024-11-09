using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerCreatureAttackImpact : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 86;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreatureAttackImpact(reader);
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

	private const int OPCODE = 86;

	private int creatureID;

	private TemplateReference attack;

	private IList<DamageToPlayer> playersHit;

	private ServerCreatureAttackImpact(MessageReader reader)
	{
		creatureID = reader.ReadInt32();
		attack = new TemplateReference(reader);
		playersHit = new List<DamageToPlayer>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			playersHit.Add(new DamageToPlayer(reader));
		}
	}

	public ServerCreatureAttackImpact(int creatureID, TemplateReference attack, IList<DamageToPlayer> playersHit)
	{
		this.creatureID = creatureID;
		this.attack = attack;
		this.playersHit = playersHit;
	}

	public int getCreatureID()
	{
		return creatureID;
	}

	public TemplateReference getAttack()
	{
		return attack;
	}

	public IList<DamageToPlayer> getPlayersHit()
	{
		return playersHit;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += attack.Size();
		foreach (DamageToPlayer item in playersHit)
		{
			num += item.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(86);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(creatureID);
		attack.Write(messageWriter);
		messageWriter.WriteInt16((short)playersHit.Count);
		foreach (DamageToPlayer item2 in playersHit)
		{
			item2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}