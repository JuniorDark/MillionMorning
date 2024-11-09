using System.Collections.Generic;
using Code.Core.EventSystem;

namespace Code.Core.Items;

public class MilMo_MysteryBoxTemplate : MilMo_RandomBoxTemplate, IMilMo_OpenableBox
{
	public string IconPathClosed => IconPath;

	public string IconPathOpen => IconPath + "Open";

	public static MilMo_MysteryBoxTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_MysteryBoxTemplate(category, path, filePath, "MysteryBox");
	}

	protected MilMo_MysteryBoxTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_MysteryBox(this, modifiers);
	}

	public void PostOpenEvent()
	{
		MilMo_EventSystem.Instance.PostEvent("tutorial_OpenMysteryBox", "");
	}
}
