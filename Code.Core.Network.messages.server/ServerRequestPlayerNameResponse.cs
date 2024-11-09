using System;

namespace Code.Core.Network.messages.server;

public class ServerRequestPlayerNameResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 273;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerRequestPlayerNameResponse(reader);
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

	private const int OPCODE = 273;

	private string name;

	private string playerId;

	private ServerRequestPlayerNameResponse(MessageReader reader)
	{
		name = reader.ReadString();
		playerId = reader.ReadString();
	}

	public ServerRequestPlayerNameResponse(string name, string playerId)
	{
		this.name = name;
		this.playerId = playerId;
	}

	public string getName()
	{
		return name;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(name);
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(273);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(name);
		messageWriter.WriteString(playerId);
		return messageWriter.GetData();
	}
}
