using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Utility;

namespace Code.Core.Items;

public class MilMo_WearableTemplate : MilMo_ItemTemplate
{
	private string _bodyPackFileFullPath;

	private string _bodyPackFilePath;

	private string _bodyPackFileName;

	private string _iconPath;

	public string BodyPackName { get; set; }

	public override string IconPath => _iconPath;

	public override string ExternThumbnailURL => _iconPath.Substring("Content/".Length) + ".png";

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		WearableTemplate wearableTemplate = t as WearableTemplate;
		if (wearableTemplate != null)
		{
			BodyPackName = wearableTemplate.GetBodypack();
		}
		base.LoadFromNetwork((Code.Core.Network.types.Template)wearableTemplate);
		SetupBodypack();
		SetupIconPath();
		return true;
	}

	public void InitFromBodyPack(string bodyPackPath)
	{
		BodyPackName = bodyPackPath;
		SetupBodypack();
		SetupIconPath();
	}

	private void SetupIconPath(string path = null)
	{
		if (path == null)
		{
			_iconPath = "Content/Bodypacks/" + _bodyPackFilePath.Replace("Scripts", "Icons") + "Icon" + _bodyPackFileName;
		}
		else
		{
			_iconPath = path;
		}
	}

	public void SetupBodypack(string path = null)
	{
		string text = (string.IsNullOrEmpty(path) ? BodyPackName : path);
		_bodyPackFileFullPath = text.Replace('.', '/');
		_bodyPackFilePath = MilMo_Utility.RemoveFileNameFromFullPath(_bodyPackFileFullPath);
		_bodyPackFileName = MilMo_Utility.ExtractNameFromPath(_bodyPackFileFullPath);
	}

	public static MilMo_WearableTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_WearableTemplate(category, path, filePath, "Wearable");
	}

	protected MilMo_WearableTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Wearable(this, modifiers);
	}
}
