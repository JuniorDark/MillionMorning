using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Collision;

public class MilMo_BoxTemplate : MilMo_VolumeTemplate
{
	public Vector3 HalfSize { get; private set; }

	public MilMo_BoxTemplate(MilMo_SFFile file)
	{
		if (file.IsNext("Size"))
		{
			HalfSize = file.GetVector3() * 0.5f;
		}
	}

	public MilMo_BoxTemplate(BoxTemplate template)
	{
		HalfSize = new Vector3(template.GetHalfSize().GetX(), template.GetHalfSize().GetY(), template.GetHalfSize().GetZ());
	}

	public MilMo_BoxTemplate(Vector3 size)
	{
		HalfSize = size / 2f;
	}

	public override MilMo_Volume Instantiate(Transform transform)
	{
		return new MilMo_Box(this, transform);
	}
}
