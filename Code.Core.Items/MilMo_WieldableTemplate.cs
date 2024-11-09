using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Items;

public class MilMo_WieldableTemplate : MilMo_WearableTemplate
{
	public float Cooldown { get; protected set; }

	public IList<string> WieldAnimations { get; protected set; }

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		WieldableTemplate wieldableTemplate = t as WieldableTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)wieldableTemplate);
		if (wieldableTemplate == null)
		{
			return false;
		}
		Cooldown = wieldableTemplate.GetCooldown();
		WieldAnimations = wieldableTemplate.GetWieldAnimations();
		return true;
	}

	public new static MilMo_WieldableTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_WieldableTemplate(category, path, filePath, "Wieldable");
	}

	protected MilMo_WieldableTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
		WieldAnimations = new List<string>();
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Wieldable(this, modifiers);
	}
}
