using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Collision;

public class MilMo_InvertedCylinderTemplate : MilMo_VolumeTemplate
{
	public float Radius { get; private set; }

	public bool DynamicRadius { get; private set; }

	public float Height { get; private set; }

	public MilMo_InvertedCylinderTemplate(MilMo_SFFile file)
	{
		if (file.IsNext("Radius"))
		{
			Radius = file.GetFloat();
		}
		if (file.IsNext("DynamicRadius"))
		{
			DynamicRadius = true;
		}
		if (file.IsNext("Height"))
		{
			Height = file.GetFloat();
		}
	}

	public MilMo_InvertedCylinderTemplate(InvertedCylinderTemplate template)
	{
		Radius = template.GetRadius();
		DynamicRadius = template.IsDynamicRadius();
		Height = template.GetHeight();
	}

	public override MilMo_Volume Instantiate(Transform transform)
	{
		return new MilMo_InvertedCylinder(this, transform);
	}
}
