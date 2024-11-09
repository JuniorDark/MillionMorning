using System;

namespace Code.Core.Network;

public class AuthenticationMessageFactory : IMessageFactory
{
	public int GetOpCode()
	{
		return 0;
	}

	public IMessage CreateMessage(MessageReader reader)
	{
		return null;
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
