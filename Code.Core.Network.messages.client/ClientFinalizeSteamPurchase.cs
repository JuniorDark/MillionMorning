using System;

namespace Code.Core.Network.messages.client;

public class ClientFinalizeSteamPurchase : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 426;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientFinalizeSteamPurchase(reader);
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

	private const int OPCODE = 426;

	private string orderId;

	private string appId;

	private ClientFinalizeSteamPurchase(MessageReader reader)
	{
		orderId = reader.ReadString();
		appId = reader.ReadString();
	}

	public ClientFinalizeSteamPurchase(string orderId, string appId)
	{
		this.orderId = orderId;
		this.appId = appId;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(orderId);
		num += MessageWriter.GetSize(appId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(426);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(orderId);
		messageWriter.WriteString(appId);
		return messageWriter.GetData();
	}
}
