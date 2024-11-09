using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerBoxOpened : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 276;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerBoxOpened(reader);
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

	private const int OPCODE = 276;

	private TemplateReference boxTemplate;

	private IList<BoxLoot> loot;

	private ServerBoxOpened(MessageReader reader)
	{
		boxTemplate = new TemplateReference(reader);
		loot = new List<BoxLoot>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			loot.Add(new BoxLoot(reader));
		}
	}

	public ServerBoxOpened(TemplateReference boxTemplate, IList<BoxLoot> loot)
	{
		this.boxTemplate = boxTemplate;
		this.loot = loot;
	}

	public TemplateReference getBoxTemplate()
	{
		return boxTemplate;
	}

	public IList<BoxLoot> getLoot()
	{
		return loot;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += boxTemplate.Size();
		foreach (BoxLoot item in loot)
		{
			num += item.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(276);
		messageWriter.WriteInt16((short)(num - 4));
		boxTemplate.Write(messageWriter);
		messageWriter.WriteInt16((short)loot.Count);
		foreach (BoxLoot item2 in loot)
		{
			item2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
