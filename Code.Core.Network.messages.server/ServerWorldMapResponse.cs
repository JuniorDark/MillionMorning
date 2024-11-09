namespace Code.Core.Network.messages.server;

public class ServerWorldMapResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 139;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerWorldMapResponse(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 1;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 139;

	private sbyte worldMapResult;

	private ServerWorldMapResponse(MessageReader reader)
	{
		worldMapResult = reader.ReadInt8();
	}

	public ServerWorldMapResponse(sbyte worldMapResult)
	{
		this.worldMapResult = worldMapResult;
	}

	public sbyte getWorldMapResult()
	{
		return worldMapResult;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(3);
		messageWriter.WriteOpCode(139);
		messageWriter.WriteInt8(worldMapResult);
		return messageWriter.GetData();
	}
}
