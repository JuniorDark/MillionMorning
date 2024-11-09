using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public abstract class MilMo_ObjectEffect
{
	protected GameObject GameObject;

	protected readonly MilMo_ObjectEffectTemplate EffectTemplate;

	public string Name => EffectTemplate.Name;

	public abstract float Duration { get; }

	protected MilMo_ObjectEffect(GameObject gameObject, MilMo_ObjectEffectTemplate template)
	{
		GameObject = gameObject;
		EffectTemplate = template;
	}

	public virtual bool Update()
	{
		Destroy();
		return false;
	}

	public virtual void FixedUpdate()
	{
	}

	public virtual void Destroy()
	{
	}
}
