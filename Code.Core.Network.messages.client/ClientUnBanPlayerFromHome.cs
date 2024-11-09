using System;

namespace Code.Core.Network.messages.client;

public class ClientUnBanPlayerFromHome : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 394;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUnBanPlayerFromHome(reader);
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

	private const int OPCODE = 394;

	private string playerToUnBan;

	private ClientUnBanPlayerFromHome(MessageReader reader)
	{
		playerToUnBan = reader.ReadString();
	}

	public ClientUnBanPlayerFromHome(string playerToUnBan)
	{
		this.playerToUnBan = playerToUnBan;
	}

	public string getPlayerToUnBan()
	{
		return playerToUnBan;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(playerToUnBan);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(394);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerToUnBan);
		return messageWriter.GetData();
	}
}