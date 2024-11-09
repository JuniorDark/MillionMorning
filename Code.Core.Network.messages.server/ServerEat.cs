using System;

namespace Code.Core.Network.messages.server;

public class ServerEat : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 206;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerEat(reader);
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

	private const int OPCODE = 206;

	private string PlayerId;

	private sbyte useNumber;

	private ServerEat(MessageReader reader)
	{
		PlayerId = reader.ReadString();
		useNumber = reader.ReadInt8();
	}

	public ServerEat(string PlayerId, sbyte useNumber)
	{
		this.PlayerId = PlayerId;
		this.useNumber = useNumber;
	}

	public string getPlayerId()
	{
		return PlayerId;
	}

	public sbyte getUseNumber()
	{
		return useNumber;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(PlayerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(206);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerId);
		messageWriter.WriteInt8(useNumber);
		return messageWriter.GetData();
	}
}
