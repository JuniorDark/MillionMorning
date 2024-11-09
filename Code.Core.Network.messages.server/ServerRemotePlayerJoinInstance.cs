using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerRemotePlayerJoinInstance : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 43;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerRemotePlayerJoinInstance(reader);
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

	private const int OPCODE = 43;

	private readonly RemotePlayer _remotePlayer;

	private ServerRemotePlayerJoinInstance(MessageReader reader)
	{
		_remotePlayer = new RemotePlayer(reader);
	}

	public ServerRemotePlayerJoinInstance(RemotePlayer remotePlayer)
	{
		_remotePlayer = remotePlayer;
	}

	public RemotePlayer GetRemotePlayer()
	{
		return _remotePlayer;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += _remotePlayer.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(43);
		messageWriter.WriteInt16((short)(num - 4));
		_remotePlayer.Write(messageWriter);
		return messageWriter.GetData();
	}
}
