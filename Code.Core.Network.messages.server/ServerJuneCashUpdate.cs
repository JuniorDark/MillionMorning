namespace Code.Core.Network.messages.server;

public class ServerJuneCashUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 34;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerJuneCashUpdate(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 34;

	private int juneCash;

	private int GUIUpdateType;

	private ServerJuneCashUpdate(MessageReader reader)
	{
		juneCash = reader.ReadInt32();
		GUIUpdateType = reader.ReadInt32();
	}

	public ServerJuneCashUpdate(int juneCash, int GUIUpdateType)
	{
		this.juneCash = juneCash;
		this.GUIUpdateType = GUIUpdateType;
	}

	public int getJuneCash()
	{
		return juneCash;
	}

	public int getGUIUpdateType()
	{
		return GUIUpdateType;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(34);
		messageWriter.WriteInt32(juneCash);
		messageWriter.WriteInt32(GUIUpdateType);
		return messageWriter.GetData();
	}
}
