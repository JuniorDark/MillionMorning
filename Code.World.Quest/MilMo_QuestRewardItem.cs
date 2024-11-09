namespace Code.World.Quest;

public sealed class MilMo_QuestRewardItem
{
	public string ItemTemplateIdentifier { get; }

	public short Amount { get; }

	public sbyte Gender { get; }

	public MilMo_QuestRewardItem(string itemTemplateIdentifier, short amount, sbyte gender)
	{
		ItemTemplateIdentifier = itemTemplateIdentifier;
		Amount = amount;
		Gender = gender;
	}
}
