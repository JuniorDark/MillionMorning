using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Code.Core.Config;
using UnityEngine;

namespace Code.Core.ResourceSystem;

public static class MilMo_BadWordFilter
{
	public enum StringIntegrity
	{
		Bad,
		IRL,
		IRLContactAttempt,
		Cheating,
		Empty,
		OK
	}

	private static readonly bool DevMode;

	private static readonly Dictionary<string, StringIntegrity> HarmfulWords;

	private static readonly char[] Delimiters;

	private static MilMo_SFFile _badWordFile;

	private static MilMo_SFFile _irlWordFile;

	private static MilMo_SFFile _cheatWordFile;

	private static int _shortestWord;

	private static int _longestWord;

	public static string DetectedBadWord { get; private set; }

	private static string DebugString { get; set; }

	public static bool IsReady
	{
		get
		{
			if (_badWordFile != null && _irlWordFile != null)
			{
				return _cheatWordFile != null;
			}
			return false;
		}
	}

	static MilMo_BadWordFilter()
	{
		DevMode = MilMo_Config.Instance.IsTrue("Debug.BadWordFilter", defaultValue: false);
		HarmfulWords = new Dictionary<string, StringIntegrity>(StringComparer.CurrentCultureIgnoreCase);
		Delimiters = new char[23]
		{
			' ', '\'', '!', '"', '#', '%', '*', '+', ',', '-',
			'.', ';', ':', '=', '^', '_', '`', '|', '~', '\ufffd',
			'\ufffd', '\ufffd', '\ufffd'
		};
		DebugString = "";
		DetectedBadWord = "no word file";
	}

	public static void AsyncInit()
	{
		DebugString = MilMo_Config.Instance.GetValue("BadWordFilter.DebugString").ToLower();
		MilMo_Localization.RegisterLanguageChangedCallback(OnLanguageChanged);
		AsyncLoadWordFiles();
	}

	public static async Task Init()
	{
		DebugString = MilMo_Config.Instance.GetValue("BadWordFilter.DebugString").ToLower();
		MilMo_Localization.RegisterLanguageChangedCallback(OnLanguageChanged);
		await AsyncLoadWordFiles();
	}

	private static async void OnLanguageChanged()
	{
		await AsyncLoadWordFiles();
	}

	private static async Task AsyncLoadWordFiles()
	{
		_badWordFile = null;
		_irlWordFile = null;
		_cheatWordFile = null;
		if (MilMo_Localization.CurrentLanguage == null)
		{
			throw new NullReferenceException("Current language is null when loading bad word files");
		}
		string currentLanguage = MilMo_Localization.CurrentLanguage.EnglishName;
		MilMo_SFFile milMo_SFFile = await MilMo_SimpleFormat.RealAsyncLoad("Content/BadWordFilter/BadWordList_" + currentLanguage);
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("MilMo_BadWordFilter.Init: Could not load bad word file");
			return;
		}
		_badWordFile = milMo_SFFile;
		MilMo_SFFile milMo_SFFile2 = await MilMo_SimpleFormat.RealAsyncLoad("Content/BadWordFilter/IRLWordList_" + currentLanguage);
		if (milMo_SFFile2 == null)
		{
			Debug.LogWarning("MilMo_BadWordFilter.Init: Could not load IRL word file");
			return;
		}
		_irlWordFile = milMo_SFFile2;
		MilMo_SFFile milMo_SFFile3 = await MilMo_SimpleFormat.RealAsyncLoad("Content/BadWordFilter/CheatWordList_" + currentLanguage);
		if (milMo_SFFile3 == null)
		{
			Debug.LogWarning("MilMo_BadWordFilter.Init: Could not load cheat word file");
			return;
		}
		_cheatWordFile = milMo_SFFile3;
		TryInitInternal();
	}

	private static void AddHarmfulWords(MilMo_SFFile wordFile, StringIntegrity stringIntegrity)
	{
		while (wordFile.NextRow())
		{
			string @string = wordFile.GetString();
			if (!HarmfulWords.ContainsKey(@string))
			{
				HarmfulWords.Add(@string, stringIntegrity);
			}
		}
	}

	private static void TryInitInternal()
	{
		if (!IsReady)
		{
			return;
		}
		HarmfulWords.Clear();
		AddHarmfulWords(_badWordFile, StringIntegrity.Bad);
		AddHarmfulWords(_irlWordFile, StringIntegrity.IRL);
		AddHarmfulWords(_cheatWordFile, StringIntegrity.Cheating);
		_shortestWord = 1000;
		_longestWord = 0;
		foreach (int item in HarmfulWords.Keys.Select((string w) => w.Length))
		{
			if (item < _shortestWord)
			{
				_shortestWord = item;
			}
			else if (item > _longestWord)
			{
				_longestWord = item;
			}
		}
		_longestWord++;
		if (!string.IsNullOrEmpty(DebugString))
		{
			DebugOutput();
		}
		if (DevMode)
		{
			Debug.Log("MilMo_BadWordFilter: Bad wordfilter loaded with " + GetWordsWithIntegrity(StringIntegrity.Bad).Count + " bad words, " + GetWordsWithIntegrity(StringIntegrity.IRL).Count + " IRL words and " + GetWordsWithIntegrity(StringIntegrity.Cheating).Count + " cheat words.");
			DebugHarmfulWords();
		}
	}

	private static List<string> GetWordsWithIntegrity(StringIntegrity stringIntegrity)
	{
		return (from pair in HarmfulWords
			where pair.Value == stringIntegrity
			select pair.Key).ToList();
	}

	public static StringIntegrity GetStringIntegrity(string str)
	{
		if (!IsReady)
		{
			Debug.LogWarning("Trying to use bad word filter before ready. String is " + str);
			return StringIntegrity.OK;
		}
		List<string> source = PotentialWordsList(str);
		if (string.IsNullOrEmpty(str.Trim()))
		{
			DetectedBadWord = "empty";
			return StringIntegrity.Empty;
		}
		if (str.Contains("@"))
		{
			DetectedBadWord = "email address";
			return StringIntegrity.IRLContactAttempt;
		}
		if (IsPhoneNumber(str))
		{
			DetectedBadWord = "long number";
			return StringIntegrity.IRLContactAttempt;
		}
		using (IEnumerator<string> enumerator = source.Where((string w) => HarmfulWords.ContainsKey(w)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				string key = (DetectedBadWord = enumerator.Current);
				return HarmfulWords[key];
			}
		}
		return StringIntegrity.OK;
	}

	private static bool IsPhoneNumber(string str)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		str += " ";
		string text = str;
		foreach (char c in text)
		{
			if (c > '/' && c < ':')
			{
				num++;
			}
			else
			{
				if (num > num3)
				{
					num3 = num;
				}
				num = 0;
			}
			if (c == '0')
			{
				num2++;
				continue;
			}
			if (num2 > num4)
			{
				num4 = num2;
			}
			num2 = 0;
		}
		if (num3 > 6 && num4 < 4)
		{
			return true;
		}
		return false;
	}

	private static List<string> PotentialWordsList(string str)
	{
		string text = str;
		str = Delimiters.Aggregate(str, (string current, char c) => current.Replace(c.ToString(), ""));
		str = str + " " + text + " ";
		if (!string.IsNullOrEmpty(DebugString))
		{
			Debug.Log("Debug string: " + str);
		}
		List<string> list = new List<string>();
		for (int i = _shortestWord; i < _longestWord; i++)
		{
			for (int j = 0; j < str.Length; j++)
			{
				if (j + i <= str.Length)
				{
					string item = str.Substring(j, i);
					list.Add(item);
				}
			}
		}
		if (!string.IsNullOrEmpty(DebugString))
		{
			string text2 = list.Aggregate("", (string current, string fw) => current + " " + fw);
			Debug.Log(list.Count + " potential words: " + text2);
		}
		return list;
	}

	public static string CensorMessage(string str)
	{
		return (from word in PotentialWordsList(str)
			where HarmfulWords.ContainsKey(word)
			select word).Aggregate(str, CensorOutBadWord);
	}

	public static string CensorDigits(string str)
	{
		return Regex.Replace(str, "[0-9]{2,}", new string('*', str.Length));
	}

	private static string CensorOutBadWord(string original, string badWord)
	{
		badWord = badWord.Trim();
		string[] array = original.Split();
		if (array.Contains(badWord))
		{
			int num = Array.IndexOf(array, badWord);
			string text = new string('*', badWord.Length);
			array[num] = text;
		}
		else
		{
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (text2.Contains(badWord))
				{
					int num2 = Array.IndexOf(array, text2);
					string text = new string('*', text2.Length);
					array[num2] = text;
				}
			}
		}
		return string.Join(" ", array);
	}

	private static void DebugOutput()
	{
		int num = GetWordsWithIntegrity(StringIntegrity.Bad).Count + GetWordsWithIntegrity(StringIntegrity.Cheating).Count + GetWordsWithIntegrity(StringIntegrity.IRL).Count;
		Debug.Log("--- BadWordFilter @ " + num + " words (shortest: " + _shortestWord + ", longest: " + _longestWord + ")");
		switch (GetStringIntegrity(DebugString))
		{
		case StringIntegrity.OK:
			Debug.Log("String is clean.");
			break;
		case StringIntegrity.Bad:
			Debug.Log("Detected BAD word: |" + DetectedBadWord + "|.");
			break;
		case StringIntegrity.Cheating:
			Debug.Log("Detected CHEATING word: |" + DetectedBadWord + "|.");
			break;
		case StringIntegrity.IRL:
			Debug.Log("Detected IRL word: |" + DetectedBadWord + "|.");
			break;
		case StringIntegrity.IRLContactAttempt:
			Debug.Log("Detected CONTACT ATTEMPT: |" + DetectedBadWord + "|.");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case StringIntegrity.Empty:
			break;
		}
	}

	private static void DebugHarmfulWords()
	{
		foreach (string key in HarmfulWords.Keys)
		{
			Debug.Log("Word: |" + key + "| StringIntegrity: |" + HarmfulWords[key].ToString() + "|.");
		}
	}
}
