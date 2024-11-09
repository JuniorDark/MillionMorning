using System.Collections.Generic;

namespace Code.Core.Network.types;

public class OrbTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new OrbTemplate(reader);
		}
	}

	private readonly IList<TemplateReference> _onUse;

	private const int TYPE_ID = 24;

	public override int GetTypeId()
	{
		return 24;
	}

	public OrbTemplate(MessageReader reader)
		: base(reader)
	{
		_onUse = new List<TemplateReference>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_onUse.Add(new TemplateReference(reader));
		}
	}

	public OrbTemplate(IList<TemplateReference> onUse, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_onUse = onUse;
	}

	public IList<TemplateReference> GetOnUse()
	{
		return _onUse;
	}

	public override int Size()
	{
		int num = 2 + base.Size();
		foreach (TemplateReference item in _onUse)
		{
			num += item.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt16((short)_onUse.Count);
		foreach (TemplateReference item in _onUse)
		{
			item.Write(writer);
		}
	}
}
