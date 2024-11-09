using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Collision;

public class MilMo_SphereTemplate : MilMo_VolumeTemplate
{
	public float SquareRadius { get; private set; }

	public MilMo_SphereTemplate(MilMo_SFFile file)
	{
		if (file.IsNext("Radius"))
		{
			float @float = file.GetFloat();
			SquareRadius = @float * @float;
		}
	}

	public MilMo_SphereTemplate(SphereTemplate template)
	{
		SquareRadius = template.GetSqrRadius();
	}

	public override MilMo_Volume Instantiate(Transform transform)
	{
		return new MilMo_Sphere(this, transform);
	}
}
