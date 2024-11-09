using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerStartScreenInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 296;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerStartScreenInfo(reader);
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

	private const int OPCODE = 296;

	private TemplateReference homeDeliveryBox;

	private IList<string> guestsInHome;

	private IList<string> newLevels;

	private string lastAdventureLevel;

	private sbyte unloadCurrentInstance;

	private ServerStartScreenInfo(MessageReader reader)
	{
		if (reader.ReadInt8() == 1)
		{
			homeDeliveryBox = new TemplateReference(reader);
		}
		guestsInHome = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			guestsInHome.Add(reader.ReadString());
		}
		newLevels = new List<string>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			newLevels.Add(reader.ReadString());
		}
		lastAdventureLevel = reader.ReadString();
		unloadCurrentInstance = reader.ReadInt8();
	}

	public ServerStartScreenInfo(TemplateReference homeDeliveryBox, IList<string> guestsInHome, IList<string> newLevels, string lastAdventureLevel, sbyte unloadCurrentInstance)
	{
		this.homeDeliveryBox = homeDeliveryBox;
		this.guestsInHome = guestsInHome;
		this.newLevels = newLevels;
		this.lastAdventureLevel = lastAdventureLevel;
		this.unloadCurrentInstance = unloadCurrentInstance;
	}

	public TemplateReference getHomeDeliveryBox()
	{
		return homeDeliveryBox;
	}

	public IList<string> getGuestsInHome()
	{
		return guestsInHome;
	}

	public IList<string> getNewLevels()
	{
		return newLevels;
	}

	public string getLastAdventureLevel()
	{
		return lastAdventureLevel;
	}

	public sbyte getUnloadCurrentInstance()
	{
		return unloadCurrentInstance;
	}

	public byte[] GetData()
	{
		int num = 12;
		if (homeDeliveryBox != null)
		{
			num += homeDeliveryBox.Size();
		}
		num += (short)(2 * guestsInHome.Count);
		foreach (string item in guestsInHome)
		{
			num += MessageWriter.GetSize(item);
		}
		num += (short)(2 * newLevels.Count);
		foreach (string newLevel in newLevels)
		{
			num += MessageWriter.GetSize(newLevel);
		}
		num += MessageWriter.GetSize(lastAdventureLevel);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(296);
		messageWriter.WriteInt16((short)(num - 4));
		if (homeDeliveryBox == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			homeDeliveryBox.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)guestsInHome.Count);
		foreach (string item2 in guestsInHome)
		{
			messageWriter.WriteString(item2);
		}
		messageWriter.WriteInt16((short)newLevels.Count);
		foreach (string newLevel2 in newLevels)
		{
			messageWriter.WriteString(newLevel2);
		}
		messageWriter.WriteString(lastAdventureLevel);
		messageWriter.WriteInt8(unloadCurrentInstance);
		return messageWriter.GetData();
	}
}
