using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Core.Input;

public class PlayerControls : IInputActionCollection2, IInputActionCollection, IEnumerable<InputAction>, IEnumerable, IDisposable
{
	public struct PlayerActions
	{
		private PlayerControls m_Wrapper;

		public InputAction Move => m_Wrapper.m_Player_Move;

		public InputAction Jump => m_Wrapper.m_Player_Jump;

		public InputAction Look => m_Wrapper.m_Player_Look;

		public InputAction Fire => m_Wrapper.m_Player_Fire;

		public InputAction PrevWeapon => m_Wrapper.m_Player_PrevWeapon;

		public InputAction NextWeapon => m_Wrapper.m_Player_NextWeapon;

		public InputAction Skill => m_Wrapper.m_Player_Skill;

		public InputAction Use => m_Wrapper.m_Player_Use;

		public InputAction AbilitySlot1 => m_Wrapper.m_Player_AbilitySlot1;

		public InputAction AbilitySlot2 => m_Wrapper.m_Player_AbilitySlot2;

		public InputAction AbilitySlot3 => m_Wrapper.m_Player_AbilitySlot3;

		public InputAction AbilitySlot4 => m_Wrapper.m_Player_AbilitySlot4;

		public InputAction AbilitySlot5 => m_Wrapper.m_Player_AbilitySlot5;

		public InputAction Emote1 => m_Wrapper.m_Player_Emote1;

		public InputAction Emote2 => m_Wrapper.m_Player_Emote2;

		public InputAction Emote3 => m_Wrapper.m_Player_Emote3;

		public InputAction Emote4 => m_Wrapper.m_Player_Emote4;

		public InputAction Emote5 => m_Wrapper.m_Player_Emote5;

		public InputAction Emote6 => m_Wrapper.m_Player_Emote6;

		public InputAction Respawn => m_Wrapper.m_Player_Respawn;

		public InputAction Bag => m_Wrapper.m_Player_Bag;

		public InputAction WorldMap => m_Wrapper.m_Player_WorldMap;

		public InputAction FriendList => m_Wrapper.m_Player_FriendList;

		public InputAction Chat => m_Wrapper.m_Player_Chat;

		public InputAction Menu => m_Wrapper.m_Player_Menu;

		public InputAction QuestLog => m_Wrapper.m_Player_QuestLog;

		public InputAction Console => m_Wrapper.m_Player_Console;

		public bool enabled => Get().enabled;

		public PlayerActions(PlayerControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Player;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PlayerActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IPlayerActions instance)
		{
			if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
			{
				Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
				Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
				Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
				Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
				Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
				Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
				Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
				Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
				Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
				Fire.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
				Fire.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
				Fire.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFire;
				PrevWeapon.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrevWeapon;
				PrevWeapon.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrevWeapon;
				PrevWeapon.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPrevWeapon;
				NextWeapon.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextWeapon;
				NextWeapon.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextWeapon;
				NextWeapon.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextWeapon;
				Skill.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill;
				Skill.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill;
				Skill.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSkill;
				Use.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUse;
				Use.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUse;
				Use.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUse;
				AbilitySlot1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot1;
				AbilitySlot1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot1;
				AbilitySlot1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot1;
				AbilitySlot2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot2;
				AbilitySlot2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot2;
				AbilitySlot2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot2;
				AbilitySlot3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot3;
				AbilitySlot3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot3;
				AbilitySlot3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot3;
				AbilitySlot4.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot4;
				AbilitySlot4.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot4;
				AbilitySlot4.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot4;
				AbilitySlot5.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot5;
				AbilitySlot5.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot5;
				AbilitySlot5.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbilitySlot5;
				Emote1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote1;
				Emote1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote1;
				Emote1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote1;
				Emote2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote2;
				Emote2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote2;
				Emote2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote2;
				Emote3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote3;
				Emote3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote3;
				Emote3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote3;
				Emote4.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote4;
				Emote4.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote4;
				Emote4.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote4;
				Emote5.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote5;
				Emote5.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote5;
				Emote5.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote5;
				Emote6.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote6;
				Emote6.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote6;
				Emote6.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnEmote6;
				Respawn.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRespawn;
				Respawn.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRespawn;
				Respawn.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRespawn;
				Bag.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBag;
				Bag.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBag;
				Bag.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBag;
				WorldMap.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWorldMap;
				WorldMap.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWorldMap;
				WorldMap.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnWorldMap;
				FriendList.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFriendList;
				FriendList.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFriendList;
				FriendList.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFriendList;
				Chat.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChat;
				Chat.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChat;
				Chat.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChat;
				Menu.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
				Menu.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
				Menu.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMenu;
				QuestLog.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuestLog;
				QuestLog.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuestLog;
				QuestLog.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuestLog;
				Console.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnConsole;
				Console.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnConsole;
				Console.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnConsole;
			}
			m_Wrapper.m_PlayerActionsCallbackInterface = instance;
			if (instance != null)
			{
				Move.started += instance.OnMove;
				Move.performed += instance.OnMove;
				Move.canceled += instance.OnMove;
				Jump.started += instance.OnJump;
				Jump.performed += instance.OnJump;
				Jump.canceled += instance.OnJump;
				Look.started += instance.OnLook;
				Look.performed += instance.OnLook;
				Look.canceled += instance.OnLook;
				Fire.started += instance.OnFire;
				Fire.performed += instance.OnFire;
				Fire.canceled += instance.OnFire;
				PrevWeapon.started += instance.OnPrevWeapon;
				PrevWeapon.performed += instance.OnPrevWeapon;
				PrevWeapon.canceled += instance.OnPrevWeapon;
				NextWeapon.started += instance.OnNextWeapon;
				NextWeapon.performed += instance.OnNextWeapon;
				NextWeapon.canceled += instance.OnNextWeapon;
				Skill.started += instance.OnSkill;
				Skill.performed += instance.OnSkill;
				Skill.canceled += instance.OnSkill;
				Use.started += instance.OnUse;
				Use.performed += instance.OnUse;
				Use.canceled += instance.OnUse;
				AbilitySlot1.started += instance.OnAbilitySlot1;
				AbilitySlot1.performed += instance.OnAbilitySlot1;
				AbilitySlot1.canceled += instance.OnAbilitySlot1;
				AbilitySlot2.started += instance.OnAbilitySlot2;
				AbilitySlot2.performed += instance.OnAbilitySlot2;
				AbilitySlot2.canceled += instance.OnAbilitySlot2;
				AbilitySlot3.started += instance.OnAbilitySlot3;
				AbilitySlot3.performed += instance.OnAbilitySlot3;
				AbilitySlot3.canceled += instance.OnAbilitySlot3;
				AbilitySlot4.started += instance.OnAbilitySlot4;
				AbilitySlot4.performed += instance.OnAbilitySlot4;
				AbilitySlot4.canceled += instance.OnAbilitySlot4;
				AbilitySlot5.started += instance.OnAbilitySlot5;
				AbilitySlot5.performed += instance.OnAbilitySlot5;
				AbilitySlot5.canceled += instance.OnAbilitySlot5;
				Emote1.started += instance.OnEmote1;
				Emote1.performed += instance.OnEmote1;
				Emote1.canceled += instance.OnEmote1;
				Emote2.started += instance.OnEmote2;
				Emote2.performed += instance.OnEmote2;
				Emote2.canceled += instance.OnEmote2;
				Emote3.started += instance.OnEmote3;
				Emote3.performed += instance.OnEmote3;
				Emote3.canceled += instance.OnEmote3;
				Emote4.started += instance.OnEmote4;
				Emote4.performed += instance.OnEmote4;
				Emote4.canceled += instance.OnEmote4;
				Emote5.started += instance.OnEmote5;
				Emote5.performed += instance.OnEmote5;
				Emote5.canceled += instance.OnEmote5;
				Emote6.started += instance.OnEmote6;
				Emote6.performed += instance.OnEmote6;
				Emote6.canceled += instance.OnEmote6;
				Respawn.started += instance.OnRespawn;
				Respawn.performed += instance.OnRespawn;
				Respawn.canceled += instance.OnRespawn;
				Bag.started += instance.OnBag;
				Bag.performed += instance.OnBag;
				Bag.canceled += instance.OnBag;
				WorldMap.started += instance.OnWorldMap;
				WorldMap.performed += instance.OnWorldMap;
				WorldMap.canceled += instance.OnWorldMap;
				FriendList.started += instance.OnFriendList;
				FriendList.performed += instance.OnFriendList;
				FriendList.canceled += instance.OnFriendList;
				Chat.started += instance.OnChat;
				Chat.performed += instance.OnChat;
				Chat.canceled += instance.OnChat;
				Menu.started += instance.OnMenu;
				Menu.performed += instance.OnMenu;
				Menu.canceled += instance.OnMenu;
				QuestLog.started += instance.OnQuestLog;
				QuestLog.performed += instance.OnQuestLog;
				QuestLog.canceled += instance.OnQuestLog;
				Console.started += instance.OnConsole;
				Console.performed += instance.OnConsole;
				Console.canceled += instance.OnConsole;
			}
		}
	}

	public struct UIActions
	{
		private PlayerControls m_Wrapper;

		public InputAction Navigate => m_Wrapper.m_UI_Navigate;

		public InputAction Submit => m_Wrapper.m_UI_Submit;

		public InputAction Cancel => m_Wrapper.m_UI_Cancel;

		public InputAction Point => m_Wrapper.m_UI_Point;

		public InputAction Click => m_Wrapper.m_UI_Click;

		public InputAction ScrollWheel => m_Wrapper.m_UI_ScrollWheel;

		public InputAction MiddleClick => m_Wrapper.m_UI_MiddleClick;

		public InputAction RightClick => m_Wrapper.m_UI_RightClick;

		public bool enabled => Get().enabled;

		public UIActions(PlayerControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_UI;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(UIActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IUIActions instance)
		{
			if (m_Wrapper.m_UIActionsCallbackInterface != null)
			{
				Navigate.started -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
				Navigate.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
				Navigate.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
				Submit.started -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
				Submit.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
				Submit.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
				Cancel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
				Cancel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
				Cancel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
				Point.started -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
				Point.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
				Point.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
				Click.started -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
				Click.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
				Click.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
				ScrollWheel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
				ScrollWheel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
				ScrollWheel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
				MiddleClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
				MiddleClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
				MiddleClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
				RightClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
				RightClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
				RightClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
			}
			m_Wrapper.m_UIActionsCallbackInterface = instance;
			if (instance != null)
			{
				Navigate.started += instance.OnNavigate;
				Navigate.performed += instance.OnNavigate;
				Navigate.canceled += instance.OnNavigate;
				Submit.started += instance.OnSubmit;
				Submit.performed += instance.OnSubmit;
				Submit.canceled += instance.OnSubmit;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
				Point.started += instance.OnPoint;
				Point.performed += instance.OnPoint;
				Point.canceled += instance.OnPoint;
				Click.started += instance.OnClick;
				Click.performed += instance.OnClick;
				Click.canceled += instance.OnClick;
				ScrollWheel.started += instance.OnScrollWheel;
				ScrollWheel.performed += instance.OnScrollWheel;
				ScrollWheel.canceled += instance.OnScrollWheel;
				MiddleClick.started += instance.OnMiddleClick;
				MiddleClick.performed += instance.OnMiddleClick;
				MiddleClick.canceled += instance.OnMiddleClick;
				RightClick.started += instance.OnRightClick;
				RightClick.performed += instance.OnRightClick;
				RightClick.canceled += instance.OnRightClick;
			}
		}
	}

	public struct MenuActions
	{
		private PlayerControls m_Wrapper;

		public InputAction GoBack => m_Wrapper.m_Menu_GoBack;

		public bool enabled => Get().enabled;

		public MenuActions(PlayerControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Menu;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(MenuActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IMenuActions instance)
		{
			if (m_Wrapper.m_MenuActionsCallbackInterface != null)
			{
				GoBack.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnGoBack;
				GoBack.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnGoBack;
				GoBack.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnGoBack;
			}
			m_Wrapper.m_MenuActionsCallbackInterface = instance;
			if (instance != null)
			{
				GoBack.started += instance.OnGoBack;
				GoBack.performed += instance.OnGoBack;
				GoBack.canceled += instance.OnGoBack;
			}
		}
	}

	public struct CutsceneActions
	{
		private PlayerControls m_Wrapper;

		public InputAction ExitCutscene => m_Wrapper.m_Cutscene_ExitCutscene;

		public bool enabled => Get().enabled;

		public CutsceneActions(PlayerControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Cutscene;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CutsceneActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(ICutsceneActions instance)
		{
			if (m_Wrapper.m_CutsceneActionsCallbackInterface != null)
			{
				ExitCutscene.started -= m_Wrapper.m_CutsceneActionsCallbackInterface.OnExitCutscene;
				ExitCutscene.performed -= m_Wrapper.m_CutsceneActionsCallbackInterface.OnExitCutscene;
				ExitCutscene.canceled -= m_Wrapper.m_CutsceneActionsCallbackInterface.OnExitCutscene;
			}
			m_Wrapper.m_CutsceneActionsCallbackInterface = instance;
			if (instance != null)
			{
				ExitCutscene.started += instance.OnExitCutscene;
				ExitCutscene.performed += instance.OnExitCutscene;
				ExitCutscene.canceled += instance.OnExitCutscene;
			}
		}
	}

	public struct InputActions
	{
		private PlayerControls m_Wrapper;

		public InputAction Submit => m_Wrapper.m_Input_Submit;

		public InputAction Cancel => m_Wrapper.m_Input_Cancel;

		public bool enabled => Get().enabled;

		public InputActions(PlayerControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Input;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(InputActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IInputActions instance)
		{
			if (m_Wrapper.m_InputActionsCallbackInterface != null)
			{
				Submit.started -= m_Wrapper.m_InputActionsCallbackInterface.OnSubmit;
				Submit.performed -= m_Wrapper.m_InputActionsCallbackInterface.OnSubmit;
				Submit.canceled -= m_Wrapper.m_InputActionsCallbackInterface.OnSubmit;
				Cancel.started -= m_Wrapper.m_InputActionsCallbackInterface.OnCancel;
				Cancel.performed -= m_Wrapper.m_InputActionsCallbackInterface.OnCancel;
				Cancel.canceled -= m_Wrapper.m_InputActionsCallbackInterface.OnCancel;
			}
			m_Wrapper.m_InputActionsCallbackInterface = instance;
			if (instance != null)
			{
				Submit.started += instance.OnSubmit;
				Submit.performed += instance.OnSubmit;
				Submit.canceled += instance.OnSubmit;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
			}
		}
	}

	public struct ConsoleActions
	{
		private PlayerControls m_Wrapper;

		public InputAction Send => m_Wrapper.m_Console_Send;

		public InputAction Exit => m_Wrapper.m_Console_Exit;

		public InputAction HistoryBackward => m_Wrapper.m_Console_HistoryBackward;

		public InputAction HistoryForward => m_Wrapper.m_Console_HistoryForward;

		public InputAction NextCandidate => m_Wrapper.m_Console_NextCandidate;

		public bool enabled => Get().enabled;

		public ConsoleActions(PlayerControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Console;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ConsoleActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IConsoleActions instance)
		{
			if (m_Wrapper.m_ConsoleActionsCallbackInterface != null)
			{
				Send.started -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnSend;
				Send.performed -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnSend;
				Send.canceled -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnSend;
				Exit.started -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnExit;
				Exit.performed -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnExit;
				Exit.canceled -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnExit;
				HistoryBackward.started -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnHistoryBackward;
				HistoryBackward.performed -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnHistoryBackward;
				HistoryBackward.canceled -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnHistoryBackward;
				HistoryForward.started -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnHistoryForward;
				HistoryForward.performed -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnHistoryForward;
				HistoryForward.canceled -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnHistoryForward;
				NextCandidate.started -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnNextCandidate;
				NextCandidate.performed -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnNextCandidate;
				NextCandidate.canceled -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnNextCandidate;
			}
			m_Wrapper.m_ConsoleActionsCallbackInterface = instance;
			if (instance != null)
			{
				Send.started += instance.OnSend;
				Send.performed += instance.OnSend;
				Send.canceled += instance.OnSend;
				Exit.started += instance.OnExit;
				Exit.performed += instance.OnExit;
				Exit.canceled += instance.OnExit;
				HistoryBackward.started += instance.OnHistoryBackward;
				HistoryBackward.performed += instance.OnHistoryBackward;
				HistoryBackward.canceled += instance.OnHistoryBackward;
				HistoryForward.started += instance.OnHistoryForward;
				HistoryForward.performed += instance.OnHistoryForward;
				HistoryForward.canceled += instance.OnHistoryForward;
				NextCandidate.started += instance.OnNextCandidate;
				NextCandidate.performed += instance.OnNextCandidate;
				NextCandidate.canceled += instance.OnNextCandidate;
			}
		}
	}

	public struct DialogueActions
	{
		private PlayerControls m_Wrapper;

		public InputAction CloseDialogue => m_Wrapper.m_Dialogue_CloseDialogue;

		public bool enabled => Get().enabled;

		public DialogueActions(PlayerControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Dialogue;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(DialogueActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IDialogueActions instance)
		{
			if (m_Wrapper.m_DialogueActionsCallbackInterface != null)
			{
				CloseDialogue.started -= m_Wrapper.m_DialogueActionsCallbackInterface.OnCloseDialogue;
				CloseDialogue.performed -= m_Wrapper.m_DialogueActionsCallbackInterface.OnCloseDialogue;
				CloseDialogue.canceled -= m_Wrapper.m_DialogueActionsCallbackInterface.OnCloseDialogue;
			}
			m_Wrapper.m_DialogueActionsCallbackInterface = instance;
			if (instance != null)
			{
				CloseDialogue.started += instance.OnCloseDialogue;
				CloseDialogue.performed += instance.OnCloseDialogue;
				CloseDialogue.canceled += instance.OnCloseDialogue;
			}
		}
	}

	public struct AvatarEditorActions
	{
		private PlayerControls m_Wrapper;

		public InputAction Click => m_Wrapper.m_AvatarEditor_Click;

		public InputAction Drag => m_Wrapper.m_AvatarEditor_Drag;

		public bool enabled => Get().enabled;

		public AvatarEditorActions(PlayerControls wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_AvatarEditor;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(AvatarEditorActions set)
		{
			return set.Get();
		}

		public void SetCallbacks(IAvatarEditorActions instance)
		{
			if (m_Wrapper.m_AvatarEditorActionsCallbackInterface != null)
			{
				Click.started -= m_Wrapper.m_AvatarEditorActionsCallbackInterface.OnClick;
				Click.performed -= m_Wrapper.m_AvatarEditorActionsCallbackInterface.OnClick;
				Click.canceled -= m_Wrapper.m_AvatarEditorActionsCallbackInterface.OnClick;
				Drag.started -= m_Wrapper.m_AvatarEditorActionsCallbackInterface.OnDrag;
				Drag.performed -= m_Wrapper.m_AvatarEditorActionsCallbackInterface.OnDrag;
				Drag.canceled -= m_Wrapper.m_AvatarEditorActionsCallbackInterface.OnDrag;
			}
			m_Wrapper.m_AvatarEditorActionsCallbackInterface = instance;
			if (instance != null)
			{
				Click.started += instance.OnClick;
				Click.performed += instance.OnClick;
				Click.canceled += instance.OnClick;
				Drag.started += instance.OnDrag;
				Drag.performed += instance.OnDrag;
				Drag.canceled += instance.OnDrag;
			}
		}
	}

	public interface IPlayerActions
	{
		void OnMove(InputAction.CallbackContext context);

		void OnJump(InputAction.CallbackContext context);

		void OnLook(InputAction.CallbackContext context);

		void OnFire(InputAction.CallbackContext context);

		void OnPrevWeapon(InputAction.CallbackContext context);

		void OnNextWeapon(InputAction.CallbackContext context);

		void OnSkill(InputAction.CallbackContext context);

		void OnUse(InputAction.CallbackContext context);

		void OnAbilitySlot1(InputAction.CallbackContext context);

		void OnAbilitySlot2(InputAction.CallbackContext context);

		void OnAbilitySlot3(InputAction.CallbackContext context);

		void OnAbilitySlot4(InputAction.CallbackContext context);

		void OnAbilitySlot5(InputAction.CallbackContext context);

		void OnEmote1(InputAction.CallbackContext context);

		void OnEmote2(InputAction.CallbackContext context);

		void OnEmote3(InputAction.CallbackContext context);

		void OnEmote4(InputAction.CallbackContext context);

		void OnEmote5(InputAction.CallbackContext context);

		void OnEmote6(InputAction.CallbackContext context);

		void OnRespawn(InputAction.CallbackContext context);

		void OnBag(InputAction.CallbackContext context);

		void OnWorldMap(InputAction.CallbackContext context);

		void OnFriendList(InputAction.CallbackContext context);

		void OnChat(InputAction.CallbackContext context);

		void OnMenu(InputAction.CallbackContext context);

		void OnQuestLog(InputAction.CallbackContext context);

		void OnConsole(InputAction.CallbackContext context);
	}

	public interface IUIActions
	{
		void OnNavigate(InputAction.CallbackContext context);

		void OnSubmit(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);

		void OnPoint(InputAction.CallbackContext context);

		void OnClick(InputAction.CallbackContext context);

		void OnScrollWheel(InputAction.CallbackContext context);

		void OnMiddleClick(InputAction.CallbackContext context);

		void OnRightClick(InputAction.CallbackContext context);
	}

	public interface IMenuActions
	{
		void OnGoBack(InputAction.CallbackContext context);
	}

	public interface ICutsceneActions
	{
		void OnExitCutscene(InputAction.CallbackContext context);
	}

	public interface IInputActions
	{
		void OnSubmit(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);
	}

	public interface IConsoleActions
	{
		void OnSend(InputAction.CallbackContext context);

		void OnExit(InputAction.CallbackContext context);

		void OnHistoryBackward(InputAction.CallbackContext context);

		void OnHistoryForward(InputAction.CallbackContext context);

		void OnNextCandidate(InputAction.CallbackContext context);
	}

	public interface IDialogueActions
	{
		void OnCloseDialogue(InputAction.CallbackContext context);
	}

	public interface IAvatarEditorActions
	{
		void OnClick(InputAction.CallbackContext context);

		void OnDrag(InputAction.CallbackContext context);
	}

	private readonly InputActionMap m_Player;

	private IPlayerActions m_PlayerActionsCallbackInterface;

	private readonly InputAction m_Player_Move;

	private readonly InputAction m_Player_Jump;

	private readonly InputAction m_Player_Look;

	private readonly InputAction m_Player_Fire;

	private readonly InputAction m_Player_PrevWeapon;

	private readonly InputAction m_Player_NextWeapon;

	private readonly InputAction m_Player_Skill;

	private readonly InputAction m_Player_Use;

	private readonly InputAction m_Player_AbilitySlot1;

	private readonly InputAction m_Player_AbilitySlot2;

	private readonly InputAction m_Player_AbilitySlot3;

	private readonly InputAction m_Player_AbilitySlot4;

	private readonly InputAction m_Player_AbilitySlot5;

	private readonly InputAction m_Player_Emote1;

	private readonly InputAction m_Player_Emote2;

	private readonly InputAction m_Player_Emote3;

	private readonly InputAction m_Player_Emote4;

	private readonly InputAction m_Player_Emote5;

	private readonly InputAction m_Player_Emote6;

	private readonly InputAction m_Player_Respawn;

	private readonly InputAction m_Player_Bag;

	private readonly InputAction m_Player_WorldMap;

	private readonly InputAction m_Player_FriendList;

	private readonly InputAction m_Player_Chat;

	private readonly InputAction m_Player_Menu;

	private readonly InputAction m_Player_QuestLog;

	private readonly InputAction m_Player_Console;

	private readonly InputActionMap m_UI;

	private IUIActions m_UIActionsCallbackInterface;

	private readonly InputAction m_UI_Navigate;

	private readonly InputAction m_UI_Submit;

	private readonly InputAction m_UI_Cancel;

	private readonly InputAction m_UI_Point;

	private readonly InputAction m_UI_Click;

	private readonly InputAction m_UI_ScrollWheel;

	private readonly InputAction m_UI_MiddleClick;

	private readonly InputAction m_UI_RightClick;

	private readonly InputActionMap m_Menu;

	private IMenuActions m_MenuActionsCallbackInterface;

	private readonly InputAction m_Menu_GoBack;

	private readonly InputActionMap m_Cutscene;

	private ICutsceneActions m_CutsceneActionsCallbackInterface;

	private readonly InputAction m_Cutscene_ExitCutscene;

	private readonly InputActionMap m_Input;

	private IInputActions m_InputActionsCallbackInterface;

	private readonly InputAction m_Input_Submit;

	private readonly InputAction m_Input_Cancel;

	private readonly InputActionMap m_Console;

	private IConsoleActions m_ConsoleActionsCallbackInterface;

	private readonly InputAction m_Console_Send;

	private readonly InputAction m_Console_Exit;

	private readonly InputAction m_Console_HistoryBackward;

	private readonly InputAction m_Console_HistoryForward;

	private readonly InputAction m_Console_NextCandidate;

	private readonly InputActionMap m_Dialogue;

	private IDialogueActions m_DialogueActionsCallbackInterface;

	private readonly InputAction m_Dialogue_CloseDialogue;

	private readonly InputActionMap m_AvatarEditor;

	private IAvatarEditorActions m_AvatarEditorActionsCallbackInterface;

	private readonly InputAction m_AvatarEditor_Click;

	private readonly InputAction m_AvatarEditor_Drag;

	private int m_KeyboardMouseSchemeIndex = -1;

	private int m_GamepadSchemeIndex = -1;

	private int m_JoystickSchemeIndex = -1;

	public InputActionAsset asset { get; }

	public InputBinding? bindingMask
	{
		get
		{
			return asset.bindingMask;
		}
		set
		{
			asset.bindingMask = value;
		}
	}

	public ReadOnlyArray<InputDevice>? devices
	{
		get
		{
			return asset.devices;
		}
		set
		{
			asset.devices = value;
		}
	}

	public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

	public IEnumerable<InputBinding> bindings => asset.bindings;

	public PlayerActions Player => new PlayerActions(this);

	public UIActions UI => new UIActions(this);

	public MenuActions Menu => new MenuActions(this);

	public CutsceneActions Cutscene => new CutsceneActions(this);

	public InputActions Input => new InputActions(this);

	public ConsoleActions Console => new ConsoleActions(this);

	public DialogueActions Dialogue => new DialogueActions(this);

	public AvatarEditorActions AvatarEditor => new AvatarEditorActions(this);

	public InputControlScheme KeyboardMouseScheme
	{
		get
		{
			if (m_KeyboardMouseSchemeIndex == -1)
			{
				m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard&Mouse");
			}
			return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
		}
	}

	public InputControlScheme GamepadScheme
	{
		get
		{
			if (m_GamepadSchemeIndex == -1)
			{
				m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
			}
			return asset.controlSchemes[m_GamepadSchemeIndex];
		}
	}

	public InputControlScheme JoystickScheme
	{
		get
		{
			if (m_JoystickSchemeIndex == -1)
			{
				m_JoystickSchemeIndex = asset.FindControlSchemeIndex("Joystick");
			}
			return asset.controlSchemes[m_JoystickSchemeIndex];
		}
	}

	public PlayerControls()
	{
		asset = InputActionAsset.FromJson("{\n    \"name\": \"PlayerControls\",\n    \"maps\": [\n        {\n            \"name\": \"Player\",\n            \"id\": \"df70fa95-8a34-4494-b137-73ab6b9c7d37\",\n            \"actions\": [\n                {\n                    \"name\": \"Move\",\n                    \"type\": \"Value\",\n                    \"id\": \"351f2ccd-1f9f-44bf-9bec-d62ac5c5f408\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"Jump\",\n                    \"type\": \"Button\",\n                    \"id\": \"90040f53-68a8-4103-a010-8ed51fd65c77\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Look\",\n                    \"type\": \"Value\",\n                    \"id\": \"6b444451-8a00-4d00-a97e-f47457f736a8\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                },\n                {\n                    \"name\": \"Fire\",\n                    \"type\": \"Button\",\n                    \"id\": \"6c2ab1b8-8984-453a-af3d-a3c78ae1679a\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"PrevWeapon\",\n                    \"type\": \"Button\",\n                    \"id\": \"ccff2769-329b-4e01-a9f8-43ed5dbf67d1\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"NextWeapon\",\n                    \"type\": \"Button\",\n                    \"id\": \"d3240b71-004f-4c9f-83b7-0fdb5356d8c5\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Skill\",\n                    \"type\": \"Button\",\n                    \"id\": \"4d629914-6cdf-4dbb-85b8-8de416ed20ef\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Use\",\n                    \"type\": \"Button\",\n                    \"id\": \"ab939d1c-eacf-46dc-b28b-75101c5f49e7\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"AbilitySlot1\",\n                    \"type\": \"Button\",\n                    \"id\": \"9ae0844a-4d11-4402-a493-bcbec76ce94c\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"AbilitySlot2\",\n                    \"type\": \"Button\",\n                    \"id\": \"927de20e-d5d9-42e2-a939-e5e7b22ed81c\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"AbilitySlot3\",\n                    \"type\": \"Button\",\n                    \"id\": \"245e2423-efee-4ab9-bc5b-cf49244346d8\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"AbilitySlot4\",\n                    \"type\": \"Button\",\n                    \"id\": \"2c38ed73-f6e3-428f-8b09-797cba8c4f2c\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"AbilitySlot5\",\n                    \"type\": \"Button\",\n                    \"id\": \"7e86d288-bca1-436d-9d3d-cab6f3e8add5\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Emote1\",\n                    \"type\": \"Button\",\n                    \"id\": \"cf772706-8a80-4626-875a-65731a0ebdc0\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Emote2\",\n                    \"type\": \"Button\",\n                    \"id\": \"f11551bd-3c0a-49f9-9097-f35f3ced4eb7\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Emote3\",\n                    \"type\": \"Button\",\n                    \"id\": \"5e9a89a0-44a9-4a89-99db-91b99862ff82\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Emote4\",\n                    \"type\": \"Button\",\n                    \"id\": \"3e58f819-7286-449c-a78c-89290f9b17db\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Emote5\",\n                    \"type\": \"Button\",\n                    \"id\": \"862c5f16-0d89-4a8a-88aa-c9dd88dfeafd\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Emote6\",\n                    \"type\": \"Button\",\n                    \"id\": \"10ba691d-1468-4218-a7fa-7e0215547a65\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Respawn\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"f934e86c-d537-490d-aea6-2874700568e6\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Bag\",\n                    \"type\": \"Button\",\n                    \"id\": \"1316dada-d28c-49b1-a136-c0f550b9db78\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"WorldMap\",\n                    \"type\": \"Button\",\n                    \"id\": \"3a3bcd8b-4e56-4255-968f-8095eb69b8f7\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"FriendList\",\n                    \"type\": \"Button\",\n                    \"id\": \"637e2b00-07f0-4d17-aea3-ef26966660d4\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Chat\",\n                    \"type\": \"Button\",\n                    \"id\": \"bf745dfe-6e66-4b01-aaa4-e306d7622f47\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Menu\",\n                    \"type\": \"Button\",\n                    \"id\": \"6a1fe996-51ae-4143-95f0-bbe2fc2b3cea\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"QuestLog\",\n                    \"type\": \"Button\",\n                    \"id\": \"0ecd542b-8f97-4353-a557-f699a2f296ef\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Console\",\n                    \"type\": \"Button\",\n                    \"id\": \"68e1506f-74d1-4b59-bd0f-371599a68470\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"978bfe49-cc26-4a3d-ab7b-7d7a29327403\",\n                    \"path\": \"<Gamepad>/leftStick\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"WASD\",\n                    \"id\": \"00ca640b-d935-4593-8157-c05846ea39b3\",\n                    \"path\": \"Dpad\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Move\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"e2062cb9-1b15-46a2-838c-2f8d72a0bdd9\",\n                    \"path\": \"<Keyboard>/w\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"8180e8bd-4097-4f4e-ab88-4523101a6ce9\",\n                    \"path\": \"<Keyboard>/upArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"320bffee-a40b-4347-ac70-c210eb8bc73a\",\n                    \"path\": \"<Keyboard>/s\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"1c5327b5-f71c-4f60-99c7-4e737386f1d1\",\n                    \"path\": \"<Keyboard>/downArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"d2581a9b-1d11-4566-b27d-b92aff5fabbc\",\n                    \"path\": \"<Keyboard>/a\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"2e46982e-44cc-431b-9f0b-c11910bf467a\",\n                    \"path\": \"<Keyboard>/leftArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"fcfe95b8-67b9-4526-84b5-5d0bc98d6400\",\n                    \"path\": \"<Keyboard>/d\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"77bff152-3580-4b21-b6de-dcd0c7e41164\",\n                    \"path\": \"<Keyboard>/rightArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3ea4d645-4504-4529-b061-ab81934c3752\",\n                    \"path\": \"<Joystick>/stick\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Move\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c1f7a91b-d0fd-4a62-997e-7fb9b69bf235\",\n                    \"path\": \"<Gamepad>/rightStick\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"8c8e490b-c610-4785-884f-f04217b23ca4\",\n                    \"path\": \"<Pointer>/delta\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3e5f5442-8668-4b27-a940-df99bad7e831\",\n                    \"path\": \"<Joystick>/{Hatswitch}\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Look\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"143bb1cd-cc10-4eca-a2f0-a3664166fe91\",\n                    \"path\": \"<Gamepad>/rightTrigger\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Fire\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"05f6913d-c316-48b2-a6bb-e225f14c7960\",\n                    \"path\": \"<Mouse>/leftButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Fire\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ee3d0cd2-254e-47a7-a8cb-bc94d9658c54\",\n                    \"path\": \"<Joystick>/trigger\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Fire\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3f503b2a-e736-40b0-8778-806b9021b6cf\",\n                    \"path\": \"<Keyboard>/c\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Fire\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c0495ad1-cc9a-407d-97be-da835e7c0806\",\n                    \"path\": \"<Keyboard>/space\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Jump\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"9c784676-65c7-4442-816e-d5f1fd3f5e70\",\n                    \"path\": \"<Gamepad>/buttonSouth\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Jump\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c3910f59-7f16-4ee8-a66d-bd5bacc1b720\",\n                    \"path\": \"<Keyboard>/q\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"PrevWeapon\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b51a5024-1e3e-4357-93f7-6437af0eb5b3\",\n                    \"path\": \"<Keyboard>/delete\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"PrevWeapon\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"87cc8f9d-27d0-4a1c-b6b6-6a4e0805b91e\",\n                    \"path\": \"<Keyboard>/r\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Skill\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3081be61-6415-4c29-88ea-babe34d935b3\",\n                    \"path\": \"<Keyboard>/1\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"AbilitySlot1\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1f4fea52-5685-47ff-a495-3914562432c2\",\n                    \"path\": \"<Keyboard>/3\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"AbilitySlot3\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"db457e2c-8c77-4050-9ea6-9ff6d1e2c2d1\",\n                    \"path\": \"<Keyboard>/4\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"AbilitySlot4\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"0c8228fb-6625-4eef-9aee-ae38a7375c0c\",\n                    \"path\": \"<Keyboard>/2\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"AbilitySlot2\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ced7f234-6d2c-43bf-b0f3-0419a0f08f68\",\n                    \"path\": \"<Keyboard>/e\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"NextWeapon\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7a80a748-7808-4039-ac46-aed7c8a345f5\",\n                    \"path\": \"<Keyboard>/pageDown\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"NextWeapon\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"5ed0248f-e77c-4712-ad52-39018ee48661\",\n                    \"path\": \"<Keyboard>/5\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Emote1\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7a55101a-25eb-48e5-b787-bf5a30215630\",\n                    \"path\": \"<Keyboard>/7\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Emote3\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"5319a5dd-f6ef-4ecc-a34a-756e0f21cc71\",\n                    \"path\": \"<Keyboard>/6\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Emote2\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"36dfdbbd-cb6b-4b10-84e6-77ab65dce8d5\",\n                    \"path\": \"<Keyboard>/9\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Emote5\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a9be1b4b-fe98-446f-9958-dd24369062de\",\n                    \"path\": \"<Keyboard>/8\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Emote4\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e31a02d5-c6a3-4584-9b45-972c4ef3c8a1\",\n                    \"path\": \"<Keyboard>/0\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Emote6\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b1224e92-9d8a-4e9d-9e9b-9f7aefe99902\",\n                    \"path\": \"<Keyboard>/f\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Use\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"37f0cbd5-e280-45c3-ba4d-43fb4a79076f\",\n                    \"path\": \"<Keyboard>/anyKey\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Respawn\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"b251f24e-2628-4940-8c02-ee8948f7ab7b\",\n                    \"path\": \"<Joystick>/trigger\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Respawn\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a5808834-fff7-4eee-89a1-bc058b2f2c1d\",\n                    \"path\": \"<Gamepad>/buttonSouth\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Respawn\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"d4c2b178-5ef5-42bc-a2e3-18a09a9059ef\",\n                    \"path\": \"<Mouse>/press\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Respawn\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a270252d-68b0-4dab-828f-95c688562669\",\n                    \"path\": \"<Keyboard>/b\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Bag\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4f0b2f9b-da04-4d8f-ab63-c9cf875dd1c8\",\n                    \"path\": \"<Keyboard>/i\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Bag\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"006ea766-d5a4-4113-9c21-ac506a512572\",\n                    \"path\": \"<Keyboard>/m\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"WorldMap\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"737f777a-f92e-42d9-9047-e04c1e7a4a97\",\n                    \"path\": \"<Keyboard>/p\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"FriendList\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"2795907e-ae53-4aeb-adf5-5388525d3ec7\",\n                    \"path\": \"<Keyboard>/enter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Chat\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7e001a0e-3318-40b8-81fa-f82f3f1828de\",\n                    \"path\": \"<Keyboard>/numpadEnter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Chat\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"0df8fece-0699-4619-a5cf-b49ada4daf6b\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"Menu\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"1cb4964c-95cf-434e-9ee6-d9593e9fcb63\",\n                    \"path\": \"<Keyboard>/l\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"QuestLog\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ea0a0ab3-54e7-4d25-ba83-7d88f30a4090\",\n                    \"path\": \"<Keyboard>/r\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"KeyboardAndMouse;Keyboard&Mouse\",\n                    \"action\": \"AbilitySlot5\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a0d85d25-d92a-40c4-8c2c-c2464098abc0\",\n                    \"path\": \"<Keyboard>/#(§)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Console\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7e88cbc1-03d2-42e1-9848-cf0a777cd96f\",\n                    \"path\": \"<Keyboard>/backquote\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Console\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"UI\",\n            \"id\": \"272f6d14-89ba-496f-b7ff-215263d3219f\",\n            \"actions\": [\n                {\n                    \"name\": \"Navigate\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"c95b2375-e6d9-4b88-9c4c-c5e76515df4b\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Submit\",\n                    \"type\": \"Button\",\n                    \"id\": \"7607c7b6-cd76-4816-beef-bd0341cfe950\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Cancel\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"15cef263-9014-4fd5-94d9-4e4a6234a6ef\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Point\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"32b35790-4ed0-4e9a-aa41-69ac6d629449\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Click\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"3c7022bf-7922-4f7c-a998-c437916075ad\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"ScrollWheel\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"0489e84a-4833-4c40-bfae-cea84b696689\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"MiddleClick\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"dad70c86-b58c-4b17-88ad-f5e53adf419e\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"RightClick\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"44b200b1-1557-4083-816c-b22cbdf77ddf\",\n                    \"expectedControlType\": \"\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"Gamepad\",\n                    \"id\": \"809f371f-c5e2-4e7a-83a1-d867598f40dd\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"14a5d6e8-4aaf-4119-a9ef-34b8c2c548bf\",\n                    \"path\": \"<Gamepad>/leftStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"9144cbe6-05e1-4687-a6d7-24f99d23dd81\",\n                    \"path\": \"<Gamepad>/rightStick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"2db08d65-c5fb-421b-983f-c71163608d67\",\n                    \"path\": \"<Gamepad>/leftStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"58748904-2ea9-4a80-8579-b500e6a76df8\",\n                    \"path\": \"<Gamepad>/rightStick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"8ba04515-75aa-45de-966d-393d9bbd1c14\",\n                    \"path\": \"<Gamepad>/leftStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"712e721c-bdfb-4b23-a86c-a0d9fcfea921\",\n                    \"path\": \"<Gamepad>/rightStick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"fcd248ae-a788-4676-a12e-f4d81205600b\",\n                    \"path\": \"<Gamepad>/leftStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"1f04d9bc-c50b-41a1-bfcc-afb75475ec20\",\n                    \"path\": \"<Gamepad>/rightStick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"fb8277d4-c5cd-4663-9dc7-ee3f0b506d90\",\n                    \"path\": \"<Gamepad>/dpad\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Gamepad\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"Joystick\",\n                    \"id\": \"e25d9774-381c-4a61-b47c-7b6b299ad9f9\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"3db53b26-6601-41be-9887-63ac74e79d19\",\n                    \"path\": \"<Joystick>/stick/up\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"0cb3e13e-3d90-4178-8ae6-d9c5501d653f\",\n                    \"path\": \"<Joystick>/stick/down\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"0392d399-f6dd-4c82-8062-c1e9c0d34835\",\n                    \"path\": \"<Joystick>/stick/left\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"942a66d9-d42f-43d6-8d70-ecb4ba5363bc\",\n                    \"path\": \"<Joystick>/stick/right\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"Keyboard\",\n                    \"id\": \"ff527021-f211-4c02-933e-5976594c46ed\",\n                    \"path\": \"2DVector\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": true,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"563fbfdd-0f09-408d-aa75-8642c4f08ef0\",\n                    \"path\": \"<Keyboard>/w\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"up\",\n                    \"id\": \"eb480147-c587-4a33-85ed-eb0ab9942c43\",\n                    \"path\": \"<Keyboard>/upArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"2bf42165-60bc-42ca-8072-8c13ab40239b\",\n                    \"path\": \"<Keyboard>/s\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"down\",\n                    \"id\": \"85d264ad-e0a0-4565-b7ff-1a37edde51ac\",\n                    \"path\": \"<Keyboard>/downArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"74214943-c580-44e4-98eb-ad7eebe17902\",\n                    \"path\": \"<Keyboard>/a\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"left\",\n                    \"id\": \"cea9b045-a000-445b-95b8-0c171af70a3b\",\n                    \"path\": \"<Keyboard>/leftArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"8607c725-d935-4808-84b1-8354e29bab63\",\n                    \"path\": \"<Keyboard>/d\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"right\",\n                    \"id\": \"4cda81dc-9edd-4e03-9d7c-a71a14345d0b\",\n                    \"path\": \"<Keyboard>/rightArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Navigate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": true\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"9e92bb26-7e3b-4ec4-b06b-3c8f8e498ddc\",\n                    \"path\": \"<Keyboard>/enter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"51842272-a607-4d38-9e09-891570012f3f\",\n                    \"path\": \"<Keyboard>/numpadEnter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"d9d92768-5774-470c-a8bc-5e640c3fa27d\",\n                    \"path\": \"<Keyboard>/space\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"a4314819-07c1-4b10-9462-4be188f38dd0\",\n                    \"path\": \"<Gamepad>/buttonSouth\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"82627dcc-3b13-4ba9-841d-e4b746d6553e\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Cancel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c25571f0-263d-45ce-b329-d02ec465fe11\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Cancel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c52c8e0b-8179-41d3-b8a1-d149033bbe86\",\n                    \"path\": \"<Mouse>/position\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Point\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e1394cbc-336e-44ce-9ea8-6007ed6193f7\",\n                    \"path\": \"<Pen>/position\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Point\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"5430a219-0e4f-4819-b093-014564ccfcab\",\n                    \"path\": \"<Joystick>/stick\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Joystick\",\n                    \"action\": \"Point\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4faf7dc9-b979-4210-aa8c-e808e1ef89f5\",\n                    \"path\": \"<Mouse>/leftButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"8d66d5ba-88d7-48e6-b1cd-198bbfef7ace\",\n                    \"path\": \"<Pen>/tip\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"38c99815-14ea-4617-8627-164d27641299\",\n                    \"path\": \"<Mouse>/scroll\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"ScrollWheel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"24066f69-da47-44f3-a07e-0015fb02eb2e\",\n                    \"path\": \"<Mouse>/middleButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"MiddleClick\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4c191405-5738-4d4b-a523-c6a301dbf754\",\n                    \"path\": \"<Mouse>/rightButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \";Keyboard&Mouse\",\n                    \"action\": \"RightClick\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Menu\",\n            \"id\": \"afc6c726-e8e6-4d21-84a3-f19827cb9697\",\n            \"actions\": [\n                {\n                    \"name\": \"GoBack\",\n                    \"type\": \"Button\",\n                    \"id\": \"ef30f12a-b923-46ea-aae5-c5e3082eb78a\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"20eb68f1-caeb-44a7-b00d-2e4d5da779dd\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"GoBack\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"eb071b35-7b90-4234-8b1f-b90aea5ff4e0\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"GoBack\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Cutscene\",\n            \"id\": \"a923d08d-68d5-47d3-ad1a-33a46661924c\",\n            \"actions\": [\n                {\n                    \"name\": \"ExitCutscene\",\n                    \"type\": \"Button\",\n                    \"id\": \"38be2292-91bf-432d-acac-3d1e009d7ff7\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"32bf83b8-f73b-49dd-9dc5-8f69078744ef\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"ExitCutscene\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"e2049fd0-55ed-4300-9cb6-3393a50e9d63\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"ExitCutscene\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"c6328a5b-2cda-4279-9923-0c53940fd9cf\",\n                    \"path\": \"<Keyboard>/space\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"ExitCutscene\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Input\",\n            \"id\": \"b36ad29d-96ec-4288-940b-cd51c509e34d\",\n            \"actions\": [\n                {\n                    \"name\": \"Submit\",\n                    \"type\": \"Button\",\n                    \"id\": \"53e8be18-4544-4ff4-b22c-5aa26f8538a7\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Cancel\",\n                    \"type\": \"Button\",\n                    \"id\": \"10ba1a4c-6a0a-47db-906f-0fbf0a28b0b5\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"131074ce-32ad-4548-ba98-142fc4d21e6b\",\n                    \"path\": \"<Keyboard>/enter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"619c4df3-67ff-4e69-8a80-ca63bb1d10be\",\n                    \"path\": \"<Keyboard>/numpadEnter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Submit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"6e04af5e-a4d3-435e-8e97-00bdbd8f3917\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Cancel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7fb56de7-28e0-4bb7-8bec-21b2ae6210db\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Cancel\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Console\",\n            \"id\": \"5ae54ea3-acae-4b71-ab87-3c0e52d2fde4\",\n            \"actions\": [\n                {\n                    \"name\": \"Send\",\n                    \"type\": \"Button\",\n                    \"id\": \"249690de-a539-4009-b143-3569fd3b1af2\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Exit\",\n                    \"type\": \"Button\",\n                    \"id\": \"3e3c4776-9cf2-4baf-a747-c8e781777a93\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"HistoryBackward\",\n                    \"type\": \"Button\",\n                    \"id\": \"deb49695-7954-4ade-a21d-c3fcefd624c2\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"HistoryForward\",\n                    \"type\": \"Button\",\n                    \"id\": \"ed90b50f-732d-442b-8223-f85d4dde697f\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"NextCandidate\",\n                    \"type\": \"Button\",\n                    \"id\": \"38ef05c6-59d4-410c-8e55-76ece6ac8a3e\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"7603e54d-3f0b-4724-8bdc-3ef281e6074c\",\n                    \"path\": \"<Keyboard>/enter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Send\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"2708d53c-51aa-4967-9104-058b55b50857\",\n                    \"path\": \"<Keyboard>/numpadEnter\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"\",\n                    \"action\": \"Send\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"35b53c29-138e-4871-8c65-38847b771980\",\n                    \"path\": \"<Keyboard>/tab\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"NextCandidate\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"7e8b4f65-39c8-43cb-93d5-baab03d65460\",\n                    \"path\": \"<Keyboard>/downArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"HistoryForward\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"568b99ad-3751-467c-9b9d-f1d7fb5ddd7f\",\n                    \"path\": \"<Keyboard>/upArrow\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"HistoryBackward\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"ace71059-9ff6-44fe-962b-b5ba1ea10572\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Exit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"6863a612-b7d7-4292-aa70-c21bd1f1d197\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"Exit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"2e2fb04e-810e-4d1e-8ab7-0ad8eca33541\",\n                    \"path\": \"<Keyboard>/backquote\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Exit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"3a7cb89d-a996-499c-88e8-7e9f73c45386\",\n                    \"path\": \"<Keyboard>/#(§)\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Exit\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Dialogue\",\n            \"id\": \"5010d77d-ecd0-4cfa-86fc-339e3bc20be6\",\n            \"actions\": [\n                {\n                    \"name\": \"CloseDialogue\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"defc9fc1-b473-45e9-9f0c-540321b8c7c3\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"de670f80-99ce-445c-9515-133f6dd8c5bb\",\n                    \"path\": \"<Keyboard>/escape\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"CloseDialogue\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"27e15f3b-a679-45b8-b3cd-4b6d592f6204\",\n                    \"path\": \"<Gamepad>/buttonEast\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Gamepad\",\n                    \"action\": \"CloseDialogue\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"AvatarEditor\",\n            \"id\": \"aad7bca3-a5bd-4b48-ad2c-975932b5853d\",\n            \"actions\": [\n                {\n                    \"name\": \"Click\",\n                    \"type\": \"Button\",\n                    \"id\": \"4ceb5320-c18d-461c-87e3-25f4dbab23b2\",\n                    \"expectedControlType\": \"Button\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": false\n                },\n                {\n                    \"name\": \"Drag\",\n                    \"type\": \"PassThrough\",\n                    \"id\": \"389df9e2-df09-4031-8a31-bca02e028a7a\",\n                    \"expectedControlType\": \"Vector2\",\n                    \"processors\": \"\",\n                    \"interactions\": \"\",\n                    \"initialStateCheck\": true\n                }\n            ],\n            \"bindings\": [\n                {\n                    \"name\": \"\",\n                    \"id\": \"e140ee15-d2d7-406a-bd88-9cb775e31464\",\n                    \"path\": \"<Mouse>/leftButton\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Click\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                },\n                {\n                    \"name\": \"\",\n                    \"id\": \"4679551c-9641-4e7d-a593-6bf673dadbc3\",\n                    \"path\": \"<Mouse>/position\",\n                    \"interactions\": \"\",\n                    \"processors\": \"\",\n                    \"groups\": \"Keyboard&Mouse\",\n                    \"action\": \"Drag\",\n                    \"isComposite\": false,\n                    \"isPartOfComposite\": false\n                }\n            ]\n        }\n    ],\n    \"controlSchemes\": [\n        {\n            \"name\": \"Keyboard&Mouse\",\n            \"bindingGroup\": \"Keyboard&Mouse\",\n            \"devices\": [\n                {\n                    \"devicePath\": \"<Keyboard>\",\n                    \"isOptional\": false,\n                    \"isOR\": false\n                },\n                {\n                    \"devicePath\": \"<Mouse>\",\n                    \"isOptional\": false,\n                    \"isOR\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Gamepad\",\n            \"bindingGroup\": \"Gamepad\",\n            \"devices\": [\n                {\n                    \"devicePath\": \"<Gamepad>\",\n                    \"isOptional\": false,\n                    \"isOR\": false\n                }\n            ]\n        },\n        {\n            \"name\": \"Joystick\",\n            \"bindingGroup\": \"Joystick\",\n            \"devices\": [\n                {\n                    \"devicePath\": \"<Joystick>\",\n                    \"isOptional\": false,\n                    \"isOR\": false\n                }\n            ]\n        }\n    ]\n}");
		m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
		m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
		m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
		m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
		m_Player_Fire = m_Player.FindAction("Fire", throwIfNotFound: true);
		m_Player_PrevWeapon = m_Player.FindAction("PrevWeapon", throwIfNotFound: true);
		m_Player_NextWeapon = m_Player.FindAction("NextWeapon", throwIfNotFound: true);
		m_Player_Skill = m_Player.FindAction("Skill", throwIfNotFound: true);
		m_Player_Use = m_Player.FindAction("Use", throwIfNotFound: true);
		m_Player_AbilitySlot1 = m_Player.FindAction("AbilitySlot1", throwIfNotFound: true);
		m_Player_AbilitySlot2 = m_Player.FindAction("AbilitySlot2", throwIfNotFound: true);
		m_Player_AbilitySlot3 = m_Player.FindAction("AbilitySlot3", throwIfNotFound: true);
		m_Player_AbilitySlot4 = m_Player.FindAction("AbilitySlot4", throwIfNotFound: true);
		m_Player_AbilitySlot5 = m_Player.FindAction("AbilitySlot5", throwIfNotFound: true);
		m_Player_Emote1 = m_Player.FindAction("Emote1", throwIfNotFound: true);
		m_Player_Emote2 = m_Player.FindAction("Emote2", throwIfNotFound: true);
		m_Player_Emote3 = m_Player.FindAction("Emote3", throwIfNotFound: true);
		m_Player_Emote4 = m_Player.FindAction("Emote4", throwIfNotFound: true);
		m_Player_Emote5 = m_Player.FindAction("Emote5", throwIfNotFound: true);
		m_Player_Emote6 = m_Player.FindAction("Emote6", throwIfNotFound: true);
		m_Player_Respawn = m_Player.FindAction("Respawn", throwIfNotFound: true);
		m_Player_Bag = m_Player.FindAction("Bag", throwIfNotFound: true);
		m_Player_WorldMap = m_Player.FindAction("WorldMap", throwIfNotFound: true);
		m_Player_FriendList = m_Player.FindAction("FriendList", throwIfNotFound: true);
		m_Player_Chat = m_Player.FindAction("Chat", throwIfNotFound: true);
		m_Player_Menu = m_Player.FindAction("Menu", throwIfNotFound: true);
		m_Player_QuestLog = m_Player.FindAction("QuestLog", throwIfNotFound: true);
		m_Player_Console = m_Player.FindAction("Console", throwIfNotFound: true);
		m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
		m_UI_Navigate = m_UI.FindAction("Navigate", throwIfNotFound: true);
		m_UI_Submit = m_UI.FindAction("Submit", throwIfNotFound: true);
		m_UI_Cancel = m_UI.FindAction("Cancel", throwIfNotFound: true);
		m_UI_Point = m_UI.FindAction("Point", throwIfNotFound: true);
		m_UI_Click = m_UI.FindAction("Click", throwIfNotFound: true);
		m_UI_ScrollWheel = m_UI.FindAction("ScrollWheel", throwIfNotFound: true);
		m_UI_MiddleClick = m_UI.FindAction("MiddleClick", throwIfNotFound: true);
		m_UI_RightClick = m_UI.FindAction("RightClick", throwIfNotFound: true);
		m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
		m_Menu_GoBack = m_Menu.FindAction("GoBack", throwIfNotFound: true);
		m_Cutscene = asset.FindActionMap("Cutscene", throwIfNotFound: true);
		m_Cutscene_ExitCutscene = m_Cutscene.FindAction("ExitCutscene", throwIfNotFound: true);
		m_Input = asset.FindActionMap("Input", throwIfNotFound: true);
		m_Input_Submit = m_Input.FindAction("Submit", throwIfNotFound: true);
		m_Input_Cancel = m_Input.FindAction("Cancel", throwIfNotFound: true);
		m_Console = asset.FindActionMap("Console", throwIfNotFound: true);
		m_Console_Send = m_Console.FindAction("Send", throwIfNotFound: true);
		m_Console_Exit = m_Console.FindAction("Exit", throwIfNotFound: true);
		m_Console_HistoryBackward = m_Console.FindAction("HistoryBackward", throwIfNotFound: true);
		m_Console_HistoryForward = m_Console.FindAction("HistoryForward", throwIfNotFound: true);
		m_Console_NextCandidate = m_Console.FindAction("NextCandidate", throwIfNotFound: true);
		m_Dialogue = asset.FindActionMap("Dialogue", throwIfNotFound: true);
		m_Dialogue_CloseDialogue = m_Dialogue.FindAction("CloseDialogue", throwIfNotFound: true);
		m_AvatarEditor = asset.FindActionMap("AvatarEditor", throwIfNotFound: true);
		m_AvatarEditor_Click = m_AvatarEditor.FindAction("Click", throwIfNotFound: true);
		m_AvatarEditor_Drag = m_AvatarEditor.FindAction("Drag", throwIfNotFound: true);
	}

	public void Dispose()
	{
		UnityEngine.Object.Destroy(asset);
	}

	public bool Contains(InputAction action)
	{
		return asset.Contains(action);
	}

	public IEnumerator<InputAction> GetEnumerator()
	{
		return asset.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Enable()
	{
		asset.Enable();
	}

	public void Disable()
	{
		asset.Disable();
	}

	public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
	{
		return asset.FindAction(actionNameOrId, throwIfNotFound);
	}

	public int FindBinding(InputBinding bindingMask, out InputAction action)
	{
		return asset.FindBinding(bindingMask, out action);
	}
}
