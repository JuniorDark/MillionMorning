using System;
using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Config;
using Code.World.Player;

namespace Code.Core.Command;

public class MilMo_Command
{
	private class CommandData
	{
		public readonly CmdCallback Callback;

		public readonly bool IsCheat;

		public CommandData(CmdCallback callback, bool isCheat)
		{
			Callback = callback;
			IsCheat = isCheat;
		}
	}

	public delegate string CmdCallback(string[] arguments);

	public delegate bool UnhandledCommandCallback(string command);

	private static MilMo_Command _theCommander;

	private readonly Dictionary<string, CommandData> _functions = new Dictionary<string, CommandData>(StringComparer.InvariantCultureIgnoreCase);

	private readonly List<UnhandledCommandCallback> _unhandledCommandCallbacks = new List<UnhandledCommandCallback>();

	private readonly bool _allowCheats;

	private bool AllowCheats
	{
		get
		{
			if (!_allowCheats)
			{
				MilMo_Player instance = MilMo_Player.Instance;
				if (instance != null)
				{
					MilMo_Avatar avatar = instance.Avatar;
					if (avatar != null)
					{
						return avatar.Role > 3;
					}
				}
				return false;
			}
			return true;
		}
	}

	public static MilMo_Command Instance
	{
		get
		{
			if (_theCommander == null)
			{
				Create();
			}
			return _theCommander;
		}
	}

	private MilMo_Command()
	{
		_allowCheats = MilMo_Config.Instance.IsTrue("Debug.Cheat", defaultValue: false);
	}

	private static void Create()
	{
		if (_theCommander == null)
		{
			_theCommander = new MilMo_Command();
		}
	}

	public void RegisterUnhandledCommandCallback(UnhandledCommandCallback callback)
	{
		_unhandledCommandCallbacks.Add(callback);
	}

	public string HandleCommand(string command)
	{
		command = command.Trim();
		string[] array = command.Split((char[])null);
		if (array.Length < 1)
		{
			return "No command specified";
		}
		if (_functions.TryGetValue(array[0], out var value))
		{
			if (!AllowCheats && value.IsCheat)
			{
				return "No such command";
			}
			return value.Callback(array);
		}
		bool flag = false;
		foreach (UnhandledCommandCallback unhandledCommandCallback in _unhandledCommandCallbacks)
		{
			flag |= unhandledCommandCallback(command);
		}
		if (!flag)
		{
			return "No such command";
		}
		return "";
	}

	public void RegisterCommand(string command, CmdCallback callback, bool isCheat = true)
	{
		command = command.Trim();
		if (_functions.TryGetValue(command, out var _))
		{
			_functions[command] = new CommandData(callback, isCheat);
		}
		else
		{
			_functions.Add(command, new CommandData(callback, isCheat));
		}
	}

	public List<string> GetCandidates(string partOfCommand)
	{
		List<string> list = new List<string>();
		foreach (var (text2, commandData2) in _functions)
		{
			if ((AllowCheats || !commandData2.IsCheat) && text2.StartsWith(partOfCommand, StringComparison.CurrentCultureIgnoreCase))
			{
				list.Add(text2);
			}
		}
		return list;
	}
}
