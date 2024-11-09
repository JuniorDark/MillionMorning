using System;
using Core.GameEvent.Types.Bool;
using Core.GameEvent.Types.ChatMessage;
using Core.GameEvent.Types.Dialogue;
using Core.GameEvent.Types.Float;
using Core.GameEvent.Types.HudState;
using Core.GameEvent.Types.Identity;
using Core.GameEvent.Types.Int;
using Core.GameEvent.Types.InventoryEntry;
using Core.GameEvent.Types.String;
using Core.GameEvent.Types.StringList;
using Core.GameEvent.Types.Tooltip;
using Core.GameEvent.Types.Void;

namespace Core.GameEvent;

public static class GameEvent
{
	public static Action<int> OnTeleportStoneUpdate;

	public static Action OnToggleInventoryEvent;

	public static Action<int> OnAbilitySlotEvent;

	public static Action OnGoBackEvent;

	public static Action OnCloseDialogue;

	public static Action OnSubmit;

	public static Action OnCancel;

	public static Action OnSend;

	public static Action OnExit;

	public static Action OnHistoryBackward;

	public static Action OnHistoryForward;

	public static Action OnNextCandidate;

	public static Action OnExitCutsceneEvent;

	public static readonly VoidEventReference ToggleQuestLOGEvent = new VoidEventReference("ToggleQuestLogEvent");

	public static readonly VoidEventReference ToggleFriendListEvent = new VoidEventReference("ToggleFriendListEvent");

	public static readonly VoidEventReference ToggleFullscreenEvent = new VoidEventReference("ToggleFullscreenEvent");

	public static readonly VoidEventReference ToggleOptionsEvent = new VoidEventReference("ToggleOptionsEvent");

	public static readonly VoidEventReference ToggleInventoryEvent = new VoidEventReference("ToggleInventoryEvent");

	public static readonly VoidEventReference ToggleHomeSettingsEvent = new VoidEventReference("ToggleHomeSettingsEvent");

	public static readonly VoidEventReference OnToggleConsoleEvent = new VoidEventReference("OnToggleConsoleEvent");

	public static readonly VoidEventReference OnToggleEscapeMenu = new VoidEventReference("OnToggleEscapeMenu");

	public static readonly VoidEventReference OpenShopEvent = new VoidEventReference("OpenShopEvent");

	public static readonly VoidEventReference OpenShopFailEvent = new VoidEventReference("OpenShopFailEvent");

	public static readonly VoidEventReference OpenTownEvent = new VoidEventReference("OpenTownEvent");

	public static readonly VoidEventReference OpenTownFailEvent = new VoidEventReference("OpenTownFailEvent");

	public static readonly VoidEventReference OpenNavigatorEvent = new VoidEventReference("OpenNavigatorEvent");

	public static readonly VoidEventReference OpenNavigatorFailEvent = new VoidEventReference("OpenNavigatorFailEvent");

	public static readonly HudStateEventReference UpdateHudStateEvent = new HudStateEventReference("UpdateHudStateEvent");

	public static readonly BoolEventReference ShowHUDEvent = new BoolEventReference("ShowHUDEvent");

	public static readonly BoolEventReference ShowExplorationCounterEvent = new BoolEventReference("ShowExplorationTokenCounterEvent");

	public static readonly BoolEventReference ShowCoinCounterEvent = new BoolEventReference("ShowCoinCounterEvent");

	public static readonly BoolEventReference ShowGemCounterEvent = new BoolEventReference("ShowGemCounterEvent");

	public static readonly BoolEventReference ShowAmmoCounterEvent = new BoolEventReference("ShowAmmoCounterEvent");

	public static readonly BoolEventReference ShowTeleportCounterEvent = new BoolEventReference("ShowTeleportCounterEvent");

	public static readonly BoolEventReference ShowActionbarEvent = new BoolEventReference("ShowActionbarEvent");

	public static readonly VoidEventReference ImpulseBagButtonEvent = new VoidEventReference("ImpulseBagButtonEvent");

	public static readonly IntEventReference LevelCoinTokenFoundEvent = new IntEventReference("LevelCoinTokenFoundEvent");

	public static readonly IntEventReference LevelExplorationTokenFoundEvent = new IntEventReference("LevelExplorationTokenFoundEvent");

	public static readonly IntEventReference ReceivedPremiumTokenRewardEvent = new IntEventReference("ReceivedPremiumTokenRewardEvent");

	public static readonly IntEventReference AmmoIncreasedEvent = new IntEventReference("AmmoIncreasedEvent");

	public static readonly IntEventReference AmmoSpentEvent = new IntEventReference("AmmoSpentEvent");

	public static readonly VoidEventReference AmmoNotEnoughEvent = new VoidEventReference("AmmoNotEnoughEvent");

	public static readonly IntEventReference GemsIncreasedEvent = new IntEventReference("GemsIncreasedEvent");

	public static readonly IntEventReference GemsIncreasedBonusEvent = new IntEventReference("GemsIncreasedBonusEvent");

	public static readonly IntEventReference GemsSpentEvent = new IntEventReference("GemsSpentEvent");

	public static readonly VoidEventReference GemsNotEnoughEvent = new VoidEventReference("GemsNotEnoughEvent");

	public static readonly IntEventReference GemsStolenEvent = new IntEventReference("GemsStolenEvent");

	public static readonly VoidEventReference FocusChatEvent = new VoidEventReference("FocusChatEvent");

	public static readonly ChatMessageEventReference OnChatMessageReceivedEvent = new ChatMessageEventReference("OnChatMessageReceivedEvent");

	public static readonly VoidEventReference OnBadWordEvent = new VoidEventReference("OnBadWordEvent");

	public static readonly BoolEventReference OnGroupChangeEvent = new BoolEventReference("OnGroupChangeEvent");

	public static readonly StringEventReference ThinkEvent = new StringEventReference("ThinkEvent");

	public static readonly StringEventReference OnTutorialArrowEvent = new StringEventReference("OnTutorialArrowEvent");

	public static readonly VoidEventReference OnCloseTutorialArrowEvent = new VoidEventReference("OnCloseTutorialArrowEvent");

	public static readonly VoidEventReference OnVideoApplyEvent = new VoidEventReference("OnVideoApplyEvent");

	public static readonly VoidEventReference OnVideoRevertEvent = new VoidEventReference("OnVideoRevertEvent");

	public static readonly FloatEventReference MasterVolumeEvent = new FloatEventReference("MasterVolumeEvent");

	public static readonly FloatEventReference SFXVolumeEvent = new FloatEventReference("SFXVolumeEvent");

	public static readonly FloatEventReference MusicVolumeEvent = new FloatEventReference("MusicVolumeEvent");

	public static readonly FloatEventReference AmbienceVolumeEvent = new FloatEventReference("AmbienceVolumeEvent");

	public static readonly TooltipEventReference ShowTooltipEvent = new TooltipEventReference("ShowTooltipEvent");

	public static readonly VoidEventReference HideTooltipEvent = new VoidEventReference("HideTooltipEvent");

	public static readonly InventoryEntryEventReference InventoryItemAddedEvent = new InventoryEntryEventReference("InventoryItemAddedEvent");

	public static readonly InventoryEntryEventReference InventoryItemRemovedEvent = new InventoryEntryEventReference("InventoryItemRemovedEvent");

	public static readonly InventoryEntryEventReference InventoryItemSetFavoriteEvent = new InventoryEntryEventReference("InventoryItemSetFavoriteEvent");

	public static readonly InventoryEntryEventReference InventoryItemUnsetFavoriteEvent = new InventoryEntryEventReference("InventoryItemUnsetFavoriteEvent");

	public static readonly VoidEventReference InventoryClearedEvent = new VoidEventReference("InventoryClearedEvent");

	public static readonly VoidEventReference PrevWeaponEvent = new VoidEventReference("PrevWeaponEvent");

	public static readonly VoidEventReference NextWeaponEvent = new VoidEventReference("NextWeaponEvent");

	public static readonly StringListEventReference SetActionbarHotkeysEvent = new StringListEventReference("SetActionbarHotkeysEvent");

	public static readonly VoidEventReference PlayerTooLowLevelEvent = new VoidEventReference("PlayerTooLowLevelEvent");

	public static readonly IntEventReference OpenProfileEvent = new IntEventReference("OpenProfileEvent");

	public static readonly BoolEventReference EnterFurnishingModeEvent = new BoolEventReference("EnterFurnishingModeEvent");

	public static readonly BoolEventReference EnterHoldFurnitureModeEvent = new BoolEventReference("EnterHoldFurnitureModeEvent");

	public static readonly InventoryEntryEventReference MoveItemToStorageEvent = new InventoryEntryEventReference("MoveItemToStorageEvent");

	public static readonly StringEventReference ShowUseWidgetEvent = new StringEventReference("ShowUseWidgetEvent");

	public static readonly VoidEventReference OnUseEvent = new VoidEventReference("OnUseEvent");

	public static readonly BoolEventReference InCombatEvent = new BoolEventReference("InCombatEvent");

	public static readonly DialogueEventReference SpawnDialogueEvent = new DialogueEventReference("SpawnDialogueEvent");

	public static readonly IdentityEventReference ShowContextMenuEvent = new IdentityEventReference("ShowContextMenuEvent");

	public static readonly VoidEventReference ShowControllerChoiceEvent = new VoidEventReference("ShowControllerChoiceEvent");
}
