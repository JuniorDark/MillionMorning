using System;
using System.Collections.Generic;

namespace Code.Core.Network.messages.server;

public class ServerAdminMessage : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 153;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerAdminMessage(reader);
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

	private const int OPCODE = 153;

	private int type;

	private string message;

	private IList<string> formatArguments;

	private ServerAdminMessage(MessageReader reader)
	{
		type = reader.ReadInt32();
		message = reader.ReadString();
		formatArguments = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			formatArguments.Add(reader.ReadString());
		}
	}

	public ServerAdminMessage(int type, string message, IList<string> formatArguments)
	{
		this.type = type;
		this.message = message;
		this.formatArguments = formatArguments;
	}

	public int getType()
	{
		return type;
	}

	public string getMessage()
	{
		return message;
	}

	public IList<string> getFormatArguments()
	{
		return formatArguments;
	}

	public byte[] GetData()
	{
		int num = 12;
		num += MessageWriter.GetSize(message);
		num += (short)(2 * formatArguments.Count);
		foreach (string formatArgument in formatArguments)
		{
			num += MessageWriter.GetSize(formatArgument);
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(153);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(type);
		messageWriter.WriteString(message);
		messageWriter.WriteInt16((short)formatArguments.Count);
		foreach (string formatArgument2 in formatArguments)
		{
			messageWriter.WriteString(formatArgument2);
		}
		return messageWriter.GetData();
	}
}
