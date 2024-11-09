using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerCoinTokensCreate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 159;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCoinTokensCreate(reader);
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

	private const int OPCODE = 159;

	private string fullLevelName;

	private IList<CoinToken> coinTokens;

	private ServerCoinTokensCreate(MessageReader reader)
	{
		fullLevelName = reader.ReadString();
		coinTokens = new List<CoinToken>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			coinTokens.Add(new CoinToken(reader));
		}
	}

	public ServerCoinTokensCreate(string fullLevelName, IList<CoinToken> coinTokens)
	{
		this.fullLevelName = fullLevelName;
		this.coinTokens = coinTokens;
	}

	public string getFullLevelName()
	{
		return fullLevelName;
	}

	public IList<CoinToken> getCoinTokens()
	{
		return coinTokens;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(fullLevelName);
		num += (short)(coinTokens.Count * 13);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(159);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(fullLevelName);
		messageWriter.WriteInt16((short)coinTokens.Count);
		foreach (CoinToken coinToken in coinTokens)
		{
			coinToken.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
