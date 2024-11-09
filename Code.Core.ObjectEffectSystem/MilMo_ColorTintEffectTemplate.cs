using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_ColorTintEffectTemplate : MilMo_ObjectEffectTemplate
{
	public Color Color { get; private set; }

	public MilMo_ColorTintEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		Color = Color.white;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Color"))
			{
				Color = file.GetColor();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_ColorTintEffect(gameObject, this);
	}
}
