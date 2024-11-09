using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerGameplayObjectStartMoving : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 175;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGameplayObjectStartMoving(reader);
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

	private const int OPCODE = 175;

	private int objectId;

	private TemplateReference spline;

	private ServerGameplayObjectStartMoving(MessageReader reader)
	{
		objectId = reader.ReadInt32();
		spline = new TemplateReference(reader);
	}

	public ServerGameplayObjectStartMoving(int objectId, TemplateReference spline)
	{
		this.objectId = objectId;
		this.spline = spline;
	}

	public int getObjectId()
	{
		return objectId;
	}

	public TemplateReference getSpline()
	{
		return spline;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += spline.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(175);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(objectId);
		spline.Write(messageWriter);
		return messageWriter.GetData();
	}
}
