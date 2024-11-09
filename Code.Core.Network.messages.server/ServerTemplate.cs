using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerTemplate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 29;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerTemplate(reader);
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

	private const int OPCODE = 29;

	private readonly Code.Core.Network.types.Template _template;

	private ServerTemplate(MessageReader reader)
	{
		_template = Code.Core.Network.types.Template.Create(reader.ReadTypeCode(), reader);
	}

	public ServerTemplate(Code.Core.Network.types.Template template)
	{
		_template = template;
	}

	public Code.Core.Network.types.Template GetTemplate()
	{
		return _template;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += _template.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(29);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteTypeCode(_template.GetTypeId());
		_template.Write(messageWriter);
		return messageWriter.GetData();
	}
}
