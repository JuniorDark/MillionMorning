using System.Collections.Generic;

namespace Code.Core.Network.types;

public class WieldableTemplate : WearableTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new WieldableTemplate(reader);
		}
	}

	private readonly float _cooldown;

	private readonly IList<string> _wieldAnimations;

	private const int TYPE_ID = 83;

	public override int GetTypeId()
	{
		return 83;
	}

	public WieldableTemplate(MessageReader reader)
		: base(reader)
	{
		_cooldown = reader.ReadFloat();
		_wieldAnimations = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_wieldAnimations.Add(reader.ReadString());
		}
	}

	public WieldableTemplate(float cooldown, IList<string> wieldAnimations, string bodypack, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(bodypack, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_cooldown = cooldown;
		_wieldAnimations = wieldAnimations;
	}

	public float GetCooldown()
	{
		return _cooldown;
	}

	public IList<string> GetWieldAnimations()
	{
		return _wieldAnimations;
	}

	public override int Size()
	{
		int num = 6 + base.Size();
		num += (short)(2 * _wieldAnimations.Count);
		foreach (string wieldAnimation in _wieldAnimations)
		{
			num += MessageWriter.GetSize(wieldAnimation);
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteFloat(_cooldown);
		writer.WriteInt16((short)_wieldAnimations.Count);
		foreach (string wieldAnimation in _wieldAnimations)
		{
			writer.WriteString(wieldAnimation);
		}
	}
}
