using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ConverterTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new ConverterTemplate(reader);
		}
	}

	private readonly TemplateReference _tool;

	private readonly IList<TemplateCountPair> _requirements;

	private readonly int _requiredGems;

	private readonly TemplateCountPair _boyReward;

	private readonly TemplateCountPair _girlReward;

	private readonly string _converterCategory;

	private const int TYPE_ID = 32;

	public override int GetTypeId()
	{
		return 32;
	}

	public ConverterTemplate(MessageReader reader)
		: base(reader)
	{
		if (reader.ReadInt8() == 1)
		{
			_tool = new TemplateReference(reader);
		}
		_requirements = new List<TemplateCountPair>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_requirements.Add(new TemplateCountPair(reader));
		}
		_requiredGems = reader.ReadInt32();
		_boyReward = new TemplateCountPair(reader);
		_girlReward = new TemplateCountPair(reader);
		_converterCategory = reader.ReadString();
	}

	public ConverterTemplate(TemplateReference tool, IList<TemplateCountPair> requirements, int requiredGems, TemplateCountPair boyReward, TemplateCountPair girlReward, string converterCategory, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_tool = tool;
		_requirements = requirements;
		_requiredGems = requiredGems;
		_boyReward = boyReward;
		_girlReward = girlReward;
		_converterCategory = converterCategory;
	}

	public TemplateReference GetTool()
	{
		return _tool;
	}

	public IList<TemplateCountPair> GetRequirements()
	{
		return _requirements;
	}

	public int GetRequiredGems()
	{
		return _requiredGems;
	}

	public TemplateCountPair GetBoyReward()
	{
		return _boyReward;
	}

	public TemplateCountPair GetGirlReward()
	{
		return _girlReward;
	}

	public string GetConverterCategory()
	{
		return _converterCategory;
	}

	public override int Size()
	{
		int num = 9 + base.Size();
		if (_tool != null)
		{
			num += _tool.Size();
		}
		foreach (TemplateCountPair requirement in _requirements)
		{
			num += requirement.Size();
		}
		num += _boyReward.Size();
		num += _girlReward.Size();
		return num + MessageWriter.GetSize(_converterCategory);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		if (_tool == null)
		{
			writer.WriteInt8(0);
		}
		else
		{
			writer.WriteInt8(1);
			_tool.Write(writer);
		}
		writer.WriteInt16((short)_requirements.Count);
		foreach (TemplateCountPair requirement in _requirements)
		{
			requirement.Write(writer);
		}
		writer.WriteInt32(_requiredGems);
		_boyReward.Write(writer);
		_girlReward.Write(writer);
		writer.WriteString(_converterCategory);
	}
}
