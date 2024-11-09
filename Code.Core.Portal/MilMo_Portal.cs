using System;
using Code.Core.Command;
using Code.Core.Config;
using Code.Core.Portal.Steam;
using Code.Core.Portal.Web;
using UnityEngine;

namespace Code.Core.Portal;

public abstract class MilMo_Portal
{
	private string Identifier { get; set; }

	public static MilMo_Portal Instance { get; private set; }

	public bool AllowZoomOutsideFullscreen => true;

	public static string DefaultControlScheme => MilMo_Config.Instance.GetValue("Controls", "MMORPG");

	protected MilMo_Portal()
	{
		Identifier = "unknown";
		if (this is MilMo_WebPortal)
		{
			Identifier = "jb_web";
		}
		else if (this is MilMo_SteamPortal)
		{
			Identifier = "jb_steam";
		}
		if (Identifier == "unknown")
		{
			throw new Exception("Can't use unknown portal identifier.");
		}
		MilMo_Command.Instance.RegisterCommand("Portal.Type", (string[] _003Cp0_003E) => GetType().FullName);
		MilMo_Command.Instance.RegisterCommand("Portal.Identifier", (string[] _003Cp0_003E) => Identifier);
	}

	public static void Initialize()
	{
		if (Instance != null)
		{
			throw new InvalidOperationException("Platform can only be initialized once.");
		}
		if (Application.isEditor)
		{
			Instance = new MilMo_WebPortal();
		}
		else
		{
			Instance = new MilMo_SteamPortal();
		}
		if (Instance == null)
		{
			throw new ArgumentException("Could not find a valid portal.");
		}
	}

	public abstract void ShowInviteInterface();
}
