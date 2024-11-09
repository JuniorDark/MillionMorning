using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_BeachEffectTemplate : MilMo_ObjectEffectTemplate
{
	public float CurveSteepness { get; private set; }

	public float Distance { get; private set; }

	public MilMo_BeachEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Distance"))
			{
				Distance = file.GetFloat();
			}
			else if (file.IsNext("CurveSlope"))
			{
				CurveSteepness = file.GetFloat();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_BeachEffect(gameObject, this);
	}
}
