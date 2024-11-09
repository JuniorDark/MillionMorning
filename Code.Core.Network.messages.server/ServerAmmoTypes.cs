using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerAmmoTypes : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 222;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerAmmoTypes(reader);
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

	private const int OPCODE = 222;

	private IList<AmmoType> ammoTypes;

	private ServerAmmoTypes(MessageReader reader)
	{
		ammoTypes = new List<AmmoType>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			ammoTypes.Add(new AmmoType(reader));
		}
	}

	public ServerAmmoTypes(IList<AmmoType> ammoTypes)
	{
		this.ammoTypes = ammoTypes;
	}

	public IList<AmmoType> getAmmoTypes()
	{
		return ammoTypes;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (AmmoType ammoType in ammoTypes)
		{
			num += ammoType.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(222);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)ammoTypes.Count);
		foreach (AmmoType ammoType2 in ammoTypes)
		{
			ammoType2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
