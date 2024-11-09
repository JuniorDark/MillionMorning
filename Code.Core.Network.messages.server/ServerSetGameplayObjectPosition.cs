using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerSetGameplayObjectPosition : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 412;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerSetGameplayObjectPosition(reader);
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

	private const int OPCODE = 412;

	private int gameplayObjectId;

	private vector3 position;

	private sbyte resetScale;

	public int GameplayObjectId => gameplayObjectId;

	public vector3 Position => position;

	public bool ResetScale => resetScale == 1;

	private ServerSetGameplayObjectPosition(MessageReader reader)
	{
		gameplayObjectId = reader.ReadInt32();
		position = new vector3(reader);
		resetScale = reader.ReadInt8();
	}

	public ServerSetGameplayObjectPosition(int gameplayObjectId, vector3 position, bool resetScale)
	{
		this.gameplayObjectId = gameplayObjectId;
		this.position = position;
		this.resetScale = (sbyte)(resetScale ? 1 : 0);
	}

	public byte[] GetData()
	{
		int num = 21;
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(412);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(gameplayObjectId);
		position.Write(messageWriter);
		messageWriter.WriteInt8(resetScale);
		return messageWriter.GetData();
	}
}
