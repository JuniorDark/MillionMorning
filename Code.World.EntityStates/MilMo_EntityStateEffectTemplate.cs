using Code.Core.Network.types;
using Code.Core.Template;

namespace Code.World.EntityStates;

public class MilMo_EntityStateEffectTemplate : MilMo_Template
{
	public int StackCount { get; private set; }

	public string ParticleEffect { get; private set; }

	public string EffectType { get; private set; }

	public string EffectName { get; private set; }

	private MilMo_EntityStateEffectTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "EntityStateEffect")
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		EntityStateEffectTemplate entityStateEffectTemplate = (EntityStateEffectTemplate)t;
		EffectType = entityStateEffectTemplate.GetEffectType();
		ParticleEffect = entityStateEffectTemplate.GetParticleEffect();
		StackCount = entityStateEffectTemplate.GetStackCount();
		EffectName = entityStateEffectTemplate.GetEffectName();
		return true;
	}

	public static MilMo_EntityStateEffectTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_EntityStateEffectTemplate(category, path, filePath);
	}
}
