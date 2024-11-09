using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_ParticleObjectEffectTemplate : MilMo_ObjectEffectTemplate
{
	public string ParticleEffectScriptFile { get; private set; }

	public MilMo_ParticleObjectEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		ParticleEffectScriptFile = file.GetString();
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_ParticleObjectEffect(gameObject, this);
	}
}
