namespace Code.Core.Network.messages.server;

public class ServerLocalPlayerJoinHome : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 246;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLocalPlayerJoinHome(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 246;

	private sbyte needsHomeBoxPosition;

	private ServerLocalPlayerJoinHome(MessageReader reader)
	{
		needsHomeBoxPosition = reader.ReadInt8();
	}

	public ServerLocalPlayerJoinHome(sbyte needsHomeBoxPosition)
	{
		this.needsHomeBoxPosition = needsHomeBoxPosition;
	}

	public sbyte getNeedsHomeBoxPosition()
	{
		return needsHomeBoxPosition;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(246);
		messageWriter.WriteInt8(needsHomeBoxPosition);
		return messageWriter.GetData();
	}
}
