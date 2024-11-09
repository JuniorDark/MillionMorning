using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientActivateSkillItem : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 437;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientActivateSkillItem(reader);
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

	private const int OPCODE = 437;

	private int SkillInventoryId;

	private AttackTarget currentTarget;

	private vector3 position;

	private vector3 rotation;

	private ClientActivateSkillItem(MessageReader reader)
	{
		SkillInventoryId = reader.ReadInt32();
		if (reader.ReadInt8() == 1)
		{
			currentTarget = AttackTarget.Create(reader.ReadTypeCode(), reader);
		}
		position = new vector3(reader);
		rotation = new vector3(reader);
	}

	public ClientActivateSkillItem(int SkillInventoryId, AttackTarget currentTarget, vector3 position, vector3 rotation)
	{
		this.SkillInventoryId = SkillInventoryId;
		this.currentTarget = currentTarget;
		this.position = position;
		this.rotation = rotation;
	}

	public int getSkillInventoryId()
	{
		return SkillInventoryId;
	}

	public AttackTarget getCurrentTarget()
	{
		return currentTarget;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public vector3 getRotation()
	{
		return rotation;
	}

	public byte[] GetData()
	{
		int num = 33;
		if (currentTarget != null)
		{
			num += 2;
			num += currentTarget.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(437);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(SkillInventoryId);
		if (currentTarget == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			messageWriter.WriteTypeCode(currentTarget.GetTypeId());
			currentTarget.Write(messageWriter);
		}
		position.Write(messageWriter);
		rotation.Write(messageWriter);
		return messageWriter.GetData();
	}
}
