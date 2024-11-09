using System;

namespace Code.Core.Network.messages.server;

public class ServerGroupKicked : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 361;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGroupKicked(reader);
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

	private const int OPCODE = 361;

	private string player;

	private ServerGroupKicked(MessageReader reader)
	{
		player = reader.ReadString();
	}

	public ServerGroupKicked(string player)
	{
		this.player = player;
	}

	public string getPlayer()
	{
		return player;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(player);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(361);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(player);
		return messageWriter.GetData();
	}
}
