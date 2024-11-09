using System;

namespace Code.Core.Network.messages.server;

public class ServerObjectDestroy : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 55;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerObjectDestroy(reader);
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

	private const int OPCODE = 55;

	private string type;

	private int itemID;

	private sbyte useDeathEffects;

	private ServerObjectDestroy(MessageReader reader)
	{
		type = reader.ReadString();
		itemID = reader.ReadInt32();
		useDeathEffects = reader.ReadInt8();
	}

	public ServerObjectDestroy(string type, int itemID, sbyte useDeathEffects)
	{
		this.type = type;
		this.itemID = itemID;
		this.useDeathEffects = useDeathEffects;
	}

	public string getType()
	{
		return type;
	}

	public int getItemID()
	{
		return itemID;
	}

	public sbyte getUseDeathEffects()
	{
		return useDeathEffects;
	}

	public byte[] GetData()
	{
		int num = 11;
		num += MessageWriter.GetSize(type);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(55);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(type);
		messageWriter.WriteInt32(itemID);
		messageWriter.WriteInt8(useDeathEffects);
		return messageWriter.GetData();
	}
}
