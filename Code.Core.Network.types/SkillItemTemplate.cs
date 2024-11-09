using System.Collections.Generic;

namespace Code.Core.Network.types;

public class SkillItemTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new SkillItemTemplate(reader);
		}
	}

	private readonly SkillMode _skillMode;

	private const int TYPE_ID = 145;

	public override int GetTypeId()
	{
		return 145;
	}

	public SkillItemTemplate(MessageReader reader)
		: base(reader)
	{
		_skillMode = new SkillMode(reader);
	}

	public SkillItemTemplate(SkillMode skillMode, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_skillMode = skillMode;
	}

	public SkillMode GetSkillMode()
	{
		return _skillMode;
	}

	public override int Size()
	{
		return _skillMode.Size() + base.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_skillMode.Write(writer);
	}
}
