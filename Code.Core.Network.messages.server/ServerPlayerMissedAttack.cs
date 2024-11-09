using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerMissedAttack : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 113;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerMissedAttack(reader);
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

	private const int OPCODE = 113;

	private string PlayerId;

	private int TargetCreatureId;

	private ServerPlayerMissedAttack(MessageReader reader)
	{
		PlayerId = reader.ReadString();
		TargetCreatureId = reader.ReadInt32();
	}

	public ServerPlayerMissedAttack(string PlayerId, int TargetCreatureId)
	{
		this.PlayerId = PlayerId;
		this.TargetCreatureId = TargetCreatureId;
	}

	public string getPlayerId()
	{
		return PlayerId;
	}

	public int getTargetCreatureId()
	{
		return TargetCreatureId;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(PlayerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(113);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerId);
		messageWriter.WriteInt32(TargetCreatureId);
		return messageWriter.GetData();
	}
}
