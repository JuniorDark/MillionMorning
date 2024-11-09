using System;

namespace Code.Core.Network.messages.server;

public class ServerLeaveChatroom : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 80;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLeaveChatroom(reader);
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

	private const int OPCODE = 80;

	private string playerId;

	private long chatroomId;

	private ServerLeaveChatroom(MessageReader reader)
	{
		playerId = reader.ReadString();
		chatroomId = reader.ReadInt64();
	}

	public ServerLeaveChatroom(string playerId, long chatroomId)
	{
		this.playerId = playerId;
		this.chatroomId = chatroomId;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public long getChatroomId()
	{
		return chatroomId;
	}

	public byte[] GetData()
	{
		int num = 14;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(80);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt64(chatroomId);
		return messageWriter.GetData();
	}
}
