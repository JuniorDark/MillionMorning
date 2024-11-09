using Code.Core.Avatar;
using Code.Core.ObjectEffectSystem;
using Code.World.Player;

namespace Code.World.Gameplay;

public class MilMo_GameplayTriggerReactionPlayerEffect : MilMo_GameplayTriggerReaction
{
	private readonly string _mObjectEffectName;

	public MilMo_GameplayTriggerReactionPlayerEffect(string objectEffectName)
	{
		_mObjectEffectName = objectEffectName;
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		if (player != null)
		{
			ActivateObjectEffect(player.Avatar);
		}
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
		if (player != null)
		{
			ActivateObjectEffect(player.Avatar);
		}
	}

	private void ActivateObjectEffect(MilMo_Avatar avatar)
	{
		if (avatar != null && !(avatar.GameObject == null))
		{
			MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(avatar.GameObject, _mObjectEffectName);
			if (objectEffect != null)
			{
				avatar.AddObjectEffect(objectEffect);
			}
		}
	}
}
