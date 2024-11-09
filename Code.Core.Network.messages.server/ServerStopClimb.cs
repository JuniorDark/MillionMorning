using System;

namespace Code.Core.Network.messages.server;

public class ServerStopClimb : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 99;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerStopClimb(reader);
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

	private const int OPCODE = 99;

	private string PlayerId;

	private ServerStopClimb(MessageReader reader)
	{
		PlayerId = reader.ReadString();
	}

	public ServerStopClimb(string PlayerId)
	{
		this.PlayerId = PlayerId;
	}

	public string getPlayerId()
	{
		return PlayerId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(PlayerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(99);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerId);
		return messageWriter.GetData();
	}
}