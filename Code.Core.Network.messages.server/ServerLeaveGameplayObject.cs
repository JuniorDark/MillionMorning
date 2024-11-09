using System;

namespace Code.Core.Network.messages.server;

public class ServerLeaveGameplayObject : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 103;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLeaveGameplayObject(reader);
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

	private const int OPCODE = 103;

	private string PlayerId;

	private ServerLeaveGameplayObject(MessageReader reader)
	{
		PlayerId = reader.ReadString();
	}

	public ServerLeaveGameplayObject(string PlayerId)
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
		messageWriter.WriteOpCode(103);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerId);
		return messageWriter.GetData();
	}
}
