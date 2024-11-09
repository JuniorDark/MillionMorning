using Code.Core.Template;
using Code.World.Player;
using Core;

namespace Code.World.Gameplay;

public sealed class MilMo_GameplayTriggerReactionMover : MilMo_GameplayTriggerReaction
{
	private bool _loaded;

	public MilMo_GameplayTriggerReactionMover(string templatePath)
	{
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("Spline", templatePath, delegate(MilMo_Template t, bool timeout)
		{
			if (!timeout && t != null)
			{
				_loaded = true;
			}
		});
	}

	public override bool MayActivate()
	{
		if (_loaded)
		{
			return base.MayActivate();
		}
		return false;
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
	}
}
