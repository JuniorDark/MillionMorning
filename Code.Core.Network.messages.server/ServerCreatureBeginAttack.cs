using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerCreatureBeginAttack : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 84;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreatureBeginAttack(reader);
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

	private const int OPCODE = 84;

	private int creatureID;

	private TemplateReference Attack;

	private ServerCreatureBeginAttack(MessageReader reader)
	{
		creatureID = reader.ReadInt32();
		Attack = new TemplateReference(reader);
	}

	public ServerCreatureBeginAttack(int creatureID, TemplateReference Attack)
	{
		this.creatureID = creatureID;
		this.Attack = Attack;
	}

	public int getCreatureID()
	{
		return creatureID;
	}

	public TemplateReference getAttack()
	{
		return Attack;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += Attack.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(84);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(creatureID);
		Attack.Write(messageWriter);
		return messageWriter.GetData();
	}
}
