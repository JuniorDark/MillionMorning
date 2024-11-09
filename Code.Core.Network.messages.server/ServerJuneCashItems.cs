using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerJuneCashItems : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 406;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerJuneCashItems(reader);
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

	private const int OPCODE = 406;

	private IList<JuneCashItem> juneCashItems;

	private ServerJuneCashItems(MessageReader reader)
	{
		juneCashItems = new List<JuneCashItem>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			juneCashItems.Add(new JuneCashItem(reader));
		}
	}

	public ServerJuneCashItems(IList<JuneCashItem> juneCashItems)
	{
		this.juneCashItems = juneCashItems;
	}

	public IList<JuneCashItem> getJuneCashItems()
	{
		return juneCashItems;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (JuneCashItem juneCashItem in juneCashItems)
		{
			num += juneCashItem.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(406);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)juneCashItems.Count);
		foreach (JuneCashItem juneCashItem2 in juneCashItems)
		{
			juneCashItem2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
