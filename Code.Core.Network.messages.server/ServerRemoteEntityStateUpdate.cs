using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerRemoteEntityStateUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 336;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerRemoteEntityStateUpdate(reader);
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

	private const int OPCODE = 336;

	private string entityId;

	private sbyte isAPlayer;

	private sbyte isToAdd;

	private IList<EntityStateEffectReference> effects;

	private ServerRemoteEntityStateUpdate(MessageReader reader)
	{
		entityId = reader.ReadString();
		isAPlayer = reader.ReadInt8();
		isToAdd = reader.ReadInt8();
		effects = new List<EntityStateEffectReference>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			effects.Add(new EntityStateEffectReference(reader));
		}
	}

	public ServerRemoteEntityStateUpdate(string entityId, sbyte isAPlayer, sbyte isToAdd, IList<EntityStateEffectReference> effects)
	{
		this.entityId = entityId;
		this.isAPlayer = isAPlayer;
		this.isToAdd = isToAdd;
		this.effects = effects;
	}

	public string getEntityId()
	{
		return entityId;
	}

	public sbyte getIsAPlayer()
	{
		return isAPlayer;
	}

	public sbyte getIsToAdd()
	{
		return isToAdd;
	}

	public IList<EntityStateEffectReference> getEffects()
	{
		return effects;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(entityId);
		foreach (EntityStateEffectReference effect in effects)
		{
			num += effect.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(336);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(entityId);
		messageWriter.WriteInt8(isAPlayer);
		messageWriter.WriteInt8(isToAdd);
		messageWriter.WriteInt16((short)effects.Count);
		foreach (EntityStateEffectReference effect2 in effects)
		{
			effect2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
