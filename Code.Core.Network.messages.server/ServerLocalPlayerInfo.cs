using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerLocalPlayerInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 24;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLocalPlayerInfo(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 4;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt32();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 24;

	private readonly string _id;

	private readonly int _juneCoins;

	private readonly sbyte _membershipStatus;

	private readonly int _membershipTimeLeftInDays;

	private readonly sbyte _role;

	private readonly int _privacyLevel;

	private readonly string _messageOfTheDay;

	private readonly FeaturedNewsItem _featuredNews;

	private readonly string _newsImage;

	private readonly string _newsImageLink;

	private readonly FullAvatar _avatar;

	private readonly IList<string> _seenSlides;

	private readonly IList<string> _hotkeys;

	private readonly NullableInt _wieldedItem;

	private readonly int _coins;

	private readonly long _playedSeconds;

	private readonly long _timeSinceLastIngame;

	private readonly IList<ABTestGroup> _abTestGroups;

	private readonly IList<InviteItem> _acceptedInviteRewards;

	private readonly IList<InviteItem> _sentInviteRewards;

	private readonly sbyte _hasPlayed;

	public long PlayedSeconds => _playedSeconds;

	private ServerLocalPlayerInfo(MessageReader reader)
	{
		_id = reader.ReadString();
		_juneCoins = reader.ReadInt32();
		_membershipStatus = reader.ReadInt8();
		_membershipTimeLeftInDays = reader.ReadInt32();
		_role = reader.ReadInt8();
		_privacyLevel = reader.ReadInt32();
		_messageOfTheDay = reader.ReadString();
		if (reader.ReadInt8() == 1)
		{
			_featuredNews = new FeaturedNewsItem(reader);
		}
		_newsImage = reader.ReadString();
		_newsImageLink = reader.ReadString();
		if (reader.ReadInt8() == 1)
		{
			_avatar = new FullAvatar(reader);
		}
		_seenSlides = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_seenSlides.Add(reader.ReadString());
		}
		_hotkeys = new List<string>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_hotkeys.Add(reader.ReadString());
		}
		if (reader.ReadInt8() == 1)
		{
			_wieldedItem = new NullableInt(reader);
		}
		_coins = reader.ReadInt32();
		_playedSeconds = reader.ReadInt64();
		_timeSinceLastIngame = reader.ReadInt64();
		_abTestGroups = new List<ABTestGroup>();
		short num5 = reader.ReadInt16();
		for (short num6 = 0; num6 < num5; num6++)
		{
			_abTestGroups.Add(new ABTestGroup(reader));
		}
		_acceptedInviteRewards = new List<InviteItem>();
		short num7 = reader.ReadInt16();
		for (short num8 = 0; num8 < num7; num8++)
		{
			_acceptedInviteRewards.Add(new InviteItem(reader));
		}
		_sentInviteRewards = new List<InviteItem>();
		short num9 = reader.ReadInt16();
		for (short num10 = 0; num10 < num9; num10++)
		{
			_sentInviteRewards.Add(new InviteItem(reader));
		}
		_hasPlayed = reader.ReadInt8();
	}

	public string GetId()
	{
		return _id;
	}

	public int GetJuneCoins()
	{
		return _juneCoins;
	}

	public sbyte GetMembershipStatus()
	{
		return _membershipStatus;
	}

	public int GetMembershipTimeLeftInDays()
	{
		return _membershipTimeLeftInDays;
	}

	public sbyte GetRole()
	{
		return _role;
	}

	public int GetPrivacyLevel()
	{
		return _privacyLevel;
	}

	public string GetMessageOfTheDay()
	{
		return _messageOfTheDay;
	}

	public FeaturedNewsItem GetFeaturedNews()
	{
		return _featuredNews;
	}

	public string GetNewsImage()
	{
		return _newsImage;
	}

	public string GetNewsImageLink()
	{
		return _newsImageLink;
	}

	public FullAvatar GetAvatar()
	{
		return _avatar;
	}

	public bool HasAvatar()
	{
		return _avatar != null;
	}

	public IList<string> GetSeenSlides()
	{
		return _seenSlides;
	}

	public IList<string> GetHotkeys()
	{
		return _hotkeys;
	}

	public NullableInt GetWieldedItem()
	{
		return _wieldedItem;
	}

	public int GetCoins()
	{
		return _coins;
	}

	public long GetTimeSinceLastIngame()
	{
		return _timeSinceLastIngame;
	}

	public IList<ABTestGroup> GetABTestGroups()
	{
		return _abTestGroups;
	}

	public IList<InviteItem> GetAcceptedInviteRewards()
	{
		return _acceptedInviteRewards;
	}

	public IList<InviteItem> GetSentInviteRewards()
	{
		return _sentInviteRewards;
	}

	public sbyte GetHasPlayed()
	{
		return _hasPlayed;
	}

	public byte[] GetData()
	{
		int num = 62;
		num += MessageWriter.GetSize(_id);
		num += MessageWriter.GetSize(_messageOfTheDay);
		if (_featuredNews != null)
		{
			num += _featuredNews.Size();
		}
		num += MessageWriter.GetSize(_newsImage);
		num += MessageWriter.GetSize(_newsImageLink);
		if (_avatar != null)
		{
			num += _avatar.Size();
		}
		num += (short)(2 * _seenSlides.Count);
		foreach (string seenSlide in _seenSlides)
		{
			num += MessageWriter.GetSize(seenSlide);
		}
		num += (short)(2 * _hotkeys.Count);
		foreach (string hotkey in _hotkeys)
		{
			num += MessageWriter.GetSize(hotkey);
		}
		if (_wieldedItem != null)
		{
			num += 4;
		}
		num += (short)(_abTestGroups.Count * 5);
		foreach (InviteItem acceptedInviteReward in _acceptedInviteRewards)
		{
			num += acceptedInviteReward.Size();
		}
		foreach (InviteItem sentInviteReward in _sentInviteRewards)
		{
			num += sentInviteReward.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(24);
		messageWriter.WriteInt32(num - 6);
		messageWriter.WriteString(_id);
		messageWriter.WriteInt32(_juneCoins);
		messageWriter.WriteInt8(_membershipStatus);
		messageWriter.WriteInt32(_membershipTimeLeftInDays);
		messageWriter.WriteInt8(_role);
		messageWriter.WriteInt32(_privacyLevel);
		messageWriter.WriteString(_messageOfTheDay);
		if (_featuredNews == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			_featuredNews.Write(messageWriter);
		}
		messageWriter.WriteString(_newsImage);
		messageWriter.WriteString(_newsImageLink);
		if (_avatar == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			_avatar.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)_seenSlides.Count);
		foreach (string seenSlide2 in _seenSlides)
		{
			messageWriter.WriteString(seenSlide2);
		}
		messageWriter.WriteInt16((short)_hotkeys.Count);
		foreach (string hotkey2 in _hotkeys)
		{
			messageWriter.WriteString(hotkey2);
		}
		if (_wieldedItem == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			_wieldedItem.Write(messageWriter);
		}
		messageWriter.WriteInt32(_coins);
		messageWriter.WriteInt64(_playedSeconds);
		messageWriter.WriteInt64(_timeSinceLastIngame);
		messageWriter.WriteInt16((short)_abTestGroups.Count);
		foreach (ABTestGroup abTestGroup in _abTestGroups)
		{
			abTestGroup.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)_acceptedInviteRewards.Count);
		foreach (InviteItem acceptedInviteReward2 in _acceptedInviteRewards)
		{
			acceptedInviteReward2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)_sentInviteRewards.Count);
		foreach (InviteItem sentInviteReward2 in _sentInviteRewards)
		{
			sentInviteReward2.Write(messageWriter);
		}
		messageWriter.WriteInt8(_hasPlayed);
		return messageWriter.GetData();
	}
}
