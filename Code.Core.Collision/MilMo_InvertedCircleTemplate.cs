using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Collision;

public class MilMo_InvertedCircleTemplate : MilMo_VolumeTemplate
{
	public float Radius { get; private set; }

	public bool DynamicRadius { get; private set; }

	public MilMo_InvertedCircleTemplate(MilMo_SFFile file)
	{
		if (file.IsNext("Radius"))
		{
			Radius = file.GetFloat();
		}
		if (file.IsNext("DynamicRadius"))
		{
			DynamicRadius = true;
		}
	}

	public MilMo_InvertedCircleTemplate(InvertedCircleTemplate template)
	{
		Radius = template.GetRadius();
		DynamicRadius = template.IsDynamicRadius();
	}

	public override MilMo_Volume Instantiate(Transform transform)
	{
		return new MilMo_InvertedCircle(this, transform);
	}
}
