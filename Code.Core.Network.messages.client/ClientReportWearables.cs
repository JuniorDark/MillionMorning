using System;
using System.Collections.Generic;

namespace Code.Core.Network.messages.client;

public class ClientReportWearables : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 186;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientReportWearables(reader);
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

	private const int OPCODE = 186;

	private IList<int> wearables;

	private ClientReportWearables(MessageReader reader)
	{
		wearables = new List<int>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			wearables.Add(reader.ReadInt32());
		}
	}

	public ClientReportWearables(IList<int> wearables)
	{
		this.wearables = wearables;
	}

	public IList<int> getWearables()
	{
		return wearables;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += (short)(wearables.Count * 4);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(186);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)wearables.Count);
		foreach (int wearable in wearables)
		{
			messageWriter.WriteInt32(wearable);
		}
		return messageWriter.GetData();
	}
}
