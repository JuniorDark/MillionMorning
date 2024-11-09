using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerCreaturePrepareAttack : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 83;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreaturePrepareAttack(reader);
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

	private const int OPCODE = 83;

	private int creatureID;

	private string targetPlayerID;

	private TemplateReference Attack;

	private ServerCreaturePrepareAttack(MessageReader reader)
	{
		creatureID = reader.ReadInt32();
		targetPlayerID = reader.ReadString();
		Attack = new TemplateReference(reader);
	}

	public ServerCreaturePrepareAttack(int creatureID, string targetPlayerID, TemplateReference Attack)
	{
		this.creatureID = creatureID;
		this.targetPlayerID = targetPlayerID;
		this.Attack = Attack;
	}

	public int getCreatureID()
	{
		return creatureID;
	}

	public string getTargetPlayerID()
	{
		return targetPlayerID;
	}

	public TemplateReference getAttack()
	{
		return Attack;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(targetPlayerID);
		num += Attack.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(83);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(creatureID);
		messageWriter.WriteString(targetPlayerID);
		Attack.Write(messageWriter);
		return messageWriter.GetData();
	}
}
