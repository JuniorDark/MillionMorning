using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientTalkToNPC : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 5;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientTalkToNPC(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 16;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 5;

	private int npcID;

	private vector3 playerPosition;

	private ClientTalkToNPC(MessageReader reader)
	{
		npcID = reader.ReadInt32();
		playerPosition = new vector3(reader);
	}

	public ClientTalkToNPC(int npcID, vector3 playerPosition)
	{
		this.npcID = npcID;
		this.playerPosition = playerPosition;
	}

	public int getNpcID()
	{
		return npcID;
	}

	public vector3 getPlayerPosition()
	{
		return playerPosition;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(18);
		messageWriter.WriteOpCode(5);
		messageWriter.WriteInt32(npcID);
		playerPosition.Write(messageWriter);
		return messageWriter.GetData();
	}
}
