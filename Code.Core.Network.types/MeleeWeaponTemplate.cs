using System.Collections.Generic;

namespace Code.Core.Network.types;

public class MeleeWeaponTemplate : WeaponTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new MeleeWeaponTemplate(reader);
		}
	}

	private readonly float _range;

	private readonly short _spread;

	private readonly float _hitTime;

	private readonly float _impact;

	private readonly IList<Damage> _damages;

	private readonly string _trail;

	private const int TYPE_ID = 141;

	public override int GetTypeId()
	{
		return 141;
	}

	public MeleeWeaponTemplate(MessageReader reader)
		: base(reader)
	{
		_range = reader.ReadFloat();
		_spread = reader.ReadInt16();
		_hitTime = reader.ReadFloat();
		_impact = reader.ReadFloat();
		_damages = new List<Damage>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_damages.Add(new Damage(reader));
		}
		_trail = reader.ReadString();
	}

	public MeleeWeaponTemplate(float range, short spread, float hitTime, float impact, IList<Damage> damages, string trail, string animation, float animationSequenceTimeout, string weaponType, string ammoType, short ammoAmount, float cooldown, IList<string> wieldAnimations, string bodypack, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(animation, animationSequenceTimeout, weaponType, ammoType, ammoAmount, cooldown, wieldAnimations, bodypack, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_range = range;
		_spread = spread;
		_hitTime = hitTime;
		_impact = impact;
		_damages = damages;
		_trail = trail;
	}

	public float GetRange()
	{
		return _range;
	}

	public short GetSpread()
	{
		return _spread;
	}

	public float GetHitTime()
	{
		return _hitTime;
	}

	public float GetImpact()
	{
		return _impact;
	}

	public IList<Damage> GetDamages()
	{
		return _damages;
	}

	public string GetTrail()
	{
		return _trail;
	}

	public override int Size()
	{
		int num = 18 + base.Size();
		foreach (Damage damage in _damages)
		{
			num += damage.Size();
		}
		return num + MessageWriter.GetSize(_trail);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteFloat(_range);
		writer.WriteInt16(_spread);
		writer.WriteFloat(_hitTime);
		writer.WriteFloat(_impact);
		writer.WriteInt16((short)_damages.Count);
		foreach (Damage damage in _damages)
		{
			damage.Write(writer);
		}
		writer.WriteString(_trail);
	}
}
