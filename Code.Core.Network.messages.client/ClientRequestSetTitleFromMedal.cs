using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestSetTitleFromMedal : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 194;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestSetTitleFromMedal(reader);
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

	private const int OPCODE = 194;

	private string achievementTemplateIdentifier;

	private ClientRequestSetTitleFromMedal(MessageReader reader)
	{
		achievementTemplateIdentifier = reader.ReadString();
	}

	public ClientRequestSetTitleFromMedal(string achievementTemplateIdentifier)
	{
		this.achievementTemplateIdentifier = achievementTemplateIdentifier;
	}

	public string getAchievementTemplateIdentifier()
	{
		return achievementTemplateIdentifier;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(achievementTemplateIdentifier);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(194);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(achievementTemplateIdentifier);
		return messageWriter.GetData();
	}
}
