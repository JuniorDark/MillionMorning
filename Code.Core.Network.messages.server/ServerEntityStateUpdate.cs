using System;
using System.Collections.Generic;

namespace Code.Core.Network.messages.server;

public class ServerEntityStateUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 335;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerEntityStateUpdate(reader);
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

	private const int OPCODE = 335;

	private string entityId;

	private IList<int> statesToTurnOff;

	private IList<int> effectsToTurnOff;

	private ServerEntityStateUpdate(MessageReader reader)
	{
		entityId = reader.ReadString();
		statesToTurnOff = new List<int>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			statesToTurnOff.Add(reader.ReadInt32());
		}
		effectsToTurnOff = new List<int>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			effectsToTurnOff.Add(reader.ReadInt32());
		}
	}

	public ServerEntityStateUpdate(string entityId, IList<int> statesToTurnOff, IList<int> effectsToTurnOff)
	{
		this.entityId = entityId;
		this.statesToTurnOff = statesToTurnOff;
		this.effectsToTurnOff = effectsToTurnOff;
	}

	public string getEntityId()
	{
		return entityId;
	}

	public IList<int> getStatesToTurnOff()
	{
		return statesToTurnOff;
	}

	public IList<int> getEffectsToTurnOff()
	{
		return effectsToTurnOff;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(entityId);
		num += (short)(statesToTurnOff.Count * 4);
		num += (short)(effectsToTurnOff.Count * 4);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(335);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(entityId);
		messageWriter.WriteInt16((short)statesToTurnOff.Count);
		foreach (int item in statesToTurnOff)
		{
			messageWriter.WriteInt32(item);
		}
		messageWriter.WriteInt16((short)effectsToTurnOff.Count);
		foreach (int item2 in effectsToTurnOff)
		{
			messageWriter.WriteInt32(item2);
		}
		return messageWriter.GetData();
	}
}
