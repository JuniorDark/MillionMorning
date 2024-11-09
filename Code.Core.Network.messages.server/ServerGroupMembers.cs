using System;
using System.Collections.Generic;
using System.Linq;

namespace Code.Core.Network.messages.server;

public class ServerGroupMembers : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 356;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerGroupMembers(reader);
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

	private const int OPCODE = 356;

	private readonly string _leader;

	private readonly IList<string> _members;

	private ServerGroupMembers(MessageReader reader)
	{
		_leader = reader.ReadString();
		_members = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_members.Add(reader.ReadString());
		}
	}

	public ServerGroupMembers(string leader, IList<string> members)
	{
		_leader = leader;
		_members = members;
	}

	public string GetLeader()
	{
		return _leader;
	}

	public IList<string> GetMembers()
	{
		return _members;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(_leader);
		num += (short)(2 * _members.Count);
		num = _members.Aggregate(num, (int current, string membersElem) => current + MessageWriter.GetSize(membersElem));
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(356);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(_leader);
		messageWriter.WriteInt16((short)_members.Count);
		foreach (string member in _members)
		{
			messageWriter.WriteString(member);
		}
		return messageWriter.GetData();
	}
}
