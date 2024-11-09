namespace Code.World.GUI.Hub;

public sealed class MilMo_HomeOfTheDay
{
	public string HomeName { get; private set; }

	public string OwnerName { get; private set; }

	public int OwnerId { get; private set; }

	public MilMo_HomeOfTheDay(string ownerName, string homeName, int ownerId)
	{
		OwnerId = ownerId;
		OwnerName = ownerName;
		HomeName = homeName;
	}
}
