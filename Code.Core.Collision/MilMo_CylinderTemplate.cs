using UnityEngine;

namespace Code.Core.Collision;

public class MilMo_CylinderTemplate : MilMo_VolumeTemplate
{
	public float Radius { get; private set; }

	public float Height { get; private set; }

	public MilMo_CylinderTemplate(float radius, float height)
	{
		Height = height;
		Radius = radius;
	}

	public override MilMo_Volume Instantiate(Transform transform)
	{
		return new MilMo_Cylinder(this, transform);
	}
}
