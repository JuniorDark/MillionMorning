using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerQuestAdded : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 9;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerQuestAdded(reader);
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

	private const int OPCODE = 9;

	private Quest addedQuest;

	private Npc questGiver;

	private IList<string> npcTextsOnAdd;

	private ServerQuestAdded(MessageReader reader)
	{
		addedQuest = new Quest(reader);
		questGiver = new Npc(reader);
		npcTextsOnAdd = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			npcTextsOnAdd.Add(reader.ReadString());
		}
	}

	public ServerQuestAdded(Quest addedQuest, Npc questGiver, IList<string> npcTextsOnAdd)
	{
		this.addedQuest = addedQuest;
		this.questGiver = questGiver;
		this.npcTextsOnAdd = npcTextsOnAdd;
	}

	public Quest getAddedQuest()
	{
		return addedQuest;
	}

	public Npc getQuestGiver()
	{
		return questGiver;
	}

	public IList<string> getNpcTextsOnAdd()
	{
		return npcTextsOnAdd;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += addedQuest.Size();
		num += questGiver.Size();
		num += (short)(2 * npcTextsOnAdd.Count);
		foreach (string item in npcTextsOnAdd)
		{
			num += MessageWriter.GetSize(item);
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(9);
		messageWriter.WriteInt16((short)(num - 4));
		addedQuest.Write(messageWriter);
		questGiver.Write(messageWriter);
		messageWriter.WriteInt16((short)npcTextsOnAdd.Count);
		foreach (string item2 in npcTextsOnAdd)
		{
			messageWriter.WriteString(item2);
		}
		return messageWriter.GetData();
	}
}
