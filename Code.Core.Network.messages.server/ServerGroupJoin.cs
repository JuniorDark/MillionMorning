using System;

namespace Code.Core.Network.messages.server;

public class ServerGroupJoin : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 359;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGroupJoin(reader);
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

	private const int OPCODE = 359;

	private string player;

	private sbyte accepted;

	private ServerGroupJoin(MessageReader reader)
	{
		player = reader.ReadString();
		accepted = reader.ReadInt8();
	}

	public ServerGroupJoin(string player, sbyte accepted)
	{
		this.player = player;
		this.accepted = accepted;
	}

	public string getPlayer()
	{
		return player;
	}

	public sbyte getAccepted()
	{
		return accepted;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(player);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(359);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(player);
		messageWriter.WriteInt8(accepted);
		return messageWriter.GetData();
	}
}
