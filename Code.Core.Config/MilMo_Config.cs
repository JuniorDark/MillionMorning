using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Config;

public class MilMo_Config
{
	private static MilMo_Config _theConfig;

	private readonly Dictionary<string, string> _configVariables = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

	public static MilMo_Config Instance
	{
		get
		{
			if (_theConfig != null)
			{
				return _theConfig;
			}
			_theConfig = new MilMo_Config();
			_theConfig.Load();
			return _theConfig;
		}
	}

	public static bool Initialize()
	{
		if (_theConfig != null)
		{
			return true;
		}
		_theConfig = new MilMo_Config();
		return _theConfig.Load();
	}

	private bool Load()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Config");
		if (textAsset == null)
		{
			Debug.LogWarning("Unable to load Config resource!");
			return false;
		}
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadFromString(textAsset.text);
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Unable to open Config for parsing!");
			return false;
		}
		while (milMo_SFFile.NextRow())
		{
			string @string = milMo_SFFile.GetString();
			string value = @string;
			if (milMo_SFFile.HasMoreTokens())
			{
				value = milMo_SFFile.GetString();
			}
			if (_configVariables.ContainsKey(@string))
			{
				_configVariables[@string] = value;
			}
			else
			{
				_configVariables.Add(@string, value);
			}
		}
		return true;
	}

	public bool IsSet(string var)
	{
		return _configVariables.ContainsKey(var);
	}

	public bool IsTrue(string var, bool defaultValue)
	{
		if (!IsSet(var))
		{
			return defaultValue;
		}
		return IsTrue(var);
	}

	private bool IsTrue(string var)
	{
		if (_configVariables.TryGetValue(var, out var value))
		{
			if (!value.Equals("0", StringComparison.InvariantCultureIgnoreCase))
			{
				return !value.Equals("False", StringComparison.InvariantCultureIgnoreCase);
			}
			return false;
		}
		return false;
	}

	public string GetValue(string var, string defaultValue = "")
	{
		if (!_configVariables.TryGetValue(var, out var value))
		{
			return defaultValue;
		}
		return value;
	}

	public int GetIntValue(string var, int defaultValue)
	{
		if (!_configVariables.TryGetValue(var, out var value))
		{
			return defaultValue;
		}
		return MilMo_Utility.StringToInt(value);
	}

	public float GetFloatValue(string var, float defaultValue)
	{
		if (!_configVariables.TryGetValue(var, out var value))
		{
			return defaultValue;
		}
		return MilMo_Utility.StringToFloat(value);
	}
}
