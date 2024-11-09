using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestCompleteAchievement : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 144;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestCompleteAchievement(reader);
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

	private const int OPCODE = 144;

	private readonly string _achievement;

	private ClientRequestCompleteAchievement(MessageReader reader)
	{
		_achievement = reader.ReadString();
	}

	public ClientRequestCompleteAchievement(string achievement)
	{
		_achievement = achievement;
	}

	public string GetAchievement()
	{
		return _achievement;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(_achievement);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(144);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(_achievement);
		return messageWriter.GetData();
	}
}
