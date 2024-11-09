using System.Collections.Generic;

namespace Code.Core.Network.types;

public class WieldableFoodTemplate : WieldableTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new WieldableFoodTemplate(reader);
		}
	}

	private readonly sbyte _uses;

	private readonly IList<WieldableFoodOnUse> _onUse;

	private readonly IList<WieldableFoodUseEmote> _useEmotes;

	private readonly color _progressBarColor;

	private const int TYPE_ID = 123;

	public override int GetTypeId()
	{
		return 123;
	}

	public WieldableFoodTemplate(MessageReader reader)
		: base(reader)
	{
		_uses = reader.ReadInt8();
		_onUse = new List<WieldableFoodOnUse>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_onUse.Add(new WieldableFoodOnUse(reader));
		}
		_useEmotes = new List<WieldableFoodUseEmote>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_useEmotes.Add(new WieldableFoodUseEmote(reader));
		}
		_progressBarColor = new color(reader);
	}

	public WieldableFoodTemplate(sbyte uses, IList<WieldableFoodOnUse> onUse, IList<WieldableFoodUseEmote> useEmotes, color progressBarColor, float cooldown, IList<string> wieldAnimations, string bodypack, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(cooldown, wieldAnimations, bodypack, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_uses = uses;
		_onUse = onUse;
		_useEmotes = useEmotes;
		_progressBarColor = progressBarColor;
	}

	public sbyte GetUses()
	{
		return _uses;
	}

	public IList<WieldableFoodOnUse> GetOnUse()
	{
		return _onUse;
	}

	public IList<WieldableFoodUseEmote> GetUseEmotes()
	{
		return _useEmotes;
	}

	public color GetProgressBarColor()
	{
		return _progressBarColor;
	}

	public override int Size()
	{
		int num = 21 + base.Size();
		foreach (WieldableFoodOnUse item in _onUse)
		{
			num += item.Size();
		}
		foreach (WieldableFoodUseEmote useEmote in _useEmotes)
		{
			num += useEmote.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt8(_uses);
		writer.WriteInt16((short)_onUse.Count);
		foreach (WieldableFoodOnUse item in _onUse)
		{
			item.Write(writer);
		}
		writer.WriteInt16((short)_useEmotes.Count);
		foreach (WieldableFoodUseEmote useEmote in _useEmotes)
		{
			useEmote.Write(writer);
		}
		_progressBarColor.write(writer);
	}
}
