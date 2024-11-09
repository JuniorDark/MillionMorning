using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_FadeEffectTemplate : MilMo_ObjectEffectTemplate
{
	public float Speed { get; private set; }

	public MilMo_FadeEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Speed"))
			{
				Speed = file.GetFloat();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_FadeEffect(gameObject, this);
	}
}
