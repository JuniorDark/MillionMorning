using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerRotateFurniture : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 257;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerRotateFurniture(reader);
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

	private const int OPCODE = 257;

	private string playerId;

	private ServerPlayerRotateFurniture(MessageReader reader)
	{
		playerId = reader.ReadString();
	}

	public ServerPlayerRotateFurniture(string playerId)
	{
		this.playerId = playerId;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(257);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		return messageWriter.GetData();
	}
}
