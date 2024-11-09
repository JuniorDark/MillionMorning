using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerChangeTitle : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 192;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerChangeTitle(reader);
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

	private const int OPCODE = 192;

	private string playerId;

	private string title;

	private ServerPlayerChangeTitle(MessageReader reader)
	{
		playerId = reader.ReadString();
		title = reader.ReadString();
	}

	public ServerPlayerChangeTitle(string playerId, string title)
	{
		this.playerId = playerId;
		this.title = title;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public string getTitle()
	{
		return title;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(playerId);
		num += MessageWriter.GetSize(title);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(192);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteString(title);
		return messageWriter.GetData();
	}
}
