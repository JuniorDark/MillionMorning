using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Items;

public class MilMo_OffhandTemplate : MilMo_WearableTemplate
{
	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		OffhandTemplate t2 = t as OffhandTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)t2);
		return true;
	}

	public new static MilMo_OffhandTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_OffhandTemplate(category, path, filePath);
	}

	protected MilMo_OffhandTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Offhand")
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Offhand(this, modifiers);
	}
}
