using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerEntityStateEffectAdded : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 337;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerEntityStateEffectAdded(reader);
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

	private const int OPCODE = 337;

	private string entityId;

	private int stateId;

	private IList<EntityStateEffectReference> effects;

	private ServerEntityStateEffectAdded(MessageReader reader)
	{
		entityId = reader.ReadString();
		stateId = reader.ReadInt32();
		effects = new List<EntityStateEffectReference>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			effects.Add(new EntityStateEffectReference(reader));
		}
	}

	public ServerEntityStateEffectAdded(string entityId, int stateId, IList<EntityStateEffectReference> effects)
	{
		this.entityId = entityId;
		this.stateId = stateId;
		this.effects = effects;
	}

	public string getEntityId()
	{
		return entityId;
	}

	public int getStateId()
	{
		return stateId;
	}

	public IList<EntityStateEffectReference> getEffects()
	{
		return effects;
	}

	public byte[] GetData()
	{
		int num = 12;
		num += MessageWriter.GetSize(entityId);
		foreach (EntityStateEffectReference effect in effects)
		{
			num += effect.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(337);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(entityId);
		messageWriter.WriteInt32(stateId);
		messageWriter.WriteInt16((short)effects.Count);
		foreach (EntityStateEffectReference effect2 in effects)
		{
			effect2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
