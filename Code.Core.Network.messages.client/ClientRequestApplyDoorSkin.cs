namespace Code.Core.Network.messages.client;

public class ClientRequestApplyDoorSkin : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 390;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestApplyDoorSkin(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 16;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 390;

	private long newDoorItemId;

	private long reskinnedDoorId;

	private ClientRequestApplyDoorSkin(MessageReader reader)
	{
		newDoorItemId = reader.ReadInt64();
		reskinnedDoorId = reader.ReadInt64();
	}

	public ClientRequestApplyDoorSkin(long newDoorItemId, long reskinnedDoorId)
	{
		this.newDoorItemId = newDoorItemId;
		this.reskinnedDoorId = reskinnedDoorId;
	}

	public long getNewDoorItemId()
	{
		return newDoorItemId;
	}

	public long getReskinnedDoorId()
	{
		return reskinnedDoorId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(18);
		messageWriter.WriteOpCode(390);
		messageWriter.WriteInt64(newDoorItemId);
		messageWriter.WriteInt64(reskinnedDoorId);
		return messageWriter.GetData();
	}
}
