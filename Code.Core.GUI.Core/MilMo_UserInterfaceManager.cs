using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Command;
using Code.Core.GUI.Widget.QuickInfoDialogs;
using Code.Core.Input;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.GUI.Core;

public sealed class MilMo_UserInterfaceManager : MonoBehaviour
{
	private static MilMo_UserInterfaceManager _theManager;

	private static readonly List<MilMo_UserInterface> UserInterfaces = new List<MilMo_UserInterface>();

	public const int DEFAULT_DEPTH = 0;

	private static int _windowID;

	private static MilMo_UserInterface _systemUI;

	private static MilMo_UserInterface _pointerUI;

	public static MilMo_Widget MouseFocus { get; set; }

	public static MilMo_Widget FinalMouseFocus { get; private set; }

	public static void Initialize(GameObject gameObject)
	{
		if (!(_theManager != null))
		{
			_theManager = gameObject.AddComponent<MilMo_UserInterfaceManager>();
			MilMo_Command.Instance.RegisterCommand("GUI.PrintUserInterfaces", Debug_PrintUserInterfaces);
			MilMo_Command.Instance.RegisterCommand("GUI.GlobalUIFade", Debug_GlobalUIFade);
			MilMo_Command.Instance.RegisterCommand("GUI.DrawIdentifierMode", MilMo_Widget.Debug_DrawIdentifierMode);
		}
	}

	public static int NextWindowID()
	{
		_windowID++;
		return _windowID - 1;
	}

	public static MilMo_UserInterface CreateUserInterface(string identifier)
	{
		MilMo_UserInterface milMo_UserInterface = new MilMo_UserInterface(identifier);
		SetUserInterfaceDepth(milMo_UserInterface, 0);
		return milMo_UserInterface;
	}

	public static MilMo_UserInterface GetOrCreateUserInterface(string identifier)
	{
		using (IEnumerator<MilMo_UserInterface> enumerator = UserInterfaces.Where((MilMo_UserInterface ui) => ui.Identifier.Equals(identifier)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return CreateUserInterface(identifier);
	}

	public static MilMo_UserInterface GetUserInterface(string identifier)
	{
		if (identifier == "SystemUI" && _systemUI == null)
		{
			_systemUI = CreateUserInterface("SystemUI");
			SetUserInterfaceDepth(_systemUI, -3000);
			_systemUI.ResetLayout();
			_systemUI.ScreenSizeDirty = true;
		}
		return UserInterfaces.Find((MilMo_UserInterface ui) => ui.Identifier.Equals(identifier));
	}

	public static void DestroyUserInterface(MilMo_UserInterface userInterface)
	{
		userInterface.Enabled = false;
		userInterface.Cleanup();
		UserInterfaces.Remove(userInterface);
	}

	public static void SetUserInterfaceDepth(MilMo_UserInterface userInterface, int depth)
	{
		if (UserInterfaces.Contains(userInterface))
		{
			UserInterfaces.Remove(userInterface);
		}
		userInterface.Depth = depth;
		for (int i = 0; i < UserInterfaces.Count; i++)
		{
			if (depth < UserInterfaces[i].Depth)
			{
				UserInterfaces.Insert(i, userInterface);
				return;
			}
		}
		UserInterfaces.Add(userInterface);
	}

	private void FixedUpdate()
	{
		for (int num = UserInterfaces.Count - 1; num >= 0; num--)
		{
			try
			{
				MilMo_UserInterface milMo_UserInterface = UserInterfaces[num];
				if (milMo_UserInterface.Enabled)
				{
					milMo_UserInterface.FixedUpdate();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				throw;
			}
		}
	}

	private void OnGUI()
	{
		if (!MilMo_Pointer.LeftButton)
		{
			FinalMouseFocus = MouseFocus;
			MouseFocus = null;
		}
		for (int num = UserInterfaces.Count - 1; num >= 0; num--)
		{
			try
			{
				MilMo_UserInterface milMo_UserInterface = UserInterfaces[num];
				if (milMo_UserInterface.Enabled)
				{
					milMo_UserInterface.OnGUI();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				throw;
			}
		}
		if (MouseFocus == null)
		{
			if (MilMo_Pointer.LeftClick)
			{
				MilMo_UserInterface.KeyboardFocus = false;
			}
			if (MilMo_Pointer.LeftButton)
			{
				MilMo_UserInterface.KeyboardFocus = false;
			}
		}
	}

	private static string Debug_PrintUserInterfaces(string[] args)
	{
		string text = "---------------------------------\n";
		foreach (MilMo_UserInterface userInterface in UserInterfaces)
		{
			text = text + userInterface.Identifier + "@" + userInterface.Depth + " with " + userInterface.Children.Count + " widgets " + (userInterface.Enabled ? "(enabled)" : "(disabled)") + "\n";
		}
		return text;
	}

	private static string Debug_GlobalUIFade(string[] args)
	{
		if (args.Length > 1)
		{
			MilMo_GUI.GlobalFade = MilMo_Utility.StringToFloat(args[1]);
		}
		return MilMo_GUI.GlobalFade.ToString() ?? "";
	}

	public void Update()
	{
		if (MilMo_QuickInfoDialogHandler.IsCreated)
		{
			MilMo_QuickInfoDialogHandler.GetInstance().Update();
		}
	}
}
