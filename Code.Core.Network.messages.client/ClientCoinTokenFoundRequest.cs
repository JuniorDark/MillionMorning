using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientCoinTokenFoundRequest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 162;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientCoinTokenFoundRequest(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 13;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 162;

	private sbyte tokenIndex;

	private vector3 playerPosition;

	private ClientCoinTokenFoundRequest(MessageReader reader)
	{
		tokenIndex = reader.ReadInt8();
		playerPosition = new vector3(reader);
	}

	public ClientCoinTokenFoundRequest(sbyte tokenIndex, vector3 playerPosition)
	{
		this.tokenIndex = tokenIndex;
		this.playerPosition = playerPosition;
	}

	public sbyte getTokenIndex()
	{
		return tokenIndex;
	}

	public vector3 getPlayerPosition()
	{
		return playerPosition;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(15);
		messageWriter.WriteOpCode(162);
		messageWriter.WriteInt8(tokenIndex);
		playerPosition.Write(messageWriter);
		return messageWriter.GetData();
	}
}
