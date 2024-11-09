using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_GemTransparencyEffectTemplate : MilMo_ObjectEffectTemplate
{
	public float Alpha { get; private set; }

	public float Duration { get; private set; }

	public MilMo_GemTransparencyEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		Duration = float.MaxValue;
		Alpha = 1f;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Duration"))
			{
				Duration = file.GetFloat();
			}
			else if (file.IsNext("Alpha"))
			{
				Alpha = file.GetFloat();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_GemTransparencyEffect(gameObject, this);
	}
}
