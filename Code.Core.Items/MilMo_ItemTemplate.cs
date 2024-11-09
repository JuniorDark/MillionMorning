using System.Collections.Generic;
using Code.Core.Avatar.HappyPickup;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using Core;
using UnityEngine;

namespace Code.Core.Items;

public abstract class MilMo_ItemTemplate : MilMo_Template
{
	public enum MilMo_FeedMode
	{
		None,
		Ingame,
		External
	}

	private float _pickupRadius;

	private string _happyPickupType;

	private List<string> _pickupSounds;

	public string VisualRepName;

	public string VisualRepPath;

	public string CustomIdiotIconPath = "";

	public string VisualRep { get; set; }

	public bool IsUnique { get; private set; }

	public bool IsHappy { get; private set; }

	public virtual bool IsAutoPickup { get; private set; }

	public string PickupSound => _pickupSounds.Count switch
	{
		0 => "none", 
		1 => _pickupSounds[0], 
		_ => _pickupSounds[Random.Range(0, _pickupSounds.Count)], 
	};

	public MilMo_LocString Description { get; set; }

	public MilMo_LocString ShopDescription { get; private set; }

	public MilMo_LocString DisplayName { get; set; }

	public string BagCategory { get; set; }

	public float PickupRadiusSquared { get; private set; }

	public HappyPickupTemplate HappyPickupTemplate { get; set; }

	public MilMo_LocString PickupMessageSingle { get; private set; }

	public MilMo_LocString PickupMessageSeveral { get; private set; }

	public MilMo_FeedMode FeedMode { get; private set; }

	public MilMo_LocString FeedEventIngame { get; private set; }

	public string FeedEventExternal { get; private set; }

	public MilMo_LocString FeedDescriptionIngame { get; protected set; }

	public MilMo_LocString FeedDescriptionExternal { get; private set; }

	public virtual string IconPath => "Content/Items/" + VisualRepPath + "Icon" + VisualRepName;

	public virtual string ExternThumbnailURL => "Items/" + VisualRepPath + "Icon" + VisualRepName + ".png";

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		ItemTemplate itemTemplate = t as ItemTemplate;
		base.LoadFromNetwork(itemTemplate);
		if (itemTemplate != null)
		{
			VisualRep = itemTemplate.GetVisualrep();
			_pickupRadius = itemTemplate.GetPickupRadius();
			IsUnique = itemTemplate.GetIsUnique() != 0;
			IsHappy = itemTemplate.GetIsHappy() != 0;
			IsAutoPickup = itemTemplate.GetIsAutoPickup() != 0;
			_happyPickupType = itemTemplate.GetHappyPickupType();
			PickupMessageSingle = MilMo_Localization.GetLocString(itemTemplate.GetPickupMessageSingle());
			PickupMessageSeveral = MilMo_Localization.GetLocString(itemTemplate.GetPickupMessageSeveral());
			_pickupSounds = (List<string>)itemTemplate.GetPickupSounds();
			Description = MilMo_Localization.GetLocString(itemTemplate.GetDescription());
			ShopDescription = MilMo_Localization.GetLocString(itemTemplate.GetShopDescription());
			DisplayName = MilMo_Localization.GetLocString(itemTemplate.GetName());
			BagCategory = itemTemplate.GetPocketCategory();
			if (itemTemplate.GetFeed() == 1)
			{
				FeedMode = MilMo_FeedMode.Ingame;
			}
			else if (itemTemplate.GetFeed() == 2)
			{
				FeedMode = MilMo_FeedMode.External;
			}
			FeedDescriptionExternal = MilMo_Localization.GetLocString(itemTemplate.GetFeedDescriptionExternal());
			FeedDescriptionIngame = MilMo_Localization.GetLocString(itemTemplate.GetFeedDescriptionIngame());
			FeedEventExternal = MilMo_Localization.GetLocString(itemTemplate.GetFeedEventExternal()).String;
			FeedEventIngame = MilMo_Localization.GetLocString(itemTemplate.GetFeedEventIngame());
		}
		PickupRadiusSquared = _pickupRadius * _pickupRadius;
		VisualRepName = MilMo_Utility.ExtractNameFromPath(VisualRep);
		VisualRepPath = MilMo_Utility.RemoveFileNameFromFullPath(VisualRep);
		if (ShopDescription == null || string.IsNullOrEmpty(ShopDescription.String))
		{
			ShopDescription = Description;
		}
		if (FeedDescriptionIngame == null || string.IsNullOrEmpty(FeedDescriptionIngame.String))
		{
			FeedDescriptionIngame = ShopDescription;
		}
		return true;
	}

	protected MilMo_ItemTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
		FeedMode = MilMo_FeedMode.None;
		FeedDescriptionExternal = null;
		FeedEventExternal = "";
	}

	public HappyPickupTemplate GetHappyPickupTemplate()
	{
		if (HappyPickupTemplate != null)
		{
			return HappyPickupTemplate;
		}
		if (string.IsNullOrEmpty(_happyPickupType))
		{
			return null;
		}
		MilMo_Template template = Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("HappyPickup", "HappyPickup/" + _happyPickupType);
		if (template == null)
		{
			return null;
		}
		HappyPickupTemplate = template as HappyPickupTemplate;
		return HappyPickupTemplate;
	}

	public abstract MilMo_Item Instantiate(Dictionary<string, string> modifiers);
}
