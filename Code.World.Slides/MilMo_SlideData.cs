using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.Slides;

public sealed class MilMo_SlideData
{
	public class Sound
	{
		public string Path { get; }

		public float Delay { get; }

		public Sound(MilMo_SFFile file)
		{
			Path = file.GetString();
			if (!Path.StartsWith("Content/Sounds/"))
			{
				Path = "Content/Sounds/" + Path;
			}
			if (file.HasMoreTokens() && file.IsNext("Delay"))
			{
				Delay = file.GetFloat();
			}
		}
	}

	public string MusicPath { get; }

	public float FadeTime { get; }

	public Color FadeColor { get; }

	public Color BorderColor { get; }

	public Color TextColor { get; }

	public string ImagePath { get; }

	public MilMo_LocString Text { get; }

	public bool HasNextButton { get; }

	public float CloseDelay { get; }

	public List<Sound> Sounds { get; }

	private MilMo_SlideData(string imagePath, MilMo_LocString text, bool hasNextButton, float closeDelay, List<Sound> sounds, Color fadeColor, Color borderColor, Color textColor, float fadeTime, string musicPath)
	{
		FadeColor = fadeColor;
		BorderColor = borderColor;
		TextColor = textColor;
		FadeTime = fadeTime;
		ImagePath = imagePath;
		Text = text;
		HasNextButton = hasNextButton;
		CloseDelay = closeDelay;
		Sounds = sounds;
		MusicPath = musicPath;
	}

	public static MilMo_SlideData Create(MilMo_SFFile file)
	{
		string imagePath = "";
		MilMo_LocString text = MilMo_LocString.Empty;
		float closeDelay = 0f;
		List<Sound> list = new List<Sound>();
		Color fadeColor = Color.black;
		Color borderColor = Color.black;
		Color textColor = Color.white;
		float fadeTime = 2.5f;
		string musicPath = "";
		while (file.NextRow())
		{
			if (file.IsNext("</SLIDE>"))
			{
				return new MilMo_SlideData(imagePath, text, hasNextButton: true, closeDelay, list, fadeColor, borderColor, textColor, fadeTime, musicPath);
			}
			if (file.IsNext("Image"))
			{
				imagePath = file.GetString();
			}
			else if (file.IsNext("Text"))
			{
				text = MilMo_Localization.GetLocString(file.GetString());
			}
			else if (file.IsNext("CloseDelay"))
			{
				closeDelay = file.GetFloat();
			}
			else if (file.IsNext("FadeColor"))
			{
				fadeColor = file.GetColor();
			}
			else if (file.IsNext("BorderColor"))
			{
				borderColor = file.GetColor();
			}
			else if (file.IsNext("TextColor"))
			{
				textColor = file.GetColor();
			}
			else if (file.IsNext("FadeTime"))
			{
				fadeTime = file.GetFloat();
			}
			else if (file.IsNext("Music"))
			{
				musicPath = file.GetString();
			}
			else
			{
				if (!file.IsNext("Sound"))
				{
					continue;
				}
				Sound sound = new Sound(file);
				bool flag = false;
				for (int i = 0; i < list.Count; i++)
				{
					if (!(sound.Delay > list[i].Delay))
					{
						list.Insert(i, sound);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(sound);
				}
			}
		}
		Debug.LogWarning("Missing </SLIDE> closing tag in slide in file " + file.Path);
		return null;
	}
}
