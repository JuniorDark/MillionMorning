using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_MemberSubscriptionTemplate : MilMo_ItemTemplate
{
	public static MilMo_MemberSubscriptionTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_MemberSubscriptionTemplate(category, path, filePath);
	}

	protected MilMo_MemberSubscriptionTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "MemberSubscription")
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_MemberSubscription(this, modifiers);
	}
}
