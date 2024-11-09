using System;

namespace Code.Core.Network.messages.server.PVP;

public class ServerPlayerDied : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 421;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerDied(reader);
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

	private const int OPCODE = 421;

	private string playerId;

	public string PlayerId => playerId;

	private ServerPlayerDied(MessageReader reader)
	{
		playerId = reader.ReadString();
	}

	public ServerPlayerDied(string playerId)
	{
		this.playerId = playerId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(421);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		return messageWriter.GetData();
	}
}
