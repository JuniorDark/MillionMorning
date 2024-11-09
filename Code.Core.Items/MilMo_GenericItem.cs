using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_GenericItem : MilMo_Item
{
	public MilMo_GenericItem(MilMo_GenericItemTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public override bool IsWearable()
	{
		return false;
	}
}
