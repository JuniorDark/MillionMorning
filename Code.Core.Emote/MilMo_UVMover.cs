using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_UVMover : MilMo_EmoteMover
{
	public MilMo_UVMover(string name)
		: base(name)
	{
	}

	public override void Update(MilMo_EmoteManager manager)
	{
		base.Update(manager);
		if (manager != null && !(manager.Renderer == null) && !(manager.Renderer.material == null))
		{
			manager.Renderer.material.SetVector(base.Name, new Vector4(base.Mover.Val.x, base.Mover.Val.y));
		}
	}
}
