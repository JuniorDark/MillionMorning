using System.Collections.Generic;

namespace Code.Core.Network.types;

public class HomeDeliveryBoxTemplate : AttachableFurnitureTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new HomeDeliveryBoxTemplate(reader);
		}
	}

	private readonly IList<string> _pickupEffects;

	private const int TYPE_ID = 144;

	public override int GetTypeId()
	{
		return 144;
	}

	public HomeDeliveryBoxTemplate(MessageReader reader)
		: base(reader)
	{
		_pickupEffects = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_pickupEffects.Add(reader.ReadString());
		}
	}

	public HomeDeliveryBoxTemplate(IList<string> pickupEffects, string homePack, sbyte isDoor, IList<FurnitureState> states, IList<FurnitureAttachNode> attachNodes, string doorEnterSound, string doorExitSound, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(homePack, isDoor, states, attachNodes, doorEnterSound, doorExitSound, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_pickupEffects = pickupEffects;
	}

	public IList<string> GetPickupEffects()
	{
		return _pickupEffects;
	}

	public override int Size()
	{
		int num = 2 + base.Size();
		num += (short)(2 * _pickupEffects.Count);
		foreach (string pickupEffect in _pickupEffects)
		{
			num += MessageWriter.GetSize(pickupEffect);
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt16((short)_pickupEffects.Count);
		foreach (string pickupEffect in _pickupEffects)
		{
			writer.WriteString(pickupEffect);
		}
	}
}
