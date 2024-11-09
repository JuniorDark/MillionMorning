using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerObjectMoveableCreate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 66;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerObjectMoveableCreate(reader);
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

	private const int OPCODE = 66;

	private LevelObject theObject;

	private vector3 targetPosition;

	private float speed;

	private ServerObjectMoveableCreate(MessageReader reader)
	{
		theObject = LevelObject.Create(reader.ReadTypeCode(), reader);
		targetPosition = new vector3(reader);
		speed = reader.ReadFloat();
	}

	public ServerObjectMoveableCreate(LevelObject theObject, vector3 targetPosition, float speed)
	{
		this.theObject = theObject;
		this.targetPosition = targetPosition;
		this.speed = speed;
	}

	public LevelObject getTheObject()
	{
		return theObject;
	}

	public vector3 getTargetPosition()
	{
		return targetPosition;
	}

	public float getSpeed()
	{
		return speed;
	}

	public byte[] GetData()
	{
		int num = 22;
		num += theObject.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(66);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteTypeCode(theObject.GetTypeId());
		theObject.Write(messageWriter);
		targetPosition.Write(messageWriter);
		messageWriter.WriteFloat(speed);
		return messageWriter.GetData();
	}
}
