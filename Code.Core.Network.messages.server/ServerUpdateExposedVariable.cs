using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerUpdateExposedVariable : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 331;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerUpdateExposedVariable(reader);
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

	private const int OPCODE = 331;

	private ExposedVariableUpdate variableUpdate;

	private ServerUpdateExposedVariable(MessageReader reader)
	{
		variableUpdate = new ExposedVariableUpdate(reader);
	}

	public ServerUpdateExposedVariable(ExposedVariableUpdate variableUpdate)
	{
		this.variableUpdate = variableUpdate;
	}

	public ExposedVariableUpdate getVariableUpdate()
	{
		return variableUpdate;
	}

	public byte[] GetData()
	{
		int num = 4;
		num += variableUpdate.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(331);
		messageWriter.WriteInt16((short)(num - 4));
		variableUpdate.Write(messageWriter);
		return messageWriter.GetData();
	}
}
