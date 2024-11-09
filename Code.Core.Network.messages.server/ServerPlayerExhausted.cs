using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayerExhausted : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 91;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerExhausted(reader);
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

	private const int OPCODE = 91;

	private string PlayerID;

	private float Delay;

	private ServerPlayerExhausted(MessageReader reader)
	{
		PlayerID = reader.ReadString();
		Delay = reader.ReadFloat();
	}

	public ServerPlayerExhausted(string PlayerID, float Delay)
	{
		this.PlayerID = PlayerID;
		this.Delay = Delay;
	}

	public string getPlayerID()
	{
		return PlayerID;
	}

	public float getDelay()
	{
		return Delay;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(PlayerID);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(91);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerID);
		messageWriter.WriteFloat(Delay);
		return messageWriter.GetData();
	}
}
