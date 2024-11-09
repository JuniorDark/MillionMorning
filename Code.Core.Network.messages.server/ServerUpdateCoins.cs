namespace Code.Core.Network.messages.server;

public class ServerUpdateCoins : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 232;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateCoins(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 5;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 232;

	private int coins;

	private sbyte updateType;

	private ServerUpdateCoins(MessageReader reader)
	{
		coins = reader.ReadInt32();
		updateType = reader.ReadInt8();
	}

	public ServerUpdateCoins(int coins, sbyte updateType)
	{
		this.coins = coins;
		this.updateType = updateType;
	}

	public int getCoins()
	{
		return coins;
	}

	public sbyte getUpdateType()
	{
		return updateType;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(7);
		messageWriter.WriteOpCode(232);
		messageWriter.WriteInt32(coins);
		messageWriter.WriteInt8(updateType);
		return messageWriter.GetData();
	}
}
