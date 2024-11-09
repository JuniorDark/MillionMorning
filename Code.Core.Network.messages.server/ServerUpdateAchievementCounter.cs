using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerUpdateAchievementCounter : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 143;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateAchievementCounter(reader);
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

	private const int OPCODE = 143;

	private AchievementCounter counter;

	private ServerUpdateAchievementCounter(MessageReader reader)
	{
		counter = new AchievementCounter(reader);
	}

	public ServerUpdateAchievementCounter(AchievementCounter counter)
	{
		this.counter = counter;
	}

	public AchievementCounter getCounter()
	{
		return counter;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += counter.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(143);
		messageWriter.WriteInt16((short)(num - 4));
		counter.Write(messageWriter);
		return messageWriter.GetData();
	}
}
