using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using UnityEngine;

namespace Code.World.Tutorial;

public sealed class MilMo_TutorialTemplate : MilMo_Template, ITutorialData
{
	public sealed class Trigger
	{
		public string Evt;

		public string Obj = "";

		public int Activations = 1;

		public bool Load(MilMo_SFFile file)
		{
			if (!file.HasMoreTokens())
			{
				return false;
			}
			Evt = file.GetString();
			if (file.HasMoreTokens() && file.IsNext("Activations"))
			{
				Activations = file.GetInt();
			}
			if (file.HasMoreTokens())
			{
				Obj = file.GetString();
			}
			else
			{
				if (Evt.Equals("Area", StringComparison.InvariantCultureIgnoreCase))
				{
					return false;
				}
				if (Evt.Equals("StopTalkTo", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("ReceiveItem", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("LevelUp", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("UseCoins", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("TalkTo", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("UnlockLevel", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("PowerUp", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("Kill", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("UseGameplayObject", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("ReceiveQuest", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("CompleteQuest", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("EnterPlayerState", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("ReceiveMedal", StringComparison.InvariantCultureIgnoreCase) || Evt.Equals("WorldMap", StringComparison.InvariantCultureIgnoreCase))
				{
					Obj = "Any";
				}
			}
			if (file.HasMoreTokens() && file.IsNext("Activations"))
			{
				Activations = file.GetInt();
			}
			return true;
		}
	}

	public sealed class Image
	{
		public string Path;

		public Vector2 Position = Vector2.zero;

		public bool Load(MilMo_SFFile file)
		{
			Path = file.GetString();
			if (!file.HasMoreTokens())
			{
				return true;
			}
			Position = file.GetVector2();
			return true;
		}
	}

	public string World { get; private set; }

	public string Level { get; private set; }

	public MilMo_LocString Headline { get; private set; }

	public MilMo_LocString Text { get; private set; }

	public string ArrowTarget { get; private set; }

	public float Delay { get; private set; }

	public List<Image> Images { get; private set; }

	public List<Trigger> ActivationTriggers { get; private set; }

	public List<Trigger> CloseTriggers { get; private set; }

	public string Dialog { get; private set; }

	public string TargetImagePath { get; private set; }

	public string KeyBindImagePath { get; set; }

	private MilMo_TutorialTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Tutorial")
	{
		World = "";
		Level = "";
		Headline = MilMo_LocString.Empty;
		Text = MilMo_LocString.Empty;
		ArrowTarget = "";
		Images = new List<Image>();
		ActivationTriggers = new List<Trigger>();
		CloseTriggers = new List<Trigger>();
		Dialog = "";
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("World"))
		{
			World = file.GetString();
		}
		else if (file.IsNext("Level"))
		{
			Level = file.GetString();
		}
		else if (file.IsNext("Headline"))
		{
			Headline = MilMo_Localization.GetLocString(file.GetString());
		}
		else if (file.IsNext("Text"))
		{
			Text = MilMo_Localization.GetLocString(file.GetString());
		}
		else if (file.IsNext("Arrow"))
		{
			ArrowTarget = file.GetString();
		}
		else if (file.IsNext("Delay"))
		{
			Delay = file.GetFloat();
		}
		else if (file.IsNext("ImageTarget"))
		{
			TargetImagePath = file.GetString();
		}
		else if (file.IsNext("ImageAction"))
		{
			KeyBindImagePath = file.GetString();
		}
		else if (file.IsNext("Image"))
		{
			Image image = new Image();
			if (!image.Load(file))
			{
				return false;
			}
			Images.Add(image);
		}
		else if (file.IsNext("Trigger"))
		{
			Trigger trigger = new Trigger();
			if (!trigger.Load(file))
			{
				return false;
			}
			ActivationTriggers.Add(trigger);
		}
		else if (file.IsNext("CloseTrigger"))
		{
			Trigger trigger2 = new Trigger();
			if (!trigger2.Load(file))
			{
				return false;
			}
			CloseTriggers.Add(trigger2);
		}
		else
		{
			if (!file.IsNext("Dialog"))
			{
				return base.ReadLine(file);
			}
			Dialog = file.GetString();
		}
		return true;
	}

	public static MilMo_TutorialTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_TutorialTemplate(category, path, filePath);
	}
}
