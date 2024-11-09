using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerBossChangeMode : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 170;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerBossChangeMode(reader);
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

	private const int OPCODE = 170;

	private int bossId;

	private TemplateReference mode;

	private ServerBossChangeMode(MessageReader reader)
	{
		bossId = reader.ReadInt32();
		mode = new TemplateReference(reader);
	}

	public ServerBossChangeMode(int bossId, TemplateReference mode)
	{
		this.bossId = bossId;
		this.mode = mode;
	}

	public int getBossId()
	{
		return bossId;
	}

	public TemplateReference getMode()
	{
		return mode;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += mode.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(170);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(bossId);
		mode.Write(messageWriter);
		return messageWriter.GetData();
	}
}
