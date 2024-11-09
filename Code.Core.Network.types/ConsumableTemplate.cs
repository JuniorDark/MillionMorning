using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ConsumableTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new ConsumableTemplate(reader);
		}
	}

	private readonly sbyte _autoPickup;

	private readonly float _duration;

	private readonly float _activationTime;

	private readonly IList<TemplateReference> _onUse;

	private const int TYPE_ID = 23;

	public override int GetTypeId()
	{
		return 23;
	}

	public ConsumableTemplate(MessageReader reader)
		: base(reader)
	{
		_autoPickup = reader.ReadInt8();
		_duration = reader.ReadFloat();
		_activationTime = reader.ReadFloat();
		_onUse = new List<TemplateReference>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_onUse.Add(new TemplateReference(reader));
		}
	}

	public ConsumableTemplate(sbyte autoPickup, float duration, float activationTime, IList<TemplateReference> onUse, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_autoPickup = autoPickup;
		_duration = duration;
		_activationTime = activationTime;
		_onUse = onUse;
	}

	public sbyte GetAutoPickup()
	{
		return _autoPickup;
	}

	public float GetDuration()
	{
		return _duration;
	}

	public float GetActivationTime()
	{
		return _activationTime;
	}

	public IList<TemplateReference> GetOnUse()
	{
		return _onUse;
	}

	public override int Size()
	{
		int num = 11 + base.Size();
		foreach (TemplateReference item in _onUse)
		{
			num += item.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt8(_autoPickup);
		writer.WriteFloat(_duration);
		writer.WriteFloat(_activationTime);
		writer.WriteInt16((short)_onUse.Count);
		foreach (TemplateReference item in _onUse)
		{
			item.Write(writer);
		}
	}
}
