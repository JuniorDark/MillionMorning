using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_GibEffectTemplate : MilMo_ObjectEffectTemplate
{
	public enum ImpactType
	{
		Random,
		Center
	}

	public enum CollisionType
	{
		Box,
		Capsule,
		Sphere
	}

	public float Duration { get; private set; }

	public float ImpactForce { get; private set; }

	public ImpactType ImpactForceType { get; private set; }

	public Vector3 ImpactOffset { get; private set; }

	public CollisionType GibCollisionType { get; private set; }

	public bool GibCollision { get; private set; }

	public Vector3 GibOffset { get; private set; }

	public float Mass { get; private set; }

	public float Drag { get; private set; }

	public float AngularDrag { get; private set; }

	public bool UseGravity { get; private set; }

	public bool FreezeRotation { get; private set; }

	public MilMo_GibEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		UseGravity = true;
		AngularDrag = 1f;
		Drag = 1f;
		Mass = 0.01f;
		GibOffset = Vector3.zero;
		GibCollision = true;
		GibCollisionType = CollisionType.Box;
		ImpactOffset = Vector3.zero;
		ImpactForceType = ImpactType.Center;
		ImpactForce = 0.4f;
		Duration = 2.5f;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Duration"))
			{
				Duration = file.GetFloat();
			}
			else if (file.IsNext("ImpactForce"))
			{
				ImpactForce = file.GetFloat();
			}
			else if (file.IsNext("Mass"))
			{
				Mass = file.GetFloat();
			}
			else if (file.IsNext("Drag"))
			{
				Drag = file.GetFloat();
			}
			else if (file.IsNext("AngularDrag"))
			{
				AngularDrag = file.GetFloat();
			}
			else if (file.IsNext("Gravity"))
			{
				UseGravity = file.GetBool();
			}
			else if (file.IsNext("FreezeRotation"))
			{
				FreezeRotation = file.GetBool();
			}
			else if (file.IsNext("ImpactType"))
			{
				if (file.IsNext("Random"))
				{
					ImpactForceType = ImpactType.Random;
				}
				else if (file.IsNext("Center"))
				{
					ImpactForceType = ImpactType.Center;
				}
				else
				{
					Debug.LogWarning("Using unknown impact type in " + file.Path + " at line " + file.GetLineNumber());
				}
			}
			else if (file.IsNext("ImpactOffset"))
			{
				ImpactOffset = file.GetVector3();
			}
			else if (file.IsNext("CollisionType"))
			{
				if (file.IsNext("Box"))
				{
					GibCollisionType = CollisionType.Box;
				}
				else if (file.IsNext("Capsule"))
				{
					GibCollisionType = CollisionType.Capsule;
				}
				else if (file.IsNext("Sphere"))
				{
					GibCollisionType = CollisionType.Sphere;
				}
				else
				{
					Debug.LogWarning("Using unknown collision type in " + file.Path + " at line " + file.GetLineNumber());
				}
			}
			else if (file.IsNext("GibCollision"))
			{
				GibCollision = file.GetBool();
			}
			else if (file.IsNext("GibOffset"))
			{
				GibOffset = file.GetVector3();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_GibEffect(gameObject, this);
	}
}
