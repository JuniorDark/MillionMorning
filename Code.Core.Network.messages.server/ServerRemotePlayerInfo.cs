using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerRemotePlayerInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 64;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerRemotePlayerInfo(reader);
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

	private const int OPCODE = 64;

	private RemotePlayer player;

	private PlayerInChatRoom chatRoomInfo;

	private ServerRemotePlayerInfo(MessageReader reader)
	{
		player = new RemotePlayer(reader);
		if (reader.ReadInt8() == 1)
		{
			chatRoomInfo = new PlayerInChatRoom(reader);
		}
	}

	public ServerRemotePlayerInfo(RemotePlayer player, PlayerInChatRoom chatRoomInfo)
	{
		this.player = player;
		this.chatRoomInfo = chatRoomInfo;
	}

	public RemotePlayer getPlayer()
	{
		return player;
	}

	public PlayerInChatRoom getChatRoomInfo()
	{
		return chatRoomInfo;
	}

	public byte[] GetData()
	{
		int num = 5;
		num += player.Size();
		if (chatRoomInfo != null)
		{
			num += chatRoomInfo.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(64);
		messageWriter.WriteInt16((short)(num - 4));
		player.Write(messageWriter);
		if (chatRoomInfo == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			chatRoomInfo.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
