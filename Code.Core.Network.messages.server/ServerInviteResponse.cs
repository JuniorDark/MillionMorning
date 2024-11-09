using System;

namespace Code.Core.Network.messages.server;

public class ServerInviteResponse : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 105;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerInviteResponse(reader);
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

	private const int OPCODE = 105;

	private string URL;

	private string email;

	private int id;

	private string inviteKey;

	private ServerInviteResponse(MessageReader reader)
	{
		URL = reader.ReadString();
		email = reader.ReadString();
		id = reader.ReadInt32();
		inviteKey = reader.ReadString();
	}

	public ServerInviteResponse(string URL, string email, int id, string inviteKey)
	{
		this.URL = URL;
		this.email = email;
		this.id = id;
		this.inviteKey = inviteKey;
	}

	public string getURL()
	{
		return URL;
	}

	public string getEmail()
	{
		return email;
	}

	public int getId()
	{
		return id;
	}

	public string getInviteKey()
	{
		return inviteKey;
	}

	public byte[] GetData()
	{
		int num = 14;
		num += MessageWriter.GetSize(URL);
		num += MessageWriter.GetSize(email);
		num += MessageWriter.GetSize(inviteKey);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(105);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(URL);
		messageWriter.WriteString(email);
		messageWriter.WriteInt32(id);
		messageWriter.WriteString(inviteKey);
		return messageWriter.GetData();
	}
}
