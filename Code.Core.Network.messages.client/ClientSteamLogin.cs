using System;

namespace Code.Core.Network.messages.client;

public class ClientSteamLogin : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 6;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientSteamLogin(reader);
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

	private const int OPCODE = 6;

	private byte[] steamTicket;

	private ClientSteamLogin(MessageReader reader)
	{
		steamTicket = reader.ReadBytes();
	}

	public ClientSteamLogin(byte[] steamTicket)
	{
		this.steamTicket = steamTicket;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(8 + steamTicket.Length);
		messageWriter.WriteOpCode(0);
		messageWriter.WriteInt16((short)(4 + steamTicket.Length));
		messageWriter.WriteOpCode(6);
		messageWriter.WriteBytes(steamTicket);
		return messageWriter.GetData();
	}
}
