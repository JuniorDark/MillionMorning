namespace Code.Core.Network.messages.server;

public class ServerSkillItemActivationFailed : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 436;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerSkillItemActivationFailed(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 436;

	private int SkillInventoryId;

	private ServerSkillItemActivationFailed(MessageReader reader)
	{
		SkillInventoryId = reader.ReadInt32();
	}

	public ServerSkillItemActivationFailed(int SkillInventoryId)
	{
		this.SkillInventoryId = SkillInventoryId;
	}

	public int getSkillInventoryId()
	{
		return SkillInventoryId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(436);
		messageWriter.WriteInt32(SkillInventoryId);
		return messageWriter.GetData();
	}
}
