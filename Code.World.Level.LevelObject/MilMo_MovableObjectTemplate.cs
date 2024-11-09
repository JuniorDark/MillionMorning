using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;

namespace Code.World.Level.LevelObject;

public class MilMo_MovableObjectTemplate : MilMo_Template
{
	public string VisualRep { get; private set; }

	public float CollisionRadius { get; private set; }

	public float ImpactRadius { get; private set; }

	public float MarkerYOffset { get; private set; }

	public float ImpactRadiusSqr { get; private set; }

	public float ImpactHeight { get; private set; }

	public float MaxHealth { get; private set; }

	public bool IsImmobile { get; private set; }

	public List<string> DeathEffectsPhase1 { get; private set; }

	public List<string> DeathEffectsPhase2 { get; private set; }

	public int Level { get; private set; }

	public MilMo_LocString DisplayName { get; private set; }

	protected MilMo_MovableObjectTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		if (!(t is MovableObjectTemplate movableObjectTemplate))
		{
			return false;
		}
		VisualRep = movableObjectTemplate.GetVisualRep();
		CollisionRadius = movableObjectTemplate.GetCollisionRadius();
		ImpactRadius = movableObjectTemplate.GetImpactRadius();
		ImpactRadiusSqr = ImpactRadius * ImpactRadius;
		ImpactHeight = movableObjectTemplate.GetImpactHeight();
		MaxHealth = movableObjectTemplate.GetMaxHealth();
		IsImmobile = movableObjectTemplate.GetImmobile() != 0;
		MarkerYOffset = movableObjectTemplate.GetMarkerYOffset();
		DeathEffectsPhase1 = (List<string>)movableObjectTemplate.GetDeathEffectsPhase1();
		DeathEffectsPhase2 = (List<string>)movableObjectTemplate.GetDeathEffectsPhase2();
		Level = movableObjectTemplate.GetLevel();
		DisplayName = MilMo_Localization.GetLocString(movableObjectTemplate.GetDisplayName());
		return true;
	}
}
