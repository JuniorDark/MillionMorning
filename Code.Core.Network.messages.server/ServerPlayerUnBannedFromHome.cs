using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerUnBannedFromHome : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 395;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerUnBannedFromHome(reader);
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

	private const int OPCODE = 395;

	private string unbannedPlayer;

	private ServerPlayerUnBannedFromHome(MessageReader reader)
	{
		unbannedPlayer = reader.ReadString();
	}

	public ServerPlayerUnBannedFromHome(string unbannedPlayer)
	{
		this.unbannedPlayer = unbannedPlayer;
	}

	public string getUnbannedPlayer()
	{
		return unbannedPlayer;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(unbannedPlayer);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(395);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(unbannedPlayer);
		return messageWriter.GetData();
	}
}
