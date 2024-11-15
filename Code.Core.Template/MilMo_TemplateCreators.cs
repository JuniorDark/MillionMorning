using Code.Core.Avatar.HappyPickup;
using Code.Core.BodyPack;
using Code.Core.Emote;
using Code.Core.HomePack;
using Code.Core.Items;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate.AttachableFurnitureTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate.FloorFurnitureTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.HomeSurfaceTemplate;
using Code.Core.PlayerState;
using Code.Core.Spline;
using Code.Core.Visual.Audio;
using Code.World.Achievements;
using Code.World.Attack;
using Code.World.CharacterShop;
using Code.World.EntityStates;
using Code.World.Feeds;
using Code.World.Gameplay;
using Code.World.Home;
using Code.World.Level.LevelObject;
using Code.World.Slides;
using Code.World.Tutorial;
using Core.Avatar.Presets;

namespace Code.Core.Template;

public static class MilMo_TemplateCreators
{
	public static void AddCreators()
	{
		MilMo_TemplateContainer milMo_TemplateContainer = MilMo_TemplateContainer.Get();
		milMo_TemplateContainer.AddCreator("CharBuilderPreset", MilMo_CharBuilderPreset.Create);
		milMo_TemplateContainer.AddCreator("BodyPack", MilMo_BodyPackSystem.BodyPackCreator);
		milMo_TemplateContainer.AddCreator("HomePack", MilMo_HomePack.Create);
		milMo_TemplateContainer.AddCreator("HomePackSurface", MilMo_HomePackSurface.Create);
		milMo_TemplateContainer.AddCreator("Emote", MilMo_EmoteSystem.EmoteCreator);
		milMo_TemplateContainer.AddCreator("Creature", MilMo_LevelCreatureTemplate.Create);
		milMo_TemplateContainer.AddCreator("DamageSusceptibility", MilMo_DamageSusceptibilityTemplate.Create);
		milMo_TemplateContainer.AddCreator("MeleeCreatureAttack", MilMo_MeleeCreatureAttackTemplate.Create);
		milMo_TemplateContainer.AddCreator("RangedCreatureAttack", MilMo_RangedCreatureAttackTemplate.Create);
		milMo_TemplateContainer.AddCreator("SpawnObjectAttack", MilMo_SpawnObjectAttackTemplate.Create);
		milMo_TemplateContainer.AddCreator("Projectile", MilMo_ProjectileTemplate.Create);
		milMo_TemplateContainer.AddCreator("PlayerState", MilMo_PlayerStateTemplate.Create);
		milMo_TemplateContainer.AddCreator("HappyPickup", HappyPickupTemplate.Create);
		milMo_TemplateContainer.AddCreator("RandomSound", MilMo_RandomSoundTemplate.Create);
		milMo_TemplateContainer.AddCreator("FeedDefault", MilMo_FeedDefaultTemplate.Create);
		milMo_TemplateContainer.AddCreator("Tutorial", MilMo_TutorialTemplate.Create);
		milMo_TemplateContainer.AddCreator("Spline", MilMo_SplineTemplate.Create);
		milMo_TemplateContainer.AddCreator("GameObjectSpline", MilMo_GameObjectSplineTemplate.Create);
		milMo_TemplateContainer.AddCreator("PlayerSpline", MilMo_PlayerSplineTemplate.Create);
		milMo_TemplateContainer.AddCreator("GameplayTrigger", MilMo_GameplayTriggerTemplate.Create);
		milMo_TemplateContainer.AddCreator("GameplayObject", MilMo_GameplayObjectTemplate.Create);
		milMo_TemplateContainer.AddCreator("InterTeleport", MilMo_InterTeleportTemplate.Create);
		milMo_TemplateContainer.AddCreator("IntraTeleport", MilMo_IntraTeleportTemplate.Create);
		milMo_TemplateContainer.AddCreator("Boss", MilMo_LevelBossTemplate.Create);
		milMo_TemplateContainer.AddCreator("BossMode", MilMo_BossModeTemplate.Create);
		milMo_TemplateContainer.AddCreator("Slides", MilMo_SlidesTemplate.Create);
		milMo_TemplateContainer.AddCreator("NPC", MilMo_NpcTemplate.Create);
		milMo_TemplateContainer.AddCreator("EntityStateEffect", MilMo_EntityStateEffectTemplate.Create);
		milMo_TemplateContainer.AddCreator("RoomPreset", MilMo_RoomPresetTemplate.Create);
		milMo_TemplateContainer.AddCreator("GenericItem", MilMo_GenericItemTemplate.Create);
		milMo_TemplateContainer.AddCreator("Coin", MilMo_CoinTemplate.Create);
		milMo_TemplateContainer.AddCreator("Wearable", MilMo_WearableTemplate.Create);
		milMo_TemplateContainer.AddCreator("Wieldable", MilMo_WieldableTemplate.Create);
		milMo_TemplateContainer.AddCreator("WieldableFood", MilMo_WieldableFoodTemplate.Create);
		milMo_TemplateContainer.AddCreator("MeleeWeapon", MilMo_MeleeWeaponTemplate.Create);
		milMo_TemplateContainer.AddCreator("RangedWeapon", MilMo_RangedWeaponTemplate.Create);
		milMo_TemplateContainer.AddCreator("Ability", MilMo_AbilityTemplate.Create);
		milMo_TemplateContainer.AddCreator("SkillItem", MilMo_SkillTemplate.Create);
		milMo_TemplateContainer.AddCreator("Armor", MilMo_ArmorTemplate.Create);
		milMo_TemplateContainer.AddCreator("ArmorShard", MilMo_ArmorShardTemplate.Create);
		milMo_TemplateContainer.AddCreator("HealthPickup", MilMo_HealthPickupTemplate.Create);
		milMo_TemplateContainer.AddCreator("Orb", MilMo_OrbTemplate.Create);
		milMo_TemplateContainer.AddCreator("Medal", MilMo_AchievementTemplate.Create);
		milMo_TemplateContainer.AddCreator("PVPMedal", MilMo_AchievementTemplate.Create);
		milMo_TemplateContainer.AddCreator("Consumable", MilMo_ConsumableTemplate.Create);
		milMo_TemplateContainer.AddCreator("Gem", MilMo_GemTemplate.Create);
		milMo_TemplateContainer.AddCreator("Ammo", MilMo_AmmoTemplate.Create);
		milMo_TemplateContainer.AddCreator("TeleportStone", MilMo_TeleportStoneTemplate.Create);
		milMo_TemplateContainer.AddCreator("MemberSubscription", MilMo_MemberSubscriptionTemplate.Create);
		milMo_TemplateContainer.AddCreator("Key", MilMo_KeyTemplate.Create);
		milMo_TemplateContainer.AddCreator("FloorFurniture", MilMo_FloorFurnitureTemplate.Create);
		milMo_TemplateContainer.AddCreator("WallFurniture", MilMo_WallFurnitureTemplate.Create);
		milMo_TemplateContainer.AddCreator("Wallpaper", MilMo_WallpaperTemplate.Create);
		milMo_TemplateContainer.AddCreator("Floor", MilMo_FloorTemplate.Create);
		milMo_TemplateContainer.AddCreator("Room", MilMo_RoomTemplate.Create);
		milMo_TemplateContainer.AddCreator("Seat", MilMo_SeatTemplate.Create);
		milMo_TemplateContainer.AddCreator("AttachableFurniture", MilMo_AttachableFurnitureTemplate.Create);
		milMo_TemplateContainer.AddCreator("LockBox", MilMo_LockBoxTemplate.Create);
		milMo_TemplateContainer.AddCreator("MysteryBox", MilMo_MysteryBoxTemplate.Create);
		milMo_TemplateContainer.AddCreator("LockBoxKey", MilMo_LockBoxKeyTemplate.Create);
		milMo_TemplateContainer.AddCreator("TravelTicket", MilMo_TravelTicketTemplate.Create);
		milMo_TemplateContainer.AddCreator("HomeDeliveryBox", MilMo_HomeDeliveryBoxTemplate.Create);
		milMo_TemplateContainer.AddCreator("ItemTrackUpgrade", MilMo_ItemTrackUpgradeTemplate.Create);
		milMo_TemplateContainer.AddCreator("Converter", MilMo_ConverterTemplate.Create);
		milMo_TemplateContainer.AddCreator("NameChange", MilMo_NameChangeTemplate.Create);
		milMo_TemplateContainer.AddCreator("Respec", MilMo_RespecTemplate.Create);
		milMo_TemplateContainer.AddCreator("Offhand", MilMo_OffhandTemplate.Create);
		milMo_TemplateContainer.AddCreator("ShopRoom", MilMo_ShopRoomTemplate.Create);
	}
}
