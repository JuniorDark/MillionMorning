using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerUpdateAvatar : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 221;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateAvatar(reader);
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

	private const int OPCODE = 221;

	private string playerId;

	private Code.Core.Network.types.Avatar avatar;

	private ServerUpdateAvatar(MessageReader reader)
	{
		playerId = reader.ReadString();
		if (reader.ReadInt8() == 1)
		{
			avatar = new Code.Core.Network.types.Avatar(reader);
		}
	}

	public ServerUpdateAvatar(string playerId, Code.Core.Network.types.Avatar avatar)
	{
		this.playerId = playerId;
		this.avatar = avatar;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public Code.Core.Network.types.Avatar getAvatar()
	{
		return avatar;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(playerId);
		if (avatar != null)
		{
			num += avatar.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(221);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		if (avatar == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			avatar.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
