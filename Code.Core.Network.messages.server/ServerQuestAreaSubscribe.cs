using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerQuestAreaSubscribe : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 173;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerQuestAreaSubscribe(reader);
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

	private const int OPCODE = 173;

	private string fullAreaName;

	private vector3 center;

	private float radiusSquared;

	private float height;

	private ServerQuestAreaSubscribe(MessageReader reader)
	{
		fullAreaName = reader.ReadString();
		center = new vector3(reader);
		radiusSquared = reader.ReadFloat();
		height = reader.ReadFloat();
	}

	public ServerQuestAreaSubscribe(string fullAreaName, vector3 center, float radiusSquared, float height)
	{
		this.fullAreaName = fullAreaName;
		this.center = center;
		this.radiusSquared = radiusSquared;
		this.height = height;
	}

	public string getFullAreaName()
	{
		return fullAreaName;
	}

	public vector3 getCenter()
	{
		return center;
	}

	public float getRadiusSquared()
	{
		return radiusSquared;
	}

	public float getHeight()
	{
		return height;
	}

	public byte[] GetData()
	{
		int num = 26;
		num += MessageWriter.GetSize(fullAreaName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(173);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(fullAreaName);
		center.Write(messageWriter);
		messageWriter.WriteFloat(radiusSquared);
		messageWriter.WriteFloat(height);
		return messageWriter.GetData();
	}
}
