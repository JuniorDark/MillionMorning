namespace Code.Core.Network.messages.client;

public class ClientStartClimb : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 96;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientStartClimb(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 96;

	private int climbingSurface;

	private ClientStartClimb(MessageReader reader)
	{
		climbingSurface = reader.ReadInt32();
	}

	public ClientStartClimb(int climbingSurface)
	{
		this.climbingSurface = climbingSurface;
	}

	public int getClimbingSurface()
	{
		return climbingSurface;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(96);
		messageWriter.WriteInt32(climbingSurface);
		return messageWriter.GetData();
	}
}
