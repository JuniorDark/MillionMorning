using System;

namespace Code.Core.Network.messages.server.PVP;

public class ServerPvPOkToJoin : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 418;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPvPOkToJoin(reader);
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

	private const int OPCODE = 418;

	private string levelToken;

	public string LevelToken => levelToken;

	private ServerPvPOkToJoin(MessageReader reader)
	{
		levelToken = reader.ReadString();
	}

	public ServerPvPOkToJoin(string levelToken)
	{
		this.levelToken = levelToken;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(levelToken);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(418);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(levelToken);
		return messageWriter.GetData();
	}
}
