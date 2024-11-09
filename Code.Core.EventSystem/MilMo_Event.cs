namespace Code.Core.EventSystem;

public static class MilMo_Event
{
	public const string DISCONNECTED_GAME = "disconnected_game";

	public const string CONNECT_INFO_GAME = "connect_info_game";

	public const string LOGIN_INFO_GAME = "login_info_game";

	public const string LOGIN_INFO_NEXUS = "login_info_nexus";

	public const string LOGOUT_INFO_GAME = "logout_info_game";

	public const string LOGIN_TOKEN_RECEIVED = "login_token_received";

	public const string PING_RESPONSE = "ping_response";

	public const string LOGIN_FAILED = "login_failed";

	public const string SAFE_DISCONNECT_RESPONSE = "safe_disconnec_response";

	public const string COMMAND_RESPONSE = "command_response";

	public const string MESSAGE_OF_THE_DAY = "message_of_the_day";

	public const string SERVER_ADMIN_MESSAGE = "server_admin_message";

	public const string TEMPLATE_RECEIVED = "template_received";

	public const string TEMPLATES_RECEIVED = "templates_received_";

	public const string TEMPLATES_RECEIVED_GENERIC = "templates_received_Generic";

	public const string TEMPLATES_DONE = "templates_done_";

	public const string TEMPLATES_DONE_GENERIC = "templates_done_Generic";

	public const string TEMPLATES_DONE_WORLD = "templates_done_World";

	public const string CHANGE_LEVEL_FAIL = "change_level_fail";

	public const string SERVER_LOGIN_INFO_LEVEL = "server_login_info_level";

	public const string SERVER_LOGIN_INFO_HOME = "server_login_info_home";

	public const string HOME_LOGIN_INFO_FAIL = "home_login_info_fail";

	public const string SERVER_LEVEL_INSTANCE_INFO = "server_level_instance_info";

	public const string GOT_LOAD_PVP_LEVEL_INFO = "got_load_pvp_level_info";

	public const string SERVER_HOME_INSTANCE_INFO = "server_home_instance_info";

	public const string SERVER_ENTER_HOME_FAIL = "server_enter_home_fail";

	public const string JOIN_LEVEL = "join_level";

	public const string JOIN_HOME = "join_home";

	public const string JOIN_HOME_FAIL = "join_home_fail";

	public const string STEAM_TRANSACTION_ERROR = "steam_transaction_error";

	public const string STEAM_TOKEN_RECEIVED = "steam_token_recieved";

	public const string WORLD_CREATED = "world_created";

	public const string AVATAR_LOADED = "avatar_loaded";

	public const string AVATAR_CREATED = "avatar_created";

	public const string AVATAR_CREATED_FAILED = "avatar_created_failed";

	public const string PLAYER_AVATAR_UPDATED = "player_avatar_updated";

	public const string AVATAR_UPDATED = "avatar_updated";

	public const string LOCAL_AVATAR_RELOADED = "local_avatar_reloaded";

	public const string BODYPACKMANAGER_DONE = "bodypackmanager_done";

	public const string PLAYER_INFO = "player_info";

	public const string PLAYER_INFO_LOADED = "player_info_loaded";

	public const string RECREATE_INVENTORY_AND_AMMO = "recreate_inventory_and_ammo";

	public const string SERVER_EXP_UPDATE = "server_exp_update";

	public const string SERVER_AVATARLEVEL_UPDATE = "server_avatarlevel_update";

	public const string EXP_UPDATED = "exp_updated";

	public const string LEVEL_EXP_RECEIVED = "level_exp_received";

	public const string AVATARLEVEL_UPDATED = "avatarlevel_updated";

	public const string CREATURE_TO_HIGH_LEVEL_TO_DAMAGE = "creature_to_high_level_to_damage";

	public const string CREATURE_TO_LOW_LEVEL_TO_XP = "creature_to_low_level_to_xp";

	public const string INVENTORY_ADD = "inventory_add";

	public const string INVENTORY_AMOUNT = "inventory_amount";

	public const string INVENTORY_EQUIP = "inventory_equip";

	public const string INVENTORY_UNEQUIP = "inventory_unequip";

	public const string INVENTORY_REMOVE = "inventory_remove";

	public const string INVENTORY_MODIFIER_CHANGED = "inventory_modifier_changed";

	public const string INVENTORY_CHANGED = "inventory_changed";

	public const string INVENTORY_BAG_ADD = "inventory_bag_add";

	public const string INVENTORY_BAG_REMOVE = "inventory_bag_remove";

	public const string INVENTORY_BAG_AMOUNT = "inventory_bag_amount";

	public const string INVENTORY_WEARABLE_CONFIRMED = "inventory_wearable_confirmed";

	public const string INVENTORY_ENTRY_UPGRADED = "inventory_entry_upgraded";

	public const string CONSUMABLE_OR_ABILITY_ADDED = "consumable_or_ability_added";

	public const string CONSUMABLE_OR_ABILITY_REMOVED = "consumable_or_ability_removed";

	public const string WIELDABLE_FOOD_USES_LEFT_CHANGED = "wieldable_food_uses_left_changed";

	public const string MEDAL_ADDED = "medal_added";

	public const string GEMS_UPDATED = "update_gems";

	public const string GEMS_CHANGED = "gems_changed";

	public const string TELEPORTSTONES_UPDATED = "teleportstones_updated";

	public const string UPDATE_EXPOSED_VARIABLE = "update_exposed_variable";

	public const string MEMBER_UPDATE = "member_update";

	public const string MEMBER_UPDATE_FROM_WEBSHOP = "member_update_from_webshop";

	public const string MEMBER_STATUS_ACTIVATED = "member_status_activated";

	public const string COLLECTABLES = "collectables";

	public const string ACHIEVEMENT_COUNTER_UPDATE = "achievement_counter_update";

	public const string ACHIEVEMENT_COMPLETED = "achievement_completed";

	public const string TOGGLE_WEARABLE = "toggle_wearable";

	public const string EQUIP_UPDATE = "equip_update";

	public const string UPDATE_ACTION_TIME = "update_action_time";

	public const string JUNECASH_UPDATED = "junecash_updated";

	public const string JUNECASH_NEED_UPDATE = "junecash_need_update";

	public const string JUNECASH_CHANGED = "junecash_changed";

	public const string JUNECASH_ITEMS = "junecash_items";

	public const string COINS_UPDATED = "voucher_points_updated";

	public const string COINS_CHANGED = "voucher_points_changed";

	public const string CURRENCY_CHANGED = "currency_changed";

	public const string GOT_CURRENCY_EXCHANGE_RATE = "got_currency_exchange_rate";

	public const string CURRENCY_EXCHANGE_RATE_CHANGED = "currency_exchange_rate_changed";

	public const string SHOP_CATEGORIES = "shop_categories";

	public const string SHOP_ITEMS = "shop_items";

	public const string SHOP_BUY_FAIL = "shop_buy_fail";

	public const string SHOP_BUY_SUCCESS = "shop_buy_success";

	public const string SHOP_RESPONSE = "shop_response";

	public const string SHOP_EXITED = "shop_exited";

	public const string PLAYER_ENTER_SHOP = "player_enter_shop";

	public const string PLAYER_LEAVE_SHOP = "player_leave_shop";

	public const string WORLDMAP_RESPONSE = "worldmap_response";

	public const string HUB_RESPONSE = "hub_response";

	public const string CHARBUILDER_RESPONSE = "charbuilder_response";

	public const string PLAYER_ENTER_WORLDMAP = "player_enter_worldmap";

	public const string PLAYER_LEAVE_WORLDMAP = "player_leave_worldmap";

	public const string PLAYER_ENTER_HUB = "player_enter_hub";

	public const string PLAYER_LEAVE_HUB = "player_leave_hub";

	public const string PLAYER_ENTER_CHARBUILDER = "player_enter_charbuilder";

	public const string PLAYER_LEAVE_CHARBUILDER = "player_leave_charbuilder";

	public const string NPC_SHOP_ITEMS = "npc_shop_items";

	public const string NPC_SHOP_BUY_FAIL = "npc_shop_buy_fail";

	public const string NPC_SHOP_BUY_SUCCESS = "npc_shop_buy_success";

	public const string NPC_SHOP_SELL_FAIL = "npc_shop_sell_fail";

	public const string NPC_SHOP_SELL_SUCCESS = "npc_shop_sell_success";

	public const string CHARBUILDER_DATA = "charbuilder_data";

	public const string SERVER_START_SCREEN_INFO = "server_start_screen_info";

	public const string PLAYER_CREATE_GROUP = "player_create_group";

	public const string PLAYER_JOIN_GROUP = "player_join_group";

	public const string PLAYER_LEAVE_GROUP = "player_leave_group";

	public const string GROUP_INVITED = "group_invited";

	public const string GROUP_MEMBER_UPDATE = "group_member_update";

	public const string GROUP_MEMBER_KICKED = "group_member_kicked";

	public const string GROUP_LEADER_CHANGED = "group_leader_changed";

	public const string GROUP_TRAVEL_INITIATED = "group_travel_initiated";

	public const string GROUP_TRAVEL_RESPONSE = "group_travel_response";

	public const string GROUP_TRAVEL_FAILED = "group_travel_failed";

	public const string GROUP_INVITE_RESPONSE = "group_invite_response";

	public const string GUEST_ENTER_HOME = "guest_enter_home";

	public const string YOURTWEET = "yourtweet";

	public const string THUMBNAIL_AVATAR_INFO = "thumbnail_avatar_info";

	public const string THUMBNAIL_AVATAR_INFO_NO_AVATAR = "thumbnail_avatar_info_no_avatar";

	public const string PLAYER_PROFILE = "player_profile";

	public const string PLAYERS_UPDATE = "players_update";

	public const string OFFER_QUEST = "offer_quest";

	public const string NO_MORE_QUESTS = "no_more_quests";

	public const string NPC_DIALOGUE = "npc_dialogue";

	public const string NPC_MESSAGE = "npc_message";

	public const string CONDITION_DIALOGUE = "condition_dialogue";

	public const string QUEST_ACCEPTED = "quest_accepted";

	public const string PLAYER_CLASS_LEVEL_UPDATE = "player_class_level_update";

	public const string CLASS_SELECTED = "class_selected";

	public const string PLAYER_AVAILABLE_SKILLS_UPDATE = "player_available_skills_update";

	public const string INTERNAL_QUEST_CONDITION_UPDATE = "internal_quest_condition_update";

	public const string QUEST_UPDATED = "quest_updated";

	public const string QUEST_ADDED = "quest_added";

	public const string QUEST_RECEIVED = "quest_received";

	public const string BODYPACK_UPDATE = "bodypack_update";

	public const string WIELD_FAIL = "wield_fail";

	public const string ATTACK_REJECTED = "attack_rejected";

	public const string CHANGE_LEVEL_OFFER = "change_level_offer";

	public const string NPC_CHANGE_INTERACTION_STATE = "npc_change_interaction_state";

	public const string LEVEL_PLAYER_COUNTS_INFO = "level_player_counts_info";

	public const string RECEIVED_VOUCHER_INFORMATION = "received_voucher_information";

	public const string VOUCHER_INFO_LOADED = "voucher_info_loaded";

	public const string AMMO_TYPES = "ammo_types";

	public const string AMMO_UPDATE = "ammo_update";

	public const string AMMOMANAGER_GOT_AMMO = "ammomanager_got_ammo";

	public const string AMMOMANAGER_LOST_AMMO = "ammomanager_lost_ammo";

	public const string WIELDABLE_CHANGED = "wieldable_changed";

	public const string LEVEL_NPC_CREATE = "level_npc_create";

	public const string LEVEL_GAMEPLAYOBJECT_CREATE = "level_gameplayobject_create";

	public const string LEVEL_EXPLORATION_TOKEN_CREATE = "level_explorationtoken_create";

	public const string LEVEL_COIN_TOKEN_CREATE = "level_cointoken_create";

	public const string LEVEL_PREMIUM_TOKEN_CREATE = "level_premiumtoken_create";

	public const string LEVEL_STATIC_GEMS_CREATE = "level_static_gems_create";

	public const string NPC_ENTER_RANGE = "npc_enter_range";

	public const string NPC_LEAVE_RANGE = "npc_leave_range";

	public const string LOCAL_PLAYER_LEAVE_LEVEL = "local_player_leave_level";

	public const string REMOTE_PLAYER_LEFT_CLICKED = "remote_player_left_clicked";

	public const string LOCAL_PLAYER_LEFT_CLICKED = "local_player_left_clicked";

	public const string FURNITURE_LEFT_CLICKED = "furniture_left_clicked";

	public const string PAUSE = "Pause";

	public const string UNPAUSE = "Unpause";

	public const string WORLD_ACTIVATED = "world_activated";

	public const string HOME_FURNITURE_ACTIVATED = "home_furniture_activated";

	public const string PVP_RANK_RECEIVED = "pvp_rank_received";

	public const string PVP_LADDER_RECEIVED = "pvp_ladder_received";

	public const string PLAYER_JOINS_TEAM = "player_joins_team";

	public const string PLAYER_LEAVES_TEAM = "player_leaves_team";

	public const string TEAMS_COMPOSITION = "team_composition";

	public const string PVP_QUEUE_SIZE_UPDATE = "pvp_queue_size_update";

	public const string PVP_QUEUES = "pvp_queues";

	public const string PVP_OK_TO_JOIN = "pvp_ok_to_join";

	public const string PLAYER_DELIVERED_FLAG = "player_delivered_flag";

	public const string PLAYER_DIED = "player_died";

	public const string PLAYERS_ALIVE_COUNT = "players_alive_count";

	public const string START_CAPTURE_ZONE_COUNTDOWN = "start_capture_zone_countdown";

	public const string STOP_CAPTURE_ZONE_COUNTDOWN = "stop_capture_zone_countdown";

	public const string TEAM_CAPTURED_ZONE = "team_captured_zone";

	public const string NAME_CHANGE_RESULT = "name_change_result";

	public const string PLAY_EMOTE = "play_emote";

	public const string BUTTON_TOGGLE_GUI = "button_ToggleGUI";

	public const string BUTTON_FORWARD = "button_Forward";

	public const string BUTTON_BACKWARD = "button_Backward";

	public const string BUTTON_RIGHT = "button_Right";

	public const string BUTTON_LEFT = "button_Left";

	public const string BUTTON_UP = "button_Up";

	public const string BUTTON_DOWN = "button_Down";

	public const string BUTTON_OK = "button_Ok";

	public const string BUTTON_CANCEL = "button_Cancel";

	public const string BUTTON_JUMP = "button_Jump";

	public const string BUTTON_ATTACK = "button_Attack";

	public const string BUTTON_ACTION = "button_Action";

	public const string BUTTON_RESET_CAMERA = "button_ResetCamera";

	public const string BUTTON_RESPAWN = "button_Respawn";

	public const string BUTTON_TOGGLE_RUN_WALK = "button_ToggleRunWalk";

	public const string BUTTON_TOGGLE_NEWSPAGE = "button_ToggleNewsPage";

	public const string BUTTON_TOGGLE_PVPSCOREBOARD = "button_TogglePVPScoreBoard";

	public const string BUTTON_TOGGLE_MAINMENU = "button_ToggleMainMenu";

	public const string BUTTON_CHANGE_CAMERA_MODE = "button_ChangeCameraMode";

	public const string BUTTON_TOGGLE_INVITEDIALOG = "button_ToggleInviteDialog";

	public const string BUTTON_TOGGLE_PAUSE = "button_TogglePause";

	public const string BUTTON_ROTATE_LEFT = "button_RotateLeft";

	public const string BUTTON_ROTATE_RIGHT = "button_RotateRight";

	public const string BUTTON_ZOOM_IN = "button_ZoomIn";

	public const string BUTTON_ZOOM_OUT = "button_ZoomOut";

	public const string TUTORIAL_TRIGGERED = "tutorial_triggered";

	public const string TUTORIAL_AREA = "tutorial_Area";

	public const string TUTORIAL_RECEIVE_GEM = "tutorial_ReceiveGem";

	public const string TUTORIAL_RECEIVE_TELEPOD = "tutorial_ReceiveTelepod";

	public const string TUTORIAL_RECEIVE_COIN = "tutorial_ReceiveCoin";

	public const string TUTORIAL_USE_COINS = "tutorial_UseCoins";

	public const string TUTORIAL_RECEIVE_MEDAL = "tutorial_ReceiveMedal";

	public const string TUTORIAL_RECEIVE_EXPLORATION_TOKEN = "tutorial_ReceiveExplorationToken";

	public const string TUTORIAL_RECEIVE_ITEM = "tutorial_ReceiveItem";

	public const string TUTORIAL_RECEIVE_CONSUMABLE = "tutorial_ReceiveConsumable";

	public const string TUTORIAL_RECEIVE_ITEM_TYPE = "tutorial_ReceiveItemType";

	public const string TUTORIAL_RECEIVE_FRIEND_REQUEST = "tutorial_ReceiveFriendRequest";

	public const string TUTORIAL_RECEIVE_QUEST = "tutorial_ReceiveQuest";

	public const string TUTORIAL_COMPLETE_QUEST = "tutorial_CompleteQuest";

	public const string TUTORIAL_FIND_ALL_EXPLORATION_TOKENS = "tutorial_FindAllExplorationTokens";

	public const string TUTORIAL_ENTER_COMBAT = "tutorial_EnterCombat";

	public const string TUTORIAL_TALK_TO = "tutorial_TalkTo";

	public const string TUTORIAL_STOP_TALK_TO = "tutorial_StopTalkTo";

	public const string TUTORIAL_ENABLE_CHAT = "tutorial_EnableChat";

	public const string TUTORIAL_CHAT = "tutorial_Chat";

	public const string TUTORIAL_TOGGLE_BAG = "tutorial_ToggleBag";

	public const string TUTORIAL_TOGGLE_PROFILE = "tutorial_ToggleProfile";

	public const string TUTORIAL_TOGGLE_QUEST_LOG = "tutorial_ToggleQuestLog";

	public const string TUTORIAL_OPEN_SHOP = "tutorial_OpenShop";

	public const string TUTORIAL_UNLOCK_LEVEL = "tutorial_UnlockLevel";

	public const string TUTORIAL_JUMP = "tutorial_Jump";

	public const string TUTORIAL_CLIMB = "tutorial_Climb";

	public const string TUTORIAL_POWER_UP = "tutorial_PowerUp";

	public const string TUTORIAL_KILL = "tutorial_Kill";

	public const string TUTORIAL_DIE = "tutorial_Die";

	public const string TUTORIAL_USE_GAMEPLAY_OBJECT = "tutorial_UseGameplayObject";

	public const string TUTORIAL_MELEE_ATTACK = "tutorial_MeleeAttack";

	public const string TUTORIAL_RANGED_ATTACK = "tutorial_RangedAttack";

	public const string TUTORIAL_TOGGLE_WIELDABLE = "tutorial_ToggleWieldable";

	public const string TUTORIAL_WIELDABLE_CHANGED = "tutorial_WieldableChanged";

	public const string TUTORIAL_ENTER_PLAYER_STATE = "tutorial_EnterPlayerState";

	public const string TUTORIAL_SIT = "tutorial_Sit";

	public const string TUTORIAL_ENTER_TOWN = "tutorial_EnterTown";

	public const string TUTORIAL_LEAVE_TOWN = "tutorial_LeaveTown";

	public const string TUTORIAL_WORLD_MAP = "tutorial_WorldMap";

	public const string TUTORIAL_ENTER_OWN_HOME = "tutorial_EnterOwnHome";

	public const string TUTORIAL_ENTER_FURNISHING_MODE = "tutorial_EnterFurnishingMode";

	public const string TUTORIAL_OPEN_STORAGE = "tutorial_OpenStorage";

	public const string TUTORIAL_SHOW_ATTACH_NODES = "tutorial_ShowAttachNodes";

	public const string TUTORIAL_GRAB_FURNITURE = "tutorial_GrabFurniture";

	public const string TUTORIAL_RELEASE_FURNITURE = "tutorial_ReleaseFurniture";

	public const string TUTORIAL_MOVE_FURNITURE_TO_STORAGE = "tutorial_MoveFurnitureToStorage";

	public const string TUTORIAL_OPEN_NAVIGATOR_AND_HAS_HOME_KEY = "tutorial_OpenNavigatorAndHasHomeKey";

	public const string TUTORIAL_OPEN_MYSTERY_BOX = "tutorial_OpenMysteryBox";

	public const string TUTORIAL_OPEN_LOCK_BOX = "tutorial_OpenLockBox";

	public const string TUTORIAL_LEVELUP = "tutorial_LevelUp";

	public const string TUTORIAL_GAIN_EXP = "tutorial_GainExp";

	public const string TUTORIAL_JOIN_PVP = "tutorial_JoinPVP";

	public const string TUTORIAL_CLASS_ABILITY = "tutorial_ClassAbility";

	public const string TUTORIAL_CLASS_QUESTS = "tutorial_ClassQuests";

	public const string TUTORIAL_CLASS_SELECTION = "tutorial_ClassSelection";

	public const string TUTORIAL_GROUPS = "tutorial_Groups";

	public const string TUTORIAL_INVITE_FRIENDS = "tutorial_InviteFriends";

	public const string TUTORIAL_ROOM_IN_STORAGE = "tutorial_RoomInStorage";

	public const string TUTORIAL_PLACE_ROOM = "tutorial_PlaceRoom";

	public const string TUTORIAL_EXIT_FURNISHING_MODE = "tutorial_ExitFurnishingMode";

	public const string TUTORIAL_ADDITIONAL_DIALOG_CLOSED = "tutorial_AdditionalDialogClosed";

	public const string TUTORIAL_TAKE_DAMAGE = "tutorial_TakeDamage";

	public const string REMOTE_PLAYER_ADDED = "remote_player_added";

	public const string REMOTE_PLAYER_JOIN_INSTANCE = "remote_player_join_level";

	public const string REMOTE_PLAYER_LEAVE_INSTANCE = "remote_player_leave_instance";

	public const string UPDATE_POSITION = "update_position";

	public const string UPDATE_KNOCKBACK = "update_knockback";

	public const string PLAYER_STOP = "player_stop";

	public const string CHAT_TO_ALL = "chat_to_all";

	public const string UPDATE_BODYPACKS = "update_bodypacks";

	public const string UPDATE_EMOTE = "update_emote";

	public const string UPDATE_MOOD = "update_mood";

	public const string SHOW_BADGE = "show_badge";

	public const string HIDE_BADGE = "hide_badge";

	public const string REMOTE_PLAYER_INFO = "remote_player_info";

	public const string ITEM_WIELD_OK = "item_wield_ok";

	public const string ITEM_UNWIELD_OK = "item_unwield_ok";

	public const string ENTER_CHATROOM = "enter_chatroom";

	public const string LEAVE_CHATROOM = "leave_chatroom";

	public const string PLAYER_HEALTH_UPDATE = "player_health_update";

	public const string ABILITY_ACTIVATED = "ability_activated";

	public const string ABILITY_DEACTIVATED = "ability_deactivated";

	public const string SKILL_ACTIVATED = "skill_activated";

	public const string SKILL_ACTIVATION_FAILED = "skill_activation_failed";

	public const string SKILL_ITEM_ACTIVATED = "skill_item_activated";

	public const string SKILL_ITEM_ACTIVATION_FAILED = "skill_item_activation_failed";

	public const string ABILITY_EQUIPPED = "ability_equipped";

	public const string ABILITY_UNEQUIPPED = "ability_unequipped";

	public const string PLAYERSTATE_ACTIVATED = "playerstate_activated";

	public const string PLAYERSTATE_DEACTIVATED = "playerstate_deactivated";

	public const string ACTIVATE_ABILITY = "activate_ability";

	public const string PLAYER_CHANGE_ROOM = "player_change_room";

	public const string TITLE_CHANGE_SUCCESS = "title_change_success";

	public const string PLAY_ANIMATION = "play_animation";

	public const string REMOTE_ENTITY_STATE_UPDATE = "remote_entity_state_update";

	public const string ENTITY_STATE_UPDATE = "entity_state_update";

	public const string ENTITY_STATE_EFFECT_ADDED = "entity_state_effect_added";

	public const string HOME_EQUIPMENT_ADD = "home_equipment_add";

	public const string HOME_EQUIPMENT_MOVE = "home_equipment_move";

	public const string HOME_EQUIPMENT_ROTATED = "home_equipment_rotated";

	public const string HOME_EQUIPMENT_GRABBED = "home_equipment_grabbed";

	public const string HOME_EQUIPMENT_MOVE_FROM_STORAGE = "home_equipment_move_from_storage";

	public const string HOME_EQUIPMENT_MOVE_FROM_OTHER_ROOM = "home_equipment_move_from_other_room";

	public const string REQUEST_MOVE_FROM_STORAGE = "request_move_from_storage";

	public const string APPLY_ROOM_SKIN = "apply_room_skin";

	public const string ALL_EQUIPMENT_IN_ROOM_SENT = "all_equipment_in_room_sent";

	public const string ROOM_GRID_CHANGED = "room_grid_changed";

	public const string ROOM_ENTRANCE_CHANGED = "room_entrance_changed";

	public const string ENTER_FURNISHING_MODE = "enter_furnishing_mode";

	public const string EXIT_FURNISHING_MODE = "exit_furnishing_mode";

	public const string GO_TO_FURNITURE = "go_to_furniture";

	public const string MOVE_FURNITURE_TO_STORAGE = "move_furniture_to_storage";

	public const string PLAYER_KICKED_FROM_HOME = "player_kicked_from_home";

	public const string KICK_PLAYER_FROM_HOME_RESULT = "kick_player_from_home_result";

	public const string DISABLE_FURNITURE_COLLISION = "disable_furniture_collision";

	public const string ENABLE_FURNITURE_COLLISION = "enable_furniture_collision";

	public const string ATTACHNODE_MARKER_CLICKED = "attachnode_marker_clicked";

	public const string PLAYER_CHANGE_ROOM_IN_HOME = "player_change_room_in_home";

	public const string GENERATE_HOME_DELIVERY_BOX_POSITION = "generate_home_delivery_box_position";

	public const string HOME_DELIVERY_BOX_PICKED_UP = "home_delivery_box_picked_up";

	public const string HOME_ROOM_CHANGE_FAILED = "home_room_change_failed";

	public const string HOME_REMOTE_PLAYER_BANNED = "home_remote_player_banned";

	public const string HOME_REMOTE_PLAYER_UNBANNED = "home_remote_player_unbanned";

	public const string SERVER_HOME_ACCESS_RESPONSE = "server_home_access_response";

	public const string HOME_OF_THE_DAY_RAFFLE_RESPONSE = "home_of_the_day_raffle_response";

	public const string OBJECT_CREATE = "object_create";

	public const string OBJECT_MOVABLE_CREATE = "object_movable_create";

	public const string LEVEL_PROJECTILE_CREATE = "level_projectile_create";

	public const string MOVABLE_HEALTH_UPDATE = "movable_health_update";

	public const string MOVABLE_IMPULSE = "movable_impulse";

	public const string MOVABLE_STUN = "movable_stun";

	public const string OBJECT_UPDATE = "object_update";

	public const string OBJECT_DESTROY = "object_destroy";

	public const string GAMEPLAY_OBJECT_START_MOVING = "gameplay_object_start_moving";

	public const string GAMEPLAY_OBJECT_CHANGE_DIRECTION = "gameplay_object_change_direction";

	public const string GAMEPLAY_OBJECT_SHRINK = "gameplay_object_shrink";

	public const string START_CLIMB = "start_climb";

	public const string STOP_CLIMB = "stop_climb";

	public const string ENTER_GAMEPLAY_OBJECT = "enter_gameplay_object";

	public const string LEAVE_GAMEPLAY_OBJECT = "leave_gameplay_object";

	public const string LEVEL_OBJECT_PICKUP = "level_object_pickup";

	public const string RECEIVE_OBJECT_HAPPY = "receive_object_happy";

	public const string EXPLORATION_TOKEN_FOUND = "exploration_token_found";

	public const string COIN_TOKEN_FOUND = "coin_token_found";

	public const string STATIC_GEM_FOUND = "static_gem_found";

	public const string PREMIUM_TOKEN_FOUND = "premium_token_found";

	public const string LEVEL_OBJECT_CREATURE_SPAWN = "level_object_creature_spawn";

	public const string PICKUP_FAIL = "pickup_fail";

	public const string RECEIVED_ON_LAND = "received_on_land";

	public const string PLAYER_EXHAUSTED = "player_exhausted";

	public const string PLAYER_RESPAWNED = "player_respawned";

	public const string PLAYER_END_INVULNERABLE = "player_end_invulnerable";

	public const string MOVABLE_OBJECT_ATTACK_INSTANT = "movable_object_attack_instant";

	public const string MOVABLE_OBJECT_PREPARE_ATTACK = "movable_object_prepare_attack";

	public const string MOVABLE_OBJECT_BEGIN_ATTACK = "movable_object_begin_attack";

	public const string MOVABLE_OBJECT_ATTACK_IMPACT = "movable_object_attack_impact";

	public const string MOVABLE_OBJECT_ATTACK_DONE = "movable_object_attack_done";

	public const string CAPSULE_ATTACK_DAMAGE = "capsule_attack_damage";

	public const string CREATURE_HIT_BY_PROJECTILE = "creature_hit_by_projectile";

	public const string PLAYER_HIT_BY_PROJECTILE = "player_hit_by_projectile";

	public const string PROJECTILE_DESTOYED = "projectile_destroyed";

	public const string PLAYER_ATTACK_HIT = "player_attack_hit";

	public const string PLAYER_ATTACK_MISS = "player_attack_miss";

	public const string PLAYER_ATTACK_UNTARGETED = "player_attack_untargeted";

	public const string PLAYER_ATTACK_REJECTED = "player_attack_rejected";

	public const string PLAYER_KILLED_PLAYER = "player_killed_player";

	public const string PLAYER_DIG = "player_dig";

	public const string PLAYER_EAT = "player_eat";

	public const string CREATURE_AGGRO = "creature_aggro";

	public const string CREATURE_NO_AGGRO = "creature_no_aggro";

	public const string BOSS_CHANGE_MODE = "boss_change_mode";

	public const string ACTIVATE_GAMEPLAY_TRIGGER = "activate_gameplay_trigger";

	public const string SET_GAMEPLAY_OBJECT_POS = "set_gameplay_object_pos";

	public const string WORLD_LEVEL_CHANGED = "world_level_changed";

	public const string WORLD_LEVEL_UPDATED = "world_level_updated";

	public const string WORLD_LEVELS = "world_levels";

	public const string EXPLORATION_TOKENS_DATA = "exploration_tokens_data";

	public const string PREMIUM_TOKENS_DATA = "premium_tokens_data";

	public const string LEVEL_STATE_UNLOCKED = "level_state_unlocked";

	public const string USED_LEVEL_EXITS_INFO = "used_level_exits_info";

	public const string VISUAL_REP_SPAWNED = "visual_rep_spawned";

	public const string UPDATE_MATCH_STATE = "update_match_state";

	public const string SCOREBOARD_ARRIVED = "scoreboard_arrived";

	public const string UPDATE_KILLSDEATHS = "update_killsdeaths";

	public const string NOTIFICATION = "notification";

	public const string REQUEST_NOTIFICATIONS = "request_notifications";

	public const string BOX_OPENED = "box_opened";

	public const string RECEIVED_INVITE_REWARDS = "received_invite_rewards";

	public const string CONVERTER_USED = "converter_used";

	public const string USE_CONVERTER_FAILED = "use_converter_failed";

	public const string PLAYER_TITLE_CHANGE_FAILED = "player_title_change_failed";

	public const string PLAYER_TITLE_CHANGE_SUCCESS = "player_title_change_success";

	public const string REQUESTED_PLAYER_NAME_RESPONSE = "requested_player_name_response";

	public const string WORLD_QUEST_AREA_SUBSCRIBE = "world_quest_area_subscribe";

	public const string WORLD_QUEST_AREA_UNSUBSCRIBE = "world_quest_area_unsubscribe";

	public const string NO_PLAYERCOLLISION_AREA_CREATED = "no_playercollision_area_created";

	public const string NO_PLAYERCOLLISION_AREA_DESTROYED = "no_playercollision_area_destroyed";

	public const string CHECK_AVATAR_NAME_RESPONSE = "check_avatar_name_response";

	public const string PLAYER_ENTER_COMBAT = "player_enter_combat";

	public const string PLAYER_EXIT_COMBAT = "player_exit_combat";

	public const string PLAYER_ENTERED_CHATROOM = "player_entered_chatroom";

	public const string PLAYER_LEFT_CHATROOM = "player_left_chatroom";

	public const string LOW_HEALTH = "low_health";

	public const string USE_RANGED_WEAPON = "use_ranged_weapon";

	public const string USE_MELEE_WEAPON = "use_melee_weapon";

	public const string SPLINE_RIDE_START = "spline_ride_start";

	public const string SPLINE_RIDE_END = "spline_ride_end";

	public const string TELEPORT_PLAYER = "teleport_player";

	public const string TELEPORT_TO_FRIEND_FAILED = "teleport_to_friend_failed";

	public const string TELEPORT_TO_FRIEND_OK = "teleport_to_friend_ok";

	public const string DEBUG_CREATURE_SYNC = "debug_creature_sync";

	public const string DEBUG_CAPSULE_VOLUME_SYNC = "debug_capsule_volume_sync";

	public const string SITPOINT_CAMERA_FOCUS = "sitpoint_camera_focus";

	public const string SITPOINT_CAMERA_RELEASE_FOCUS = "sitpoint_camera_release_focus";

	public const string EQUIP_ARMOR = "equip_armor";

	public const string ON_INVITE_REQUEST = "on_invite_request";

	public const string HUBITEM_STATE_OFF = "hubitem_state_off";

	public const string HUBITEM_STATE_ON = "hubitem_state_on";

	public const string HOME_OF_THE_DAY_RESPONSE = "home_of_the_day_response";

	public const string VOTE_INFO_RECEIVED = "vote_info_received";

	public const string VOTE_SCORE_RECEIVED = "vote_score_received";

	public const string VOTE_NUMVOTES_RECEIVED = "vote_numvotes_received";

	public const string LADDER_PAGE_RECEIVED = "ladder_page_received";

	public const string RESET_AFK_TIMER = "reset_afk_timer";

	public const string SET_AFK = "set_afk";

	public const string INITIAL_SETTINGS_RECEIVED = "initial_settings_received";
}
