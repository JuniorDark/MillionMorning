using System.Collections.Generic;
using System.Linq;
using Code.Core.ResourceSystem;
using UnityEngine.Localization;

namespace Core.Utilities;

public static class LocalizationHelper
{
	public static string GetLocaleString(string identifier, Dictionary<string, string> locales)
	{
		string value;
		bool num = locales.TryGetValue(identifier, out value);
		string @string = MilMo_Localization.GetLocString(value).String;
		if (!num)
		{
			return "Unknown";
		}
		return @string;
	}

	public static string GetLocaleString(string identifier)
	{
		MilMo_LocString locString = MilMo_Localization.GetLocString(identifier);
		if (!(locString == MilMo_LocString.Empty))
		{
			return locString.String;
		}
		return "Unknown";
	}

	public static string GetLocalizedString(string table, string key)
	{
		return new LocalizedString(table, key).GetLocalizedString();
	}

	public static string GetLocalizedString(string table, string key, IEnumerable<string> args)
	{
		List<object> arguments = args.Cast<object>().ToList();
		return new LocalizedString(table, key)
		{
			Arguments = arguments
		}.GetLocalizedString();
	}
}
