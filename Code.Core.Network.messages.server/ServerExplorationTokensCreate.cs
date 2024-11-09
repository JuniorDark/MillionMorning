using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerExplorationTokensCreate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 158;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerExplorationTokensCreate(reader);
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

	private const int OPCODE = 158;

	private string fullLevelName;

	private IList<ExplorationToken> explorationTokens;

	private NullableExplorationTokenList foundGoldTokens;

	private ServerExplorationTokensCreate(MessageReader reader)
	{
		fullLevelName = reader.ReadString();
		explorationTokens = new List<ExplorationToken>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			explorationTokens.Add(new ExplorationToken(reader));
		}
		if (reader.ReadInt8() == 1)
		{
			foundGoldTokens = new NullableExplorationTokenList(reader);
		}
	}

	public ServerExplorationTokensCreate(string fullLevelName, IList<ExplorationToken> explorationTokens, NullableExplorationTokenList foundGoldTokens)
	{
		this.fullLevelName = fullLevelName;
		this.explorationTokens = explorationTokens;
		this.foundGoldTokens = foundGoldTokens;
	}

	public string getFullLevelName()
	{
		return fullLevelName;
	}

	public IList<ExplorationToken> getExplorationTokens()
	{
		return explorationTokens;
	}

	public NullableExplorationTokenList getFoundExplorationTokens()
	{
		return foundGoldTokens;
	}

	public byte[] GetData()
	{
		int num = 9;
		num += MessageWriter.GetSize(fullLevelName);
		num += (short)(explorationTokens.Count * 14);
		if (foundGoldTokens != null)
		{
			num += foundGoldTokens.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(158);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(fullLevelName);
		messageWriter.WriteInt16((short)explorationTokens.Count);
		foreach (ExplorationToken explorationToken in explorationTokens)
		{
			explorationToken.Write(messageWriter);
		}
		if (foundGoldTokens == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			foundGoldTokens.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
