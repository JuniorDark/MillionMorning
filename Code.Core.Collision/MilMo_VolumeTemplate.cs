using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Collision;

public abstract class MilMo_VolumeTemplate
{
	public abstract MilMo_Volume Instantiate(Transform transform);

	public static MilMo_VolumeTemplate Create(MilMo_SFFile file)
	{
		if (file.IsNext("Sphere"))
		{
			return new MilMo_SphereTemplate(file);
		}
		if (file.IsNext("Box"))
		{
			return new MilMo_BoxTemplate(file);
		}
		if (file.IsNext("InvertedCircle"))
		{
			return new MilMo_InvertedCircleTemplate(file);
		}
		if (file.IsNext("InvertedCylinder"))
		{
			return new MilMo_InvertedCylinderTemplate(file);
		}
		return null;
	}

	public static MilMo_VolumeTemplate Create(VolumeTemplate template)
	{
		if (template is SphereTemplate)
		{
			return new MilMo_SphereTemplate((SphereTemplate)template);
		}
		if (template is BoxTemplate)
		{
			return new MilMo_BoxTemplate((BoxTemplate)template);
		}
		if (template is InvertedCircleTemplate)
		{
			return new MilMo_InvertedCircleTemplate((InvertedCircleTemplate)template);
		}
		if (template is InvertedCylinderTemplate)
		{
			return new MilMo_InvertedCylinderTemplate((InvertedCylinderTemplate)template);
		}
		return null;
	}
}
