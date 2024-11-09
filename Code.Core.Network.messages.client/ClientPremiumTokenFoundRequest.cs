using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientPremiumTokenFoundRequest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 197;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientPremiumTokenFoundRequest(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 12;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 197;

	private vector3 playerPosition;

	private ClientPremiumTokenFoundRequest(MessageReader reader)
	{
		playerPosition = new vector3(reader);
	}

	public ClientPremiumTokenFoundRequest(vector3 playerPosition)
	{
		this.playerPosition = playerPosition;
	}

	public vector3 getPlayerPosition()
	{
		return playerPosition;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(14);
		messageWriter.WriteOpCode(197);
		playerPosition.Write(messageWriter);
		return messageWriter.GetData();
	}
}
