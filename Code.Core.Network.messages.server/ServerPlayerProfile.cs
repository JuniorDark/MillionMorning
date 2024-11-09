using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerPlayerProfile : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 201;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerProfile(reader);
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

	private const int OPCODE = 201;

	private readonly sbyte _allowed;

	private readonly string _playerId;

	private readonly string _avatarName;

	private readonly string _title;

	private readonly IList<string> _medals;

	private readonly IList<FoundTokensInfo> _explorationTokens;

	private readonly IList<InventoryEntry> _favoriteItems;

	private readonly string _mood;

	private readonly sbyte _role;

	private readonly int _memberDaysLeft;

	private readonly int _avatarLevel;

	private ServerPlayerProfile(MessageReader reader)
	{
		_allowed = reader.ReadInt8();
		_playerId = reader.ReadString();
		_avatarName = reader.ReadString();
		_title = reader.ReadString();
		_medals = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_medals.Add(reader.ReadString());
		}
		_explorationTokens = new List<FoundTokensInfo>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_explorationTokens.Add(new FoundTokensInfo(reader));
		}
		_favoriteItems = new List<InventoryEntry>();
		short num5 = reader.ReadInt16();
		for (short num6 = 0; num6 < num5; num6++)
		{
			_favoriteItems.Add(InventoryEntry.Create(reader.ReadTypeCode(), reader));
		}
		_mood = reader.ReadString();
		_role = reader.ReadInt8();
		_memberDaysLeft = reader.ReadInt32();
		_avatarLevel = reader.ReadInt32();
	}

	public ServerPlayerProfile(sbyte allowed, string playerId, string avatarName, string title, IList<string> medals, IList<FoundTokensInfo> explorationTokens, IList<InventoryEntry> favoriteItems, string mood, sbyte role, int memberDaysLeft, int avatarLevel)
	{
		_allowed = allowed;
		_playerId = playerId;
		_avatarName = avatarName;
		_title = title;
		_medals = medals;
		_explorationTokens = explorationTokens;
		_favoriteItems = favoriteItems;
		_mood = mood;
		_role = role;
		_memberDaysLeft = memberDaysLeft;
		_avatarLevel = avatarLevel;
	}

	public sbyte GetAllowed()
	{
		return _allowed;
	}

	public string GetPlayerId()
	{
		return _playerId;
	}

	public string GetAvatarName()
	{
		return _avatarName;
	}

	public string GetTitle()
	{
		return _title;
	}

	public IList<string> GetMedals()
	{
		return _medals;
	}

	public IList<FoundTokensInfo> GetExplorationTokens()
	{
		return _explorationTokens;
	}

	public IList<InventoryEntry> GetFavoriteItems()
	{
		return _favoriteItems;
	}

	public string GetMood()
	{
		return _mood;
	}

	public sbyte GetRole()
	{
		return _role;
	}

	public int GetMemberDaysLeft()
	{
		return _memberDaysLeft;
	}

	public int GetAvatarLevel()
	{
		return _avatarLevel;
	}

	public byte[] GetData()
	{
		int num = 28;
		num += MessageWriter.GetSize(_playerId);
		num += MessageWriter.GetSize(_avatarName);
		num += MessageWriter.GetSize(_title);
		num += (short)(2 * _medals.Count);
		foreach (string medal in _medals)
		{
			num += MessageWriter.GetSize(medal);
		}
		foreach (FoundTokensInfo explorationToken in _explorationTokens)
		{
			num += explorationToken.Size();
		}
		num += (short)(_favoriteItems.Count * 2);
		foreach (InventoryEntry favoriteItem in _favoriteItems)
		{
			num += favoriteItem.Size();
		}
		num += MessageWriter.GetSize(_mood);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(201);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt8(_allowed);
		messageWriter.WriteString(_playerId);
		messageWriter.WriteString(_avatarName);
		messageWriter.WriteString(_title);
		messageWriter.WriteInt16((short)_medals.Count);
		foreach (string medal2 in _medals)
		{
			messageWriter.WriteString(medal2);
		}
		messageWriter.WriteInt16((short)_explorationTokens.Count);
		foreach (FoundTokensInfo explorationToken2 in _explorationTokens)
		{
			explorationToken2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)_favoriteItems.Count);
		foreach (InventoryEntry favoriteItem2 in _favoriteItems)
		{
			messageWriter.WriteTypeCode(favoriteItem2.GetTypeId());
			favoriteItem2.Write(messageWriter);
		}
		messageWriter.WriteString(_mood);
		messageWriter.WriteInt8(_role);
		messageWriter.WriteInt32(_memberDaysLeft);
		messageWriter.WriteInt32(_avatarLevel);
		return messageWriter.GetData();
	}
}
