using System.Collections.Generic;

namespace Code.Core.Network.types;

public class AttachableFurnitureTemplate : FurnitureTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new AttachableFurnitureTemplate(reader);
		}
	}

	private const int TYPE_ID = 132;

	public override int GetTypeId()
	{
		return 132;
	}

	public AttachableFurnitureTemplate(MessageReader reader)
		: base(reader)
	{
	}

	public AttachableFurnitureTemplate(string homePack, sbyte isDoor, IList<FurnitureState> states, IList<FurnitureAttachNode> attachNodes, string doorEnterSound, string doorExitSound, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(homePack, isDoor, states, attachNodes, doorEnterSound, doorExitSound, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
	}

	public override int Size()
	{
		return base.Size();
	}
}
