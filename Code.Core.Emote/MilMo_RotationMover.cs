namespace Code.Core.Emote;

public sealed class MilMo_RotationMover : MilMo_EmoteMover
{
	public MilMo_RotationMover(string name)
		: base(name)
	{
	}

	public override void Update(MilMo_EmoteManager manager)
	{
		base.Update(manager);
		if (manager != null && !(manager.Renderer == null) && !(manager.Renderer.material == null))
		{
			manager.Renderer.material.SetFloat(base.Name, base.Mover.Val.x);
		}
	}
}
