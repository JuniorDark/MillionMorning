using System;

namespace Code.Core.Network.messages.server;

public class ServerStartClimb : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 97;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerStartClimb(reader);
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

	private const int OPCODE = 97;

	private string PlayerId;

	private int climbingSurface;

	private ServerStartClimb(MessageReader reader)
	{
		PlayerId = reader.ReadString();
		climbingSurface = reader.ReadInt32();
	}

	public ServerStartClimb(string PlayerId, int climbingSurface)
	{
		this.PlayerId = PlayerId;
		this.climbingSurface = climbingSurface;
	}

	public string getPlayerId()
	{
		return PlayerId;
	}

	public int getClimbingSurface()
	{
		return climbingSurface;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(PlayerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(97);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerId);
		messageWriter.WriteInt32(climbingSurface);
		return messageWriter.GetData();
	}
}
