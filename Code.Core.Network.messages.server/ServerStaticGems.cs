using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerStaticGems : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 202;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerStaticGems(reader);
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

	private const int OPCODE = 202;

	private string fullLevelName;

	private IList<StaticGem> gems;

	private ServerStaticGems(MessageReader reader)
	{
		fullLevelName = reader.ReadString();
		gems = new List<StaticGem>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			gems.Add(new StaticGem(reader));
		}
	}

	public ServerStaticGems(string fullLevelName, IList<StaticGem> gems)
	{
		this.fullLevelName = fullLevelName;
		this.gems = gems;
	}

	public string getFullLevelName()
	{
		return fullLevelName;
	}

	public IList<StaticGem> getGems()
	{
		return gems;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(fullLevelName);
		foreach (StaticGem gem in gems)
		{
			num += gem.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(202);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(fullLevelName);
		messageWriter.WriteInt16((short)gems.Count);
		foreach (StaticGem gem2 in gems)
		{
			gem2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
