using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerLastActionTimeUpdated : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 295;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLastActionTimeUpdated(reader);
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

	private const int OPCODE = 295;

	private ActionTime actionTime;

	private ServerLastActionTimeUpdated(MessageReader reader)
	{
		actionTime = new ActionTime(reader);
	}

	public ServerLastActionTimeUpdated(ActionTime actionTime)
	{
		this.actionTime = actionTime;
	}

	public ActionTime getActionTime()
	{
		return actionTime;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += actionTime.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(295);
		messageWriter.WriteInt16((short)(num - 4));
		actionTime.Write(messageWriter);
		return messageWriter.GetData();
	}
}
