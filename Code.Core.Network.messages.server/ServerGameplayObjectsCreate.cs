using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerGameplayObjectsCreate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 154;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGameplayObjectsCreate(reader);
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

	private const int OPCODE = 154;

	private IList<GameplayObject> gameplayObjects;

	private ServerGameplayObjectsCreate(MessageReader reader)
	{
		gameplayObjects = new List<GameplayObject>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			gameplayObjects.Add(new GameplayObject(reader));
		}
	}

	public ServerGameplayObjectsCreate(IList<GameplayObject> gameplayObjects)
	{
		this.gameplayObjects = gameplayObjects;
	}

	public IList<GameplayObject> getGameplayObjects()
	{
		return gameplayObjects;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (GameplayObject gameplayObject in gameplayObjects)
		{
			num += gameplayObject.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(154);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)gameplayObjects.Count);
		foreach (GameplayObject gameplayObject2 in gameplayObjects)
		{
			gameplayObject2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
