using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_Offhand : MilMo_Wearable
{
	public override ItemGender UseableGender => ItemGender.Both;

	public MilMo_Offhand(MilMo_OffhandTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public override bool IsWearable()
	{
		return true;
	}
}
