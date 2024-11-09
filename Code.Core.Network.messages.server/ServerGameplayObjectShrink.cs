using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerGameplayObjectShrink : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 417;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGameplayObjectShrink(reader);
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

	private const int OPCODE = 417;

	private int objectId;

	private vector3 targetScale;

	private long shrinkEndTimeStamp;

	private ServerGameplayObjectShrink(MessageReader reader)
	{
		objectId = reader.ReadInt32();
		targetScale = new vector3(reader);
		shrinkEndTimeStamp = reader.ReadInt64();
	}

	public ServerGameplayObjectShrink(int objectId, vector3 targetScale, long shrinkEndTimeStamp)
	{
		this.objectId = objectId;
		this.targetScale = targetScale;
		this.shrinkEndTimeStamp = shrinkEndTimeStamp;
	}

	public int getObjectId()
	{
		return objectId;
	}

	public vector3 getTargetScale()
	{
		return targetScale;
	}

	public long getShrinkEndTimeStamp()
	{
		return shrinkEndTimeStamp;
	}

	public byte[] GetData()
	{
		int num = 16;
		num += targetScale.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(417);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(objectId);
		targetScale.Write(messageWriter);
		messageWriter.WriteInt64(shrinkEndTimeStamp);
		return messageWriter.GetData();
	}
}
