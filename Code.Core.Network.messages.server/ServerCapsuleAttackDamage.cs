using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerCapsuleAttackDamage : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 87;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCapsuleAttackDamage(reader);
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

	private const int OPCODE = 87;

	private IList<DamageToPlayer> playersHit;

	private vector3 startPos;

	private vector3 endPos;

	private float playerImpulseY;

	private float playerImpulseXZ;

	private ServerCapsuleAttackDamage(MessageReader reader)
	{
		playersHit = new List<DamageToPlayer>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			playersHit.Add(new DamageToPlayer(reader));
		}
		startPos = new vector3(reader);
		endPos = new vector3(reader);
		playerImpulseY = reader.ReadFloat();
		playerImpulseXZ = reader.ReadFloat();
	}

	public ServerCapsuleAttackDamage(IList<DamageToPlayer> playersHit, vector3 startPos, vector3 endPos, float playerImpulseY, float playerImpulseXZ)
	{
		this.playersHit = playersHit;
		this.startPos = startPos;
		this.endPos = endPos;
		this.playerImpulseY = playerImpulseY;
		this.playerImpulseXZ = playerImpulseXZ;
	}

	public IList<DamageToPlayer> getPlayersHit()
	{
		return playersHit;
	}

	public vector3 getStartPos()
	{
		return startPos;
	}

	public vector3 getEndPos()
	{
		return endPos;
	}

	public float getPlayerImpulseY()
	{
		return playerImpulseY;
	}

	public float getPlayerImpulseXZ()
	{
		return playerImpulseXZ;
	}

	public byte[] GetData()
	{
		int num = 38;
		foreach (DamageToPlayer item in playersHit)
		{
			num += item.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(87);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)playersHit.Count);
		foreach (DamageToPlayer item2 in playersHit)
		{
			item2.Write(messageWriter);
		}
		startPos.Write(messageWriter);
		endPos.Write(messageWriter);
		messageWriter.WriteFloat(playerImpulseY);
		messageWriter.WriteFloat(playerImpulseXZ);
		return messageWriter.GetData();
	}
}
