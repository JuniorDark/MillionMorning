using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Emote;

public abstract class MilMo_EmoteMover
{
	protected Vector2 DefaultValues = new Vector2(0f, 0f);

	protected string Name { get; private set; }

	public MilMo_Mover Mover { get; private set; }

	public bool Snapping { get; set; }

	protected MilMo_EmoteMover(string name)
	{
		Mover = new MilMo_Mover();
		Name = name;
	}

	public virtual void ResetDefaultValues()
	{
		DefaultValues.Set(0f, 0f);
	}

	public void SetDefaultValues(Vector2 defaultValues)
	{
		DefaultValues = defaultValues;
		Mover.Target.x = DefaultValues.x;
		Mover.Target.y = DefaultValues.y;
		Mover.Val.x = DefaultValues.x;
		Mover.Val.y = DefaultValues.y;
	}

	public virtual void Update(MilMo_EmoteManager manager)
	{
		Mover.Update();
	}

	public virtual void FixedUpdate(MilMo_EmoteManager manager)
	{
	}

	public virtual void LateUpdate(MilMo_EmoteManager manager)
	{
	}

	public virtual void Abort(MilMo_EmoteManager manager)
	{
		Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		Mover.Target.x = DefaultValues.x;
		Mover.Target.y = DefaultValues.y;
		if (Snapping)
		{
			Mover.Val.x = DefaultValues.x;
			Mover.Val.y = DefaultValues.y;
		}
	}
}
