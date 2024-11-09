using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.Config;
using Core.Settings;
using UnityEngine;

namespace Code.Core.ResourceSystem;

public static class MilMo_Localization
{
	public delegate void LanguageChanged();

	public class LanguageInfo
	{
		public bool IsEnglish => EnglishName.Equals("English");

		public bool IsPortugueseBrazilian => EnglishName.Equals("PortugueseBrazilian");

		public string NativeName { get; private set; }

		public string EnglishName { get; private set; }

		public string LanguageCode { get; private set; }

		public LanguageInfo(string nativeName, string englishName, string languageCode)
		{
			NativeName = nativeName;
			EnglishName = englishName;
			LanguageCode = languageCode;
		}
	}

	private static readonly bool DevMode = MilMo_Config.Instance.IsTrue("Debug.Localization", defaultValue: false);

	private static readonly bool ForceIdentifier = MilMo_Config.Instance.IsTrue("Debug.Localization.ForceIdentifier", defaultValue: false);

	private const string LANGUAGE_CONTENT_PATH = "Content/Localization/";

	private static readonly Dictionary<string, MilMo_LocString> LocalizedStrings = new Dictionary<string, MilMo_LocString>();

	private static readonly Dictionary<string, MilMo_LocString> DefaultStrings = new Dictionary<string, MilMo_LocString>();

	private static readonly List<LanguageChanged> LanguageChangedCallbacks = new List<LanguageChanged>();

	private static readonly LanguageInfo[] Languages = new LanguageInfo[9]
	{
		new LanguageInfo("English", "English", "en"),
		new LanguageInfo("Português (Brasil)", "PortugueseBrazilian", "pt-BR"),
		new LanguageInfo("Español", "Spanish", "es-ES"),
		new LanguageInfo("Deutsch", "German", "de"),
		new LanguageInfo("Түркмен", "Turkish", "tr"),
		new LanguageInfo("Français", "French", "fr"),
		new LanguageInfo("Svenska", "Swedish", "sv"),
		new LanguageInfo("Pусский язык", "Russian", "ru"),
		new LanguageInfo("Polski", "Polish", "pl")
	};

	public static LanguageInfo CurrentLanguage { get; private set; }

	public static async Task AsyncInitializeSystemLanguage()
	{
		string language = "English";
		await AsyncInitializeDefaultLanguage(Languages[0]);
		LanguageInfo[] languages = Languages;
		foreach (LanguageInfo languageInfo in languages)
		{
			if (languageInfo.LanguageCode.ToLower().Equals(Settings.Language.Identifier.Code.ToLower()))
			{
				language = languageInfo.EnglishName;
			}
		}
		if (DevMode)
		{
			Debug.Log("MilMo_Localization: Localization requested: " + language);
		}
		languages = Languages;
		foreach (LanguageInfo languageInfo2 in languages)
		{
			if (languageInfo2.EnglishName.Equals(language, StringComparison.InvariantCultureIgnoreCase))
			{
				await AsyncInitializeLanguage(languageInfo2);
				return;
			}
		}
		Debug.LogWarning("Failed to load language " + language + ". Language is not supported. Using english as fallback.");
		await AsyncInitializeLanguage(Languages[0]);
	}

	private static async Task AsyncInitializeDefaultLanguage(LanguageInfo languageInfo)
	{
		MilMo_SFFile milMo_SFFile = await MilMo_SimpleFormat.RealAsyncLoad("Content/Localization/" + languageInfo.EnglishName);
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load default localization assets for '" + languageInfo.NativeName + "'.");
			return;
		}
		try
		{
			string text = "";
			while (milMo_SFFile.NextRow())
			{
				while (milMo_SFFile.HasMoreTokens())
				{
					string @string = milMo_SFFile.GetString();
					if (@string.Length < 2 || !@string.Contains(":"))
					{
						continue;
					}
					string text2 = @string.Substring(0, @string.IndexOf(':'));
					string text3 = @string.Substring(@string.IndexOf(':') + 1);
					if (!(text2 == "source") && !(text3 == "target"))
					{
						if (!text2.Contains("_") && !text2.Contains("Emotes."))
						{
							Debug.LogWarning("Detected possible non-escaped quotation marks in localization file for " + languageInfo.EnglishName + ": Got identifier " + text2 + " and string " + text3 + ". Last valid identifier is " + text + ".");
						}
						else
						{
							text = text2;
						}
						text3 = RemoveBrackets(text3);
						if (DefaultStrings.TryGetValue(text2, out var value))
						{
							value.UpdateString(text3);
						}
						else
						{
							DefaultStrings.Add(text2, new MilMo_LocString(text3, text2));
						}
					}
				}
			}
			if (DevMode)
			{
				Debug.Log("MilMo_Localization: Default Localization strings ready (" + languageInfo.EnglishName + ": " + DefaultStrings.Count + ")");
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to parse default localization file 'Content/Localization/" + languageInfo.EnglishName + "' at line " + milMo_SFFile.GetLineNumber() + "\r\nexception:\r\n" + ex.Message + "\r\nCallStack:\r\n" + ex.StackTrace);
			return;
		}
		CurrentLanguage = languageInfo;
		GC.Collect();
		Resources.UnloadUnusedAssets();
		foreach (LanguageChanged languageChangedCallback in LanguageChangedCallbacks)
		{
			languageChangedCallback();
		}
	}

	private static async Task AsyncInitializeLanguage(LanguageInfo languageInfo)
	{
		MilMo_SFFile milMo_SFFile = await MilMo_SimpleFormat.RealAsyncLoad("Content/Localization/" + languageInfo.EnglishName);
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load localization assets for '" + languageInfo.NativeName + "'.");
			return;
		}
		try
		{
			string text = "";
			while (milMo_SFFile.NextRow())
			{
				while (milMo_SFFile.HasMoreTokens())
				{
					string @string = milMo_SFFile.GetString();
					if (@string.Length < 2 || !@string.Contains(":"))
					{
						continue;
					}
					string text2 = @string.Substring(0, @string.IndexOf(':'));
					string text3 = @string.Substring(@string.IndexOf(':') + 1);
					if (!(text2 == "source") && !(text3 == "target"))
					{
						if (!text2.Contains("_") && !text2.Contains("Emotes."))
						{
							Debug.LogWarning("Detected possible non-escaped quotation marks in localization file for " + languageInfo.EnglishName + ": Got identifier " + text2 + " and string " + text3 + ". Last valid identifier is " + text + ".");
						}
						else
						{
							text = text2;
						}
						text3 = RemoveBrackets(text3);
						if (LocalizedStrings.TryGetValue(text2, out var value))
						{
							value.UpdateString(text3);
						}
						else
						{
							LocalizedStrings.Add(text2, new MilMo_LocString(text3, text2));
						}
					}
				}
			}
			if (DevMode)
			{
				Debug.Log("MilMo_Localization: Localization strings ready (" + languageInfo.EnglishName + ": " + LocalizedStrings.Count + ")");
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to parse localization file 'Content/Localization/" + languageInfo.EnglishName + "' at line " + milMo_SFFile.GetLineNumber() + "\r\nexception:\r\n" + ex.Message + "\r\nCallStack:\r\n" + ex.StackTrace);
			return;
		}
		CurrentLanguage = languageInfo;
		GC.Collect();
		Resources.UnloadUnusedAssets();
		foreach (LanguageChanged languageChangedCallback in LanguageChangedCallbacks)
		{
			languageChangedCallback();
		}
	}

	public static void RegisterLanguageChangedCallback(LanguageChanged callback)
	{
		LanguageChangedCallbacks.Add(callback);
	}

	private static string RemoveBrackets(string locString)
	{
		locString = locString.Replace("[", "");
		locString = locString.Replace("]", "");
		return locString;
	}

	public static MilMo_LocString GetLocString(string identifier)
	{
		if (string.IsNullOrEmpty(identifier))
		{
			return MilMo_LocString.Empty;
		}
		if (ForceIdentifier)
		{
			return new MilMo_LocString(RemoveBrackets(identifier), removeTags: true);
		}
		if (LocalizedStrings.TryGetValue(identifier, out var value) && value.String != "")
		{
			return value;
		}
		if (DefaultStrings.TryGetValue(identifier, out value))
		{
			return value;
		}
		if (DevMode)
		{
			Debug.LogWarning("MilMo_Localization: Asking for localized string '" + identifier + "' that does not exist in the localization database.");
		}
		return new MilMo_LocString(RemoveBrackets(identifier), removeTags: true);
	}

	public static bool TryGetLocString(string identifier, out MilMo_LocString locString)
	{
		if (!string.IsNullOrEmpty(identifier))
		{
			return LocalizedStrings.TryGetValue(identifier, out locString);
		}
		locString = null;
		return false;
	}

	public static MilMo_LocString GetNotLocalizedLocString(string text)
	{
		return new MilMo_LocString(text, removeTags: false);
	}

	public static string GetLocTexturePath(string path)
	{
		if (CurrentLanguage.IsEnglish)
		{
			return path;
		}
		return path + "_" + CurrentLanguage.EnglishName;
	}

	public static async Task ChangeLanguage(string languageCode)
	{
		LanguageInfo[] languages = Languages;
		foreach (LanguageInfo lanInfo in languages)
		{
			if (lanInfo.LanguageCode.ToLower().Equals(languageCode.ToLower()))
			{
				await AsyncInitializeLanguage(lanInfo);
				Debug.Log("Changing language to '" + lanInfo.NativeName + "'");
				return;
			}
		}
		Debug.Log("No such language '" + languageCode + "'");
	}
}
