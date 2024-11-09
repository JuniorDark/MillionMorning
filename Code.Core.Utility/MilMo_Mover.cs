using UnityEngine;

namespace Code.Core.Utility;

public class MilMo_Mover
{
	public enum UpdateFunc
	{
		Nothing,
		Linear,
		Spring,
		Sinus
	}

	public delegate void ArriveFunc();

	public Vector2 Val;

	public Vector2 Target;

	public Vector2 Vel;

	public Vector2 Pull;

	public Vector2 Drag;

	public Vector2 MinVel;

	public Vector2 MinVal;

	public Vector2 LoopVal;

	public Vector2 LoopReset;

	public bool Looping;

	public Vector2 SinVel;

	public Vector2 SinRate;

	public Vector2 SinAmp;

	public UpdateFunc UpdateType;

	public ArriveFunc Arrive;

	public MilMo_Mover()
	{
		Val.x = 0f;
		Target.x = 0f;
		Vel.x = 0f;
		Pull.x = 0.05f;
		Drag.x = 0.95f;
		MinVel.x = 1E-06f;
		MinVal.x = -1000000f;
		Looping = false;
		LoopVal = Vector2.zero;
		LoopReset = Vector2.zero;
		Val.y = 0f;
		Target.y = 0f;
		Vel.y = 0f;
		Pull.y = 0.05f;
		Drag.y = 0.95f;
		MinVel.y = 1E-06f;
		MinVal.y = -1000000f;
		SinRate = new Vector2(5f, 5f);
		SinAmp = new Vector2(1f, 1f);
		Arrive = null;
	}

	public void Update()
	{
		switch (UpdateType)
		{
		case UpdateFunc.Linear:
			Linear();
			break;
		case UpdateFunc.Spring:
			Spring();
			break;
		case UpdateFunc.Sinus:
			Spring();
			Sinus();
			break;
		}
	}

	public void SetUpdateFunc(UpdateFunc updateType)
	{
		UpdateType = updateType;
	}

	public void ClearArriveFunc()
	{
		Arrive = null;
	}

	private void Linear()
	{
		Val += Vel;
		if (Looping)
		{
			if ((Vel.x > 0f && Val.x >= LoopVal.x) || (Vel.x < 0f && Val.x <= LoopVal.x))
			{
				Val.x = LoopReset.x;
			}
			if ((Vel.y > 0f && Val.y >= LoopVal.y) || (Vel.y < 0f && Val.y <= LoopVal.y))
			{
				Val.y = LoopReset.y;
			}
		}
	}

	private void Spring()
	{
		Vel.x += (Target.x - Val.x) * Pull.x;
		Vel.x += Vel.x * Drag.x - Vel.x;
		Vel.y += (Target.y - Val.y) * Pull.y;
		Vel.y += Vel.y * Drag.y - Vel.y;
		Val += Vel;
		if (Val.x < MinVal.x)
		{
			Val.x = MinVal.x;
		}
		if (Val.y < MinVal.y)
		{
			Val.y = MinVal.y;
		}
		if (UpdateType != UpdateFunc.Sinus && !(Mathf.Abs(Vel.x) > MinVel.x) && !(Mathf.Abs(Vel.y) > MinVel.y))
		{
			Vel.x = 0f;
			Val.x = Target.x;
			Vel.y = 0f;
			Val.y = Target.y;
			UpdateType = UpdateFunc.Nothing;
			if (Arrive != null)
			{
				Arrive();
			}
		}
	}

	private void Sinus()
	{
		SinVel.x = Mathf.Sin(Time.time * SinRate.x) * SinAmp.x;
		SinVel.y = Mathf.Sin(Time.time * SinRate.y) * SinAmp.y;
		Val += SinVel;
	}
}
