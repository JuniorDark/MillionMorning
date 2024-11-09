using UnityEngine.Localization;

namespace Templates;

public abstract class ItemTemplate : Template
{
	public LocalizedString displayName;

	public LocalizedString description;

	public string visualRep;

	public float pickupRadius;

	public bool isUnique;

	public bool isHappy;

	public bool isPremium;

	public bool autoPickup;

	public bool noSell;

	public string happyPickupType;

	public string pickupMessageSingle;

	public string pickupMessageSeveral;

	public string[] pickupSounds;

	public string shopDescription;

	public string bagCategory;

	public string collectableCategory;

	public string[] spawnEffects;

	public byte feedMode;

	public string feedEventIngame;

	public string feedEventExternal;

	public string feedDescriptionIngame;

	public string feedDescriptionExternal;

	public int requiredAvatarLevel;

	public string[] validLevels;

	public bool isWieldable;

	public bool isWearable;

	public bool isTeleportStone;

	public bool isAmmo;

	public bool isMemberSubscription;

	public bool isItemKit;

	public bool isShopRoom;

	public bool isHomeItem;

	public bool isDoor;

	public bool isVoucher;
}
