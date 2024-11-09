using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_Key : MilMo_Item
{
	public new MilMo_KeyTemplate Template => base.Template as MilMo_KeyTemplate;

	public MilMo_Key(MilMo_KeyTemplate template, Dictionary<string, string> modifiers)
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
		return true;
	}
}
