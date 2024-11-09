using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_RandomBox : MilMo_Item
{
	public new MilMo_RandomBoxTemplate Template => base.Template as MilMo_RandomBoxTemplate;

	public MilMo_RandomBox(MilMo_RandomBoxTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool IsWearable()
	{
		return false;
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public override bool AutoPickup()
	{
		return false;
	}
}
