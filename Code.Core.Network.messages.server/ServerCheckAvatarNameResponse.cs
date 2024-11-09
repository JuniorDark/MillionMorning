using System;

namespace Code.Core.Network.messages.server;

public class ServerCheckAvatarNameResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 179;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCheckAvatarNameResponse(reader);
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

	private const int OPCODE = 179;

	private string avatarName;

	private sbyte response;

	private ServerCheckAvatarNameResponse(MessageReader reader)
	{
		avatarName = reader.ReadString();
		response = reader.ReadInt8();
	}

	public ServerCheckAvatarNameResponse(string avatarName, sbyte response)
	{
		this.avatarName = avatarName;
		this.response = response;
	}

	public string getAvatarName()
	{
		return avatarName;
	}

	public sbyte getResponse()
	{
		return response;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(avatarName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(179);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(avatarName);
		messageWriter.WriteInt8(response);
		return messageWriter.GetData();
	}
}
