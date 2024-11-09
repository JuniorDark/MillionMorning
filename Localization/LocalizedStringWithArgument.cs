using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Localization;

[Serializable]
public class LocalizedStringWithArgument
{
	private MilMo_LocString _locString;

	[SerializeField]
	private string message;

	public LocalizedStringWithArgument(string identifier, params object[] arg)
	{
		_locString = MilMo_Localization.GetLocString(identifier);
		if (arg != null)
		{
			_locString.SetFormatArgs(arg);
		}
		message = _locString.String;
	}

	public void SetFormatArgs(params object[] arg)
	{
		if (!(_locString == null))
		{
			if (arg != null)
			{
				_locString.SetFormatArgs(arg);
			}
			message = _locString.String;
		}
	}

	public string GetMessage()
	{
		return message;
	}

	public string GetIdentifier()
	{
		return _locString?.Identifier;
	}

	public List<MilMo_LocString.Tag> GetTags()
	{
		return _locString?.Tags;
	}
}
