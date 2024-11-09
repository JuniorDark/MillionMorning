using Code.Core.ObjectEffectSystem;
using Code.World.Player;

namespace Code.World.Gameplay;

public class MilMo_GameplayTriggerReactionObjectEffect : MilMo_GameplayTriggerReaction
{
	private readonly string _mObjectEffectName;

	public MilMo_GameplayTriggerReactionObjectEffect(string objectEffectName)
	{
		_mObjectEffectName = objectEffectName;
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		ActivateObjectEffect(obj);
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
		ActivateObjectEffect(obj);
	}

	private void ActivateObjectEffect(MilMo_GameplayObject obj)
	{
		MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(obj.GameObject, _mObjectEffectName);
		if (objectEffect != null)
		{
			obj.AddObjectEffect(objectEffect);
		}
	}
}
