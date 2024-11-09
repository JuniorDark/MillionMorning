using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerInviteItemReceived : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 285;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerInviteItemReceived(reader);
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

	private const int OPCODE = 285;

	private IList<InviteItem> inviteItems;

	private IList<InviteItem> acceptItems;

	private InviteItem nextInviteItems;

	private InviteItem nextAcceptItems;

	private int totalInvites;

	private int totalAccepts;

	private ServerInviteItemReceived(MessageReader reader)
	{
		inviteItems = new List<InviteItem>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			inviteItems.Add(new InviteItem(reader));
		}
		acceptItems = new List<InviteItem>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			acceptItems.Add(new InviteItem(reader));
		}
		if (reader.ReadInt8() == 1)
		{
			nextInviteItems = new InviteItem(reader);
		}
		if (reader.ReadInt8() == 1)
		{
			nextAcceptItems = new InviteItem(reader);
		}
		totalInvites = reader.ReadInt32();
		totalAccepts = reader.ReadInt32();
	}

	public ServerInviteItemReceived(IList<InviteItem> inviteItems, IList<InviteItem> acceptItems, InviteItem nextInviteItems, InviteItem nextAcceptItems, int totalInvites, int totalAccepts)
	{
		this.inviteItems = inviteItems;
		this.acceptItems = acceptItems;
		this.nextInviteItems = nextInviteItems;
		this.nextAcceptItems = nextAcceptItems;
		this.totalInvites = totalInvites;
		this.totalAccepts = totalAccepts;
	}

	public IList<InviteItem> getInviteItems()
	{
		return inviteItems;
	}

	public IList<InviteItem> getAcceptItems()
	{
		return acceptItems;
	}

	public InviteItem getNextInviteItems()
	{
		return nextInviteItems;
	}

	public InviteItem getNextAcceptItems()
	{
		return nextAcceptItems;
	}

	public int getTotalInvites()
	{
		return totalInvites;
	}

	public int getTotalAccepts()
	{
		return totalAccepts;
	}

	public byte[] GetData()
	{
		int num = 18;
		foreach (InviteItem inviteItem in inviteItems)
		{
			num += inviteItem.Size();
		}
		foreach (InviteItem acceptItem in acceptItems)
		{
			num += acceptItem.Size();
		}
		if (nextInviteItems != null)
		{
			num += nextInviteItems.Size();
		}
		if (nextAcceptItems != null)
		{
			num += nextAcceptItems.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(285);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)inviteItems.Count);
		foreach (InviteItem inviteItem2 in inviteItems)
		{
			inviteItem2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)acceptItems.Count);
		foreach (InviteItem acceptItem2 in acceptItems)
		{
			acceptItem2.Write(messageWriter);
		}
		if (nextInviteItems == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			nextInviteItems.Write(messageWriter);
		}
		if (nextAcceptItems == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			nextAcceptItems.Write(messageWriter);
		}
		messageWriter.WriteInt32(totalInvites);
		messageWriter.WriteInt32(totalAccepts);
		return messageWriter.GetData();
	}
}
