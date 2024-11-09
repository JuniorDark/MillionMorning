using System;

namespace Code.Core.Network.messages.client;

public class ClientLevelTravelRequest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 171;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientLevelTravelRequest(reader);
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

	private const int OPCODE = 171;

	private string world;

	private string level;

	private ClientLevelTravelRequest(MessageReader reader)
	{
		world = reader.ReadString();
		level = reader.ReadString();
	}

	public ClientLevelTravelRequest(string world, string level)
	{
		this.world = world;
		this.level = level;
	}

	public string getWorld()
	{
		return world;
	}

	public string getLevel()
	{
		return level;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(world);
		num += MessageWriter.GetSize(level);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(171);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(world);
		messageWriter.WriteString(level);
		return messageWriter.GetData();
	}
}
