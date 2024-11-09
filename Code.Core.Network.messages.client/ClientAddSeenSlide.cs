using System;

namespace Code.Core.Network.messages.client;

public class ClientAddSeenSlide : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 286;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientAddSeenSlide(reader);
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

	private const int OPCODE = 286;

	private string slideName;

	private ClientAddSeenSlide(MessageReader reader)
	{
		slideName = reader.ReadString();
	}

	public ClientAddSeenSlide(string slideName)
	{
		this.slideName = slideName;
	}

	public string getSlideName()
	{
		return slideName;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(slideName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(286);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(slideName);
		return messageWriter.GetData();
	}
}
