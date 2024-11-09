using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientRequestSpawnLevelProjectile : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 328;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestSpawnLevelProjectile(reader);
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

	private const int OPCODE = 328;

	private int weaponId;

	private vector3 startPosition;

	private AttackTarget target;

	private float collisionDistance;

	private ClientRequestSpawnLevelProjectile(MessageReader reader)
	{
		weaponId = reader.ReadInt32();
		startPosition = new vector3(reader);
		if (reader.ReadInt8() == 1)
		{
			target = AttackTarget.Create(reader.ReadTypeCode(), reader);
		}
		collisionDistance = reader.ReadFloat();
	}

	public ClientRequestSpawnLevelProjectile(int weaponId, vector3 startPosition, AttackTarget target, float collisionDistance)
	{
		this.weaponId = weaponId;
		this.startPosition = startPosition;
		this.target = target;
		this.collisionDistance = collisionDistance;
	}

	public int getWeaponId()
	{
		return weaponId;
	}

	public vector3 getStartPosition()
	{
		return startPosition;
	}

	public AttackTarget getTarget()
	{
		return target;
	}

	public float getCollisionDistance()
	{
		return collisionDistance;
	}

	public byte[] GetData()
	{
		int num = 25;
		if (target != null)
		{
			num += 2;
			num += target.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(328);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(weaponId);
		startPosition.Write(messageWriter);
		if (target == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			messageWriter.WriteTypeCode(target.GetTypeId());
			target.Write(messageWriter);
		}
		messageWriter.WriteFloat(collisionDistance);
		return messageWriter.GetData();
	}
}
