using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ItemTemplate : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new ItemTemplate(reader);
		}
	}

	private readonly string _visualrep;

	private readonly float _pickupRadius;

	private readonly sbyte _isUnique;

	private readonly sbyte _isHappy;

	private readonly sbyte _isAutoPickup;

	private readonly string _happyPickupType;

	private readonly string _pickupMessageSingle;

	private readonly string _pickupMessageSeveral;

	private readonly IList<string> _pickupSounds;

	private readonly string _description;

	private readonly string _shopDescription;

	private readonly string _name;

	private readonly string _pocketCategory;

	private readonly sbyte _feed;

	private readonly string _feedEventIngame;

	private readonly string _feedEventExternal;

	private readonly string _feedDescriptionIngame;

	private readonly string _feedDescriptionExternal;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	protected ItemTemplate(MessageReader reader)
		: base(reader)
	{
		_visualrep = reader.ReadString();
		_pickupRadius = reader.ReadFloat();
		_isUnique = reader.ReadInt8();
		_isHappy = reader.ReadInt8();
		_isAutoPickup = reader.ReadInt8();
		_happyPickupType = reader.ReadString();
		_pickupMessageSingle = reader.ReadString();
		_pickupMessageSeveral = reader.ReadString();
		_pickupSounds = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_pickupSounds.Add(reader.ReadString());
		}
		_description = reader.ReadString();
		_shopDescription = reader.ReadString();
		_name = reader.ReadString();
		_pocketCategory = reader.ReadString();
		_feed = reader.ReadInt8();
		_feedEventIngame = reader.ReadString();
		_feedEventExternal = reader.ReadString();
		_feedDescriptionIngame = reader.ReadString();
		_feedDescriptionExternal = reader.ReadString();
	}

	protected ItemTemplate(string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(type, reference)
	{
		_visualrep = visualrep;
		_pickupRadius = pickupRadius;
		_isUnique = isUnique;
		_isHappy = isHappy;
		_isAutoPickup = isAutoPickup;
		_happyPickupType = happyPickupType;
		_pickupMessageSingle = pickupMessageSingle;
		_pickupMessageSeveral = pickupMessageSeveral;
		_pickupSounds = pickupSounds;
		_description = description;
		_shopDescription = shopDescription;
		_name = name;
		_pocketCategory = pocketCategory;
		_feed = feed;
		_feedEventIngame = feedEventIngame;
		_feedEventExternal = feedEventExternal;
		_feedDescriptionIngame = feedDescriptionIngame;
		_feedDescriptionExternal = feedDescriptionExternal;
	}

	public string GetVisualrep()
	{
		return _visualrep;
	}

	public float GetPickupRadius()
	{
		return _pickupRadius;
	}

	public sbyte GetIsUnique()
	{
		return _isUnique;
	}

	public sbyte GetIsHappy()
	{
		return _isHappy;
	}

	public sbyte GetIsAutoPickup()
	{
		return _isAutoPickup;
	}

	public string GetHappyPickupType()
	{
		return _happyPickupType;
	}

	public string GetPickupMessageSingle()
	{
		return _pickupMessageSingle;
	}

	public string GetPickupMessageSeveral()
	{
		return _pickupMessageSeveral;
	}

	public IList<string> GetPickupSounds()
	{
		return _pickupSounds;
	}

	public string GetDescription()
	{
		return _description;
	}

	public string GetShopDescription()
	{
		return _shopDescription;
	}

	public string GetName()
	{
		return _name;
	}

	public string GetPocketCategory()
	{
		return _pocketCategory;
	}

	public sbyte GetFeed()
	{
		return _feed;
	}

	public string GetFeedEventIngame()
	{
		return _feedEventIngame;
	}

	public string GetFeedEventExternal()
	{
		return _feedEventExternal;
	}

	public string GetFeedDescriptionIngame()
	{
		return _feedDescriptionIngame;
	}

	public string GetFeedDescriptionExternal()
	{
		return _feedDescriptionExternal;
	}

	public override int Size()
	{
		int num = 34 + base.Size();
		num += MessageWriter.GetSize(_visualrep);
		num += MessageWriter.GetSize(_happyPickupType);
		num += MessageWriter.GetSize(_pickupMessageSingle);
		num += MessageWriter.GetSize(_pickupMessageSeveral);
		num += (short)(2 * _pickupSounds.Count);
		foreach (string pickupSound in _pickupSounds)
		{
			num += MessageWriter.GetSize(pickupSound);
		}
		num += MessageWriter.GetSize(_description);
		num += MessageWriter.GetSize(_shopDescription);
		num += MessageWriter.GetSize(_name);
		num += MessageWriter.GetSize(_pocketCategory);
		num += MessageWriter.GetSize(_feedEventIngame);
		num += MessageWriter.GetSize(_feedEventExternal);
		num += MessageWriter.GetSize(_feedDescriptionIngame);
		return num + MessageWriter.GetSize(_feedDescriptionExternal);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_visualrep);
		writer.WriteFloat(_pickupRadius);
		writer.WriteInt8(_isUnique);
		writer.WriteInt8(_isHappy);
		writer.WriteInt8(_isAutoPickup);
		writer.WriteString(_happyPickupType);
		writer.WriteString(_pickupMessageSingle);
		writer.WriteString(_pickupMessageSeveral);
		writer.WriteInt16((short)_pickupSounds.Count);
		foreach (string pickupSound in _pickupSounds)
		{
			writer.WriteString(pickupSound);
		}
		writer.WriteString(_description);
		writer.WriteString(_shopDescription);
		writer.WriteString(_name);
		writer.WriteString(_pocketCategory);
		writer.WriteInt8(_feed);
		writer.WriteString(_feedEventIngame);
		writer.WriteString(_feedEventExternal);
		writer.WriteString(_feedDescriptionIngame);
		writer.WriteString(_feedDescriptionExternal);
	}
}
