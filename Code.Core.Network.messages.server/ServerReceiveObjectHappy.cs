using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerReceiveObjectHappy : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 42;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerReceiveObjectHappy(reader);
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

	private const int OPCODE = 42;

	private readonly string _playerID;

	private readonly TemplateReference _reference;

	private ServerReceiveObjectHappy(MessageReader reader)
	{
		_playerID = reader.ReadString();
		_reference = new TemplateReference(reader);
	}

	public ServerReceiveObjectHappy(string playerID, TemplateReference reference)
	{
		_playerID = playerID;
		_reference = reference;
	}

	public string GetPlayerID()
	{
		return _playerID;
	}

	public TemplateReference GetReference()
	{
		return _reference;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(_playerID);
		num += _reference.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(42);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(_playerID);
		_reference.Write(messageWriter);
		return messageWriter.GetData();
	}
}
