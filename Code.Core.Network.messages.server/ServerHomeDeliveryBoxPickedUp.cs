using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerHomeDeliveryBoxPickedUp : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 302;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerHomeDeliveryBoxPickedUp(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 2;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt16();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 302;

	private string playerId;

	private long itemId;

	private long millisecondsUntilNextBoxSpawn;

	private TemplateReference nextDeliveryBox;

	private ServerHomeDeliveryBoxPickedUp(MessageReader reader)
	{
		playerId = reader.ReadString();
		itemId = reader.ReadInt64();
		millisecondsUntilNextBoxSpawn = reader.ReadInt64();
		nextDeliveryBox = new TemplateReference(reader);
	}

	public ServerHomeDeliveryBoxPickedUp(string playerId, long itemId, long millisecondsUntilNextBoxSpawn, TemplateReference nextDeliveryBox)
	{
		this.playerId = playerId;
		this.itemId = itemId;
		this.millisecondsUntilNextBoxSpawn = millisecondsUntilNextBoxSpawn;
		this.nextDeliveryBox = nextDeliveryBox;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public long getItemId()
	{
		return itemId;
	}

	public long getMillisecondsUntilNextBoxSpawn()
	{
		return millisecondsUntilNextBoxSpawn;
	}

	public TemplateReference getNextDeliveryBox()
	{
		return nextDeliveryBox;
	}

	public byte[] GetData()
	{
		int num = 22;
		num += MessageWriter.GetSize(playerId);
		num += nextDeliveryBox.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(302);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt64(itemId);
		messageWriter.WriteInt64(millisecondsUntilNextBoxSpawn);
		nextDeliveryBox.Write(messageWriter);
		return messageWriter.GetData();
	}
}
