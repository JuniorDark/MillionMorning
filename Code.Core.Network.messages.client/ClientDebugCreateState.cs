using System;

namespace Code.Core.Network.messages.client;

public class ClientDebugCreateState : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 338;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientDebugCreateState(reader);
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

	private const int OPCODE = 338;

	private string state;

	private string target;

	private sbyte targetType;

	private ClientDebugCreateState(MessageReader reader)
	{
		state = reader.ReadString();
		target = reader.ReadString();
		targetType = reader.ReadInt8();
	}

	public ClientDebugCreateState(string state, string target, sbyte targetType)
	{
		this.state = state;
		this.target = target;
		this.targetType = targetType;
	}

	public string getState()
	{
		return state;
	}

	public string getTarget()
	{
		return target;
	}

	public sbyte getTargetType()
	{
		return targetType;
	}

	public byte[] GetData()
	{
		int num = 9;
		num += MessageWriter.GetSize(state);
		num += MessageWriter.GetSize(target);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(338);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(state);
		messageWriter.WriteString(target);
		messageWriter.WriteInt8(targetType);
		return messageWriter.GetData();
	}
}
