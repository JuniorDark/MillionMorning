using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_TransparentEffectTemplate : MilMo_ObjectEffectTemplate
{
	public float Alpha { get; private set; }

	public float Duration { get; private set; }

	public MilMo_TransparentEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		Duration = float.MaxValue;
		Alpha = 0.5f;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Alpha"))
			{
				Alpha = file.GetFloat();
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
		return new MilMo_TransparentEffect(gameObject, this);
	}
}
