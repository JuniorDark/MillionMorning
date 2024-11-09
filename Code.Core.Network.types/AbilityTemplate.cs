using System.Collections.Generic;

namespace Code.Core.Network.types;

public class AbilityTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new AbilityTemplate(reader);
		}
	}

	private readonly string _activationKey;

	private readonly string _activationEvent;

	private readonly float _duration;

	private readonly float _cooldown;

	private readonly float _activationTime;

	private readonly float _reactivationTime;

	private readonly IList<TemplateReference> _onEquip;

	private readonly IList<TemplateReference> _onActivation;

	private const int TYPE_ID = 22;

	public override int GetTypeId()
	{
		return 22;
	}

	public AbilityTemplate(MessageReader reader)
		: base(reader)
	{
		_activationKey = reader.ReadString();
		_activationEvent = reader.ReadString();
		_duration = reader.ReadFloat();
		_cooldown = reader.ReadFloat();
		_activationTime = reader.ReadFloat();
		_reactivationTime = reader.ReadFloat();
		_onEquip = new List<TemplateReference>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_onEquip.Add(new TemplateReference(reader));
		}
		_onActivation = new List<TemplateReference>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_onActivation.Add(new TemplateReference(reader));
		}
	}

	public AbilityTemplate(string activationKey, string activationEvent, float duration, float cooldown, float activationTime, float reactivationTime, IList<TemplateReference> onEquip, IList<TemplateReference> onActivation, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_activationKey = activationKey;
		_activationEvent = activationEvent;
		_duration = duration;
		_cooldown = cooldown;
		_activationTime = activationTime;
		_reactivationTime = reactivationTime;
		_onEquip = onEquip;
		_onActivation = onActivation;
	}

	public string GetActivationKey()
	{
		return _activationKey;
	}

	public string GetActivationEvent()
	{
		return _activationEvent;
	}

	public float GetDuration()
	{
		return _duration;
	}

	public float GetCooldown()
	{
		return _cooldown;
	}

	public float GetActivationTime()
	{
		return _activationTime;
	}

	public float GetReactivationTime()
	{
		return _reactivationTime;
	}

	public IList<TemplateReference> GetOnEquip()
	{
		return _onEquip;
	}

	public IList<TemplateReference> GetOnActivation()
	{
		return _onActivation;
	}

	public override int Size()
	{
		int num = 24 + base.Size();
		num += MessageWriter.GetSize(_activationKey);
		num += MessageWriter.GetSize(_activationEvent);
		foreach (TemplateReference item in _onEquip)
		{
			num += item.Size();
		}
		foreach (TemplateReference item2 in _onActivation)
		{
			num += item2.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_activationKey);
		writer.WriteString(_activationEvent);
		writer.WriteFloat(_duration);
		writer.WriteFloat(_cooldown);
		writer.WriteFloat(_activationTime);
		writer.WriteFloat(_reactivationTime);
		writer.WriteInt16((short)_onEquip.Count);
		foreach (TemplateReference item in _onEquip)
		{
			item.Write(writer);
		}
		writer.WriteInt16((short)_onActivation.Count);
		foreach (TemplateReference item2 in _onActivation)
		{
			item2.Write(writer);
		}
	}
}
