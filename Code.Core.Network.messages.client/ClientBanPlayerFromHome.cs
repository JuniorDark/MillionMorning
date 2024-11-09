using System;

namespace Code.Core.Network.messages.client;

public class ClientBanPlayerFromHome : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 392;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientBanPlayerFromHome(reader);
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

	private const int OPCODE = 392;

	private string playerToBan;

	private ClientBanPlayerFromHome(MessageReader reader)
	{
		playerToBan = reader.ReadString();
	}

	public ClientBanPlayerFromHome(string playerToBan)
	{
		this.playerToBan = playerToBan;
	}

	public string getPlayerToBan()
	{
		return playerToBan;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(playerToBan);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(392);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerToBan);
		return messageWriter.GetData();
	}
}
