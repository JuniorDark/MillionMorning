using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_TravelTicketTemplate : MilMo_ItemTemplate
{
	public static MilMo_TravelTicketTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_TravelTicketTemplate(category, path, filePath);
	}

	protected MilMo_TravelTicketTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "TravelTicket")
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_TravelTicket(this, modifiers);
	}
}
