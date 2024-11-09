using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerUpdateAvatarResult : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 220;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateAvatarResult(reader);
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

	private const int OPCODE = 220;

	private readonly sbyte _enterResult;

	private readonly string _playerId;

	private readonly Code.Core.Network.types.Avatar _avatar;

	private ServerUpdateAvatarResult(MessageReader reader)
	{
		_enterResult = reader.ReadInt8();
		_playerId = reader.ReadString();
		if (reader.ReadInt8() == 1)
		{
			_avatar = new Code.Core.Network.types.Avatar(reader);
		}
	}

	public ServerUpdateAvatarResult(sbyte enterResult, string playerId, Code.Core.Network.types.Avatar avatar)
	{
		_enterResult = enterResult;
		_playerId = playerId;
		_avatar = avatar;
	}

	public sbyte GetEnterResult()
	{
		return _enterResult;
	}

	public string GetPlayerId()
	{
		return _playerId;
	}

	public Code.Core.Network.types.Avatar GetAvatar()
	{
		return _avatar;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(_playerId);
		if (_avatar != null)
		{
			num += _avatar.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(220);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt8(_enterResult);
		messageWriter.WriteString(_playerId);
		if (_avatar == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			_avatar.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
