using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ItemKitTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new ItemKitTemplate(reader);
		}
	}

	private readonly IList<ItemKitItem> _items;

	private const int TYPE_ID = 34;

	public override int GetTypeId()
	{
		return 34;
	}

	public ItemKitTemplate(MessageReader reader)
		: base(reader)
	{
		_items = new List<ItemKitItem>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_items.Add(new ItemKitItem(reader));
		}
	}

	public ItemKitTemplate(IList<ItemKitItem> items, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_items = items;
	}

	public IList<ItemKitItem> GetItems()
	{
		return _items;
	}

	public override int Size()
	{
		int num = 2 + base.Size();
		foreach (ItemKitItem item in _items)
		{
			num += item.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt16((short)_items.Count);
		foreach (ItemKitItem item in _items)
		{
			item.Write(writer);
		}
	}
}
