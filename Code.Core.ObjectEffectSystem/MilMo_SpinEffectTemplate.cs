using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_SpinEffectTemplate : MilMo_ObjectEffectTemplate
{
	public float Velocity { get; private set; }

	public float Acceleration { get; private set; }

	public float Duration { get; private set; }

	public MilMo_SpinEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		Duration = float.MaxValue;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Velocity"))
			{
				Velocity = file.GetFloat();
			}
			else if (file.IsNext("Acceleration"))
			{
				Acceleration = file.GetFloat();
			}
			else if (file.IsNext("Duration"))
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
		return new MilMo_SpinEffect(gameObject, this);
	}
}
