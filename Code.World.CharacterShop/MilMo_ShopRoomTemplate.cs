using System.Collections.Generic;
using Code.Core.Items;
using Code.Core.Network.types;

namespace Code.World.CharacterShop;

public class MilMo_ShopRoomTemplate : MilMo_ItemTemplate
{
	public TemplateReference RoomPresetTemplateReference { get; private set; }

	public override bool LoadFromNetwork(Template t)
	{
		ShopRoomTemplate shopRoomTemplate = t as ShopRoomTemplate;
		base.LoadFromNetwork((Template)shopRoomTemplate);
		if (shopRoomTemplate != null)
		{
			RoomPresetTemplateReference = shopRoomTemplate.GetRoomPreset();
		}
		return true;
	}

	public static MilMo_ShopRoomTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_ShopRoomTemplate(category, path, filePath, "ShopRoom");
	}

	private MilMo_ShopRoomTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_ShopRoom(this, modifiers);
	}
}
