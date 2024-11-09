using System;

namespace Code.Core.Network.messages.server;

public class ServerOnLand : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 82;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerOnLand(reader);
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

	private const int OPCODE = 82;

	private int ObjectId;

	private string PlayerId;

	private ServerOnLand(MessageReader reader)
	{
		ObjectId = reader.ReadInt32();
		PlayerId = reader.ReadString();
	}

	public ServerOnLand(int ObjectId, string PlayerId)
	{
		this.ObjectId = ObjectId;
		this.PlayerId = PlayerId;
	}

	public int getObjectId()
	{
		return ObjectId;
	}

	public string getPlayerId()
	{
		return PlayerId;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(PlayerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(82);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(ObjectId);
		messageWriter.WriteString(PlayerId);
		return messageWriter.GetData();
	}
}
