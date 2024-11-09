using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerAmmoUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 223;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerAmmoUpdate(reader);
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

	private const int OPCODE = 223;

	private AmmoType ammoType;

	private ServerAmmoUpdate(MessageReader reader)
	{
		ammoType = new AmmoType(reader);
	}

	public ServerAmmoUpdate(AmmoType ammoType)
	{
		this.ammoType = ammoType;
	}

	public string getAmmoType()
	{
		return ammoType.GetCategoryType();
	}

	public int getAmmoAmount()
	{
		return ammoType.GetAmount();
	}

	public byte[] GetData()
	{
		int num = 4;
		num += ammoType.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(223);
		messageWriter.WriteInt16((short)(num - 4));
		ammoType.Write(messageWriter);
		return messageWriter.GetData();
	}
}
