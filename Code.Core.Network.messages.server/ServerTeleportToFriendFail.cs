using System;

namespace Code.Core.Network.messages.server;

public class ServerTeleportToFriendFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 267;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerTeleportToFriendFail(reader);
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

	private const int OPCODE = 267;

	private sbyte reason;

	private string additionalInfo;

	private ServerTeleportToFriendFail(MessageReader reader)
	{
		reason = reader.ReadInt8();
		additionalInfo = reader.ReadString();
	}

	public ServerTeleportToFriendFail(sbyte reason, string additionalInfo)
	{
		this.reason = reason;
		this.additionalInfo = additionalInfo;
	}

	public sbyte getReason()
	{
		return reason;
	}

	public string getAdditionalInfo()
	{
		return additionalInfo;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(additionalInfo);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(267);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt8(reason);
		messageWriter.WriteString(additionalInfo);
		return messageWriter.GetData();
	}
}
