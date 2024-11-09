using UnityEngine.Localization.Settings;

namespace Localization;

public static class LocalizationUtility
{
	public static string GetLocalizedString(string category, string keyword)
	{
		return LocalizationSettings.StringDatabase.GetLocalizedString(category, keyword, null, FallbackBehavior.UseProjectSettings);
	}
}
