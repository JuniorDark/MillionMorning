using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerNPCMessage : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 10;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerNPCMessage(reader);
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

	private const int OPCODE = 10;

	private readonly int _npcID;

	private readonly sbyte _type;

	private readonly sbyte _customData;

	private readonly string _voice;

	private readonly IList<NpcMessagePart> _parts;

	private readonly IList<NpcLevelOffer> _levelOffers;

	private ServerNPCMessage(MessageReader reader)
	{
		_npcID = reader.ReadInt32();
		_type = reader.ReadInt8();
		_customData = reader.ReadInt8();
		_voice = reader.ReadString();
		_parts = new List<NpcMessagePart>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_parts.Add(new NpcMessagePart(reader));
		}
		_levelOffers = new List<NpcLevelOffer>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_levelOffers.Add(new NpcLevelOffer(reader));
		}
	}

	public ServerNPCMessage(int npcID, sbyte type, sbyte customData, string voice, IList<NpcMessagePart> parts, IList<NpcLevelOffer> levelOffers)
	{
		_npcID = npcID;
		_type = type;
		_customData = customData;
		_voice = voice;
		_parts = parts;
		_levelOffers = levelOffers;
	}

	public int GetNpcID()
	{
		return _npcID;
	}

	public byte GetTheType()
	{
		return (byte)_type;
	}

	public sbyte GetCustomData()
	{
		return _customData;
	}

	public string GetVoice()
	{
		return _voice;
	}

	public IList<NpcMessagePart> GetParts()
	{
		return _parts;
	}

	public IList<NpcLevelOffer> GetLevelOffers()
	{
		return _levelOffers;
	}

	public byte[] GetData()
	{
		int num = 16;
		num += MessageWriter.GetSize(_voice);
		num += _parts.Sum((NpcMessagePart partsElem) => partsElem.Size());
		num += _levelOffers.Sum((NpcLevelOffer levelOffersElem) => levelOffersElem.Size());
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(10);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(_npcID);
		messageWriter.WriteInt8(_type);
		messageWriter.WriteInt8(_customData);
		messageWriter.WriteString(_voice);
		messageWriter.WriteInt16((short)_parts.Count);
		foreach (NpcMessagePart part in _parts)
		{
			part.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)_levelOffers.Count);
		foreach (NpcLevelOffer levelOffer in _levelOffers)
		{
			levelOffer.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
