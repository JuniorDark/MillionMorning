using System;
using System.Collections.Generic;
using System.Linq;

namespace Code.Core.Network.messages.server.PVP;

public class ServerPvPQueues : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 413;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPvPQueues(reader);
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

	private const int OPCODE = 413;

	private IList<QueueInfo> queues;

	private ServerPvPQueues(MessageReader reader)
	{
		queues = new List<QueueInfo>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			queues.Add(new QueueInfo(reader));
		}
	}

	public ServerPvPQueues(IList<QueueInfo> queues)
	{
		this.queues = queues;
	}

	public IList<QueueInfo> GetQueues()
	{
		return queues;
	}

	public byte[] GetData()
	{
		int num = 6 + queues.Sum((QueueInfo queue) => QueueInfo.size());
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(413);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)queues.Count);
		foreach (QueueInfo queue in queues)
		{
			queue.write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
