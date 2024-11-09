using System;

namespace Code.Core.Network.messages.client;

public class ClientSaveUsedLevelExit : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 228;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientSaveUsedLevelExit(reader);
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

	private const int OPCODE = 228;

	private string levelTemplateIdentifier;

	private string exitIdentifier;

	private ClientSaveUsedLevelExit(MessageReader reader)
	{
		levelTemplateIdentifier = reader.ReadString();
		exitIdentifier = reader.ReadString();
	}

	public ClientSaveUsedLevelExit(string levelTemplateIdentifier, string exitIdentifier)
	{
		this.levelTemplateIdentifier = levelTemplateIdentifier;
		this.exitIdentifier = exitIdentifier;
	}

	public string getLevelTemplateIdentifier()
	{
		return levelTemplateIdentifier;
	}

	public string getExitIdentifier()
	{
		return exitIdentifier;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(levelTemplateIdentifier);
		num += MessageWriter.GetSize(exitIdentifier);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(228);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(levelTemplateIdentifier);
		messageWriter.WriteString(exitIdentifier);
		return messageWriter.GetData();
	}
}
