using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_BlinkEffectTemplate : MilMo_ObjectEffectTemplate
{
	public float Speed { get; private set; }

	public float Duration { get; private set; }

	public MilMo_BlinkEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		Duration = float.MaxValue;
		Speed = 6f;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Speed"))
			{
				Speed = file.GetFloat();
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
		return new MilMo_BlinkEffect(gameObject, this);
	}
}
