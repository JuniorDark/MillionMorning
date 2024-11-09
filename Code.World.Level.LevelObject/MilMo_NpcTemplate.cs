using Code.Core.Network.types;
using Code.Core.Template;
using Code.Core.Utility;

namespace Code.World.Level.LevelObject;

public class MilMo_NpcTemplate : MilMo_Template
{
	public string VisualRep { get; private set; }

	public string Portrait { get; private set; }

	public string NPCName { get; private set; }

	private MilMo_NpcTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "NPC")
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			return false;
		}
		if (!(t is NpcTemplate npcTemplate))
		{
			return false;
		}
		NPCName = npcTemplate.GetName();
		VisualRep = npcTemplate.GetVisualRep();
		SetPortrait();
		return true;
	}

	public string GetPortraitKey()
	{
		if (VisualRep == null)
		{
			return "";
		}
		string text = MilMo_Utility.ExtractNameFromPath(VisualRep);
		return "Icon" + text;
	}

	private void SetPortrait()
	{
		string portrait = GetPortraitKey() + "0";
		Portrait = portrait;
	}

	public static MilMo_NpcTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_NpcTemplate(category, path, filePath);
	}
}
