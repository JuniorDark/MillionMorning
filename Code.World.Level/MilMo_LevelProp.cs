using Code.Core.Visual;
using UnityEngine;

namespace Code.World.Level;

public class MilMo_LevelProp
{
	public const float SQRD_LOW_LOAD_PRIORITY_DISTANCE = 2500f;

	public const float SQRD_MEDIUM_LOAD_PRIORITY_DISTANCE = 400f;

	public MilMo_VisualRep VisualRep;

	public GameObject GameObject => VisualRep.GameObject;

	public MilMo_LevelProp(MilMo_VisualRep visualRep)
	{
		VisualRep = visualRep;
	}

	public void Destroy()
	{
		if (VisualRep != null)
		{
			VisualRep.Destroy();
			VisualRep = null;
		}
	}
}
