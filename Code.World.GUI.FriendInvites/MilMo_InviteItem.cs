using Code.Core.Items;

namespace Code.World.GUI.FriendInvites;

public class MilMo_InviteItem
{
	public MilMo_Item Item { get; }

	public int Amount { get; }

	public MilMo_InviteItem(MilMo_Item item, int amount)
	{
		Item = item;
		Amount = amount;
	}
}
