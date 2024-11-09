using Code.Core.Global;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public abstract class MilMo_SubEffect
{
	protected float CurrentTime;

	protected float Duration;

	protected bool IsStopped = true;

	protected bool YLocked;

	protected float YPos;

	public GameObject EmittingObject { get; protected set; }

	public abstract bool Update();

	public abstract void Stop();

	public abstract void DestroyWhenDone();

	public void Destroy()
	{
		IsStopped = true;
		MilMo_Global.Destroy(EmittingObject);
		EmittingObject = null;
	}
}
