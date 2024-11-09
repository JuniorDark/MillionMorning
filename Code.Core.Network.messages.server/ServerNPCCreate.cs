using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerNPCCreate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 68;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerNPCCreate(reader);
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

	private const int OPCODE = 68;

	private IList<Npc> npcs;

	private ServerNPCCreate(MessageReader reader)
	{
		npcs = new List<Npc>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			npcs.Add(new Npc(reader));
		}
	}

	public ServerNPCCreate(IList<Npc> npcs)
	{
		this.npcs = npcs;
	}

	public IList<Npc> getNpcs()
	{
		return npcs;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (Npc npc in npcs)
		{
			num += npc.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(68);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)npcs.Count);
		foreach (Npc npc2 in npcs)
		{
			npc2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
