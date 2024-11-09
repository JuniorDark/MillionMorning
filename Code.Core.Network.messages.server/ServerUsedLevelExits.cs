using System;
using System.Collections.Generic;

namespace Code.Core.Network.messages.server;

public class ServerUsedLevelExits : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 229;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUsedLevelExits(reader);
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

	private const int OPCODE = 229;

	private IList<string> usedExitIdentifiers;

	private ServerUsedLevelExits(MessageReader reader)
	{
		usedExitIdentifiers = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			usedExitIdentifiers.Add(reader.ReadString());
		}
	}

	public ServerUsedLevelExits(IList<string> usedExitIdentifiers)
	{
		this.usedExitIdentifiers = usedExitIdentifiers;
	}

	public IList<string> getUsedExitIdentifiers()
	{
		return usedExitIdentifiers;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += (short)(2 * usedExitIdentifiers.Count);
		foreach (string usedExitIdentifier in usedExitIdentifiers)
		{
			num += MessageWriter.GetSize(usedExitIdentifier);
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(229);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)usedExitIdentifiers.Count);
		foreach (string usedExitIdentifier2 in usedExitIdentifiers)
		{
			messageWriter.WriteString(usedExitIdentifier2);
		}
		return messageWriter.GetData();
	}
}
