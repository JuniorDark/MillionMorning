using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerEnterChatroom : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 78;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerEnterChatroom(reader);
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

	private const int OPCODE = 78;

	private PlayerInChatRoom playerChatroomInfo;

	private ServerEnterChatroom(MessageReader reader)
	{
		playerChatroomInfo = new PlayerInChatRoom(reader);
	}

	public ServerEnterChatroom(PlayerInChatRoom playerChatroomInfo)
	{
		this.playerChatroomInfo = playerChatroomInfo;
	}

	public PlayerInChatRoom getPlayerChatroomInfo()
	{
		return playerChatroomInfo;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += playerChatroomInfo.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(78);
		messageWriter.WriteInt16((short)(num - 4));
		playerChatroomInfo.Write(messageWriter);
		return messageWriter.GetData();
	}
}
