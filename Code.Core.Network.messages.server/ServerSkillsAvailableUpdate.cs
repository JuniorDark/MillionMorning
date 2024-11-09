using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerSkillsAvailableUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 342;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerSkillsAvailableUpdate(reader);
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

	private const int OPCODE = 342;

	private IList<SkillTemplate> availableSkills;

	private ServerSkillsAvailableUpdate(MessageReader reader)
	{
		availableSkills = new List<SkillTemplate>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			availableSkills.Add(new SkillTemplate(reader));
		}
	}

	public ServerSkillsAvailableUpdate(IList<SkillTemplate> availableSkills)
	{
		this.availableSkills = availableSkills;
	}

	public IList<SkillTemplate> getAvailableSkills()
	{
		return availableSkills;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (SkillTemplate availableSkill in availableSkills)
		{
			num += availableSkill.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(342);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)availableSkills.Count);
		foreach (SkillTemplate availableSkill2 in availableSkills)
		{
			availableSkill2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
