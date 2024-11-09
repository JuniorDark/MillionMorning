using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Emote;

public class MilMo_RandomEmoteContainer
{
	private readonly string _configFileName;

	private List<MilMo_RandomEmote> _randomEmotes;

	public MilMo_RandomEmoteContainer(string configFileName)
	{
		_configFileName = configFileName;
	}

	public MilMo_Emote GetRandomEmote()
	{
		if (_randomEmotes == null)
		{
			LoadRandomEmoteConfig();
		}
		switch (_randomEmotes.Count)
		{
		case 0:
			return null;
		case 1:
			return new MilMo_Emote(_randomEmotes[0].Emote);
		default:
		{
			float number = Random.Range(0f, 1f);
			foreach (MilMo_RandomEmote randomEmote in _randomEmotes)
			{
				if (randomEmote.IsMyNumber(number))
				{
					return new MilMo_Emote(randomEmote.Emote);
				}
			}
			return null;
		}
		}
	}

	private void LoadRandomEmoteConfig()
	{
		_randomEmotes = new List<MilMo_RandomEmote>();
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal(_configFileName);
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load blink config file (" + _configFileName + ".txt)");
			return;
		}
		float num = 0f;
		while (milMo_SFFile.NextRow())
		{
			string @string = milMo_SFFile.GetString();
			float @float = milMo_SFFile.GetFloat();
			MilMo_Emote emoteByName = MilMo_EmoteSystem.GetEmoteByName(@string);
			if (emoteByName == null)
			{
				Debug.LogWarning("Random emote '" + @string + "' referenced in " + _configFileName + ".txt could not be found.");
			}
			else
			{
				_randomEmotes.Add(new MilMo_RandomEmote(emoteByName, @float));
				num += @float;
			}
		}
		float num2 = 0f;
		foreach (MilMo_RandomEmote randomEmote in _randomEmotes)
		{
			float num3 = num2 + randomEmote.Weight / num;
			randomEmote.RouletteWheelSection = new Vector2(num2, num3);
			num2 = num3;
		}
	}
}
