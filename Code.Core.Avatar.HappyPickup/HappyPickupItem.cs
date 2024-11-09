using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.Core.Visual;
using Code.World.Level.LevelObject;
using UnityEngine;

namespace Code.Core.Avatar.HappyPickup;

public class HappyPickupItem
{
	public MilMo_VisualRep VisualRep;

	public readonly HappyPickupTemplate HappyPickupTemplate;

	public readonly MilMo_LocString ThinkBubbleText;

	public GameObject GameObject => VisualRep?.GameObject;

	private HappyPickupItem(MilMo_VisualRep visualRep, HappyPickupTemplate template, MilMo_LocString thinkText)
	{
		VisualRep = visualRep;
		HappyPickupTemplate = template;
		ThinkBubbleText = thinkText;
	}

	public void Destroy()
	{
		if (VisualRep != null)
		{
			VisualRep.Destroy();
			VisualRep = null;
		}
	}

	public static HappyPickupItem Create(MilMo_LevelItem item)
	{
		HappyPickupTemplate happyPickupTemplate = null;
		MilMo_LocString milMo_LocString = MilMo_LocString.Empty;
		if (item == null)
		{
			return new HappyPickupItem(null, null, milMo_LocString);
		}
		MilMo_ItemTemplate template = item.Template;
		if (template != null && template.HappyPickupTemplate != null)
		{
			happyPickupTemplate = item.Template.HappyPickupTemplate;
			if (happyPickupTemplate.Think != null)
			{
				if (happyPickupTemplate.Think.WantsFormatArgs)
				{
					milMo_LocString = happyPickupTemplate.Think.GetCopy();
					milMo_LocString.SetFormatArgs(item.Template.DisplayName);
				}
				else
				{
					milMo_LocString = happyPickupTemplate.Think;
				}
			}
		}
		return new HappyPickupItem(item.VisualRep, happyPickupTemplate, milMo_LocString);
	}
}
