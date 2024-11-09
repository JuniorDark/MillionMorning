using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Items;

public class MilMo_CoinTemplate : MilMo_ItemTemplate
{
	protected int Value;

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		CoinTemplate coinTemplate = t as CoinTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)coinTemplate);
		if (coinTemplate == null)
		{
			return false;
		}
		Value = coinTemplate.GetValue();
		return true;
	}

	public static MilMo_CoinTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_CoinTemplate(category, path, filePath);
	}

	protected MilMo_CoinTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Coin")
	{
		CustomIdiotIconPath = "Content/GUI/Batch01/Textures/HUD/IconVoucherPointCounter";
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Coin(this, modifiers);
	}
}
