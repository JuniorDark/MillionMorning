using System.Collections.Generic;

namespace Code.Core.Network.types;

public class Avatar
{
	private readonly string _name;

	private readonly sbyte _gender;

	private readonly string _skinColor;

	private readonly int _hairColor;

	private readonly string _eyeColor;

	private readonly string _mouth;

	private readonly string _eyes;

	private readonly string _eyeBrows;

	private readonly string _hair;

	private readonly float _height;

	private readonly float _health;

	private readonly float _maxHealth;

	private readonly string _mood;

	private readonly string _title;

	private readonly int _avatarLevel;

	private readonly int _gems;

	private readonly int _teleportStones;

	private readonly IList<Item> _items;

	public Avatar(FullAvatar fullAvatar)
		: this(fullAvatar.GetName(), fullAvatar.GetGender(), fullAvatar.GetSkinColor(), fullAvatar.GetHairColor(), fullAvatar.GetEyeColor(), fullAvatar.GetMouth(), fullAvatar.GetEyes(), fullAvatar.GetEyeBrows(), fullAvatar.GetHair(), fullAvatar.GetHeight(), fullAvatar.GetHealth(), fullAvatar.GetMaxHealth(), fullAvatar.GetMood(), fullAvatar.GetTitle(), fullAvatar.GetAvatarLevel(), fullAvatar.GetGems(), fullAvatar.GetTeleportStones(), fullAvatar.GetEquippedItems())
	{
	}

	public Avatar(MessageReader reader)
	{
		_name = reader.ReadString();
		_gender = reader.ReadInt8();
		_skinColor = reader.ReadString();
		_hairColor = reader.ReadInt32();
		_eyeColor = reader.ReadString();
		_mouth = reader.ReadString();
		_eyes = reader.ReadString();
		_eyeBrows = reader.ReadString();
		_hair = reader.ReadString();
		_height = reader.ReadFloat();
		_health = reader.ReadFloat();
		_maxHealth = reader.ReadFloat();
		_mood = reader.ReadString();
		_title = reader.ReadString();
		_avatarLevel = reader.ReadInt32();
		_gems = reader.ReadInt32();
		_teleportStones = reader.ReadInt32();
		_items = new List<Item>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_items.Add(Item.Create(reader.ReadTypeCode(), reader));
		}
	}

	public Avatar(string name, sbyte gender, string skinColor, int hairColor, string eyeColor, string mouth, string eyes, string eyeBrows, string hair, float height, float health, float maxHealth, string mood, string title, int avatarLevel, int gems, int teleportStones, IList<Item> items)
	{
		_name = name;
		_gender = gender;
		_skinColor = skinColor;
		_hairColor = hairColor;
		_eyeColor = eyeColor;
		_mouth = mouth;
		_eyes = eyes;
		_eyeBrows = eyeBrows;
		_hair = hair;
		_height = height;
		_health = health;
		_maxHealth = maxHealth;
		_mood = mood;
		_title = title;
		_avatarLevel = avatarLevel;
		_gems = gems;
		_teleportStones = teleportStones;
		_items = items;
	}

	public string GetName()
	{
		return _name;
	}

	public sbyte GetGender()
	{
		return _gender;
	}

	public string GetSkinColor()
	{
		return _skinColor;
	}

	public int GetHairColor()
	{
		return _hairColor;
	}

	public string GetEyeColor()
	{
		return _eyeColor;
	}

	public string GetMouth()
	{
		return _mouth;
	}

	public string GetEyes()
	{
		return _eyes;
	}

	public string GetEyeBrows()
	{
		return _eyeBrows;
	}

	public string GetHair()
	{
		return _hair;
	}

	public float GetHeight()
	{
		return _height;
	}

	public float GetHealth()
	{
		return _health;
	}

	public float GetMaxHealth()
	{
		return _maxHealth;
	}

	public string GetMood()
	{
		return _mood;
	}

	public string GetTitle()
	{
		return _title;
	}

	public int GetAvatarLevel()
	{
		return _avatarLevel;
	}

	public int GetGems()
	{
		return _gems;
	}

	public int GetTeleportStones()
	{
		return _teleportStones;
	}

	public IList<Item> GetItems()
	{
		return _items;
	}

	public int Size()
	{
		int num = 49;
		num += MessageWriter.GetSize(_name);
		num += MessageWriter.GetSize(_skinColor);
		num += MessageWriter.GetSize(_eyeColor);
		num += MessageWriter.GetSize(_mouth);
		num += MessageWriter.GetSize(_eyes);
		num += MessageWriter.GetSize(_eyeBrows);
		num += MessageWriter.GetSize(_hair);
		num += MessageWriter.GetSize(_mood);
		num += MessageWriter.GetSize(_title);
		num += (short)(_items.Count * 2);
		foreach (Item item in _items)
		{
			num += item.Size();
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_name);
		writer.WriteInt8(_gender);
		writer.WriteString(_skinColor);
		writer.WriteInt32(_hairColor);
		writer.WriteString(_eyeColor);
		writer.WriteString(_mouth);
		writer.WriteString(_eyes);
		writer.WriteString(_eyeBrows);
		writer.WriteString(_hair);
		writer.WriteFloat(_height);
		writer.WriteFloat(_health);
		writer.WriteFloat(_maxHealth);
		writer.WriteString(_mood);
		writer.WriteString(_title);
		writer.WriteInt32(_avatarLevel);
		writer.WriteInt32(_gems);
		writer.WriteInt32(_teleportStones);
		writer.WriteInt16((short)_items.Count);
		foreach (Item item in _items)
		{
			writer.WriteTypeCode(item.GetTypeId());
			item.Write(writer);
		}
	}
}
