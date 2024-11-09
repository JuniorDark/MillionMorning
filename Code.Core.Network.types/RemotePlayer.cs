namespace Code.Core.Network.types;

public class RemotePlayer
{
	private readonly string _playerID;

	private readonly vector3 _position;

	private readonly float _rotation;

	private readonly Avatar _avatar;

	private readonly int _memberDaysLeft;

	private readonly sbyte _role;

	private readonly Item _wieldedItem;

	private readonly string _room;

	private readonly sbyte _isClimbing;

	private readonly int _climbingSurface;

	private readonly int _onGameplayObject;

	private readonly sbyte _isInShop;

	private readonly sbyte _showBadgeIcon;

	private readonly sbyte _afk;

	public RemotePlayer(MessageReader reader)
	{
		_playerID = reader.ReadString();
		_position = new vector3(reader);
		_rotation = reader.ReadFloat();
		_avatar = new Avatar(reader);
		_memberDaysLeft = reader.ReadInt32();
		_role = reader.ReadInt8();
		if (reader.ReadInt8() == 1)
		{
			_wieldedItem = Item.Create(reader.ReadTypeCode(), reader);
		}
		_room = reader.ReadString();
		_isClimbing = reader.ReadInt8();
		_climbingSurface = reader.ReadInt32();
		_onGameplayObject = reader.ReadInt32();
		_isInShop = reader.ReadInt8();
		_showBadgeIcon = reader.ReadInt8();
		_afk = reader.ReadInt8();
	}

	public RemotePlayer(string playerID, vector3 position, float rotation, Avatar avatar, int memberDaysLeft, sbyte role, Item wieldedItem, string room, sbyte isClimbing, int climbingSurface, int onGameplayObject, sbyte isInShop, sbyte showBadgeIcon, sbyte afk)
	{
		_playerID = playerID;
		_position = position;
		_rotation = rotation;
		_avatar = avatar;
		_memberDaysLeft = memberDaysLeft;
		_role = role;
		_wieldedItem = wieldedItem;
		_room = room;
		_isClimbing = isClimbing;
		_climbingSurface = climbingSurface;
		_onGameplayObject = onGameplayObject;
		_isInShop = isInShop;
		_showBadgeIcon = showBadgeIcon;
		_afk = afk;
	}

	public string GetPlayerID()
	{
		return _playerID;
	}

	public vector3 GetPosition()
	{
		return _position;
	}

	public float GetRotation()
	{
		return _rotation;
	}

	public Avatar GetAvatar()
	{
		return _avatar;
	}

	public int GetMemberDaysLeft()
	{
		return _memberDaysLeft;
	}

	public sbyte GetRole()
	{
		return _role;
	}

	public Item GetWieldedItem()
	{
		return _wieldedItem;
	}

	public string GetRoom()
	{
		return _room;
	}

	public sbyte GetIsClimbing()
	{
		return _isClimbing;
	}

	public int GetClimbingSurface()
	{
		return _climbingSurface;
	}

	public int GetOnGameplayObject()
	{
		return _onGameplayObject;
	}

	public sbyte GetIsInShop()
	{
		return _isInShop;
	}

	public sbyte GetShowBadgeIcon()
	{
		return _showBadgeIcon;
	}

	public bool IsAfk()
	{
		return _afk == 1;
	}

	public int Size()
	{
		int num = 38;
		num += MessageWriter.GetSize(_playerID);
		num += _avatar.Size();
		if (_wieldedItem != null)
		{
			num += 2;
			num += _wieldedItem.Size();
		}
		return num + MessageWriter.GetSize(_room);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_playerID);
		_position.Write(writer);
		writer.WriteFloat(_rotation);
		_avatar.Write(writer);
		writer.WriteInt32(_memberDaysLeft);
		writer.WriteInt8(_role);
		if (_wieldedItem == null)
		{
			writer.WriteInt8(0);
		}
		else
		{
			writer.WriteInt8(1);
			writer.WriteTypeCode(_wieldedItem.GetTypeId());
			_wieldedItem.Write(writer);
		}
		writer.WriteString(_room);
		writer.WriteInt8(_isClimbing);
		writer.WriteInt32(_climbingSurface);
		writer.WriteInt32(_onGameplayObject);
		writer.WriteInt8(_isInShop);
		writer.WriteInt8(_showBadgeIcon);
		writer.WriteInt8(_afk);
	}
}
