using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Template;

namespace Code.World.Gameplay;

public class MilMo_GameplayObjectTemplate : MilMo_Template
{
	public List<TemplateReference> TriggerTemplateReferences { get; private set; }

	public string VisualRep { get; private set; }

	public float Shrinkage { get; private set; }

	public long ShrinkDuration { get; private set; }

	private MilMo_GameplayObjectTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "GameplayObject")
	{
		TriggerTemplateReferences = new List<TemplateReference>();
	}

	public override bool LoadFromNetwork(Template t)
	{
		if (!(t is GameplayObjectTemplate gameplayObjectTemplate))
		{
			return false;
		}
		VisualRep = gameplayObjectTemplate.GetVisualRep();
		TriggerTemplateReferences = (List<TemplateReference>)gameplayObjectTemplate.GetTriggers();
		Shrinkage = gameplayObjectTemplate.GetShrinkage();
		ShrinkDuration = gameplayObjectTemplate.GetShrinkDuration();
		return true;
	}

	public static MilMo_GameplayObjectTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_GameplayObjectTemplate(category, path, filePath);
	}
}
