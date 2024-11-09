using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public abstract class MilMo_ObjectEffectTemplate
{
	public string Name { get; private set; }

	protected MilMo_ObjectEffectTemplate(MilMo_SFFile file)
	{
		Name = file.GetString();
	}

	public abstract MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject);
}
