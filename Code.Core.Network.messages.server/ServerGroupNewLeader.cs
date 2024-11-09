using System;

namespace Code.Core.Network.messages.server;

public class ServerGroupNewLeader : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 362;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGroupNewLeader(reader);
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

	private const int OPCODE = 362;

	private string newLeader;

	private sbyte succeeded;

	private ServerGroupNewLeader(MessageReader reader)
	{
		newLeader = reader.ReadString();
		succeeded = reader.ReadInt8();
	}

	public ServerGroupNewLeader(string newLeader, sbyte succeeded)
	{
		this.newLeader = newLeader;
		this.succeeded = succeeded;
	}

	public string getNewLeader()
	{
		return newLeader;
	}

	public sbyte getSucceeded()
	{
		return succeeded;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(newLeader);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(362);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(newLeader);
		messageWriter.WriteInt8(succeeded);
		return messageWriter.GetData();
	}
}
