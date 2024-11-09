using System.Collections.Generic;
using Code.Core.Items;

namespace Code.World.CharBuilder.WearableHandlers;

public class WearableHandler
{
	protected static readonly Dictionary<string, MilMo_Wearable> LoadedItems = new Dictionary<string, MilMo_Wearable>();

	protected MilMo_Wearable GetWearable(IItem item)
	{
		if (!LoadedItems.TryGetValue(item.GetBodyPack(), out var value))
		{
			return LoadWearableAsync(item);
		}
		return value;
	}

	private MilMo_Wearable LoadWearableAsync(IItem item)
	{
		string category = item.GetCategory();
		string identifier = item.GetIdentifier();
		string filePath = item.GetFilePath();
		MilMo_WearableTemplate milMo_WearableTemplate = MilMo_WearableTemplate.Create(category, identifier, filePath);
		milMo_WearableTemplate.InitFromBodyPack(item.GetBodyPack());
		MilMo_Wearable milMo_Wearable = milMo_WearableTemplate.Instantiate(new Dictionary<string, string>()) as MilMo_Wearable;
		LoadedItems.Add(item.GetBodyPack(), milMo_Wearable);
		return milMo_Wearable;
	}
}
