using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_Coin : MilMo_Item
{
	public new MilMo_CoinTemplate Template => (MilMo_CoinTemplate)base.Template;

	public MilMo_Coin(MilMo_CoinTemplate template, Dictionary<string, string> modifiers)
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
