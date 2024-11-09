using System;

namespace Code.Core.Network.messages.server;

public class ServerRemotePlayerLeaveInstance : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 45;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerRemotePlayerLeaveInstance(reader);
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

	private const int OPCODE = 45;

	private string playerId;

	private string destination;

	private ServerRemotePlayerLeaveInstance(MessageReader reader)
	{
		playerId = reader.ReadString();
		destination = reader.ReadString();
	}

	public ServerRemotePlayerLeaveInstance(string playerId, string destination)
	{
		this.playerId = playerId;
		this.destination = destination;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public string getDestination()
	{
		return destination;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(playerId);
		num += MessageWriter.GetSize(destination);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(45);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteString(destination);
		return messageWriter.GetData();
	}
}
