using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.Level;
using Core;

namespace Code.World.Feeds;

public class MilMo_FeedDefaultTemplate : MilMo_Template
{
	public string AfterObjectNameExternal { get; private set; }

	public MilMo_LocString FeedEventIngame { get; private set; }

	public string FeedEventExternal { get; private set; }

	public string DialogSound { get; private set; }

	public MilMo_FeedDefaultTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "FeedDefault")
	{
		DialogSound = "";
		FeedEventExternal = "";
		FeedEventIngame = MilMo_LocString.Empty;
		AfterObjectNameExternal = "";
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("AfterObjectName"))
		{
			AfterObjectNameExternal = MilMo_Localization.GetLocString(file.GetString()).String;
		}
		else if (file.IsNext("FeedEventIngame"))
		{
			FeedEventIngame = MilMo_Localization.GetLocString(file.GetString());
		}
		else if (file.IsNext("FeedEventExternal"))
		{
			FeedEventExternal = MilMo_Localization.GetLocString(file.GetString()).String;
		}
		else
		{
			if (!file.IsNext("DialogSound"))
			{
				return base.ReadLine(file);
			}
			DialogSound = file.GetString();
		}
		return true;
	}

	public static MilMo_FeedDefaultTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_FeedDefaultTemplate(category, path, filePath);
	}

	public static MilMo_FeedDefaultTemplate GetDefaults(string feedType)
	{
		MilMo_Template milMo_Template = null;
		if (MilMo_Level.CurrentLevel != null)
		{
			milMo_Template = Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("FeedDefault", "FeedScript.Defaults." + MilMo_Level.CurrentLevel.WorldContentName + "." + feedType);
		}
		if (milMo_Template == null)
		{
			milMo_Template = Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("FeedDefault", "FeedScript.Defaults." + feedType);
		}
		return milMo_Template as MilMo_FeedDefaultTemplate;
	}
}
