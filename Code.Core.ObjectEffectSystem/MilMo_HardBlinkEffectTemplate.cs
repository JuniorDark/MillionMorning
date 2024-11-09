using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_HardBlinkEffectTemplate : MilMo_ObjectEffectTemplate
{
	public float VisibleTime { get; private set; }

	public float InvisibleTime { get; private set; }

	public float Duration { get; private set; }

	public MilMo_HardBlinkEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		Duration = float.MaxValue;
		InvisibleTime = 0.25f;
		VisibleTime = 0.25f;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Visible"))
			{
				VisibleTime = file.GetFloat();
			}
			else if (file.IsNext("Invisible"))
			{
				InvisibleTime = file.GetFloat();
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
		return new MilMo_HardBlinkEffect(gameObject, this);
	}
}
