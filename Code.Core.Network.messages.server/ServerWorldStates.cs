using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerWorldStates : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 225;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerWorldStates(reader);
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

	private const int OPCODE = 225;

	private IList<WorldLevel> worldLevels;

	private ServerWorldStates(MessageReader reader)
	{
		worldLevels = new List<WorldLevel>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			worldLevels.Add(new WorldLevel(reader));
		}
	}

	public ServerWorldStates(IList<WorldLevel> worldLevels)
	{
		this.worldLevels = worldLevels;
	}

	public IList<WorldLevel> getWorldLevels()
	{
		return worldLevels;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (WorldLevel worldLevel in worldLevels)
		{
			num += worldLevel.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(225);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)worldLevels.Count);
		foreach (WorldLevel worldLevel2 in worldLevels)
		{
			worldLevel2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
