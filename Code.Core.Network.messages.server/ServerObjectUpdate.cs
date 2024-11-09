using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerObjectUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 54;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerObjectUpdate(reader);
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

	public const int OPCODE = 54;

	private LevelObjectUpdate update;

	private ServerObjectUpdate(MessageReader reader)
	{
		update = LevelObjectUpdate.Create(reader.ReadTypeCode(), reader);
	}

	public ServerObjectUpdate(LevelObjectUpdate update)
	{
		this.update = update;
	}

	public LevelObjectUpdate getUpdate()
	{
		return update;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += update.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(54);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteTypeCode(update.GetTypeId());
		update.Write(messageWriter);
		return messageWriter.GetData();
	}
}
