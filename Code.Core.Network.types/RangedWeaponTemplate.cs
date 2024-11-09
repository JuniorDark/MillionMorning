using System.Collections.Generic;

namespace Code.Core.Network.types;

public class RangedWeaponTemplate : WeaponTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new RangedWeaponTemplate(reader);
		}
	}

	private readonly float _fireTime;

	private readonly TemplateReference _projectile;

	private readonly vector3 _projectileSpawnOffset;

	private readonly string _weaponAnimation;

	private const int TYPE_ID = 142;

	public override int GetTypeId()
	{
		return 142;
	}

	public RangedWeaponTemplate(MessageReader reader)
		: base(reader)
	{
		_fireTime = reader.ReadFloat();
		_projectile = new TemplateReference(reader);
		_projectileSpawnOffset = new vector3(reader);
		_weaponAnimation = reader.ReadString();
	}

	public RangedWeaponTemplate(float fireTime, TemplateReference projectile, vector3 projectileSpawnOffset, string weaponAnimation, string animation, float animationSequenceTimeout, string weaponType, string ammoType, short ammoAmount, float cooldown, IList<string> wieldAnimations, string bodypack, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(animation, animationSequenceTimeout, weaponType, ammoType, ammoAmount, cooldown, wieldAnimations, bodypack, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_fireTime = fireTime;
		_projectile = projectile;
		_projectileSpawnOffset = projectileSpawnOffset;
		_weaponAnimation = weaponAnimation;
	}

	public float GetFireTime()
	{
		return _fireTime;
	}

	public TemplateReference GetProjectile()
	{
		return _projectile;
	}

	public vector3 GetProjectileSpawnOffset()
	{
		return _projectileSpawnOffset;
	}

	public string GetWeaponAnimation()
	{
		return _weaponAnimation;
	}

	public override int Size()
	{
		return 18 + base.Size() + _projectile.Size() + MessageWriter.GetSize(_weaponAnimation);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteFloat(_fireTime);
		_projectile.Write(writer);
		_projectileSpawnOffset.Write(writer);
		writer.WriteString(_weaponAnimation);
	}
}
