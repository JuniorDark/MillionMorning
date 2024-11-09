using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.ResourceSystem;
using Code.World.GUI.Ladder;
using Code.World.GUI.PVP;
using Code.World.Player;
using Code.World.Voting;
using UI;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_GlobalUI
{
	public delegate void OpenFunction();

	private class GUIItem
	{
		public readonly OpenFunction CloseFunction;

		public readonly OpenFunction OpenFunction;

		public readonly MilMo_Widget Widget;

		public GUIItem(MilMo_Widget widget, OpenFunction function, OpenFunction closeFunction)
		{
			OpenFunction = function;
			CloseFunction = closeFunction;
			Widget = widget;
		}
	}

	public const string SYSTEM_UI = "SystemUI";

	public const string NAVIGATOR_MENU = "NavigatorMenu";

	public const string MESSGENGER_DIALOG = "MessengerDialog";

	public const string ABOUT_MEMBERSHIP = "AboutMembership";

	public const string PVP_LADDER_WINDOW = "PvpLadderWindow";

	public const string HOME_LADDER_WINDOW = "HomeLadderWindow";

	private static MilMo_GlobalUI _theGlobalUI;

	private readonly Dictionary<string, GUIItem> _guiItems;

	private readonly MilMo_UserInterface _userInterface;

	private readonly Dictionary<string, MilMo_Window> _guiWindows;

	public static MilMo_GlobalUI Instance => Create();

	public static MilMo_UserInterface GetSystemUI => MilMo_UserInterfaceManager.GetUserInterface("SystemUI");

	private static MilMo_GlobalUI Create()
	{
		return _theGlobalUI ?? (_theGlobalUI = new MilMo_GlobalUI());
	}

	private void CreateGlobalGUIItems()
	{
		AddWindow(new MilMoAboutMembershipScreen(), "AboutMembership");
		AddItem(new MilMoPvpLadderWindow(GetSystemUI), "PvpLadderWindow");
		List<string> columns = new List<string> { "Homes_13312", "Homes_13313" };
		List<float> columnSizes = new List<float> { 12f, 12f };
		AddItem(new MilMoLadderWindow(GetSystemUI, MilMo_LocString.Empty, 10, columns, columnSizes, MilMo_VoteManager.VoteTypes.HOMES, new Vector2(530f, 400f)), "HomeLadderWindow");
	}

	private MilMo_GlobalUI()
	{
		_userInterface = MilMo_UserInterfaceManager.GetUserInterface("SystemUI");
		_guiItems = new Dictionary<string, GUIItem>();
		_guiWindows = new Dictionary<string, MilMo_Window>();
		CreateGlobalGUIItems();
	}

	public void AddItem(MilMo_Widget widget, string identifier, OpenFunction functionToOpenWith = null, OpenFunction functionToCloseWith = null)
	{
		GUIItem gUIItem = new GUIItem(widget, functionToOpenWith, functionToCloseWith);
		if (!_guiItems.ContainsKey(identifier))
		{
			_guiItems.Add(identifier, gUIItem);
			_userInterface.AddChild(gUIItem.Widget);
			gUIItem.Widget.UI = _userInterface;
		}
	}

	public void AddWindow(MilMo_Window window, string identifier)
	{
		if (!_guiWindows.ContainsKey(identifier))
		{
			_guiWindows.Add(identifier, window);
			_userInterface.AddChild(window);
			window.UI = _userInterface;
		}
	}

	public MilMo_Widget GetItem(string identifier)
	{
		if (_guiItems.ContainsKey(identifier))
		{
			return _guiItems[identifier].Widget;
		}
		return null;
	}

	public MilMo_Window GetWindow(string identifier)
	{
		if (_guiWindows.ContainsKey(identifier))
		{
			return _guiWindows[identifier];
		}
		return null;
	}

	public void OpenItem(string identifier)
	{
		if (_guiItems.ContainsKey(identifier))
		{
			if (_guiItems[identifier].OpenFunction == null)
			{
				_guiItems[identifier].Widget.Enabled = true;
			}
			else
			{
				_guiItems[identifier].OpenFunction();
			}
		}
		else if (_guiWindows.ContainsKey(identifier))
		{
			_guiWindows[identifier].Open();
		}
	}

	public void CloseItem(string identifier)
	{
		if (_guiItems.ContainsKey(identifier))
		{
			if (_guiItems[identifier].CloseFunction == null)
			{
				_guiItems[identifier].Widget.Enabled = false;
			}
			else
			{
				_guiItems[identifier].CloseFunction();
			}
		}
		else if (_guiWindows.ContainsKey(identifier))
		{
			_guiWindows[identifier].Toggle();
		}
	}

	public void ClosePanels()
	{
		MilMo_Player instance = MilMo_Player.Instance;
		if (instance != null && instance.QuestLog != null)
		{
			MilMo_Player.Instance.QuestLog.Close(null);
		}
		UIManager uIManager = UIManager.Get();
		if (uIManager != null)
		{
			uIManager.HideAllPanels();
		}
	}
}
