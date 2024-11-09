using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.client;

public class ClientRequestBuyItem : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 72;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestBuyItem(reader);
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

	private const int OPCODE = 72;

	private int itemId;

	private IList<string> modifiers;

	private sbyte gender;

	private sbyte useCoins;

	private NullableString giftAvatarName;

	private ClientRequestBuyItem(MessageReader reader)
	{
		itemId = reader.ReadInt32();
		modifiers = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			modifiers.Add(reader.ReadString());
		}
		gender = reader.ReadInt8();
		useCoins = reader.ReadInt8();
		if (reader.ReadInt8() == 1)
		{
			giftAvatarName = new NullableString(reader);
		}
	}

	public ClientRequestBuyItem(int itemId, IList<string> modifiers, sbyte gender, sbyte useCoins, NullableString giftAvatarName)
	{
		this.itemId = itemId;
		this.modifiers = modifiers;
		this.gender = gender;
		this.useCoins = useCoins;
		this.giftAvatarName = giftAvatarName;
	}

	public int getItemId()
	{
		return itemId;
	}

	public IList<string> getModifiers()
	{
		return modifiers;
	}

	public sbyte getGender()
	{
		return gender;
	}

	public sbyte getUseCoins()
	{
		return useCoins;
	}

	public NullableString getGiftAvatarName()
	{
		return giftAvatarName;
	}

	public byte[] GetData()
	{
		int num = 13;
		num += (short)(2 * modifiers.Count);
		foreach (string modifier in modifiers)
		{
			num += MessageWriter.GetSize(modifier);
		}
		if (giftAvatarName != null)
		{
			num += giftAvatarName.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(72);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(itemId);
		messageWriter.WriteInt16((short)modifiers.Count);
		foreach (string modifier2 in modifiers)
		{
			messageWriter.WriteString(modifier2);
		}
		messageWriter.WriteInt8(gender);
		messageWriter.WriteInt8(useCoins);
		if (giftAvatarName == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			giftAvatarName.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
