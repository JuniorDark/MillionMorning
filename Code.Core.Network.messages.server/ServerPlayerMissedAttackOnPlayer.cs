using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerMissedAttackOnPlayer : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 114;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerMissedAttackOnPlayer(reader);
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

	private const int OPCODE = 114;

	private string attackerId;

	private string targetId;

	private ServerPlayerMissedAttackOnPlayer(MessageReader reader)
	{
		attackerId = reader.ReadString();
		targetId = reader.ReadString();
	}

	public ServerPlayerMissedAttackOnPlayer(string attackerId, string targetId)
	{
		this.attackerId = attackerId;
		this.targetId = targetId;
	}

	public string getAttackerId()
	{
		return attackerId;
	}

	public string getTargetId()
	{
		return targetId;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(attackerId);
		num += MessageWriter.GetSize(targetId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(114);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(attackerId);
		messageWriter.WriteString(targetId);
		return messageWriter.GetData();
	}
}
