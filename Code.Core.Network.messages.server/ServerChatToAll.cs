using System;

namespace Code.Core.Network.messages.server;

public class ServerChatToAll : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 4;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerChatToAll(reader);
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

	private const int OPCODE = 4;

	private string playerID;

	private string message;

	private sbyte chanel;

	private ServerChatToAll(MessageReader reader)
	{
		playerID = reader.ReadString();
		message = reader.ReadString();
		chanel = reader.ReadInt8();
	}

	public ServerChatToAll(string playerID, string message, sbyte chanel)
	{
		this.playerID = playerID;
		this.message = message;
		this.chanel = chanel;
	}

	public string getPlayerID()
	{
		return playerID;
	}

	public string getMessage()
	{
		return message;
	}

	public sbyte getChanel()
	{
		return chanel;
	}

	public byte[] GetData()
	{
		int num = 9;
		num += MessageWriter.GetSize(playerID);
		num += MessageWriter.GetSize(message);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(4);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerID);
		messageWriter.WriteString(message);
		messageWriter.WriteInt8(chanel);
		return messageWriter.GetData();
	}
}
