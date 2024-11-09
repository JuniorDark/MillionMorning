using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerPremiumTokens : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 191;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPremiumTokens(reader);
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

	private const int OPCODE = 191;

	private IList<PremiumToken> premiumTokens;

	private ServerPremiumTokens(MessageReader reader)
	{
		premiumTokens = new List<PremiumToken>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			premiumTokens.Add(new PremiumToken(reader));
		}
	}

	public ServerPremiumTokens(IList<PremiumToken> premiumTokens)
	{
		this.premiumTokens = premiumTokens;
	}

	public IList<PremiumToken> getPremiumTokens()
	{
		return premiumTokens;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (PremiumToken premiumToken in premiumTokens)
		{
			num += premiumToken.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(191);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)premiumTokens.Count);
		foreach (PremiumToken premiumToken2 in premiumTokens)
		{
			premiumToken2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
