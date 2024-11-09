using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestRemotePlayerInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 63;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestRemotePlayerInfo(reader);
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

	private const int OPCODE = 63;

	private string playerID;

	private ClientRequestRemotePlayerInfo(MessageReader reader)
	{
		playerID = reader.ReadString();
	}

	public ClientRequestRemotePlayerInfo(string playerID)
	{
		this.playerID = playerID;
	}

	public string getPlayerID()
	{
		return playerID;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(playerID);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(63);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerID);
		return messageWriter.GetData();
	}
}
