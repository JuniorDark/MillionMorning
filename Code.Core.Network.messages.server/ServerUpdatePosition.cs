using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerUpdatePosition : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 50;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdatePosition(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 22;
			return buffer.Remaining() >= length + 2;
		}
	}

	public const int OPCODE = 50;

	private int playerID;

	private short x;

	private short y;

	private short z;

	private vector3 velocity;

	private ServerUpdatePosition(MessageReader reader)
	{
		playerID = reader.ReadInt32();
		x = reader.ReadInt16();
		y = reader.ReadInt16();
		z = reader.ReadInt16();
		velocity = new vector3(reader);
	}

	public ServerUpdatePosition(int playerID, short x, short y, short z, vector3 velocity)
	{
		this.playerID = playerID;
		this.x = x;
		this.y = y;
		this.z = z;
		this.velocity = velocity;
	}

	public int getPlayerID()
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

	public vector3 getVelocity()
	{
		return velocity;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(24);
		messageWriter.WriteOpCode(50);
		messageWriter.WriteInt32(playerID);
		messageWriter.WriteInt16(x);
		messageWriter.WriteInt16(y);
		messageWriter.WriteInt16(z);
		velocity.Write(messageWriter);
		return messageWriter.GetData();
	}
}
