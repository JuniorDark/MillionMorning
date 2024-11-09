using System;

namespace Code.Core.Network.messages.server;

public class ServerInventoryModifierChanged : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 207;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerInventoryModifierChanged(reader);
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

	private const int OPCODE = 207;

	private int itemId;

	private string modifierKey;

	private string modifierValue;

	private ServerInventoryModifierChanged(MessageReader reader)
	{
		itemId = reader.ReadInt32();
		modifierKey = reader.ReadString();
		modifierValue = reader.ReadString();
	}

	public ServerInventoryModifierChanged(int itemId, string modifierKey, string modifierValue)
	{
		this.itemId = itemId;
		this.modifierKey = modifierKey;
		this.modifierValue = modifierValue;
	}

	public int getItemId()
	{
		return itemId;
	}

	public string getModifierKey()
	{
		return modifierKey;
	}

	public string getModifierValue()
	{
		return modifierValue;
	}

	public byte[] GetData()
	{
		int num = 12;
		num += MessageWriter.GetSize(modifierKey);
		num += MessageWriter.GetSize(modifierValue);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(207);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(itemId);
		messageWriter.WriteString(modifierKey);
		messageWriter.WriteString(modifierValue);
		return messageWriter.GetData();
	}
}
