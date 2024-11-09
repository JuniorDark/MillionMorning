using System.Collections.Generic;

namespace Code.Core.Network.types;

public class WeaponTemplate : WieldableTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new WeaponTemplate(reader);
		}
	}

	private readonly string _animation;

	private readonly float _animationSequenceTimeout;

	private readonly string _weaponType;

	private readonly string _ammoType;

	private readonly short _ammoAmount;

	private const int TYPE_ID = 122;

	public override int GetTypeId()
	{
		return 122;
	}

	public WeaponTemplate(MessageReader reader)
		: base(reader)
	{
		_animation = reader.ReadString();
		_animationSequenceTimeout = reader.ReadFloat();
		_weaponType = reader.ReadString();
		_ammoType = reader.ReadString();
		_ammoAmount = reader.ReadInt16();
	}

	public WeaponTemplate(string animation, float animationSequenceTimeout, string weaponType, string ammoType, short ammoAmount, float cooldown, IList<string> wieldAnimations, string bodypack, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(cooldown, wieldAnimations, bodypack, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_animation = animation;
		_animationSequenceTimeout = animationSequenceTimeout;
		_weaponType = weaponType;
		_ammoType = ammoType;
		_ammoAmount = ammoAmount;
	}

	public string GetAnimation()
	{
		return _animation;
	}

	public float GetAnimationSequenceTimeout()
	{
		return _animationSequenceTimeout;
	}

	public string GetWeaponType()
	{
		return _weaponType;
	}

	public string GetAmmoType()
	{
		return _ammoType;
	}

	public short GetAmmoAmount()
	{
		return _ammoAmount;
	}

	public override int Size()
	{
		return 12 + base.Size() + MessageWriter.GetSize(_animation) + MessageWriter.GetSize(_weaponType) + MessageWriter.GetSize(_ammoType);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_animation);
		writer.WriteFloat(_animationSequenceTimeout);
		writer.WriteString(_weaponType);
		writer.WriteString(_ammoType);
		writer.WriteInt16(_ammoAmount);
	}
}
