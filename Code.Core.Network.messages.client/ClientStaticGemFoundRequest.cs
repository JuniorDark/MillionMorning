using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientStaticGemFoundRequest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 203;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientStaticGemFoundRequest(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 13;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 203;

	private sbyte gemIndex;

	private vector3 playerPosition;

	private ClientStaticGemFoundRequest(MessageReader reader)
	{
		gemIndex = reader.ReadInt8();
		playerPosition = new vector3(reader);
	}

	public ClientStaticGemFoundRequest(sbyte gemIndex, vector3 playerPosition)
	{
		this.gemIndex = gemIndex;
		this.playerPosition = playerPosition;
	}

	public sbyte getGemIndex()
	{
		return gemIndex;
	}

	public vector3 getPlayerPosition()
	{
		return playerPosition;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(15);
		messageWriter.WriteOpCode(203);
		messageWriter.WriteInt8(gemIndex);
		playerPosition.Write(messageWriter);
		return messageWriter.GetData();
	}
}
