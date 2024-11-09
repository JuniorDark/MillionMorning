using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_TravelTicket : MilMo_Item
{
	public const string HELICOPTER_TICKET_IDENTIFIER = "Shop:Batch01.Items.HeliTicket";

	public MilMo_TravelTicket(MilMo_TravelTicketTemplate template, Dictionary<string, string> modifiers)
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
