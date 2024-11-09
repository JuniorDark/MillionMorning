using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientDig : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 94;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientDig(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 12;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 94;

	private vector3 Position;

	private ClientDig(MessageReader reader)
	{
		Position = new vector3(reader);
	}

	public ClientDig(vector3 Position)
	{
		this.Position = Position;
	}

	public vector3 getPosition()
	{
		return Position;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(14);
		messageWriter.WriteOpCode(94);
		Position.Write(messageWriter);
		return messageWriter.GetData();
	}
}
