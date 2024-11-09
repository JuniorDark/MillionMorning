using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerStop : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 52;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerStop(reader);
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

	public const int OPCODE = 52;

	private string playerID;

	private short x;

	private short y;

	private short z;

	private sbyte rotation;

	private ServerPlayerStop(MessageReader reader)
	{
		playerID = reader.ReadString();
		x = reader.ReadInt16();
		y = reader.ReadInt16();
		z = reader.ReadInt16();
		rotation = reader.ReadInt8();
	}

	public ServerPlayerStop(string playerID, short x, short y, short z, sbyte rotation)
	{
		this.playerID = playerID;
		this.x = x;
		this.y = y;
		this.z = z;
		this.rotation = rotation;
	}

	public string getPlayerID()
	{
		return playerID;
	}

	public short getX()
	{
		return x;
	}

	public short getY()
	{
		return y;
	}

	public short getZ()
	{
		return z;
	}

	public sbyte getRotation()
	{
		return rotation;
	}

	public byte[] GetData()
	{
		int num = 13;
		num += MessageWriter.GetSize(playerID);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(52);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerID);
		messageWriter.WriteInt16(x);
		messageWriter.WriteInt16(y);
		messageWriter.WriteInt16(z);
		messageWriter.WriteInt8(rotation);
		return messageWriter.GetData();
	}
}
