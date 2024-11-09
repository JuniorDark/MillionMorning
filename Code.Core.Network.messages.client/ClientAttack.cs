using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientAttack : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 112;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientAttack(reader);
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

	private const int OPCODE = 112;

	private int WeaponInventoryId;

	private AttackTarget target;

	private ClientAttack(MessageReader reader)
	{
		WeaponInventoryId = reader.ReadInt32();
		target = AttackTarget.Create(reader.ReadTypeCode(), reader);
	}

	public ClientAttack(int WeaponInventoryId, AttackTarget target)
	{
		this.WeaponInventoryId = WeaponInventoryId;
		this.target = target;
	}

	public int getWeaponInventoryId()
	{
		return WeaponInventoryId;
	}

	public AttackTarget getTarget()
	{
		return target;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += target.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(112);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(WeaponInventoryId);
		messageWriter.WriteTypeCode(target.GetTypeId());
		target.Write(messageWriter);
		return messageWriter.GetData();
	}
}
