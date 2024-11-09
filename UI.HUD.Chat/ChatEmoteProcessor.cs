using System;
using System.Collections.Generic;
using Code.Core.Emote;
using Code.Core.ResourceSystem;
using Code.World.Player;
using Core.Settings;

namespace UI.HUD.Chat;

public class ChatEmoteProcessor
{
	private Dictionary<string, string> _mWordEmotes;

	private Dictionary<string, string> _mSmileyEmotes;

	public ChatEmoteProcessor()
	{
		_mSmileyEmotes = new Dictionary<string, string>();
		_mWordEmotes = new Dictionary<string, string>();
		MilMo_Localization.RegisterLanguageChangedCallback(LoadEmoteFiles);
		LoadEmoteFiles();
	}

	public void CheckEmotes(string message)
	{
		if (!Settings.PlayEmotesOnChat)
		{
			return;
		}
		string text = CheckTextForEmotes(message);
		if (text != "")
		{
			string text2 = "";
			if (_mWordEmotes.ContainsKey(text))
			{
				text2 = _mWordEmotes[text];
			}
			else if (_mSmileyEmotes.ContainsKey(text))
			{
				text2 = _mSmileyEmotes[text];
			}
			if (text2 != "")
			{
				MilMo_Emote emoteByName = MilMo_EmoteSystem.GetEmoteByName(text2);
				MilMo_Player.Instance.Avatar.PlayEmote(emoteByName);
			}
		}
	}

	private void ReadTextEmotes(MilMo_SFFile file)
	{
		while (file.NextRow())
		{
			string @string = file.GetString();
			if (@string == "</TEXT>")
			{
				break;
			}
			if (MilMo_Localization.GetLocString(@string).String.Length <= 0)
			{
				continue;
			}
			string[] array = MilMo_Localization.GetLocString(@string).String.Split('#');
			foreach (string text in array)
			{
				if (!(text == "") && !_mWordEmotes.ContainsKey(text.ToUpper()))
				{
					_mWordEmotes.Add(text.ToUpper(), @string);
				}
			}
		}
	}

	private void ReadSmileyEmotes(MilMo_SFFile file)
	{
		while (file.NextRow())
		{
			string @string = file.GetString();
			if (@string == "</SMILY>")
			{
				break;
			}
			string[] array = file.GetString().Split('#');
			foreach (string key in array)
			{
				if (!_mSmileyEmotes.ContainsKey(key))
				{
					_mSmileyEmotes.Add(key, @string);
				}
			}
		}
	}

	private void LoadEmoteFiles()
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("ChatEmotes") ?? MilMo_SimpleFormat.LoadLocal("Default/ChatEmotes");
		_mWordEmotes.Clear();
		_mSmileyEmotes.Clear();
		while (milMo_SFFile.NextRow())
		{
			if (milMo_SFFile.IsNext("<TEXT>"))
			{
				ReadTextEmotes(milMo_SFFile);
			}
			else if (milMo_SFFile.IsNext("<SMILY>"))
			{
				ReadSmileyEmotes(milMo_SFFile);
			}
		}
	}

	private string CheckTextForEmotes(string text)
	{
		string result = "";
		int num = -1;
		foreach (KeyValuePair<string, string> mSmileyEmote in _mSmileyEmotes)
		{
			if (text.Contains(mSmileyEmote.Key) && text.LastIndexOf(mSmileyEmote.Key, StringComparison.Ordinal) > num)
			{
				result = mSmileyEmote.Key;
				num = text.LastIndexOf(mSmileyEmote.Key, StringComparison.Ordinal);
			}
		}
		foreach (KeyValuePair<string, string> mWordEmote in _mWordEmotes)
		{
			if (text.ToUpper().Contains(mWordEmote.Key))
			{
				int num2 = text.ToUpper().LastIndexOf(mWordEmote.Key, StringComparison.Ordinal);
				if (num2 > num && (num2 == 0 || !char.IsLetter(text[num2 - 1])) && (text.Length == mWordEmote.Key.Length + num2 || !char.IsLetter(text[num2 + mWordEmote.Key.Length])))
				{
					result = mWordEmote.Key;
					num = num2;
				}
			}
		}
		return result;
	}
}
