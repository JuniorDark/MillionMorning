using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_OutlineEffectTemplate : MilMo_ObjectEffectTemplate
{
	public Color Color { get; private set; }

	public float Width { get; set; }

	public MilMo_OutlineEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		Color = Color.white;
		Width = 0.1f;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Color"))
			{
				Color = file.GetColor();
			}
			else if (file.IsNext("Width"))
			{
				Width = file.GetFloat();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_OutlineEffect(gameObject, this);
	}
}
