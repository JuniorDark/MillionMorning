using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerUntargetedAttack : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 118;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerUntargetedAttack(reader);
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

	private const int OPCODE = 118;

	private string PlayerId;

	private ServerPlayerUntargetedAttack(MessageReader reader)
	{
		PlayerId = reader.ReadString();
	}

	public ServerPlayerUntargetedAttack(string PlayerId)
	{
		this.PlayerId = PlayerId;
	}

	public string getPlayerId()
	{
		return PlayerId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(PlayerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(118);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerId);
		return messageWriter.GetData();
	}
}
