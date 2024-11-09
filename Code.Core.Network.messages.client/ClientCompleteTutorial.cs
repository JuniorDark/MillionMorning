using System;

namespace Code.Core.Network.messages.client;

public class ClientCompleteTutorial : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 185;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientCompleteTutorial(reader);
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

	private const int OPCODE = 185;

	private string tutorial;

	private ClientCompleteTutorial(MessageReader reader)
	{
		tutorial = reader.ReadString();
	}

	public ClientCompleteTutorial(string tutorial)
	{
		this.tutorial = tutorial;
	}

	public string getTutorial()
	{
		return tutorial;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(tutorial);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(185);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(tutorial);
		return messageWriter.GetData();
	}
}
