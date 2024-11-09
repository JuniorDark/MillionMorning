using System;

namespace Code.Core.Network.messages.client;

public class ClientWorldTravelRequest : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 227;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientWorldTravelRequest(reader);
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

	private const int OPCODE = 227;

	private string world;

	private ClientWorldTravelRequest(MessageReader reader)
	{
		world = reader.ReadString();
	}

	public ClientWorldTravelRequest(string world)
	{
		this.world = world;
	}

	public string getWorld()
	{
		return world;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(world);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(227);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(world);
		return messageWriter.GetData();
	}
}
