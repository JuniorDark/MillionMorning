using Code.Core.Network.types;
using Code.World.Gameplay;
using UnityEngine;

namespace Code.Core.Collision;

public abstract class MilMo_Volume
{
	protected readonly Transform Transform;

	protected readonly MilMo_VolumeTemplate VolumeTemplate;

	protected MilMo_Volume(MilMo_VolumeTemplate template, Transform transform)
	{
		Transform = transform;
		VolumeTemplate = template;
	}

	public abstract bool IsInside(Vector3 position);

	public virtual float GetSurface()
	{
		return 0f;
	}

	public virtual bool IsInsideDynamic(Vector3 position, MilMo_GameplayObject gameplayObject)
	{
		return true;
	}

	public virtual bool IsDynamicVolume()
	{
		return false;
	}

	public void SetPosition(vector3 position)
	{
		Transform.position = new Vector3(position.GetX(), position.GetY(), position.GetZ());
	}

	public abstract void Debug_Draw();
}
