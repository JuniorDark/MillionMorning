using System;

namespace Code.Core.Network.messages.server;

public class ServerClassLevelUp : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 345;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerClassLevelUp(reader);
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

	private const int OPCODE = 345;

	private sbyte level;

	private string dpsDescription;

	private string healerDescription;

	private string tankDescription;

	private ServerClassLevelUp(MessageReader reader)
	{
		level = reader.ReadInt8();
		dpsDescription = reader.ReadString();
		healerDescription = reader.ReadString();
		tankDescription = reader.ReadString();
	}

	public ServerClassLevelUp(sbyte level, string dpsDescription, string healerDescription, string tankDescription)
	{
		this.level = level;
		this.dpsDescription = dpsDescription;
		this.healerDescription = healerDescription;
		this.tankDescription = tankDescription;
	}

	public sbyte getLevel()
	{
		return level;
	}

	public string getDpsDescription()
	{
		return dpsDescription;
	}

	public string getHealerDescription()
	{
		return healerDescription;
	}

	public string getTankDescription()
	{
		return tankDescription;
	}

	public byte[] GetData()
	{
		int num = 11;
		num += MessageWriter.GetSize(dpsDescription);
		num += MessageWriter.GetSize(healerDescription);
		num += MessageWriter.GetSize(tankDescription);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(345);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt8(level);
		messageWriter.WriteString(dpsDescription);
		messageWriter.WriteString(healerDescription);
		messageWriter.WriteString(tankDescription);
		return messageWriter.GetData();
	}
}
