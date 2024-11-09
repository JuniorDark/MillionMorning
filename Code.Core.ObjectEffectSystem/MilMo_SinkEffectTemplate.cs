using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_SinkEffectTemplate : MilMo_ObjectEffectTemplate
{
	public float Duration { get; private set; }

	public MilMo_SinkEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Duration"))
			{
				Duration = file.GetFloat();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_SinkEffect(gameObject, this);
	}
}
