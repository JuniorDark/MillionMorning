using System;

namespace Code.Core.Network.messages.server;

public class ServerCommand : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 2;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCommand(reader);
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

	private const int OPCODE = 2;

	private string commandResult;

	private ServerCommand(MessageReader reader)
	{
		commandResult = reader.ReadString();
	}

	public ServerCommand(string commandResult)
	{
		this.commandResult = commandResult;
	}

	public string getCommandResult()
	{
		return commandResult;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(commandResult);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(2);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(commandResult);
		return messageWriter.GetData();
	}
}
