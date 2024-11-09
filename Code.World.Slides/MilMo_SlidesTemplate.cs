using System.Collections.Generic;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using UnityEngine;

namespace Code.World.Slides;

public sealed class MilMo_SlidesTemplate : MilMo_Template
{
	public string Music { get; private set; }

	public List<MilMo_SlideData> Slides { get; private set; }

	public Color FirstFadeColor { get; private set; }

	public float FirstFadeTime { get; private set; }

	private MilMo_SlidesTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Slides")
	{
		FirstFadeColor = Color.black;
		Slides = new List<MilMo_SlideData>();
		Music = "";
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("Music"))
		{
			Music = file.GetString();
		}
		else if (file.IsNext("FirstFadeColor"))
		{
			FirstFadeColor = file.GetColor();
		}
		else if (file.IsNext("FirstFadeTime"))
		{
			FirstFadeTime = file.GetFloat();
		}
		else
		{
			if (!file.IsNext("<SLIDE>"))
			{
				return base.ReadLine(file);
			}
			MilMo_SlideData milMo_SlideData = MilMo_SlideData.Create(file);
			if (milMo_SlideData == null)
			{
				Debug.LogWarning("Failed to load Slides template " + base.Identifier);
				return false;
			}
			Slides.Add(milMo_SlideData);
		}
		return true;
	}

	public static MilMo_SlidesTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_SlidesTemplate(category, path, filePath);
	}
}
