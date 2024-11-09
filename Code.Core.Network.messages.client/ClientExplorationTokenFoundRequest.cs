using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientExplorationTokenFoundRequest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 160;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientExplorationTokenFoundRequest(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 13;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 160;

	private sbyte tokenIndex;

	private vector3 playerPosition;

	private ClientExplorationTokenFoundRequest(MessageReader reader)
	{
		tokenIndex = reader.ReadInt8();
		playerPosition = new vector3(reader);
	}

	public ClientExplorationTokenFoundRequest(sbyte tokenIndex, vector3 playerPosition)
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
		messageWriter.WriteOpCode(160);
		messageWriter.WriteInt8(tokenIndex);
		playerPosition.Write(messageWriter);
		return messageWriter.GetData();
	}
}
