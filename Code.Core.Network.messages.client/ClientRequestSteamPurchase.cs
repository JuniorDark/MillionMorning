using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestSteamPurchase : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 425;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestSteamPurchase(reader);
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

	private const int OPCODE = 425;

	private string steamId;

	private string language;

	private int package;

	private ClientRequestSteamPurchase(MessageReader reader)
	{
		steamId = reader.ReadString();
		language = reader.ReadString();
		package = reader.ReadInt32();
	}

	public ClientRequestSteamPurchase(string steamId, string language, int package)
	{
		this.steamId = steamId;
		this.language = language;
		this.package = package;
	}

	public byte[] GetData()
	{
		int num = 12;
		num += MessageWriter.GetSize(steamId);
		num += MessageWriter.GetSize(language);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(425);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(steamId);
		messageWriter.WriteString(language);
		messageWriter.WriteInt32(package);
		return messageWriter.GetData();
	}
}
