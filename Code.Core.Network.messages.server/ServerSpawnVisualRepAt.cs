using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerSpawnVisualRepAt : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 278;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerSpawnVisualRepAt(reader);
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

	private const int OPCODE = 278;

	private string visualRep;

	private vector3 position;

	private vector3 rotation;

	private ServerSpawnVisualRepAt(MessageReader reader)
	{
		visualRep = reader.ReadString();
		position = new vector3(reader);
		rotation = new vector3(reader);
	}

	public ServerSpawnVisualRepAt(string visualRep, vector3 position, vector3 rotation)
	{
		this.visualRep = visualRep;
		this.position = position;
		this.rotation = rotation;
	}

	public string getVisualRep()
	{
		return visualRep;
	}

	public vector3 getPosition()
	{
		return position;
	}

	public vector3 getRotation()
	{
		return rotation;
	}

	public byte[] GetData()
	{
		int num = 30;
		num += MessageWriter.GetSize(visualRep);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(278);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(visualRep);
		position.Write(messageWriter);
		rotation.Write(messageWriter);
		return messageWriter.GetData();
	}
}
