using System;

namespace Code.Core.Network.messages.client;

public class ClientSteamInvite : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 429;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientSteamInvite(reader);
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

	private const int OPCODE = 429;

	private string steamId;

	private ClientSteamInvite(MessageReader reader)
	{
		steamId = reader.ReadString();
	}

	public ClientSteamInvite(string steamId)
	{
		this.steamId = steamId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(steamId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(429);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(steamId);
		return messageWriter.GetData();
	}
}
