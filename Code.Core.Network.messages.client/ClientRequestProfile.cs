using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestProfile : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 200;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestProfile(reader);
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

	private const int OPCODE = 200;

	private string playerId;

	private sbyte isFriend;

	private ClientRequestProfile(MessageReader reader)
	{
		playerId = reader.ReadString();
		isFriend = reader.ReadInt8();
	}

	public ClientRequestProfile(string playerId, sbyte isFriend)
	{
		this.playerId = playerId;
		this.isFriend = isFriend;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public sbyte getIsFriend()
	{
		return isFriend;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(200);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt8(isFriend);
		return messageWriter.GetData();
	}
}
