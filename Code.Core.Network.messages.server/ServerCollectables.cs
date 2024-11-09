using System;
using System.Collections.Generic;

namespace Code.Core.Network.messages.server;

public class ServerCollectables : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 130;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCollectables(reader);
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

	private const int OPCODE = 130;

	private string category;

	private IList<string> collectables;

	private ServerCollectables(MessageReader reader)
	{
		category = reader.ReadString();
		collectables = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			collectables.Add(reader.ReadString());
		}
	}

	public ServerCollectables(string category, IList<string> collectables)
	{
		this.category = category;
		this.collectables = collectables;
	}

	public string getCategory()
	{
		return category;
	}

	public IList<string> getCollectables()
	{
		return collectables;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(category);
		num += (short)(2 * collectables.Count);
		foreach (string collectable in collectables)
		{
			num += MessageWriter.GetSize(collectable);
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(130);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(category);
		messageWriter.WriteInt16((short)collectables.Count);
		foreach (string collectable2 in collectables)
		{
			messageWriter.WriteString(collectable2);
		}
		return messageWriter.GetData();
	}
}
