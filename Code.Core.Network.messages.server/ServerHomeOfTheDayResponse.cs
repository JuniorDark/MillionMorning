using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerHomeOfTheDayResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 404;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerHomeOfTheDayResponse(reader);
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

	private const int OPCODE = 404;

	private HomeOfTheDay home;

	private ServerHomeOfTheDayResponse(MessageReader reader)
	{
		home = new HomeOfTheDay(reader);
	}

	public ServerHomeOfTheDayResponse(HomeOfTheDay home)
	{
		this.home = home;
	}

	public HomeOfTheDay getHome()
	{
		return home;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += home.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(404);
		messageWriter.WriteInt16((short)(num - 4));
		home.Write(messageWriter);
		return messageWriter.GetData();
	}
}
